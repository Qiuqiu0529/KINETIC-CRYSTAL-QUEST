using UnityEngine;

public class PoseDataServer : TrackTarget
{
    private bool upBodyDetected;

    public bool upBodyCaptured
    {
        get { return upBodyDetected; }
    }


    private int[] requiredKeypoints;

    private PoseIndex[] upperBodyKeypoints = { PoseIndex.nose, PoseIndex.lshoulder, PoseIndex.rshoulder, PoseIndex.lelbow, PoseIndex.relbow, PoseIndex.lwrist};

    private void Start()
    {
        PoseTrack.instance.AddPoseObserver(this);
        SetKeypoints();        
    }

    private void Update()
    {
        upBodyDetected = Detect();
    }

    

    public bool IsStable()
    {
        if (stable)
        {
            return true;
        }
        return false;
    }
        

    /// <summary>
    /// 获取关节三维坐标点（归一化屏幕坐标系）
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetJointPosition(int index)
    {
        Vector3 pos;
        try
        {
            switch (index)
            {
                case ((int)PoseIndex.hip):
                    pos = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lhip)]) +
                        LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rhip)])) / 2;
                    break;
                case ((int)PoseIndex.neck):
                    pos = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lshoulder)]) +
                        LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rshoulder)])) / 2;
                    break;
                case ((int)PoseIndex.spine):
                    Vector3 hipPos = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lhip)]) +
                        LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rhip)])) / 2;
                    Vector3 neckPos = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lshoulder)]) +
                        LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rshoulder)])) / 2;
                    pos = hipPos + Vector3.Scale(neckPos - hipPos, new Vector3(0.2f, 0.2f, 0.2f));
                    break;
                default:
                    pos = LandmarkUtil.LandMarkToVector(landmarks[(index)]);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("error when getting jointPosition of joint " + index.ToString() + " " + e);
            pos = Vector3.zero;
        }
        return pos;
    }

    /// <summary>
    /// 检测是否识别到特定的人体关键点
    /// </summary>
    /// <returns></returns>
    public bool Detect()
    {
        if (landmarks == null || landmarks.Count == 0)
        {
            return false;
        }
        foreach (var keypointIndex in requiredKeypoints)
        {
            if (landmarks[keypointIndex].Visibility < 0.3)
            {
                //Debug.Log("Keypoint missing: " + (PoseIndex)keypointIndex);                
                return false;
            }
        }

        return true;
    }

    private void SetKeypoints()
    {
        requiredKeypoints = new int[upperBodyKeypoints.Length];
        for (int i = 0; i < upperBodyKeypoints.Length; i++)
        {
            requiredKeypoints[i] = (int)upperBodyKeypoints[i];
        }        
    }
}

