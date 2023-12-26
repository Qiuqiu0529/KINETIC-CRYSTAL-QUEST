using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;

public class EndlessStarTarget : OneTarget
{
    public EndlessStarPlayer runPlayer;
    int laneCount=3;
    float laneWidth=0;
    int laneIndexNow=2;
    
    public UIBlock poseBlock;
    
    
    public new void Start()
    {
        base.Start();
        if(useHolistic)//也许后面会改？
        {
            HolisticMotion.instance.AddPoseObserver(this);
        }
        else
        {
            PoseTrack.instance.AddPoseObserver(this);
            Debug.Log(" PoseTrack.instance.AddPoseObserver(this);");
        }
        laneCount=runPlayer.LandCount();
        laneWidth=1f/laneCount;
    }
   
    public override void ChangeTransform()
    {
        poseBlock.Position.X.Percent = targetJP.Pos3D.x;
        
        poseBlock.Position.Y.Percent = targetJP.Pos3D.y;
        int temp= (int)(targetJP.Pos3D.x/laneWidth)+1;
        if(temp!=laneIndexNow)
        {
            runPlayer.SetLane(temp);
            laneIndexNow=temp;
        }
    }
}