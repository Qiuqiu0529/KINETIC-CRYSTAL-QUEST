using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class MotionRHand : TrackTarget
{
    float width, height;
    public bool useHandTrack = true;

    public Transform[] targets;

    [SerializeField] Animator anim;
    #region transformbind
    [SerializeField] Transform L_Hand;
    [SerializeField] Transform L_Index1;//整根手指，食指
    [SerializeField] Transform L_Index2;//上半部分
    [SerializeField] Transform L_Index3;//上面一小节
    [SerializeField] Transform L_Index4;
    [SerializeField] Transform L_Little1;//小指
    [SerializeField] Transform L_Little2;
    [SerializeField] Transform L_Little3;
    [SerializeField] Transform L_Little4;
    [SerializeField] Transform L_Middle1;
    [SerializeField] Transform L_Middle2;
    [SerializeField] Transform L_Middle3;
    [SerializeField] Transform L_Middle4;
    [SerializeField] Transform L_Ring1;
    [SerializeField] Transform L_Ring2;
    [SerializeField] Transform L_Ring3;
    [SerializeField] Transform L_Ring4;
    [SerializeField] Transform L_Thumb1;
    [SerializeField] Transform L_Thumb2;
    [SerializeField] Transform L_Thumb3;
    [SerializeField] Transform L_Thumb4;


    #endregion
    List<JointPoint> righthand = new();

    public void FindTransform()//找到对应关节，更换了模型后舍弃，暂时保留
    {
        L_Index1 = L_Hand.Find("L_Index1");
        L_Index2 = L_Index1.Find("L_Index2");
        L_Index3 = L_Index2.Find("L_Index3");

        L_Little1 = L_Hand.Find("L_Little1");
        L_Little2 = L_Little1.Find("L_Little2");
        L_Little3 = L_Little2.Find("L_Little3");

        L_Middle1 = L_Hand.Find("L_Middle1");
        L_Middle2 = L_Middle1.Find("L_Middle2");
        L_Middle3 = L_Middle2.Find("L_Middle3");

        L_Ring1 = L_Hand.Find("L_Ring1");
        L_Ring2 = L_Ring1.Find("L_Ring2");
        L_Ring3 = L_Ring2.Find("L_Ring3");

        L_Thumb1 = L_Hand.Find("L_Thumb1");
        L_Thumb2 = L_Thumb1.Find("L_Thumb2");
        L_Thumb3 = L_Thumb2.Find("L_Thumb3");

    }

    public void FindTransformMethodTwo()//找到对应关节
    {
        //anim = GetComponent<Animator>();

        L_Hand = anim.GetBoneTransform(HumanBodyBones.LeftHand);

        L_Index1 = anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
        L_Index2 = anim.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
        L_Index3 = anim.GetBoneTransform(HumanBodyBones.LeftIndexDistal);

        L_Little1 = anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
        L_Little2 = anim.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
        L_Little3 = anim.GetBoneTransform(HumanBodyBones.LeftLittleDistal);

        L_Middle1 = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        L_Middle2 = anim.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
        L_Middle3 = anim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);

        L_Ring1 = anim.GetBoneTransform(HumanBodyBones.LeftRingProximal);
        L_Ring2 = anim.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
        L_Ring3 = anim.GetBoneTransform(HumanBodyBones.LeftRingDistal);

        L_Thumb1 = anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
        L_Thumb2 = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        L_Thumb3 = anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal);

    }
    public void Bind()
    {
        for (var i = 0; i < Global.handnum; i++)
        {
            var joinpoint = new JointPoint();
            righthand.Add(joinpoint);
        }


        righthand[((int)HandIndex.wrist)].Transform = L_Hand;

        righthand[((int)HandIndex.tcmc)].Transform = L_Thumb1;
        righthand[((int)HandIndex.tmcp)].Transform = L_Thumb2;
        righthand[((int)HandIndex.tdip)].Transform = L_Thumb3;
        righthand[((int)HandIndex.ttip)].Transform = L_Thumb4;

        righthand[((int)HandIndex.ifmcp)].Transform = L_Index1;
        righthand[((int)HandIndex.ifpip)].Transform = L_Index2;
        righthand[((int)HandIndex.ifdip)].Transform = L_Index3;
        righthand[((int)HandIndex.iftip)].Transform = L_Index4;

        righthand[((int)HandIndex.mfmcp)].Transform = L_Middle1;
        righthand[((int)HandIndex.mfpip)].Transform = L_Middle2;
        righthand[((int)HandIndex.mfdip)].Transform = L_Middle3;
        righthand[((int)HandIndex.mftip)].Transform = L_Middle4;

        righthand[((int)HandIndex.rfmcp)].Transform = L_Ring1;
        righthand[((int)HandIndex.rfpip)].Transform = L_Ring2;
        righthand[((int)HandIndex.rfdip)].Transform = L_Ring3;
        righthand[((int)HandIndex.rftip)].Transform = L_Ring4;

        righthand[((int)HandIndex.pmcp)].Transform = L_Little1;
        righthand[((int)HandIndex.ppip)].Transform = L_Little2;
        righthand[((int)HandIndex.pdip)].Transform = L_Little3;
        righthand[((int)HandIndex.ptip)].Transform = L_Little4;


        righthand[((int)HandIndex.tcmc)].Child = righthand[((int)HandIndex.tmcp)];
        righthand[((int)HandIndex.tmcp)].Child = righthand[((int)HandIndex.tdip)];
        // righthand[((int)HandIndex.tmcp)].Parent = righthand[((int)HandIndex.tdip)];
        righthand[((int)HandIndex.tdip)].Child = righthand[((int)HandIndex.ttip)];
        //righthand[((int)HandIndex.tdip)].Parent = righthand[((int)HandIndex.tmcp)];

        righthand[((int)HandIndex.ifmcp)].Child = righthand[((int)HandIndex.ifpip)];
        righthand[((int)HandIndex.ifpip)].Child = righthand[((int)HandIndex.ifdip)];
        righthand[((int)HandIndex.ifpip)].Parent = righthand[((int)HandIndex.ifmcp)];
        righthand[((int)HandIndex.ifdip)].Child = righthand[((int)HandIndex.iftip)];
        righthand[((int)HandIndex.ifdip)].Parent = righthand[((int)HandIndex.ifpip)];

        righthand[((int)HandIndex.mfmcp)].Child = righthand[((int)HandIndex.mfpip)];
        righthand[((int)HandIndex.mfpip)].Child = righthand[((int)HandIndex.mfdip)];
        righthand[((int)HandIndex.mfpip)].Parent = righthand[((int)HandIndex.mfmcp)];
        righthand[((int)HandIndex.mfdip)].Child = righthand[((int)HandIndex.mftip)];
        righthand[((int)HandIndex.mfdip)].Parent = righthand[((int)HandIndex.mfpip)];

        righthand[((int)HandIndex.rfmcp)].Child = righthand[((int)HandIndex.rfpip)];
        righthand[((int)HandIndex.rfpip)].Child = righthand[((int)HandIndex.rfdip)];
        righthand[((int)HandIndex.rfpip)].Parent = righthand[((int)HandIndex.rfmcp)];
        righthand[((int)HandIndex.rfdip)].Child = righthand[((int)HandIndex.rftip)];
        righthand[((int)HandIndex.rfdip)].Parent = righthand[((int)HandIndex.rfpip)];

        righthand[((int)HandIndex.pmcp)].Child = righthand[((int)HandIndex.ppip)];
        righthand[((int)HandIndex.ppip)].Child = righthand[((int)HandIndex.pdip)];
        righthand[((int)HandIndex.ppip)].Parent = righthand[((int)HandIndex.pmcp)];
        righthand[((int)HandIndex.pdip)].Child = righthand[((int)HandIndex.ptip)];
        righthand[((int)HandIndex.pdip)].Parent = righthand[((int)HandIndex.ppip)];


        var rHand = righthand[((int)HandIndex.wrist)];
        var rf = LandmarkUtil.TriangleNormal(rHand.Transform.position, righthand[((int)HandIndex.ifmcp)].Transform.position, righthand[((int)HandIndex.pmcp)].Transform.position);
        //Debug.Log("rf" + rf);

        foreach (var jointPoint in righthand)
        {
            jointPoint.InitJointPoints(rf);
        }

        rHand.InitRotation = rHand.Transform.rotation;
        rHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(righthand[((int)HandIndex.ifmcp)].Transform.position - righthand[(int)HandIndex.pmcp].Transform.position, rf));
        rHand.InverseRotation = rHand.Inverse * rHand.InitRotation;

    }

    private void Awake()
    {
        useHandTrack=PlayerPrefs.GetInt(Global.useHandTrack, 1) > 0 ? true : false;
        if (!useHandTrack)
        {
            this.enabled = false;
            return;
        }
        Bind();

    }

    protected void Start()
    {
        // startpos=InitPos.Instance.startpos;
        // endpos=InitPos.Instance.endpos;
        HolisticMotion.instance.AddRhandObserver(this);
    }
    private void Update()
    {
        if (stable)
        {
            MoveRightHand();
            stable = false;
        }
    }


    public void MoveRightHand()
    {
        for (int i = 0; i < Global.handnum; i++)
        {
            righthand[i].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[i]);
            
            //righthand[i].Kalman(kalmanParamR,kalmanParamQ);
            
            righthand[i].Smooth(lowPassParam);
        }

        var rHand = righthand[((int)HandIndex.wrist)];
        var rf = LandmarkUtil.TriangleNormal(rHand.Pos3D, righthand[((int)HandIndex.ifmcp)].Pos3D, righthand[((int)HandIndex.pmcp)].Pos3D);
        rHand.Transform.rotation = Quaternion.LookRotation(righthand[((int)HandIndex.ifmcp)].Pos3D - righthand[((int)HandIndex.pmcp)].Pos3D, rf) * rHand.InverseRotation;


        for (int i = 1; i < Global.handnum; i++)
        {
            var jointPoint = righthand[i];
            if (jointPoint.Parent != null)
            {
                //var fv=LandmarkUtil.TriangleNormal(jointPoint.Pos3D,jointPoint.Parent.Pos3D,jointPoint.Child.Pos3D);

                //jointPoint.Transform.right=-fromchild;
                //jointPoint.Transform.forward=jointPoint.Parent.Transform.forward;

                //jointPoint.Transform.localEulerAngles=new Vector3(0,0,-angle);
                if (i > 5)
                {
                    float angle = Vector3.Angle(jointPoint.Pos3D - jointPoint.Parent.Pos3D, jointPoint.Child.Pos3D - jointPoint.Pos3D);
                    angle = Mathf.Clamp(angle, 0, 110);
                    //Debug.Log(jointPoint.Transform.name + angle);
                    // var uv = -Vector3.Cross(jointPoint.Child.Pos3D - jointPoint.Pos3D, jointPoint.Parent.Transform.forward);
                    // jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Parent.Transform.forward, uv);\
                    jointPoint.Transform.localEulerAngles = new Vector3(0, 0, -angle);
                }
                else
                {
                    var fv = jointPoint.Pos3D - jointPoint.Parent.Pos3D;
                    jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, fv) * jointPoint.InverseRotation;
                }

            }
            else if (jointPoint.Child != null)
            {
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, rf) * jointPoint.InverseRotation;
                //Debug.Log(jointPoint.Transform.name + Mathf.Abs(jointPoint.Transform.localEulerAngles.y));
                if (Mathf.Abs(jointPoint.Transform.localEulerAngles.y) > 95f && Mathf.Abs(jointPoint.Transform.localEulerAngles.y) < 250f)
                {
                    jointPoint.Transform.localEulerAngles = new Vector3(jointPoint.Transform.localEulerAngles.x, 0, jointPoint.Transform.localEulerAngles.z);
                }
            }
        }
    }
}
