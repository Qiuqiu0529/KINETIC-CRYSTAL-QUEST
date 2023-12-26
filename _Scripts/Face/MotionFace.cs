using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class MotionFace : TrackTarget
{
    [SerializeField] int testnode;
    float width, height;

    [SerializeField] Transform startpos;
    [SerializeField] Transform endpos;

    public SkinnedMeshRenderer skinnedMesh;
    public SkinnedMeshRenderer[] eyebrowMeshs;
    public SkinnedMeshRenderer chooseEyebrow;

    [Range(0, 1)]
    public float BrowParamL = 0;
    [Range(0, 1)]
    public float BrowParamR = 0;

    [Range(0, 1)]
    public float EyeParamL = 0;

    [Range(0, 1)]
    public float EyeParamR = 0;


    [Range(0, 1)]
    public float MouthOpenParam = 0;

    [Range(0, 1)]
    public float MouthSizeLParam = 0;
    [Range(0, 1)]
    public float MouthSizeRParam = 0;

    [Range(0, 1)]
    public float browLeapT = 0.6f;

    [Range(0, 1)]
    public float eyeLeapT = 0.6f;

    [Range(0, 1)]
    public float mouthLeapT = 0.6f;

    protected float distanceOfLeftEyeHeight;

    protected float distanceOfRightEyeHeight;

    protected float distanceOfLeftEyeWidth;

    protected float distanceOfRightEyeWidth;

    protected float distanceOfNoseHeight;

    protected float distanceBetweenLeftPupliAndEyebrow;

    protected float distanceBetweenRightPupliAndEyebrow;

    protected float distanceOfMouthHeight;

    protected float distanceOfMouthWidthL;
    protected float distanceOfMouthWidthR;

    protected float distanceBetweenEyes;
    protected float distanceSquintL;

    public Transform nose;

    public Transform headtrans;

    JointPoint head;

    public bool changeBlendShape = true;
    public bool changEyebrow;
    public Transform[] targets;

    public void Awake() {
        
        changeBlendShape=PlayerPrefs.GetInt(Global.useBlendShape, 1) > 0 ? true : false;
    }

    private void Start()
    {
        head = new JointPoint();
        head.Transform = headtrans;
        head.InitRotation = headtrans.rotation;
        var gaze = nose.position - headtrans.position;
        head.Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));
        head.InverseRotation = head.Inverse * head.InitRotation;
        if (changeBlendShape)
        {
            StartCoroutine(ChooseEyebrow());
        }

        startpos=InitPos.Instance.startpos;
        endpos=InitPos.Instance.endpos;
        width = endpos.position.x - startpos.position.x;
        height = endpos.position.y - startpos.position.y;

        HolisticMotion.instance.AddFaceObserver(this);
    }


    private void Update()
    {
        if (stable)
        {
            CalcFace();
            stable = false;
        }
    }
    public void CalcFace()
    {
        //testnode
        // var target = LandmarkUtil.LandMarkToVector(landmarks[testnode]);
        // targets[0].position = startpos.position + new Vector3(target.x * width, target.y * height, target.z * width);
        //head rot
        var noseup = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.noseup)]);
        var lear = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lear)]);
        var rear = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.rear)]);
        var gaze = noseup - (lear + rear) / 2;

        var f = LandmarkUtil.TriangleNormal(noseup, lear, rear);
        head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.InverseRotation;

        #region blendshape

        if (changeBlendShape)
        {
            #region  eyes
            var lp6 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lp6)]);
            var lp2 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lp2)]);
            var lp3 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lp3)]);
            var lp5 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lp5)]);
            var leyeinner = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.leyeinner)]);
            var leyeouter = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.leyeouter)]);


            distanceOfLeftEyeHeight = ((lp2 - lp6 + lp3 - lp5) / 2).sqrMagnitude;
            distanceOfLeftEyeWidth = (leyeinner - leyeouter).sqrMagnitude;

            var rp6 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.rp6)]);
            var rp2 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.rp2)]);
            var rp3 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.rp3)]);
            var rp5 = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.rp5)]);
            var reyeinner = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.reyeinner)]);
            var reyeouter = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.reyeouter)]);
            distanceOfRightEyeHeight = ((rp2 - rp6 + rp3 - rp5) / 2).sqrMagnitude;
            distanceOfRightEyeWidth = (reyeinner - reyeouter).sqrMagnitude;


            distanceSquintL=(LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lsquint1)])-LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lsquint2)])).sqrMagnitude;

            #endregion

            var nosedown = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.nosedown)]);

            distanceOfNoseHeight = ((leyeinner + reyeinner) / 2 - nosedown).sqrMagnitude;

            if (changEyebrow)
            {
                distanceBetweenLeftPupliAndEyebrow = (LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.lbrow)]) - (leyeinner + leyeouter) / 2).sqrMagnitude;
                distanceBetweenRightPupliAndEyebrow = (LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.rbrow)]) - (reyeinner + reyeouter) / 2).sqrMagnitude;
                float eyebrowUpL = GetLeftEyebrowUPRatio();
                float eyebrowUpR = GetRightEyebrowUPRatio();
                BrowParamL = Mathf.Lerp(BrowParamL, eyebrowUpL, browLeapT);
                BrowParamR = Mathf.Lerp(BrowParamR, eyebrowUpR, browLeapT);
            }


            var mouthoutterup = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.mouthoutterup)]);
            var mouthoutterdown = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.mouthoutterdown)]);
            var mouthleft = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.mouthleft)]);
            var mouthright = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.mouthright)]);
            var mouthinnerdown = LandmarkUtil.LandMarkToVector(landmarks[((int)FaceIndex.mouthinnerdown)]);



            distanceOfMouthHeight = (mouthoutterup - mouthoutterdown).sqrMagnitude;
            distanceOfMouthWidthL = (mouthleft - mouthinnerdown).sqrMagnitude;
            distanceOfMouthWidthR = (mouthright - mouthinnerdown).sqrMagnitude;
            distanceBetweenEyes = (leyeinner - reyeinner).sqrMagnitude;

            float eyeOpenL = GetLeftEyeOpenRatio();
            float eyeOpenR = GetRightEyeOpenRatio();
            if (eyeOpenL >= 0.55f)
            {
                eyeOpenL = 1.0f;
            }
            else if (eyeOpenL < 0.3f)
            {
                eyeOpenL = 0.0f;
            }
            EyeParamL = Mathf.Lerp(EyeParamL, 1.0f - eyeOpenL, eyeLeapT);
            // Debug.Log("EyeParamL: " + EyeParamL);
            if (eyeOpenR >= 0.55f)
            {
                eyeOpenR = 1.0f;
            }
            else if (eyeOpenR < 0.3f)
            {
                eyeOpenR = 0.0f;
            }
            EyeParamR = Mathf.Lerp(EyeParamR, 1.0f - eyeOpenR, eyeLeapT);
            //Debug.Log(" EyeParamR : " + EyeParamR );

            float mouthOpen = GetMouthOpenYRatio();
            //Debug.Log("mouthOpen " + mouthOpen);

            if (mouthOpen >= 0.7f)
            {
                mouthOpen = 1.0f;
            }
            else if (mouthOpen >= 0.25f)
            {
                mouthOpen = 0.5f;
            }
            else
            {
                mouthOpen = 0.0f;
            }
            MouthOpenParam = Mathf.Lerp(MouthOpenParam, mouthOpen, mouthLeapT);

            float mouthSizeL = GetMouthOpenXLRatio();
            float mouthSizeR = GetMouthOpenXRRatio();
            //Debug.Log("mouthSize " + mouthSize);

            if (mouthSizeL >= 0.8f)
            {
                mouthSizeL = 1.0f;
            }
            else if (mouthSizeL >= 0.6f)
            {
                mouthSizeL = 0.5f;
            }
            else
            {
                mouthSizeL = 0.0f;
            }
            if (mouthSizeR >= 0.8f)
            {
                mouthSizeR = 1.0f;
            }
            else if (mouthSizeR >= 0.6f)
            {
                mouthSizeR = 0.5f;
            }
            else
            {
                mouthSizeR = 0.0f;
            }
            MouthSizeLParam = Mathf.Lerp(MouthSizeLParam, mouthSizeL, mouthLeapT);
            MouthSizeRParam = Mathf.Lerp(MouthSizeRParam, mouthSizeR, mouthLeapT);

            SetBlendShape();
        }
        #endregion

    }

    public void SetBlendShape()
    {
        skinnedMesh.SetBlendShapeWeight(((int)ModelFaceIndex.leyeclose), EyeParamL * 100);
        skinnedMesh.SetBlendShapeWeight(((int)ModelFaceIndex.reyeclose), EyeParamR * 100);

        skinnedMesh.SetBlendShapeWeight(((int)ModelFaceIndex.mouthopen), MouthOpenParam * 100);
        //skinnedMesh.SetBlendShapeWeight(((int)VRMFaceIndex.mouthO), MouthOpenParam * 0.7f * 100);
        //skinnedMesh.SetBlendShapeWeight(((int)VRMFaceIndex.mouthI), MouthSizeParam * 100);

        if (changEyebrow)
        {
            chooseEyebrow.SetBlendShapeWeight(((int)EyebrowIndex.browUpL), BrowParamL * 100);
            chooseEyebrow.SetBlendShapeWeight(((int)EyebrowIndex.browUpR), BrowParamR * 100);
        }
    }

    protected float GetLeftEyeOpenRatio()
    {
        float ratio = distanceOfLeftEyeHeight / distanceOfLeftEyeWidth;
        //Debug.Log("raw LeftEyeOpen ratio: " + ratio);
        return Mathf.InverseLerp(0.04f, 0.25f, ratio);
    }

    protected float GetRightEyeOpenRatio()
    {
        float ratio = distanceOfRightEyeHeight / distanceOfRightEyeWidth;
        //Debug.Log("raw RightEyeOpen ratio: " + ratio);
        return Mathf.InverseLerp(0.04f, 0.25f, ratio);
    }

    protected float GetLeftEyebrowUPRatio()
    {
        float ratio = distanceBetweenLeftPupliAndEyebrow / distanceOfNoseHeight;
        //Debug.Log ("raw LeftEyebrowUP ratio: " + ratio);
        return Mathf.InverseLerp(0.18f, 0.4f, ratio);
    }

    protected float GetRightEyebrowUPRatio()
    {
        float ratio = distanceBetweenRightPupliAndEyebrow / distanceOfNoseHeight;
        //Debug.Log ("raw RightEyebrowUP ratio: " + ratio);
        return Mathf.InverseLerp(0.18f, 0.4f, ratio);
    }

    protected float GetMouthOpenYRatio()
    {
        float ratio = distanceOfMouthHeight / distanceOfNoseHeight;
        //Debug.Log ("raw MouthOpenY ratio: " + ratio);
        return Mathf.InverseLerp(0.15f, 0.8f, ratio);
    }

    protected float GetMouthOpenXLRatio()
    {
        float ratio = distanceOfMouthWidthL / distanceBetweenEyes;
        //Debug.Log("raw MouthOpenXL ratio: " + ratio);
        return Mathf.InverseLerp(0.4f, 0.6f, ratio);
    }
    protected float GetMouthOpenXRRatio()
    {
        float ratio = distanceOfMouthWidthR / distanceBetweenEyes;
        //Debug.Log("raw MouthOpenXR ratio: " + ratio);
        return Mathf.InverseLerp(0.4f, 0.6f, ratio);
    }

    public IEnumerator ChooseEyebrow()
    {
        yield return new WaitForSeconds(1f);
        for (int i = eyebrowMeshs.Length - 1; i >= 0; --i)
        {
            if (eyebrowMeshs[i] != null)
            {
                chooseEyebrow = eyebrowMeshs[i];
                changEyebrow = true;
                break;
            }
        }
    }
}
