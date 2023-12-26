using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
public class JointPoint
{
    public Vector3 Pos3D = new Vector3();
    public Transform Transform = null;
    public Quaternion InitRotation;
    public Quaternion Inverse;
    public Quaternion InverseRotation;

    public JointPoint Parent = null;
    public JointPoint Child = null;
    //kalman
    public Vector3 P = new Vector3();
    public Vector3 X = new Vector3();
    public Vector3 K = new Vector3();

    public Vector3[] PrevPos3D = new Vector3[6];

    public float score = 0;


    public void InitJointPoints(Vector3 forward)
    {
        if (Transform != null)
        {
            Pos3D = Transform.position;
            InitRotation = Transform.rotation;
        }
        if (Child != null)
        {
            Inverse = LandmarkUtil.GetInverse(this, this.Child, forward);
            InverseRotation = Inverse * InitRotation;
        }

    }
    public void Smooth(float lowPassParam)
    {
        PrevPos3D[0] = Pos3D;
        for (var i = 1; i < PrevPos3D.Length; i++)
        {
            PrevPos3D[i] = PrevPos3D[i] * lowPassParam + PrevPos3D[i - 1] * (1f - lowPassParam);
        }
        Pos3D = PrevPos3D[PrevPos3D.Length - 1];

    }

    public void Kalman(float kalmanParamR, float kalmanParamQ)
    {
        K.x = (P.x + kalmanParamQ) / (P.x + kalmanParamQ + kalmanParamR);
        K.y = (P.y + kalmanParamQ) / (P.y + kalmanParamQ + kalmanParamR);
        K.z = (P.z + kalmanParamQ) / (P.z + kalmanParamQ + kalmanParamR);
        P.x = kalmanParamR * (P.x + kalmanParamQ) / (kalmanParamR + P.x + kalmanParamQ);
        P.y = kalmanParamR * (P.y + kalmanParamQ) / (kalmanParamR + P.y + kalmanParamQ);
        P.z = kalmanParamR * (P.z + kalmanParamQ) / (kalmanParamR + P.z + kalmanParamQ);

        Pos3D.x = X.x + (Pos3D.x - X.x) * K.x;
        Pos3D.y = X.y + (Pos3D.y - X.y) * K.y;
        Pos3D.z = X.z + (Pos3D.z - X.z) * K.z;
        X = Pos3D;
    }
}

public class MotionModelPose : WorldTrackTarget, ILandmarkObserver //vrm模型的骨骼物体，加到vrm根物体上
{
    public static MotionModelPose instance;
    public List<JointPoint> jointPoints = new();
    [SerializeField] Vector3 initPosition;
    public Vector3 forward;

    private float centerTall = 224 * 0.75f;
    private float tall = 224 * 0.75f;
    private float prevTall = 224 * 0.75f;
    public float zScale = 0.8f;

    [SerializeField] Animator anim;

    [SerializeField] Transform startpos;
    [SerializeField] Transform endpos;
    float width, height;
    public Transform[] targets;
    Vector3 hippos;

