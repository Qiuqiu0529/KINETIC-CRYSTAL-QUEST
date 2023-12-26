using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glove :   TrackTarget //vrm模型的骨骼物体，加到vrm根物体上
{
    public List<JointPoint> jointPoints = new();
    [SerializeField] Vector3 initPosition;
    public Vector3 forward;
    public float zScale = 0.8f;


    [SerializeField] Animator anim;
    int lwrist,rwrist;

    private void Awake()
    {
        Bind();
    }

    #region transformbind

    [SerializeField] Transform J_Bip_L_Hand;

    [SerializeField] Transform J_Bip_R_Hand;

    #endregion
 [SerializeField] Transform startpos;
    [SerializeField] Transform endpos;
    float width, height;

    public Transform[] targets;

    private void Start()
    {
        if (useHolistic)
        {
            HolisticMotion.instance.AddPoseObserver(this);

        }
        else
        {
            PoseTrack.instance.AddPoseObserver(this);
        }
        startpos = InitPos.Instance.startpos;
        endpos = InitPos.Instance.endpos;
        width = endpos.position.x - startpos.position.x;
        height = endpos.position.y - startpos.position.y;
       
    }

    public void Bind()
    {
        for (var i = 0; i < Global.poseLandmarks; i++)
        {
            var joinpoint = new JointPoint();
            jointPoints.Add(joinpoint);
        }
        lwrist=((int)PoseIndex.lwrist);
        rwrist=((int)PoseIndex.rwrist);

        jointPoints[lwrist].Transform = J_Bip_L_Hand;

        jointPoints[rwrist].Transform = J_Bip_R_Hand;
        


    }


    public void FindTransformMethodTwo()//找到对应关节
    {

        J_Bip_L_Hand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        J_Bip_R_Hand = anim.GetBoneTransform(HumanBodyBones.RightHand);
    }

    private void Update()
    {
        if (stable)
        {
            Move();
            stable = false;
        }
    }

    public void Move()
    {
        
        jointPoints[lwrist].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[lwrist]);
        jointPoints[rwrist].Pos3D = LandmarkUtil.LandMarkToVector(landmarks[rwrist]);

        jointPoints[lwrist].Kalman(kalmanParamR, kalmanParamQ);//卡曼滤波，数据更稳定
        jointPoints[lwrist].Smooth(lowPassParam);
        jointPoints[rwrist].Kalman(kalmanParamR, kalmanParamQ);//卡曼滤波，数据更稳定
        jointPoints[rwrist].Smooth(lowPassParam);
        
        jointPoints[lwrist].Transform.position=  new Vector3(startpos.position.x+jointPoints[lwrist].Pos3D.x*width,
        startpos.position.y+jointPoints[lwrist].Pos3D.y*height,-jointPoints[lwrist].Pos3D.z*width);
        jointPoints[rwrist].Transform.position= new Vector3(startpos.position.x+jointPoints[rwrist].Pos3D.x*width,
          startpos.position.y+jointPoints[rwrist].Pos3D.y*height,-jointPoints[rwrist].Pos3D.z*width);
        
        

    }
}