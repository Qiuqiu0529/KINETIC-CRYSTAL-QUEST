using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class MotionLHand : TrackTarget
{
    float width, height;
    public bool useHandTrack = true;


    public Transform[] targets;

    [SerializeField] Animator anim;

    #region transformbind

    [SerializeField] Transform R_Hand;
    [SerializeField] Transform R_Index1;//整根手指，食指
    [SerializeField] Transform R_Index2;//上半部分
    [SerializeField] Transform R_Index3;//上面一小节\
    [SerializeField] Transform R_Index4;
    [SerializeField] Transform R_Little1;//小指
    [SerializeField] Transform R_Little2;
    [SerializeField] Transform R_Little3;
    [SerializeField] Transform R_Little4;
    [SerializeField] Transform R_Middle1;
    [SerializeField] Transform R_Middle2;
    [SerializeField] Transform R_Middle3;
    [SerializeField] Transform R_Middle4;
    [SerializeField] Transform R_Ring1;
    [SerializeField] Transform R_Ring2;
    [SerializeField] Transform R_Ring3;
    [SerializeField] Transform R_Ring4;
    [SerializeField] Transform R_Thumb1;
    [SerializeField] Transform R_Thumb2;
    [SerializeField] Transform R_Thumb3;
    [SerializeField] Transform R_Thumb4;
    #endregion
    List<JointPoint> lefthand = new();

    public void FindTransform()//找到对应关节，更换了模型后舍弃，暂时保留
    {
        R_Index1 = R_Hand.Find("R_Index1");
        R_Index2 = R_Index1.Find("R_Index2");
        R_Index3 = R_Index2.Find("R_Index3");

        R_Little1 = R_Hand.Find("R_Little1");
        R_Little2 = R_Little1.Find("R_Little2");
        R_Little3 = R_Little2.Find("R_Little3");

        R_Middle1 = R_Hand.Find("R_Middle1");
        R_Middle2 = R_Middle1.Find("R_Middle2");
        R_Middle3 = R_Middle2.Find("R_Middle3");

        R_Ring1 = R_Hand.Find("R_Ring1");
        R_Ring2 = R_Ring1.Find("R_Ring2");
        R_Ring3 = R_Ring2.Find("R_Ring3");

        R_Thumb1 = R_Hand.Find("R_Thumb1");
        R_Thumb2 = R_Thumb1.Find("R_Thumb2");
        R_Thumb3 = R_Thumb2.Find("R_Thumb3");
    }

    public void FindTransformMethodTwo()//找到对应关节
    {
        //anim = GetComponent<Animator>();

        R_Hand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        R_Index1 = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        R_Index2 = anim.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
        R_Index3 = anim.GetBoneTransform(HumanBodyBones.RightIndexDistal);

        R_Little1 = anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
        R_Little2 = anim.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
        R_Little3 = anim.GetBoneTransform(HumanBodyBones.RightLittleDistal);

        R_Middle1 = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        R_Middle2 = anim.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
        R_Middle3 = anim.GetBoneTransform(HumanBodyBones.RightMiddleDistal);

        R_Ring1 = anim.GetBoneTransform(HumanBodyBones.RightRingProximal);
        R_Ring2 = anim.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
        R_Ring3 = anim.GetBoneTransform(HumanBodyBones.RightRingDistal);

        R_Thumb1 = anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);
        R_Thumb2 = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        R_Thumb3 = anim.GetBoneTransform(HumanBodyBones.RightThumbDistal);
    }
    public void Bind()
    {
        for (var i = 0; i < Global.handnum; i++)
        {
            var joinpoint = new JointPoint();
            lefthand.Add(joinpoint);
        }

        lefthand[((int)HandIndex.wrist)].Transform = R_Hand;
        lefthand[((int)HandIndex.tcmc)].Transform = R_Thumb1;
        lefthand[((int)HandIndex.tmcp)].Transform = R_Thumb2;
        lefthand[((int)HandIndex.tdip)].Transform = R_Thumb3;
        lefthand[((int)HandIndex.ttip)].Transform = R_Thumb4;

        lefthand[((int)HandIndex.ifmcp)].Transform = R_Index1;
        lefthand[((int)HandIndex.ifpip)].Transform = R_Index2;
        lefthand[((int)HandIndex.ifdip)].Transform = R_Index3;
        lefthand[((int)HandIndex.iftip)].Transform = R_Index4;

        lefthand[((int)HandIndex.mfmcp)].Transform = R_Middle1;
        lefthand[((int)HandIndex.mfpip)].Transform = R_Middle2;
        lefthand[((int)HandIndex.mfdip)].Transform = R_Middle3;
        lefthand[((int)HandIndex.mftip)].Transform = R_Middle4;

        lefthand[((int)HandIndex.rfmcp)].Transform = R_Ring1;
        lefthand[((int)HandIndex.rfpip)].Transform = R_Ring2;
        lefthand[((int)HandIndex.rfdip)].Transform = R_Ring3;
        lefthand[((int)HandIndex.rftip)].Transform = R_Ring4;

        lefthand[((int)HandIndex.pmcp)].Transform = R_Little1;
        lefthand[((int)HandIndex.ppip)].Transform = R_Little2;
        lefthand[((int)HandIndex.pdip)].Transform = R_Little3;
        lefthand[((int)HandIndex.ptip)].Transform = R_Little4;



        lefthand[((int)HandIndex.tcmc)].Child = lefthand[((int)HandIndex.tmcp)];
        lefthand[((int)HandIndex.tmcp)].Child = lefthand[((int)HandIndex.tdip)];
        lefthand[((int)HandIndex.tdip)].Child = lefthand[((int)HandIndex.ttip)];

        lefthand[((int)HandIndex.ifmcp)].Child = lefthand[((int)HandIndex.ifpip)];
        lefthand[((int)HandIndex.ifpip)].Child = lefthand[((int)HandIndex.ifdip)];
        lefthand[((int)HandIndex.ifpip)].Parent = lefthand[((int)HandIndex.ifmcp)];
        lefthand[((int)HandIndex.ifdip)].Child = lefthand[((int)HandIndex.iftip)];
        lefthand[((int)HandIndex.ifdip)].Parent = lefthand[((int)HandIndex.ifpip)];

        lefthand[((int)HandIndex.mfmcp)].Child = lefthand[((int)HandIndex.mfpip)];
        lefthand[((int)HandIndex.mfpip)].Child = lefthand[((int)HandIndex.mfdip)];
        lefthand[((int)HandIndex.mfpip)].Parent = lefthand[((int)HandIndex.mfmcp)];
        lefthand[((int)HandIndex.mfdip)].Child = lefthand[((int)HandIndex.mftip)];
        lefthand[((int)HandIndex.mfdip)].Parent = lefthand[((int)HandIndex.mfpip)];

        lefthand[((int)HandIndex.rfmcp)].Child = lefthand[((int)HandIndex.rfpip)];
        lefthand[((int)HandIndex.rfpip)].Child = lefthand[((int)HandIndex.rfdip)];
        lefthand[((int)HandIndex.rfpip)].Parent = lefthand[((int)HandIndex.rfmcp)];
        lefthand[((int)HandIndex.rfdip)].Child = lefthand[((int)HandIndex.rftip)];
        lefthand[((int)HandIndex.rfdip)].Parent = lefthand[((int)HandIndex.rfpip)];

        lefthand[((int)HandIndex.pmcp)].Child = lefthand[((int)HandIndex.ppip)];
        lefthand[((int)HandIndex.ppip)].Child = lefthand[((int)HandIndex.pdip)];
        lefthand[((int)HandIndex.ppip)].Parent = lefthand[((int)HandIndex.pmcp)];
        lefthand[((int)HandIndex.pdip)].Child = lefthand[((int)HandIndex.ptip)];
        lefthand[((int)HandIndex.pdip)].Parent = lefthand[((int)HandIndex.ppip)];



        var lHand = lefthand[((int)HandIndex.wrist)];
        var lf = LandmarkUtil.TriangleNormal(lHand.Transform.position, lefthand[((int)HandIndex.pmcp)].Transform.position, lefthand[((int)HandIndex.ifmcp)].Transform.position);
        Debug.Log("lf" + lf);

        foreach (var jointPoint in lefthand)
        {
            jointPoint.InitJointPoints(lf);
        }

        lHand.InitRotation = lHand.Transform.rotation;
        lHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(lefthand[((int)HandIndex.ifmcp)].Transform.position - lefthand[((int)HandIndex.pmcp)].Transform.position, lf));
        lHand.InverseRotation = lHand.Inverse * lHand.InitRotation;
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
        //startpos=startpos.
        // startpos=InitPos.Instance.startpos;
        // endpos=InitPos.Instance.endpos;
        HolisticMotion.instance.AddLhandObserver(this);
    }
    private void Update()
    {
        if (stable)
        {
            MoveLeftHand();
            stable = false;
        }

    }

    public void MoveLeftHand()
    {
        for (int i = 0; i < Global.handnum; i++)
        {
            lefthand[i].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[i]);
            //lefthand[i].Kalman(kalmanParamR,kalmanParamQ);
            lefthand[i].Smooth(lowPassParam);// targets[i].position = startpos.position + new Vector3(lefthand[i].Pos3D.x * width, lefthand[i].Pos3D.y * height, lefthand[i].Pos3D.z * width);
        }

        var lHand = lefthand[((int)HandIndex.wrist)];
        var lf = LandmarkUtil.TriangleNormal(lHand.Pos3D, lefthand[((int)HandIndex.pmcp)].Pos3D, lefthand[((int)HandIndex.ifmcp)].Pos3D);
        lHand.Transform.rotation = Quaternion.LookRotation(lefthand[((int)HandIndex.ifmcp)].Pos3D - lefthand[((int)HandIndex.pmcp)].Pos3D, lf) * lHand.InverseRotation;
        
        for (int i = 1; i < Global.handnum; i++)
        {
            var jointPoint = lefthand[i];
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
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, lf) * jointPoint.InverseRotation;
                if (Mathf.Abs(jointPoint.Transform.localEulerAngles.y) > 95f && Mathf.Abs(jointPoint.Transform.localEulerAngles.y) < 250f)
                {
                    jointPoint.Transform.localEulerAngles = new Vector3(jointPoint.Transform.localEulerAngles.x, 0, jointPoint.Transform.localEulerAngles.z);
                }

            }

        }


        // for (int i = 1; i < Global.handnum; i++)
        // {
        //     var jointPoint = lefthand[i];
        //     if (jointPoint.Parent != null)
        //     {
        //         var fv = jointPoint.Pos3D - jointPoint.Parent.Pos3D;
        //         jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, fv) * jointPoint.InverseRotation;

        //     }
        //     else if (jointPoint.Child != null)
        //     {
        //         jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, lf) * jointPoint.InverseRotation;
        //     }
        // }

        // for (int i = 1; i < Global.handnum; i++)
        // {
        //     var jointPoint = lefthand[i];
        //     if (jointPoint.Parent != null)
        //     {
        //         Debug.Log((i/4+1)*4);
        //        targetpos[i]= lefthand[(i/4+1)*4].Transform.position;
        //     }
        // }

        // for (int n = 0; n < 3; n++)//ccdIK测试
        // {
        //     for (int i = Global.handnum - 1; i > 0; i--)//遍历骨骼
        //     {
        //         var jointPoint = lefthand[i];
        //         if (jointPoint.Parent != null)
        //         {
        //             Vector3 toLastBone = lefthand[(i/4+1)*4].Transform.position - jointPoint.Transform.position;
        //             Vector3 toTarget = jointPoint.Child.Transform.position - jointPoint.Transform.position;
        //             Quaternion targetRotation = Quaternion.FromToRotation(toLastBone, toTarget) * jointPoint.Transform.rotation;
        //             jointPoint.Transform.rotation = targetRotation;
        //         }
        //     }
        // }

    }

}
