using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Mediapipe;
using System;
using TMPro;

public class DetectNotice : TrackTarget
{
    public static DetectNotice Instance;
    public bool dontUseModeMgr;

    public CapMode capMode;
    [SerializeField] bool preTrack;
    [SerializeField] TMP_Text tMP_Text;
    public MMF_Player outBoxFB, correctFB;
    private int[] requiredKeypoints;

    private int leftKeypoint;
    private int rightKeypoint;
    private int[] upKeypoint;
    private int[] downKeypoint;


    private PoseIndex[] fullBodyKeypoints = {  PoseIndex.nose,PoseIndex.lshoulder, PoseIndex.rshoulder, PoseIndex.lelbow, PoseIndex.relbow, PoseIndex.lwrist,
       PoseIndex.lhip, PoseIndex.rhip, PoseIndex.lknee, PoseIndex.rknee, };
    private PoseIndex[] upperBodyKeypoints = { PoseIndex.nose, PoseIndex.lshoulder, PoseIndex.rshoulder, PoseIndex.lelbow, PoseIndex.relbow, PoseIndex.lwrist, PoseIndex.lhip, PoseIndex.rhip };
    //private PoseIndex[] lowerBodyKeypoints = {  PoseIndex.lhip, PoseIndex.rhip, PoseIndex.lknee, PoseIndex.rknee, PoseIndex.lankle,
    //    PoseIndex.rankle, PoseIndex.lfoot, PoseIndex.rfoot};
    private PoseIndex[] lowerBodyKeypoints = { PoseIndex.lhip, PoseIndex.rhip, PoseIndex.lknee, PoseIndex.rknee, };
    [SerializeField] bool isAnimating;

    private float timer;

    public event Action lostCap;
    public event Action normalCap;

    public GameObject leftNotice, rightNotice, upNotice, downNotice;




    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(!dontUseModeMgr)
        {
            capMode = ModeMgr.Instance.GetCapMode();
        }

        SetKeypoints();
        if (useHolistic)//也许后面会改？
        {
            HolisticMotion.instance.AddPoseObserver(this);
        }
        else
        {
            PoseTrack.instance.AddPoseObserver(this);
        }
    }


    void Update()
    {
        if (!stable)
        {
            return;
        }
        if (!isAnimating)
        {
            bool temp = Detect();
            //Debug.Log(temp);
            if (preTrack != temp)
            {
                isAnimating = true;
                if (temp)
                {
                    normalCap?.Invoke();
                    correctFB.PlayFeedbacks();
                }
                else
                {
                    lostCap?.Invoke();
                    outBoxFB.PlayFeedbacks();
                }

            }
            if (!temp)
            {
                // Debug.Log("lhipVisi"+landmarks[leftKeypoints[0]].Visibility );
                // Debug.Log("lhipPOs"+(1-landmarks[leftKeypoints[0]].X) );


                leftNotice.SetActive(landmarks[leftKeypoint].X>1);//1-x <0 
                rightNotice.SetActive(landmarks[rightKeypoint].X<0);//1-x>1

                upNotice.SetActive(landmarks[upKeypoint[0]].Y+landmarks[upKeypoint[1]].Y<0);// (1-y1)+(1-y2) >1
                downNotice.SetActive(landmarks[downKeypoint[0]].Y+landmarks[downKeypoint[1]].Y>2);//1-y<0

            }

            preTrack = temp;
        }

    }

    public void ResetIsAnim()
    {
        isAnimating = false;
    }

    private void SetKeypoints()
    {
        switch (capMode)
        {
            case CapMode.full:
                requiredKeypoints = new int[fullBodyKeypoints.Length];
                for (int i = 0; i < fullBodyKeypoints.Length; i++)
                {
                    requiredKeypoints[i] = (int)fullBodyKeypoints[i];
                }
                leftKeypoint = (int)PoseIndex.lhip;
                rightKeypoint = (int)PoseIndex.rhip;

                upKeypoint= new int[2];
                upKeypoint[0]=(int)PoseIndex.lshoulder;
                upKeypoint[1]=(int)PoseIndex.rshoulder;

                downKeypoint=new int[2];
                downKeypoint[0]=(int)PoseIndex.lankle;
                downKeypoint[1]=(int)PoseIndex.rankle;


                break;
            case CapMode.up:
                requiredKeypoints = new int[upperBodyKeypoints.Length];
                for (int i = 0; i < upperBodyKeypoints.Length; i++)
                {
                    requiredKeypoints[i] = (int)upperBodyKeypoints[i];
                }

                leftKeypoint = (int)PoseIndex.lhip;
                rightKeypoint = (int)PoseIndex.rhip;

                upKeypoint= new int[2];
                upKeypoint[0]=(int)PoseIndex.lshoulder;
                upKeypoint[1]=(int)PoseIndex.rshoulder;

                downKeypoint=new int[2];
                downKeypoint[0]=(int)PoseIndex.lhip;
                downKeypoint[1]=(int)PoseIndex.rhip;

                break;
            case CapMode.down:
                requiredKeypoints = new int[lowerBodyKeypoints.Length];
                for (int i = 0; i < lowerBodyKeypoints.Length; i++)
                {
                    requiredKeypoints[i] = (int)lowerBodyKeypoints[i];
                }

                leftKeypoint = (int)PoseIndex.lhip;
                rightKeypoint = (int)PoseIndex.rhip;

                upKeypoint= new int[2];
                upKeypoint[0]=(int)PoseIndex.lhip;
                upKeypoint[1]=(int)PoseIndex.rhip;

                downKeypoint=new int[2];
                downKeypoint[0]=(int)PoseIndex.lankle;
                downKeypoint[1]=(int)PoseIndex.rankle;
                break;
        }
        Debug.Log("set key joints");
        Debug.Log(requiredKeypoints);
    }

    public bool Detect()
    {
        foreach (var keypointIndex in requiredKeypoints)
        {
            if (landmarks[keypointIndex].Visibility < 0.3)
            {
                //Debug.Log("Keypoint missing: " + (PoseIndex)keypointIndex);
                if (tMP_Text != null)
                {
                    tMP_Text.text = ((PoseIndex)keypointIndex).ToString() + ": " + landmarks[keypointIndex].Visibility.ToString();
                }
                return false;
            }
        }
        

        return true;
    }

}
