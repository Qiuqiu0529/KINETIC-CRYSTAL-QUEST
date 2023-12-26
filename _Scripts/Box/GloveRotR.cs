using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveRotR :TrackTarget
{
    [SerializeField] Animator anim;

    #region transformbind

    [SerializeField] Transform J_Bip_R_Hand;
    [SerializeField] Transform J_Bip_R_Index1;//整根手指，食指
    [SerializeField] Transform J_Bip_R_Little1;//小指
    #endregion
    List<JointPoint> righthand = new();



    public void FindTransformMethodTwo()//找到对应关节
    {
        //anim = GetComponent<Animator>();

        J_Bip_R_Hand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        J_Bip_R_Index1 = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);

        J_Bip_R_Little1 = anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
    }
    public void Bind()
    {
        for (var i = 0; i < Global.handnum; i++)
        {
            var joinpoint = new JointPoint();
            righthand.Add(joinpoint);
        }

        righthand[((int)HandIndex.wrist)].Transform = J_Bip_R_Hand;
        righthand[((int)HandIndex.ifmcp)].Transform = J_Bip_R_Index1;
        righthand[((int)HandIndex.pmcp)].Transform = J_Bip_R_Little1;


        var rHand = righthand[((int)HandIndex.wrist)];
        var rf = LandmarkUtil.TriangleNormal(rHand.Transform.position, righthand[((int)HandIndex.ifmcp)].Transform.position, righthand[((int)HandIndex.pmcp)].Transform.position);


        rHand.InitRotation = rHand.Transform.rotation;
        rHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(righthand[((int)HandIndex.ifmcp)].Transform.position - righthand[((int)HandIndex.pmcp)].Transform.position, rf));
        rHand.InverseRotation = rHand.Inverse * rHand.InitRotation;
    }

    private void Awake()
    {
        Bind();
    }

    protected void Start()
    {
        HolisticMotion.instance.AddRhandObserver(this);
    }
    private void Update()
    {
        if (stable)
        {
            Moverighthand();
            stable = false;
        }

    }


    public void Moverighthand()
    {
        
        righthand[((int)HandIndex.wrist)].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)HandIndex.wrist)]);
        righthand[((int)HandIndex.pmcp)].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)HandIndex.pmcp)]);
        righthand[((int)HandIndex.ifmcp)].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)HandIndex.ifmcp)]);
        righthand[((int)HandIndex.wrist)].Kalman(kalmanParamR,kalmanParamQ);//卡曼滤波，数据更稳定
        righthand[((int)HandIndex.wrist)].Smooth(lowPassParam);
        righthand[((int)HandIndex.pmcp)].Kalman(kalmanParamR,kalmanParamQ);//卡曼滤波，数据更稳定
        righthand[((int)HandIndex.pmcp)].Smooth(lowPassParam);
        righthand[((int)HandIndex.ifmcp)].Kalman(kalmanParamR,kalmanParamQ);//卡曼滤波，数据更稳定
        righthand[((int)HandIndex.ifmcp)].Smooth(lowPassParam);

        var rHand = righthand[((int)HandIndex.wrist)];
        var rf = LandmarkUtil.TriangleNormal(rHand.Pos3D, righthand[((int)HandIndex.ifmcp)].Pos3D, righthand[((int)HandIndex.pmcp)].Pos3D);
        rHand.Transform.rotation = Quaternion.LookRotation(righthand[((int)HandIndex.ifmcp)].Pos3D - righthand[((int)HandIndex.pmcp)].Pos3D, rf) * rHand.InverseRotation;
        rHand.Transform.localEulerAngles = new Vector3(rHand.Transform.localEulerAngles.x,rHand.Transform.localEulerAngles.y,-rHand.Transform.localEulerAngles.z);
    }

}
