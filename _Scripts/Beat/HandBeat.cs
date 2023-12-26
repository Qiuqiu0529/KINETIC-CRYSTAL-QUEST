using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBeat : TrackTarget
{
    [SerializeField] Transform lhand;
    [SerializeField] Transform rhand;
    float width,height;

    [SerializeField] Transform startpos;
    [SerializeField] Transform endpos;

    JointPoint lhandJP=new JointPoint();
    JointPoint rhandJP=new JointPoint();
    private void Start()
    {

        lhandJP.Transform=lhand;
        rhandJP.Transform=rhand;
        startpos=InitPos.Instance.startpos;
        endpos=InitPos.Instance.endpos;
        width=endpos.position.x-startpos.position.x;
        height=endpos.position.y-startpos.position.y;
        
        if(useHolistic)//也许后面会改？
        {
            HolisticMotion.instance.AddPoseObserver(this);
        }
        else
        {
            PoseTrack.instance.AddPoseObserver(this);
        }
    }
    private void Update()
    {
        if (stable)
        {
            Move();
           // stable = false;
        }
    }

    public void Move()
    {
        lhandJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lwrist)]);
        rhandJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rwrist)]);
        lhandJP.Kalman(kalmanParamR,kalmanParamQ);
        lhandJP.Smooth(lowPassParam);
        rhandJP.Kalman(kalmanParamR,kalmanParamQ);
        rhandJP.Smooth(lowPassParam);
        // Debug.Log(".Pos3D.x" + lhandJP.Pos3D.x);
        // Debug.Log(".Pos3D.y" + lhandJP.Pos3D.y);
        lhand.position= new Vector3( startpos.position.x+lhandJP.Pos3D.x*width,  startpos.position.y+lhandJP.Pos3D.y*height, lhand.position.z);
        rhand.position= new Vector3( startpos.position.x+rhandJP.Pos3D.x*width,  startpos.position.y+rhandJP.Pos3D.y*height, rhand.position.z);
    }


}
