using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

/// <summary>
/// 将播放期间标准模型和动捕模型的关节角度保存成一个时间序列(理论上应该等长)，
/// 在单个动画播放完毕时，调用DTW计算两个序列之间的距离，并用序列的长度进行归一化。
/// (pose: 静止姿势; action: 静止姿势序列构成的动作表示)
/// </summary>
public class TaichiScoreDebuger : MonoBehaviour
{   
    
    private Animator anim;   // 动画控制器

    /// <summary>
    /// 每个关节采用的数据维数
    /// </summary>
    private const int numOfJointsDataDim = 3;

    [Header("需要赋值的模型组件")]
    public Transform[] jointsMaster; // 在此处定义需要进行误差计算的关节
    public Transform[] jointsUser; // 动捕模型中与上述关节对应的Transform组件

    [Header("根关节")]
    public Transform rootJointMaster;
    public Transform rootJointUser;

    [Header("关节表示方式")]
    [SerializeField]
    private DTWAlgorithm.JointCalculationMode jointCalculationMode = DTWAlgorithm.JointCalculationMode.VectorMode;

    [Header("得分")]
    [SerializeField]
    private TMP_Text score_text;

    [Header("master关节数据文本")]
    [SerializeField]
    private TMP_Text master_text;

    [Header("user关节数据文本")]
    [SerializeField]
    private TMP_Text user_text;

    /// <summary>
    /// 各个动作的分数
    /// </summary>
    private List<int> actionScores = new();

    /// <summary>
    /// 关卡总分
    /// </summary>
    private int allScore = 0;

    /// <summary>
    /// 一个关卡内动作的数量
    /// </summary>
    private int totalAnim;

    /// <summary>
    /// 标准模型n个关节,各m个数据的二维数组组成的时间序列
    /// </summary>
    private List<float[,]> masterAction = new(); 
   
    /// <summary>
    /// 动捕模型n个关节,各m个角度的二维数组组成的时间序列
    /// </summary>
    private List<float[,]> userAction = new();

    /// <summary>
    /// 标准模型帧计数器
    /// </summary>
    private int frameCountMaster = 0;

    /// <summary>
    /// 动捕模型帧计数器
    /// </summary>
    private int frameCountUser = 0;

    [Space(15)]
    [Tooltip("每n帧记录一次两个模型的pose,用于评分")]
    [SerializeField] 
    private int poseSaveNum = 10;

    /// <summary>
    /// 允许记录master模型数据
    /// </summary>
    private bool enableRecordMaster = true;

    /// <summary>
    /// 允许记录user模型数据
    /// </summary>
    private bool enableRecordUser= true;

    /// <summary>
    /// 是否暂停播放动画
    /// </summary>
    private bool isPaused = false;


    private void Start()
    {
        anim = GetComponent<Animator>();   // 获取动画控制器组件 

        if (jointsMaster.Length != jointsUser.Length)
        {
            throw new Exception("标准与动捕模型指定的关节数目不同.");
        }     
    }

    private void Update()
    {       

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPaused)
            {
                anim.speed = 1.0f; // 继续播放动画
            }
            else
            {
                anim.speed = 0.0f; // 暂停播放动画
            }

