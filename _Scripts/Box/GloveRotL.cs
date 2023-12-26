using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveRotL : TrackTarget
{
    [SerializeField] Animator anim;

    #region transformbind

    [SerializeField] Transform J_Bip_L_Hand;
    [SerializeField] Transform J_Bip_L_Index1;//整根手指，食指
    [SerializeField] Transform J_Bip_L_Little1;//小指
    #endregion
    List<JointPoint> lefthand = new();



    public void FindTransformMethodTwo()//找到对应关节
    {
        //anim = GetComponent<Animator>();

        J_Bip_L_Hand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        J_Bip_L_Index1 = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);

        J_Bip_L_Little1 = anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
    }
    public void Bind()
    {
        for (var i = 0; i < Global.handnum; i++)
        {
            var joinpoint = new JointPoint();
            lefthand.Add(joinpoint);
        }

        lefthand[((int)HandIndex.wrist)].Transform = J_Bip_L_Hand;
        lefthand[((int)HandIndex.ifmcp)].Transform = J_Bip_L_Index1;
        lefthand[((int)HandIndex.pmcp)].Transform = J_Bip_L_Little1;


        var lHand = lefthand[((int)HandIndex.wrist)];
        var lf = LandmarkUtil.TriangleNormal(lHand.Transform.position, lefthand[((int)HandIndex.pmcp)].Transform.position, lefthand[((int)HandIndex.ifmcp)].Transform.position);
        Debug.Log("lf" + lf);

        lHand.InitRotation = lHand.Transform.rotation;
        lHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(lefthand[((int)HandIndex.ifmcp)].Transform.position - lefthand[((int)HandIndex.pmcp)].Transform.position, lf));
        lHand.InverseRotation = lHand.Inverse * lHand.InitRotation;
    }

    private void Awake()
    {
        Bind();
    }

    protected void Start()
    {
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
        
        lefthand[((int)HandIndex.wrist)].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)HandIndex.wrist)]);
        lefthand[((int)HandIndex.pmcp)].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)HandIndex.pmcp)]);
        lefthand[((int)HandIndex.ifmcp)].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)HandIndex.ifmcp)]);
        lefthand[((int)HandIndex.wrist)].Kalman(kalmanParamR,kalmanParamQ);//卡曼滤波，数据更稳定
        lefthand[((int)HandIndex.wrist)].Smooth(lowPassParam);
        lefthand[((int)HandIndex.pmcp)].Kalman(kalmanParamR,kalmanParamQ);//卡曼滤波，数据更稳定
        lefthand[((int)HandIndex.pmcp)].Smooth(lowPassParam);
        lefthand[((int)HandIndex.ifmcp)].Kalman(kalmanParamR,kalmanParamQ);//卡曼滤波，数据更稳定
        lefthand[((int)HandIndex.ifmcp)].Smooth(lowPassParam);

        var lHand = lefthand[((int)HandIndex.wrist)];
        var lf = LandmarkUtil.TriangleNormal(lHand.Pos3D, lefthand[((int)HandIndex.pmcp)].Pos3D, lefthand[((int)HandIndex.ifmcp)].Pos3D);
        lHand.Transform.rotation = Quaternion.LookRotation(lefthand[((int)HandIndex.ifmcp)].Pos3D - lefthand[((int)HandIndex.pmcp)].Pos3D, lf) * lHand.InverseRotation;
        lHand.Transform.localEulerAngles = new Vector3(lHand.Transform.localEulerAngles.x,lHand.Transform.localEulerAngles.y,-lHand.Transform.localEulerAngles.z);
    }

}
