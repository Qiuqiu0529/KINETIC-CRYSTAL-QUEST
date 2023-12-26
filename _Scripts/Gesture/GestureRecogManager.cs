using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 姿势识别的方法与参数控制器
/// （tpose/左右划动/上下划动/跳跃蹲下/推拉/跑/张开合拢）
/// </summary>
public class GestureRecogManager : MonoBehaviour
{
    /// <summary>
    /// The gesture types.
    /// </summary>
    public enum Gestures
    {
        None = 0,

        Tpose,

        SwipeLeft,
        SwipeRight,
        SwipeUp,
        SwipeDown,

        Jump,
        Squat,

        Push,
        Pull,

        Run,

        Extend,
        Squash,  
    }

    public interface IGestureListenerInterface
    {
        void RegisterGestures();
        
        void GestureInProgress(Gestures gesture, float progress, PoseIndex joint);

        
        bool GestureCompleted(Gestures gesture, PoseIndex joint);

    
        bool GestureCancelled(Gestures gesture, PoseIndex joint);
    }

    public struct GestureData
    {        
        public Gestures gesture;
        public int state;
        public float timestamp;
        public int joint;
        public Vector3 jointPos;
        public Vector3 screenPos;
        public float tagFloat;
        public Vector3 tagVector;
        public Vector3 tagVector2;
        public float progress;
        public bool complete;
        public bool cancelled;
        public List<Gestures> checkForGestures;
        public float startTrackingAtTime;
    }

    protected int leftHandIndex;
    protected int rightHandIndex;

    protected int leftFingerIndex;
    protected int rightFingerIndex;

    protected int leftElbowIndex;
    protected int rightElbowIndex;

    protected int leftShoulderIndex;
    protected int rightShoulderIndex;

    protected int hipCenterIndex;
    protected int shoulderCenterIndex;

    protected int leftHipIndex;
    protected int rightHipIndex;

    protected int leftKneeIndex;
    protected int rightKneeIndex;

    protected int leftAnkleIndex;
    protected int rightAnkleIndex;

    protected float squashLength;

    public virtual int[] GetNeededJointIndexes(GestureControlManager manager)
    {
        //		if (manager == null)
        //			return new int[0];

        leftHandIndex = (int)PoseIndex.lwrist;
        rightHandIndex = (int)PoseIndex.rwrist;

        leftFingerIndex = (int)PoseIndex.lindex;
        rightFingerIndex = (int)PoseIndex.rindex;

        leftElbowIndex = (int)PoseIndex.lelbow;
        rightElbowIndex = (int)PoseIndex.relbow;

        leftShoulderIndex = (int)PoseIndex.lshoulder;
        rightShoulderIndex = (int)PoseIndex.rshoulder;

        hipCenterIndex = (int)PoseIndex.hip;
        shoulderCenterIndex = (int)PoseIndex.neck;

        leftHipIndex = (int)PoseIndex.lhip;
        rightHipIndex = (int)PoseIndex.rhip;

        leftKneeIndex = (int)PoseIndex.lknee;
        rightKneeIndex = (int)PoseIndex.rknee;

        leftAnkleIndex = (int)PoseIndex.lankle;
        rightAnkleIndex = (int)PoseIndex.rankle;

        int[] neededJointIndexes = {
            leftHandIndex, rightHandIndex, leftFingerIndex, rightFingerIndex, leftElbowIndex, rightElbowIndex, leftShoulderIndex, rightShoulderIndex,
            hipCenterIndex, shoulderCenterIndex, leftHipIndex, rightHipIndex, leftKneeIndex, rightKneeIndex, leftAnkleIndex, rightAnkleIndex
        };

        return neededJointIndexes;
    }

    protected void SetGestureJoint(ref GestureData gestureData, float timestamp, int joint, Vector3 jointPos)
    {
        gestureData.joint = joint;
        gestureData.jointPos = jointPos;
        gestureData.timestamp = timestamp;
        gestureData.state++;
    }

    protected void SetGestureCancelled(ref GestureData gestureData)
    {
        gestureData.state = 0;
        gestureData.progress = 0f;
        gestureData.cancelled = true;
        // print(string.Format("gesture {0} cancelled", gestureData.gesture));
    }

    protected void CheckPoseComplete(ref GestureData gestureData, float timestamp, Vector3 jointPos, bool isInPose, float durationToComplete)
    {
        if (isInPose)
        {
            float timeLeft = timestamp - gestureData.timestamp;
            gestureData.progress = durationToComplete > 0f ? Mathf.Clamp01(timeLeft / durationToComplete) : 1.0f;

            if (timeLeft >= durationToComplete)
            {
                gestureData.timestamp = timestamp;
                gestureData.jointPos = jointPos;
                gestureData.state++;
                gestureData.complete = true;
                print(string.Format("gesture {0} complete", gestureData.gesture));
            }
        }
        else
        {
            SetGestureCancelled(ref gestureData);
        }
    }


