using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Nova;
public class MotionUIControl : TrackTarget
{
    [SerializeField] Transform lhand;
    [SerializeField] Transform rhand;

    JointPoint lhandJP = new JointPoint();
    JointPoint rhandJP = new JointPoint();

    public UIBlock lhandBlock;
    public UIBlock rhandBlock;
    bool normalInControl = true;
    public MainUIMgr mainUIMgr;

    public GameObject gestureMgr;

    private void Start()
    {
        lhandJP.Transform = lhand;
        rhandJP.Transform = rhand;
        PoseTrack.instance.AddPoseObserver(this);
        mainUIMgr = MainUIMgr.Instance;
        mainUIMgr.startTransing += DisableHandWhenTransing;
        mainUIMgr.endTransing += ResetHand;
    }

    private void OnDisable()
    {
        if (mainUIMgr != null)
        {
            mainUIMgr.startTransing -= DisableHandWhenTransing;
            mainUIMgr.endTransing -= ResetHand;

        }
    }
    private void Update()
    {
        if (stable && gestureMgr.GetComponent<PoseDataServer>().upBodyCaptured 
        && !TutorialUIController.Instance.IsTutorialPanelActive())
        {
            Move();
            //stable = false;
            if (normalInControl)
            {
                //Debug.Log("detect");
                if (PlanetGestureListener.Instance.IsSwipeLeft())
                {
                    mainUIMgr.MoveUILeft();
                }
                else if (PlanetGestureListener.Instance.IsSwipeRight())
                {
                    mainUIMgr.MoveUIRight();
                }

                if (PlanetGestureListener.Instance.IsExtend())
                {
                    mainUIMgr.CancleUIButton();

                }
                else if (PlanetGestureListener.Instance.IsSquash())
                {
                    mainUIMgr.SelectUIButton();
                }
            }
        }
    }
    public void DisableHandWhenTransing()
    {
        normalInControl = false;
        lhand.gameObject.SetActive(false);
        rhand.gameObject.SetActive(false);
    }

    public void ResetHand()
    {
        lhand.gameObject.SetActive(true);
        rhand.gameObject.SetActive(true);
        normalInControl = true;
    }

    public void Move()
    {
        lhandJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lwrist)]);
        rhandJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rwrist)]);
        lhandJP.Kalman(kalmanParamR, kalmanParamQ);
        lhandJP.Smooth(lowPassParam);
        rhandJP.Kalman(kalmanParamR, kalmanParamQ);
        rhandJP.Smooth(lowPassParam);

        // Debug.Log("lhandJP.pos3d" + lhandJP.Pos3D.x);
        lhandBlock.Position.X.Percent = lhandJP.Pos3D.x;
        // Debug.Log("lhandJP.pos3d" + lhandJP.Pos3D.y);
        lhandBlock.Position.Y.Percent = lhandJP.Pos3D.y;

        rhandBlock.Position.X.Percent = rhandJP.Pos3D.x;
        rhandBlock.Position.Y.Percent = rhandJP.Pos3D.y;

    }




}