            isPaused = !isPaused; // 切换暂停状态
        }

    }


    /// <summary>
    /// 每n帧记录一次两个模型的pose数据
    /// </summary>
    private void RecordPoses()
    {
        if (enableRecordMaster)
        {
            if (frameCountMaster >= poseSaveNum)
            {
                RecordPoseOfModel(true, jointCalculationMode);
                frameCountMaster = 0;
                float cost = CalSingleFrameScore(DTWAlgorithm.JointCalculationMode.VectorMode);
                if (score_text is not null)
                {
                    score_text.text = cost.ToString();
                }
            }
            else
            {
                frameCountMaster += 1;
            }
        }

        if (enableRecordUser)
        {
            if (frameCountUser >= poseSaveNum)
            {
                RecordPoseOfModel(false, jointCalculationMode);
                frameCountUser = 0;
            }
            else
            {
                frameCountUser += 1;
            }
        }
    }

    /// <summary>
    /// animclip调用，开始记录master
    /// </summary>
    public void StartRecordMaster()
    {
        enableRecordMaster = true;
    }

    /// <summary>
    /// animclip调用，开始记录user
    /// </summary>
    public void StartRecordUser()
    {
        enableRecordUser = true;
    }

    /// <summary>
    /// animclip调用，停止记录master
    /// </summary>
    public void StopRecordMaster()
    {
        enableRecordMaster = false;
    }

    /// <summary>
    /// animclip调用，停止记录user
    /// </summary>
    public void StopRecordUser()
    {
        enableRecordUser = false;
    }


    /// <summary>
    /// 记录标准模型或动捕模型的关节相关数据(角度或者向量)到序列中
    /// </summary>
    /// <param name="isMaster">是标准模型还是动捕模型</param>
    private void RecordPoseOfModel(bool isMaster, DTWAlgorithm.JointCalculationMode mode)
    {
        float[,] pose = new float[4, 3];

        switch (mode)
        {
            case DTWAlgorithm.JointCalculationMode.AngleMode:
                pose = GetAnglePose(isMaster);
                break;
            case DTWAlgorithm.JointCalculationMode.VectorMode:
                pose = GetVectorPose(isMaster);
                break;
        }

        if (isMaster)
        {
            masterAction.Add(pose);
        }
        else
        {
            userAction.Add(pose);  
        }
    }

    private float CalSingleFrameScore(DTWAlgorithm.JointCalculationMode mode)
    {
        float[,] masterPose = new float[4, 3];
        float[,] userPose = new float[4, 3];

        float cost = 0.0f;

        switch (mode)
        {
            case DTWAlgorithm.JointCalculationMode.AngleMode:
                masterPose = GetAnglePose(true);
                userPose = GetAnglePose(false);
                cost = PoseDiffCalculator.CalDiffOf2AnglePoses(
                                        masterPose, userPose);
                break;
            case DTWAlgorithm.JointCalculationMode.VectorMode:
                masterPose = GetVectorPose(true);
                userPose = GetVectorPose(false);
                cost = PoseDiffCalculator.CalDiffOf2VecPoses(
                            masterPose, userPose, 2);
                break;
        }

        if (master_text is not null)
        {
            master_text.text = ArrayToString(masterPose);
        }

        if (user_text is not null)
        {
            user_text.text = ArrayToString(userPose);
        }

        return cost;
    }

    string ArrayToString(float[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        string result = "";

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result += array[i, j].ToString();

                if (j < cols - 1)
                {
                    result += ", ";
                }
            }

            if (i < rows - 1)
            {
                result += "\n";
            }
        }

        return result;
    }


    /// <summary>
    /// 获得关节欧拉角组成的pose
    /// </summary>
    /// <param name="isMaster">是否是标准模型</param>
    /// <returns></returns>
    private float[,] GetAnglePose(bool isMaster)
    {
        float[,] pose = new float[jointsMaster.Length, numOfJointsDataDim];

        int numOfJoints = jointsMaster.Length;

        for (int i = 0; i < numOfJoints; i++)
        {
            float xa;
            float ya;
            float za;

            if (isMaster)
            {
                //Quaternion jointRotation = joints[i].localRotation;
                xa = GetJointLocalEulerAngle(jointsMaster, i, 0);
                ya = GetJointLocalEulerAngle(jointsMaster, i, 1);
                za = GetJointLocalEulerAngle(jointsMaster, i, 2);
            }
            else
            {
                //Quaternion userJointRotation = userJoints[i].localRotation;
                xa = GetJointLocalEulerAngle(jointsUser, i, 0);
                ya = GetJointLocalEulerAngle(jointsUser, i, 1);
                za = GetJointLocalEulerAngle(jointsUser, i, 2);
            }

            pose[i, 0] = xa;
            pose[i, 1] = ya;
            pose[i, 2] = za;
        }

        return pose;
    }

    /// <summary>
    /// 获得关节向量组成的pose
    /// </summary>
    /// <param name="isMaster">是否是标准模型</param>
    /// <returns></returns>
    private float[,] GetVectorPose(bool isMaster)
    {
        // 模型的关节组
        Transform rootJoint;
        Transform[] _joints;
        if (isMaster)
        {
            rootJoint = rootJointMaster;
            _joints = jointsMaster;
        }
        else
        {
            rootJoint = rootJointUser;
            _joints = jointsUser;
        }

        // 用关节坐标相减的方法计算出关节向量，
        // 并以根结点为参考进行旋转复原，从而消除用户侧身带来的模型整体旋转对动作评估的影响
        float[,] vectors = new float[4, 3];

        for (int i = 0; i < 5; i++)
        {
            if (i == 2)
            {
                continue;
            }

            int vectorIndex = i < 2 ? i : i - 1;

            Vector3 start = _joints[i].position;
            Vector3 end = _joints[i + 1].position;
            Vector3 jointVector = end - start;

            if (!isMaster)
            {
                jointVector = GetOriVector3(rootJoint, jointVector);
            }

            for (int j = 0; j < 3; j++)
            {
                vectors[vectorIndex, j] = jointVector[j];
            }
        }
        return vectors;
    }

    private void RestAngleList()
    {
        masterAction.Clear();
        userAction.Clear();
    }

    /// <summary>
    /// 在anim clip最后调用，用于计算用户动作分数
    /// </summary>
    /// <returns></returns>
    public float GetTotalScore()
    {
        float startTime = Time.realtimeSinceStartup;

        //int score = CalScoreOfSingleAnimInMainTh(masterAction, userAction);

        //actionScores.Add(score);

        float elapsedTime = Time.realtimeSinceStartup - startTime;

        Debug.Log("代码执行时间：" + elapsedTime*1e3 + " ms");

        return 1.0f;
    }

    /// <summary>
    /// 用DTW计算用户动作得分
    /// </summary>
    /// <param name="masterAction"></param>
    /// <param name="userAction"></param>
    /// <returns></returns>
    public int CalScoreOfSingleAnimInMainTh(
        List<float[,]> masterAction,
        List<float[,]> userAction)
    {
        float distance = DTWAlgorithm.CalculateDistance(masterAction, userAction, normalize: true, mode: jointCalculationMode);

        print("anim distance is: " + distance.ToString());
        print("masterAction length: "+ masterAction.Count.ToString());
        print("userAction length: " + userAction.Count.ToString());

        // for debug: 保存action到txt文件中
        //SaveActionToFile();

        // 清除action序列
        RestAngleList();

        return (int)distance;
    }

    private void SaveActionToFile()
    {
        SaveListToTxt(masterAction, "masterAction.txt");
        SaveListToTxt(userAction, "userAction.txt");
    }

    private void SaveListToTxt(List<float[,]> dataList, string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        // 检查数据是否存在
        if (dataList == null || dataList.Count == 0)
        {
            Debug.LogWarning("数据为空，无法保存到文件。");
            if (dataList == null)
            {
                Debug.LogWarning("datalist is null");
            }
            return;
        }

        try
        {
            // 打开文件流
            using (StreamWriter sw = new(filePath))
            {
                // 遍历 dataList 中的每个 float[,] 数据
                foreach (float[,] data in dataList)
                {
                    int rowCount = data.GetLength(0);
                    int colCount = data.GetLength(1);

                    // 将每个数据点写入文件
                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < colCount; j++)
                        {
                            sw.Write(data[i, j].ToString() + "\t");
                        }
                        sw.WriteLine();
                    }
                    // 写入一个空行用于分隔数据
                    sw.WriteLine();
                }
            }

            Debug.Log("数据已保存到文件：" + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("保存数据到文件时发生错误：" + e.Message);
        }
    }


    private float GetJointLocalEulerAngle(Transform[] _joints, int _index, int _dimension)
    {
        float angle = _joints[_index].localEulerAngles[_dimension];
        if (angle >= 180.0f)
        {
            angle = 360.0f - angle;
        }
        return angle;
    }



    /// <summary>
    /// 获得旋转之前的坐标数据
    /// </summary>
    /// <param name="bodyTransform">作为基准的关节</param>
    /// <param name="pointAfter">旋转之后的关节坐标</param>
    /// <returns></returns>
    private Vector3 GetOriVector3(Transform bodyTransform, Vector3 pointAfter)
    {
        Quaternion rotationAfter = bodyTransform.rotation;

        // 计算逆旋转
        Quaternion inverseRotation = Quaternion.Inverse(rotationAfter);

        // 通过逆旋转将旋转后的点坐标转换为旋转前的坐标
        Vector3 pointBefore = inverseRotation * pointAfter;

        //Debug.Log("Point before rotation: " + pointBefore);
        return pointBefore;
    }


    
    public void EndGame()
    {
        CountTime.Instance.SetScore((int)allScore);
        CountTime.Instance.SetRank(GetRank());
    }

    public int GetRank()
    {
        if (allScore > totalAnim * 90)//一个动作最高100分
        {
            return 5;
        }
        else if (allScore > totalAnim *80)
        {
            return 4;
        }
        else if (allScore > totalAnim * 60)
        {
            return 3;
        }
        else if (allScore > totalAnim * 40)
        {
            return 2;
        }
        else if (allScore > totalAnim * 20)
        {
            return 1;
        }
        return 0;
    }

    private string ConvertScore2Level(float score)
    {
        string level = "F";
        if (score == 5)
        {
            level = "SS";
        }
        else if (score >= 4)
        {
            level = "S";
        }
        else if (score >= 3)
        {
            level = "A";
        }
        else if (score >= 2)
        {
            level = "B";
        }
        else if (score >= 1)
        {
            level = "C";
        }
        return level;
    }
}