    public virtual void CheckForGesture(ref GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked)
    {
        //print("check " + gestureData.gesture);
        if (gestureData.complete)
            return;

        float bandTopY = jointsPos[rightShoulderIndex].y > jointsPos[leftShoulderIndex].y ? jointsPos[rightShoulderIndex].y : jointsPos[leftShoulderIndex].y;
        float bandBotY = jointsPos[rightHipIndex].y < jointsPos[leftHipIndex].y ? jointsPos[rightHipIndex].y : jointsPos[leftHipIndex].y;

        float bandCenter = (bandTopY + bandBotY) / 2f;
        float bandSize = (bandTopY - bandBotY);

        float gestureTop = bandCenter + bandSize * 1.2f / 2f;
        float gestureBottom = bandCenter - bandSize * 1.3f / 4f;
        float gestureRight = jointsPos[rightHipIndex].x;
        float gestureLeft = jointsPos[leftHipIndex].x;

        float gestureSize = gestureRight - gestureLeft;

        switch (gestureData.gesture)
        {
            // check for Tpose
            case Gestures.Tpose:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex] &&
                            Mathf.Abs(jointsPos[rightElbowIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.07f
                            Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
                            jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex] &&
                            Mathf.Abs(jointsPos[leftElbowIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f &&
                            Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex] &&
                            Mathf.Abs(jointsPos[rightElbowIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
                            Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
                            jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex] &&
                            Mathf.Abs(jointsPos[leftElbowIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f &&
                            Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, Global.Constants.PoseCompleteDuration);
                        break;
                }
                break;

            // check for SwipeLeft
            case Gestures.SwipeLeft:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                               jointsPos[rightHandIndex].x >= gestureRight /**&& jointsPos[rightHandIndex].x > gestureLeft*/)
                        {
                            print("swipeleft state 0");
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.1f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        print("swipeleft state 1");
                        if ((timestamp - gestureData.timestamp) <= 1.0f)
                        {
                            bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                                    jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                                    jointsPos[rightHandIndex].x <= (gestureLeft + (gestureRight - gestureLeft) * 0.4);

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                            else if (jointsPos[rightHandIndex].x <= gestureRight)
                            {                                
                                gestureData.progress = gestureSize > 0.01f ? (gestureRight - jointsPos[rightHandIndex].x) / gestureSize : 0f;
                            }

                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for SwipeRight
            case Gestures.SwipeRight:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                               jointsPos[leftHandIndex].x <= gestureLeft /**&& jointsPos[leftHandIndex].x < gestureRight*/)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.1f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) <= 1.0f)
                        {
                            bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                                    jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                                    jointsPos[leftHandIndex].x >= (gestureRight - (gestureRight - gestureLeft) * 0.4);

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                            else if (jointsPos[leftHandIndex].x >= gestureLeft)
                            {                                
                                gestureData.progress = gestureSize > 0.01f ? (jointsPos[leftHandIndex].x - gestureLeft) / gestureSize : 0f;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for SwipeUp
            case Gestures.SwipeUp:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) < -0.0f &&
                           (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.15f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        // disable左手上滑
                        // else if (jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] &&
                        //         (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) < -0.0f &&
                        //         (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.15f)
                        // {
                        //     SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                        //     gestureData.progress = 0.5f;
                        // }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.05f &&
                                Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) <= 0.15f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.05f &&
                                Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) <= 0.15f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for SwipeDown
            case Gestures.SwipeDown:
                switch (gestureData.state)
                {
                    case 0:
                        // disable右手下滑
                        // gesture detection - phase 1
                        // if (jointsTracked[rightHandIndex] && jointsTracked[leftShoulderIndex] &&
                        //    (jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) >= 0.05f)
                        // {
                        //     SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        //     gestureData.progress = 0.5f;
                        // }
                        if (jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) >= 0.05f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) < -0.15f &&
                                Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) <= 0.15f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) < -0.15f &&
                                Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) <= 0.15f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;


            case Gestures.Extend:
                Vector3 vectorExtend = (Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
                float distExtend = vectorExtend.magnitude;
                // print(string.Format("两手之间的距离：{0}",distExtend));


                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1

                        if (jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           distExtend <= 0.15f)
                        {
                            // print("extend-state0");
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.tagVector = Vector3.right;
                            gestureData.tagFloat = distExtend;
                            gestureData.progress = 0.1f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) <= 1.0f)
                        {
                            bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           distExtend >= 0.45f && jointsPos[rightHandIndex].x >= gestureRight && jointsPos[leftHandIndex].x <= gestureLeft;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                            else if (jointsPos[rightHandIndex].x <= gestureRight)
                            {                                
                                gestureData.progress = gestureSize > 0.01f ? (gestureRight - jointsPos[rightHandIndex].x) / gestureSize : 0f;
                            }

                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            case Gestures.Squash:
                Vector3 vectorSquash = (Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
                float distSquash = vectorSquash.magnitude;

                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        squashLength = PlayerPrefs.GetFloat(Global.squashGestureLength, 0.35f);

                        if (jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           distSquash >= squashLength)
                        {

                            // print("Squash-state0");
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.tagVector = Vector3.right;
                            gestureData.tagFloat = distSquash;
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) <= 1.0f)
                        {
                            bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           distSquash <= 0.2f && jointsPos[rightHandIndex].x <= gestureRight && jointsPos[leftHandIndex].x >= gestureLeft;

                            if (isInPose)
                            {
                                print("squash in pose");
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                            else if (jointsPos[rightHandIndex].x <= gestureRight + (gestureRight - gestureLeft) * 0.7f)
                            {                                
                                gestureData.progress = gestureSize > 0.01f ? (gestureRight + gestureSize * 0.7f - jointsPos[rightHandIndex].x) / gestureSize : 0f;
                            }

                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Jump
            case Gestures.Jump:
                //print(jointsPos[shoulderCenterIndex].y);
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[shoulderCenterIndex] &&
                            (jointsPos[shoulderCenterIndex].y > 0.3f) && (jointsPos[shoulderCenterIndex].y < 0.9f))
                        {
                            //print("jump state 0");
                            SetGestureJoint(ref gestureData, timestamp, shoulderCenterIndex, jointsPos[shoulderCenterIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[shoulderCenterIndex] &&
                                (jointsPos[shoulderCenterIndex].y - gestureData.jointPos.y) > 0.18f &&
                                Mathf.Abs(jointsPos[shoulderCenterIndex].x - gestureData.jointPos.x) < 0.2f;

                            if (isInPose)
                            {
                                //print("is in pose_________________");
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Squat
            case Gestures.Squat:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[shoulderCenterIndex] &&
                            (jointsPos[shoulderCenterIndex].y <= 0.7f))
                        {
                            SetGestureJoint(ref gestureData, timestamp, shoulderCenterIndex, jointsPos[shoulderCenterIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[shoulderCenterIndex] &&
                                (jointsPos[shoulderCenterIndex].y - gestureData.jointPos.y) < -0.15f &&
                                Mathf.Abs(jointsPos[shoulderCenterIndex].x - gestureData.jointPos.x) < 0.2f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Push
            case Gestures.Push:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                               (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                               Mathf.Abs(jointsPos[rightHandIndex].x - jointsPos[rightShoulderIndex].x) < 0.2f &&
                               (jointsPos[rightHandIndex].z - jointsPos[leftElbowIndex].z) < -0.2f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Mathf.Abs(jointsPos[leftHandIndex].x - jointsPos[leftShoulderIndex].x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - jointsPos[rightElbowIndex].z) < -0.2f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                                Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[rightHandIndex].z - gestureData.jointPos.z) < -0.2f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - gestureData.jointPos.z) < -0.2f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Pull
            case Gestures.Pull:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                           Mathf.Abs(jointsPos[rightHandIndex].x - jointsPos[rightShoulderIndex].x) < 0.2f &&
                           (jointsPos[rightHandIndex].z - jointsPos[leftElbowIndex].z) < -0.3f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Mathf.Abs(jointsPos[leftHandIndex].x - jointsPos[leftShoulderIndex].x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - jointsPos[rightElbowIndex].z) < -0.3f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                                Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[rightHandIndex].z - gestureData.jointPos.z) > 0.25f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - gestureData.jointPos.z) > 0.25f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;


            case Gestures.Run:

                float runDifThd = PlayerPrefs.GetFloat(Global.ballRunKneeDis, 0.03f);
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                             // check if the left knee is up
                        if (jointsTracked[leftKneeIndex] && jointsTracked[rightKneeIndex] &&
                           (jointsPos[leftKneeIndex].y - jointsPos[rightKneeIndex].y) > runDifThd)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftKneeIndex, jointsPos[leftKneeIndex]);
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture complete
                        if ((timestamp - gestureData.timestamp) < 1.0f)
                        {
                            // check if the right knee is up
                            bool isInPose = jointsTracked[rightKneeIndex] && jointsTracked[leftKneeIndex] &&
                                (jointsPos[rightKneeIndex].y - jointsPos[leftKneeIndex].y) > runDifThd;

                            if (isInPose)
                            {
                                // go to state 2
                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.7f;
                                gestureData.state = 2;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;

                    case 2:  // gesture complete
                        if ((timestamp - gestureData.timestamp) < 1.0f)
                        {
                            // check if the left knee is up again
                            bool isInPose = jointsTracked[leftKneeIndex] && jointsTracked[rightKneeIndex] &&
                                (jointsPos[leftKneeIndex].y - jointsPos[rightKneeIndex].y) > runDifThd;

                            if (isInPose)
                            {
                                // go back to state 1
                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.8f;
                                gestureData.state = 1;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;
        }
    }

}