    bool fixedHip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        fixedHip = PlayerPrefs.GetInt(Global.fixedHip, 0) > 0 ? true : false;
        Bind();
    }
    #region transformbind
    public Transform nose;
    [SerializeField] Transform Root;

    [SerializeField] Transform C_Hips;
    [SerializeField] Transform C_Spine;
    [SerializeField] Transform C_Chest;
    [SerializeField] Transform C_UpperChest;
    [SerializeField] Transform C_Neck;
    [SerializeField] Transform C_Head;
    [SerializeField] Transform L_FaceEye;
    [SerializeField] Transform R_FaceEye;
    [SerializeField] Transform L_Shoulder;
    [SerializeField] Transform L_UpperArm;
    [SerializeField] Transform L_LowerArm;
    [SerializeField] Transform L_Hand;
    [SerializeField] Transform L_Index1;//整根手指，食指
    [SerializeField] Transform L_Little1;//小指
    [SerializeField] Transform L_Thumb1;

    [SerializeField] Transform R_Shoulder;
    [SerializeField] Transform R_UpperArm;
    [SerializeField] Transform R_LowerArm;
    [SerializeField] Transform R_Hand;
    [SerializeField] Transform R_Index1;//整根手指，食指
    [SerializeField] Transform R_Little1;//小指
    [SerializeField] Transform R_Thumb1;
    [SerializeField] Transform L_UpperLeg;
    [SerializeField] Transform L_LowerLeg;
    [SerializeField] Transform L_Foot;
    [SerializeField] Transform L_ToeBase;

    [SerializeField] Transform R_UpperLeg;
    [SerializeField] Transform R_LowerLeg;
    [SerializeField] Transform R_Foot;
    [SerializeField] Transform R_ToeBase;

    #endregion


    private void Start()
    {
        startpos = InitPos.Instance.startpos;
        endpos = InitPos.Instance.endpos;
        width = endpos.position.x - startpos.position.x;
        height = endpos.position.y - startpos.position.y;
        if (useHolistic)
        {
            HolisticMotion.instance.AddWorldPoseObserver(this);
            HolisticMotion.instance.AddPoseObserver(this);

        }
        else
        {
            PoseTrack.instance.AddWorldPoseObserver(this);
            PoseTrack.instance.AddPoseObserver(this);
        }
        Debug.Log(width);
    }

    public void UpdateLandmarks(NormalizedLandmarkList landmarkList)
    {
        if (landmarkList == null)
            return;

        Vector3 lhip = LandmarkUtil.LandMarkToVector(landmarkList.Landmark[((int)PoseIndex.lhip)], zScale);
        Vector3 rhip = LandmarkUtil.LandMarkToVector(landmarkList.Landmark[((int)PoseIndex.rhip)], zScale);
        hippos = (lhip + rhip) / 2;
    }

    public void Bind()
    {
        for (var i = 0; i < Global.posenum; i++)
        {
            var joinpoint = new JointPoint();
            jointPoints.Add(joinpoint);
        }

        jointPoints[((int)PoseIndex.hip)].Transform = C_Hips;
        jointPoints[((int)PoseIndex.head)].Transform = C_Head;
        jointPoints[((int)PoseIndex.neck)].Transform = C_Neck;
        jointPoints[((int)PoseIndex.spine)].Transform = C_Spine;


        jointPoints[((int)PoseIndex.lear)].Transform = C_Head;
        jointPoints[((int)PoseIndex.leye)].Transform = R_FaceEye;

        jointPoints[((int)PoseIndex.rear)].Transform = C_Head;
        jointPoints[((int)PoseIndex.reye)].Transform = L_FaceEye;

        jointPoints[((int)PoseIndex.nose)].Transform = nose;

        jointPoints[((int)PoseIndex.lshoulder)].Transform = R_UpperArm;//模型的左右和人镜像
        jointPoints[((int)PoseIndex.lelbow)].Transform = R_LowerArm;
        jointPoints[((int)PoseIndex.lwrist)].Transform = R_Hand;
        jointPoints[((int)PoseIndex.lpinkey)].Transform = R_Little1;
        jointPoints[((int)PoseIndex.lindex)].Transform = R_Index1;
        jointPoints[((int)PoseIndex.lthumb)].Transform = R_Thumb1;

        jointPoints[((int)PoseIndex.lhip)].Transform = R_UpperLeg;
        jointPoints[((int)PoseIndex.lknee)].Transform = R_LowerLeg;
        jointPoints[((int)PoseIndex.lfoot)].Transform = R_ToeBase;//脚前
        jointPoints[((int)PoseIndex.lankle)].Transform = R_Foot;

        jointPoints[((int)PoseIndex.rshoulder)].Transform = L_UpperArm;
        jointPoints[((int)PoseIndex.relbow)].Transform = L_LowerArm;
        jointPoints[((int)PoseIndex.rwrist)].Transform = L_Hand;
        jointPoints[((int)PoseIndex.rpinkey)].Transform = L_Little1;
        jointPoints[((int)PoseIndex.rindex)].Transform = L_Index1;
        jointPoints[((int)PoseIndex.rthumb)].Transform = L_Thumb1;

        jointPoints[((int)PoseIndex.rhip)].Transform = L_UpperLeg;
        jointPoints[((int)PoseIndex.rknee)].Transform = L_LowerLeg;
        jointPoints[((int)PoseIndex.rfoot)].Transform = L_ToeBase;//脚前
        jointPoints[((int)PoseIndex.rankle)].Transform = L_Foot;


        jointPoints[((int)PoseIndex.lshoulder)].Child = jointPoints[((int)PoseIndex.lelbow)];
        jointPoints[((int)PoseIndex.lelbow)].Parent = jointPoints[((int)PoseIndex.lshoulder)];
        jointPoints[((int)PoseIndex.lelbow)].Child = jointPoints[((int)PoseIndex.lwrist)];

        jointPoints[((int)PoseIndex.rshoulder)].Child = jointPoints[((int)PoseIndex.relbow)];
        jointPoints[((int)PoseIndex.relbow)].Parent = jointPoints[((int)PoseIndex.rshoulder)];
        jointPoints[((int)PoseIndex.relbow)].Child = jointPoints[((int)PoseIndex.rwrist)];

        jointPoints[((int)PoseIndex.lhip)].Child = jointPoints[((int)PoseIndex.lknee)];
        jointPoints[((int)PoseIndex.lknee)].Child = jointPoints[((int)PoseIndex.lankle)];
        jointPoints[((int)PoseIndex.lankle)].Child = jointPoints[((int)PoseIndex.lfoot)];
        jointPoints[((int)PoseIndex.lankle)].Parent = jointPoints[((int)PoseIndex.lknee)];


        jointPoints[((int)PoseIndex.rhip)].Child = jointPoints[((int)PoseIndex.rknee)];
        jointPoints[((int)PoseIndex.rknee)].Child = jointPoints[((int)PoseIndex.rankle)];
        jointPoints[((int)PoseIndex.rankle)].Child = jointPoints[((int)PoseIndex.rfoot)];
        jointPoints[((int)PoseIndex.rankle)].Parent = jointPoints[((int)PoseIndex.rknee)];


        jointPoints[((int)PoseIndex.spine)].Child = jointPoints[((int)PoseIndex.neck)];
        jointPoints[((int)PoseIndex.neck)].Child = jointPoints[((int)PoseIndex.head)];

        // var forward = LandmarkUtil.TriangleNormal(jointPoints[((int)PoseIndex.hip)].Transform.position,
        // jointPoints[((int)PoseIndex.lhip)].Transform.position, jointPoints[((int)PoseIndex.rhip)].Transform.position);
        var forward = Vector3.forward;
        //Debug.Log("posinitforward"+forward);

        foreach (var jointPoint in jointPoints)
        {
            jointPoint.InitJointPoints(forward);
        }

        var hip = jointPoints[((int)PoseIndex.hip)];
        initPosition = jointPoints[((int)PoseIndex.hip)].Transform.position;
        hip.Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        hip.InverseRotation = hip.Inverse * hip.InitRotation;


        // var lHand = jointPoints[((int)PoseIndex.lwrist)];
        // var lf = LandmarkUtil.TriangleNormal(lHand.Pos3D, jointPoints[((int)PoseIndex.lpinkey)].Pos3D,
        // jointPoints[((int)PoseIndex.lindex)].Pos3D);
        // lHand.InitRotation = lHand.Transform.rotation;
        // lHand.Inverse = Quaternion.Inverse(
        //     Quaternion.LookRotation(jointPoints[((int)PoseIndex.lindex)].Transform.position - jointPoints[((int)PoseIndex.lpinkey)].Transform.position, lf));
        // lHand.InverseRotation = lHand.Inverse * lHand.InitRotation;

        // var rHand = jointPoints[((int)PoseIndex.rwrist)];
        // var rf = LandmarkUtil.TriangleNormal(rHand.Pos3D, 
        // jointPoints[((int)PoseIndex.rindex)].Pos3D, jointPoints[((int)PoseIndex.rpinkey)].Pos3D);
        // rHand.InitRotation = rHand.Transform.rotation;
        // rHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[((int)PoseIndex.rindex)].Transform.position - jointPoints[((int)PoseIndex.rpinkey)].Transform.position, rf));
        // rHand.InverseRotation = rHand.Inverse * rHand.InitRotation;

    }

    public void FindTransform()//找到对应关节，更换了模型后舍弃，暂时保留
    {
        Root = transform.Find("Root");
        C_Hips = Root.Find("C_Hips");
        C_Spine = C_Hips.Find("C_Spine");
        L_UpperLeg = C_Hips.Find("L_UpperLeg");
        R_UpperLeg = C_Hips.Find("R_UpperLeg");

        C_Chest = C_Spine.Find("C_Chest");
        C_UpperChest = C_Chest.Find("C_UpperChest");

        C_Neck = C_UpperChest.Find("C_Neck");
        C_Head = C_Neck.Find("C_Head");
        L_FaceEye = C_Head.Find("L_FaceEye");
        R_FaceEye = C_Head.Find("R_FaceEye");

        L_Shoulder = C_UpperChest.Find("L_Shoulder");
        L_UpperArm = L_Shoulder.Find("L_UpperArm");
        L_LowerArm = L_UpperArm.Find("L_LowerArm");
        L_Hand = L_LowerArm.Find("L_Hand");

        L_Index1 = L_Hand.Find("L_Index1");
        L_Little1 = L_Hand.Find("L_Little1");
        L_Thumb1 = L_Hand.Find("L_Thumb1");
        R_Shoulder = C_UpperChest.Find("R_Shoulder");
        R_UpperArm = R_Shoulder.Find("R_UpperArm");
        R_LowerArm = R_UpperArm.Find("R_LowerArm");
        R_Hand = R_LowerArm.Find("R_Hand");

        R_Index1 = R_Hand.Find("R_Index1");
        R_Little1 = R_Hand.Find("R_Little1");
        R_Thumb1 = R_Hand.Find("R_Thumb1");
        L_LowerLeg = L_UpperLeg.Find("L_LowerLeg");
        L_Foot = L_LowerLeg.Find("L_Foot");
        L_ToeBase = L_Foot.Find("L_ToeBase");

        R_LowerLeg = R_UpperLeg.Find("R_LowerLeg");
        R_Foot = R_LowerLeg.Find("R_Foot");
        R_ToeBase = R_Foot.Find("R_ToeBase");
    }

    public void FindTransformMethodTwo()//找到对应关节
    {
        // anim = GetComponent<Animator>();
        // Root = transform.Find("Root");
        C_Hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        C_Spine = anim.GetBoneTransform(HumanBodyBones.Spine);
        L_UpperLeg = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        R_UpperLeg = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);

        C_Chest = anim.GetBoneTransform(HumanBodyBones.Chest);
        C_UpperChest = anim.GetBoneTransform(HumanBodyBones.UpperChest);

        C_Neck = anim.GetBoneTransform(HumanBodyBones.Neck);
        C_Head = anim.GetBoneTransform(HumanBodyBones.Head);
        L_FaceEye = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        R_FaceEye = anim.GetBoneTransform(HumanBodyBones.RightEye);

        L_Shoulder = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
        L_UpperArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        L_LowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        L_Hand = anim.GetBoneTransform(HumanBodyBones.LeftHand);

        L_Index1 = anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
        L_Little1 = anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
        L_Thumb1 = anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);

        R_Shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        R_UpperArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        R_LowerArm = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        R_Hand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        R_Index1 = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        R_Little1 = anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
        R_Thumb1 = anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);

        L_LowerLeg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        L_Foot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        L_ToeBase = anim.GetBoneTransform(HumanBodyBones.LeftToes);

        R_LowerLeg = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        R_Foot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        R_ToeBase = anim.GetBoneTransform(HumanBodyBones.RightToes);
    }



    private void Update()
    {
        if (stable)
        {
            Move();
            //stable = false;
        }
    }
    public Quaternion GetJointRot(int index)
    {
        return jointPoints[index].Transform.rotation;
    }

    public void Move()
    {

        for (int i = 0; i < Global.poseLandmarks; i++)
        {
            jointPoints[i].score = landmarks[i].Visibility;
            jointPoints[i].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[i]);
            //targets[i].position = jointPoints[i].Pos3D;
        }
        jointPoints[((int)PoseIndex.neck)].Pos3D = (jointPoints[((int)PoseIndex.lshoulder)].Pos3D + jointPoints[((int)PoseIndex.rshoulder)].Pos3D) / 2;
        var earCenter = (jointPoints[(int)PoseIndex.lear].Pos3D + jointPoints[(int)PoseIndex.rear].Pos3D) / 2f;
        var nhv = Vector3.Normalize(earCenter - jointPoints[((int)PoseIndex.neck)].Pos3D);
        var nv = jointPoints[((int)PoseIndex.nose)].Pos3D - jointPoints[((int)PoseIndex.neck)].Pos3D;
        jointPoints[((int)PoseIndex.head)].Pos3D = jointPoints[((int)PoseIndex.neck)].Pos3D + nhv * Vector3.Dot(nhv, nv);
        jointPoints[((int)PoseIndex.hip)].Pos3D = (jointPoints[((int)PoseIndex.lhip)].Pos3D + jointPoints[((int)PoseIndex.rhip)].Pos3D) / 2;//中心
        jointPoints[((int)PoseIndex.spine)].Pos3D = jointPoints[((int)PoseIndex.hip)].Pos3D + (jointPoints[((int)PoseIndex.neck)].Pos3D - jointPoints[((int)PoseIndex.hip)].Pos3D) * 0.1f;

        for (int i = 0; i < jointPoints.Count; i++)
        {
            jointPoints[i].Kalman(kalmanParamR, kalmanParamQ);//卡曼滤波，数据更稳定
            jointPoints[i].Smooth(lowPassParam);
        }

        // var t1 = Vector3.Distance(jointPoints[(int)PoseIndex.head].Pos3D, jointPoints[(int)PoseIndex.neck].Pos3D);
        // var t2 = Vector3.Distance(jointPoints[(int)PoseIndex.neck].Pos3D, jointPoints[(int)PoseIndex.spine].Pos3D);
        // var pm = jointPoints[((int)PoseIndex.hip)].Pos3D;
        // var t3 = Vector3.Distance(jointPoints[(int)PoseIndex.spine].Pos3D, pm);
        // var t4r = Vector3.Distance(jointPoints[(int)PoseIndex.rhip].Pos3D, jointPoints[(int)PoseIndex.rknee].Pos3D);
        // var t4l = Vector3.Distance(jointPoints[(int)PoseIndex.lhip].Pos3D, jointPoints[(int)PoseIndex.lknee].Pos3D);
        // var t4 = (t4r + t4l) / 2f;
        // var t5r = Vector3.Distance(jointPoints[(int)PoseIndex.rknee].Pos3D, jointPoints[(int)PoseIndex.rankle].Pos3D);
        // var t5l = Vector3.Distance(jointPoints[(int)PoseIndex.lknee].Pos3D, jointPoints[(int)PoseIndex.lankle].Pos3D);
        // var t5 = (t5r + t5l) / 2f;
        // var t = t1 + t2 + t3 + t4 + t5;
        // tall = t * 0.7f + prevTall * 0.3f;

        // prevTall = tall;

        // if (tall == 0)
        // {
        //     tall = centerTall;
        // }
        // var dz = (centerTall - tall) / centerTall * zScale;


        forward = LandmarkUtil.TriangleNormal(jointPoints[(int)PoseIndex.spine].Pos3D, jointPoints[(int)PoseIndex.lhip].Pos3D, jointPoints[(int)PoseIndex.rhip].Pos3D);
        //Debug.Log(forward);

        if (!fixedHip)
        {
            jointPoints[(int)PoseIndex.hip].Transform.position = new Vector3(hippos.x * width + startpos.position.x,
         hippos.y * height + startpos.position.y, jointPoints[(int)PoseIndex.hip].Transform.position.z);
        }

        jointPoints[(int)PoseIndex.hip].Transform.rotation = Quaternion.LookRotation(forward) * jointPoints[(int)PoseIndex.hip].InverseRotation;

        foreach (var jointPoint in jointPoints)
        {

            if (jointPoint.Parent != null && jointPoint.score > 0.3f && jointPoint.Parent.score > 0.3f)
            {
                var fv = jointPoint.Pos3D - jointPoint.Parent.Pos3D;

                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, fv) * jointPoint.InverseRotation;


            }
            else if (jointPoint.Child != null && jointPoint.score > 0.3f && jointPoint.Child.score > 0.3f)
            {
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, forward) * jointPoint.InverseRotation;
                // if (jointPoint.useRotRange)
                // {
                //     float rotationX = Mathf.Clamp(jointPoint.Transform.localRotation.x, jointPoint.rotRange[0].x, jointPoint.rotRange[1].x);
                //     float rotationY = Mathf.Clamp(jointPoint.Transform.localRotation.y, jointPoint.rotRange[0].y, jointPoint.rotRange[1].y);
                //     float rotationZ = Mathf.Clamp(jointPoint.Transform.localRotation.z, jointPoint.rotRange[0].z, jointPoint.rotRange[1].z);
                //     jointPoint.Transform.localRotation = new Quaternion(rotationX, rotationY, rotationZ, jointPoint.Transform.localRotation.w);
                // }
            }
        }


        // // Wrist rotation (Test code)
        // if (jointPoints[(int)PoseIndex.lwrist].score > 0.3f)
        // {
        //     Debug.Log("lhand");
        //     var lHand = jointPoints[(int)PoseIndex.lwrist];
        //     var lf = LandmarkUtil.TriangleNormal(lHand.Pos3D, jointPoints[(int)PoseIndex.lpinkey].Pos3D, jointPoints[(int)PoseIndex.lindex].Pos3D);
        //     lHand.Transform.rotation = Quaternion.LookRotation(jointPoints[(int)PoseIndex.lindex].Pos3D - jointPoints[(int)PoseIndex.lpinkey].Pos3D, lf) * lHand.InverseRotation;


        // }

        // if (jointPoints[(int)PoseIndex.rwrist].score > 0.3f)
        // {
            
        //     Debug.Log("rhand");
        //     var rHand = jointPoints[(int)PoseIndex.rwrist];
        //     var rf = LandmarkUtil.TriangleNormal(rHand.Pos3D, jointPoints[(int)PoseIndex.rindex].Pos3D, jointPoints[(int)PoseIndex.rpinkey].Pos3D);
        //     //rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.InverseRotation;
        //     rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[(int)PoseIndex.rindex].Pos3D - jointPoints[(int)PoseIndex.rpinkey].Pos3D, rf) * rHand.InverseRotation;
        // }



    }

}