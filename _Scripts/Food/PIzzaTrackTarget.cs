using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIzzaTrackTarget :TrackTarget
{
    public bool init;
    [SerializeField] Transform lhand;
    [SerializeField] Transform rhand;
    [SerializeField] Transform pizza;
    float width,height;

    [SerializeField] Transform startpos;
    [SerializeField] Transform endpos;

    JointPoint lhandJP=new JointPoint();
    JointPoint rhandJP=new JointPoint();
    JointPoint pizzaJP=new JointPoint();

    private void Start()
    {

        //lhandJP.Transform=lhand;
        rhandJP.Transform=rhand;
        pizzaJP.Transform=pizza;
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
    public void Init()
    {
        init=true;
    }
    private void Update()
    {
        if(!init)
        {
            return;
        }

        if (stable)
        {
            Move();
           // stable = false;
        }
    }

    public void Move()
    {
        pizzaJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lhip)]);///待改

        // lhandJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lindex)]);
        rhandJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rindex)]);
       // lhandJP.Kalman(kalmanParamR,kalmanParamQ);
        //lhandJP.Smooth(lowPassParam);
        rhandJP.Kalman(kalmanParamR,kalmanParamQ);
        rhandJP.Smooth(lowPassParam);
       // lhand.position= new Vector3( startpos.position.x+lhandJP.Pos3D.x*width,  startpos.position.y+lhandJP.Pos3D.y*height, lhand.position.z);
        rhand.position= new Vector3( startpos.position.x+rhandJP.Pos3D.x*width,  startpos.position.y+rhandJP.Pos3D.y*height, rhand.position.z);
        pizza.position = new Vector3( startpos.position.x+pizzaJP.Pos3D.x*width,  pizza.position.y, pizza.position.z);
        if(pizza.position.y<-3.41f)
        {
            pizza.position = new Vector3( pizza.position.x,  -3.41f, pizza.position.z);
        }

    }


}
