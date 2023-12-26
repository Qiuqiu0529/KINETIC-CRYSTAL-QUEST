using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GestureControlManager : MonoBehaviour
{
    protected static GestureControlManager instance = null;

    public List<GestureRecogManager.GestureData> playerGesturesData = new();

    protected float gesturesTrackingAtTime = new();

    public GestureRecogManager gestureRecogManager;

    public List<MonoBehaviour> gestureListeners = new();

    [Tooltip("Minimum time between gesture detections (in seconds).")]
    public float minTimeBetweenGestures = 0.7f;

    public PoseDataServer poseDataServer;

    private const int JOINT_COUNT = 38;

    public static GestureControlManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FindGesListeners();
        RegisterListeners();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGestures();

        for (int g = 0; g < playerGesturesData.Count; g++)
        { 
            GestureRecogManager.GestureData gestureData = playerGesturesData[g];

            if (gestureData.complete)
            {
                foreach (GestureRecogManager.IGestureListenerInterface listener in gestureListeners)
                {         
                    if (listener != null && listener.GestureCompleted(gestureData.gesture, (PoseIndex)gestureData.joint))
                    {
                        ResetPlayerGestures();
                    }
                }
            }
            else if (gestureData.cancelled)
            {
                foreach (GestureRecogManager.IGestureListenerInterface listener in gestureListeners)
                {
                    if (listener != null && listener.GestureCancelled(gestureData.gesture, (PoseIndex)gestureData.joint))
                    {
                        ResetGesture(gestureData.gesture);
                    }
                }
            }
            else if (gestureData.progress >= 0.1f)
            {
                foreach (GestureRecogManager.IGestureListenerInterface listener in gestureListeners)
                {
                    if (listener != null)
                    {
                        listener.GestureInProgress(gestureData.gesture, gestureData.progress,
                                                   (PoseIndex)gestureData.joint);
                    }
                }
            }
        }
    }

    private void RegisterListeners()
    {
        foreach (GestureRecogManager.IGestureListenerInterface listener in gestureListeners)
        {
            if (listener != null)
            {
                listener.RegisterGestures();
            }
        }
    }

    /// <summary>
    /// 找到场景中所有的listener
    /// </summary>
    private void FindGesListeners()
    {
        // try to automatically use the available gesture listeners in the scene
        if (gestureListeners.Count == 0)
        {
            MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

            foreach (MonoBehaviour monoScript in monoScripts)
            {                
                if ((monoScript is GestureRecogManager.IGestureListenerInterface) && monoScript.enabled)
                {
                    gestureListeners.Add(monoScript);
                }
            }
        }
    }


    private void CheckForGestures()
    {
        if (!gestureRecogManager)
            return;

        // check for gestures
        if (Time.realtimeSinceStartup >= gesturesTrackingAtTime)
        {
            // get joint positions and tracking
            int iAllJointsCount = JOINT_COUNT;
            bool[] playerJointsTracked = new bool[iAllJointsCount];
            for (int i = 0; i < playerJointsTracked.Count(); i++)
            {
                playerJointsTracked[i] = true;
            }

            Vector3[] playerJointsPos = new Vector3[iAllJointsCount];

            // get joint pos and track state
            int[] aiNeededJointIndexes = gestureRecogManager.GetNeededJointIndexes(instance);
            int iNeededJointsCount = aiNeededJointIndexes.Length;

            for (int i = 0; i < iNeededJointsCount; i++)
            {
                int joint = aiNeededJointIndexes[i];
                //print(string.Format("{0}th joint is number {1}", i, joint));

                if (joint >= 0)
                {
                    playerJointsPos[joint] = poseDataServer.GetJointPosition(joint);
                }
            }


            // check for gestures
            List<GestureRecogManager.GestureData> gesturesData = playerGesturesData;

            int listGestureSize = gesturesData.Count;
            float timestampNow = Time.realtimeSinceStartup;

            for (int g = 0; g < listGestureSize; g++)
            {
                GestureRecogManager.GestureData gestureData = gesturesData[g];

                if ((timestampNow >= gestureData.startTrackingAtTime) &&
                    !IsConflictingGestureInProgress(gestureData, ref gesturesData))
                {
                    gestureRecogManager.CheckForGesture(ref gestureData, Time.realtimeSinceStartup,
                        ref playerJointsPos, ref playerJointsTracked);
                    gesturesData[g] = gestureData;

                    if (gestureData.complete)
                    {
                        gesturesTrackingAtTime = timestampNow + minTimeBetweenGestures;
                    }
                }
            }

            playerGesturesData = gesturesData;


        }
    }

    private bool IsConflictingGestureInProgress(GestureRecogManager.GestureData gestureData, ref List<GestureRecogManager.GestureData> gesturesData)
    {
        foreach (GestureRecogManager.Gestures gesture in gestureData.checkForGestures)
        {
            int index = GetGestureIndex(gesture, ref gesturesData);

            if (index >= 0)
            {
                if (gesturesData[index].progress > 0f)
                {
                    Debug.LogWarning(gestureData.gesture + " conflict: " + gesturesData[index].gesture);
                    return true;
                }
            }
        }

        return false;
    }

    private int GetGestureIndex(GestureRecogManager.Gestures gesture, ref List<GestureRecogManager.GestureData> gesturesData)
    {
        int listSize = gesturesData.Count;

        for (int i = 0; i < listSize; i++)
        {
            if (gesturesData[i].gesture == gesture)
                return i;
        }

        return -1;
    }


    public bool ResetGesture(GestureRecogManager.Gestures gesture)
    {
        List<GestureRecogManager.GestureData> gesturesData = playerGesturesData;
        int index = gesturesData != null ? GetGestureIndex(gesture, ref gesturesData) : -1;
        if (index < 0)
            return false;

        GestureRecogManager.GestureData gestureData = gesturesData[index];

        gestureData.state = 0;
        gestureData.joint = 0;
        gestureData.progress = 0f;
        gestureData.complete = false;
        gestureData.cancelled = false;
        gestureData.startTrackingAtTime = Time.realtimeSinceStartup + Global.Constants.MinTimeBetweenSameGestures;

        gesturesData[index] = gestureData;
        playerGesturesData = gesturesData;
        return true;
    }


    public void ResetPlayerGestures()
    {        
        List<GestureRecogManager.GestureData> gesturesData = playerGesturesData;

        if (gesturesData != null)
        {
            int listSize = gesturesData.Count;

            for (int i = 0; i < listSize; i++)
            {
                ResetGesture(gesturesData[i].gesture);
            }
        }
    }

    public bool DeleteGesture(GestureRecogManager.Gestures gesture)
    {
        List<GestureRecogManager.GestureData> gesturesData = playerGesturesData;
        int index = gesturesData != null ? GetGestureIndex(gesture, ref gesturesData) : -1;
        if (index < 0)
            return false;

        print("delete gesture: " + gesturesData[index]);
        gesturesData.RemoveAt(index);
        playerGesturesData = gesturesData;

        return true;
    }

    /// <summary>
	/// Adds a gesture to the list of detected gestures for the specified user.
	/// </summary>
	/// <param name="UserId">User ID</param>
	/// <param name="gesture">Gesture type</param>
	public void DetectGesture(GestureRecogManager.Gestures gesture)
    {
        List<GestureRecogManager.GestureData> gesturesData = playerGesturesData;
        int index = GetGestureIndex(gesture, ref gesturesData);

        if (index >= 0)
        {
            DeleteGesture(gesture);
        }

        GestureRecogManager.GestureData gestureData = new()
        {
            gesture = gesture,
            state = 0,
            joint = 0,
            progress = 0f,
            complete = false,
            cancelled = false,

            checkForGestures = new List<GestureRecogManager.Gestures>()
        };

        switch (gesture)
        {
            //case GestureRecogManager.Gestures.SwipeLeft:
            //    gestureData.checkForGestures.Add(GestureRecogManager.Gestures.SwipeRight);
            //    break;

            //case GestureRecogManager.Gestures.SwipeRight:
            //    gestureData.checkForGestures.Add(GestureRecogManager.Gestures.SwipeLeft);
            //    break;

            case GestureRecogManager.Gestures.Extend:
                gestureData.checkForGestures.Add(GestureRecogManager.Gestures.Squash);
                break;

            case GestureRecogManager.Gestures.Squash:
                gestureData.checkForGestures.Add(GestureRecogManager.Gestures.Extend);
                break;
        }

        gesturesData.Add(gestureData);
        playerGesturesData = gesturesData;
    }

}

