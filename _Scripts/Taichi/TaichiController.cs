using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

public class TaichiController : MonoBehaviour
{

    [SerializeField] float speed;
    public ClolorChange change;

    public float width, height;
    
    [SerializeField] Transform startpos;
    [SerializeField] Transform endpos;

    /// <summary>
    /// animClip控制器
    /// </summary>
    private Animator anim;

    /// <summary>
    /// 每个关节采用的数据维数
    /// </summary>
    private const int numOfJointsDataDim = 3;

    [Header("master模型上的关节组件")]
    public Transform[] masterJoints;

    [Header("太极数据库")]
    public TaichiActionDB actionDB;

    [Header("user模型上的关节组件")]
    public Transform[] userJoints;

    [Header("根关节")]
    public Transform rootJointMaster;
    public Transform rootJointUser;

    [Header("关节表示方式")]
    [SerializeField]
    private DTWAlgorithm.JointCalculationMode jointCalculationMode = DTWAlgorithm.JointCalculationMode.VectorMode;

    public TMP_Text actionNameText;
    public TMP_Text actionNameText1;
    
    public TMP_Text finalscoretext,totalActiontext,correctActionText,wrongActionText;
    public TMP_Text scoretext;


    float multiplier=1;
    DetectNotice detectNotice;

    /// <summary>
    /// 动作得分(0~5)组成的序列
    /// </summary>
    private List<int> animScores = new();

    public void LostCap()
    {
        multiplier=0.5f;
    }

    public void NormalCap()
    {
        multiplier=1f;
    }
    private void OnEnable() {
        detectNotice=DetectNotice.Instance;
        detectNotice.lostCap+=LostCap;
        detectNotice.normalCap+=NormalCap;
    }
    private void OnDisable() {
        detectNotice.lostCap-=LostCap;
        detectNotice.normalCap-=NormalCap;
    }


    public void SetScoreText()
    {
        scoretext.text=((int)sumOfAnimScores).ToString();
    }

    /// <summary>
    /// 播放动画index
    /// </summary>
    public int indexOfAnim;

    /// <summary>
    /// 每帧pose的分数
    /// </summary>
    public List<int> poseScores = new List<int>();

    /// <summary>
    /// 关卡中所有动作的总分
    /// </summary>
    int sumOfAnimScores = 0;

    /// <summary>
    /// 一个关卡内动作的数量
    /// </summary>
    int totalAnim;

    public MMF_Player completeFB;
    public MMF_Player addnormalFB;
    public MMF_Player nextAnimFB;

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
    private bool enableRecordUser = true;

    private List<float> animsCost = new List<float>();

    private CircleColorController circleColorController = new();

    private void Start()
    {
        anim = GetComponent<Animator>();   // 获取动画控制器组件
        indexOfAnim = 0;
        totalAnim = actionDB.taichiActions.Count;
        startpos = InitPos.Instance.startpos;
        endpos = InitPos.Instance.endpos;
        width = endpos.position.x - startpos.position.x;
        height = endpos.position.y - startpos.position.y;
        ProcessSlider.Instance.SetTotalAction(totalAnim);

        animScores.Clear();
    }

    private void Update()
    {
        // 间隔帧记录标准模型和动捕模型的pose，用于动作结束的DTW评分
        RecordPoses();

        // 实时计算当前帧两个pose之间的差异值
        float difVal = CalSingleFrameDif(jointCalculationMode);

        // 将差异值转换为0～5的分数
        int singleScore = ScoreCalculator.ConvertDifToScore(difVal);

        // 根据分数高低更改太极环的颜色
        circleColorController.Update(singleScore, change);
    }

    /// <summary>
    /// 太极环颜色控制器，为避免颜色频繁跳变，采用阈值变化
    /// </summary>
    private class CircleColorController
    {
        private int state = (int)State.none;
        private enum State
        {
            red = 0,
            green = 1,
            none = -1
        }

        /// <summary>
        /// 从绿色变为红色需要的帧数
        /// </summary>
        private const int Green2Red = 20;

        /// <summary>
        /// 从红色到绿色需要的帧数
        /// </summary>
        private const int Red2Green = 2;

        private int greenPoseCount = 0;
        private int redPoseCount = 0;

        private const int GreenScore = 3;

        public void Update(float poseScore, ClolorChange change)
        {
            int poseState = GetNormScore(poseScore);

            if (state == (int)State.none)
            {
                state = poseState;                
                return;
            }

            if (poseState == (int)State.green && state == (int)State.red)
            {
                greenPoseCount += 1;
                if (greenPoseCount > Red2Green)
                {
                    state = (int)State.green;
                    greenPoseCount = 0;
                }
            }
            else if(poseState == (int)State.red && state == (int)State.green)
            {
                redPoseCount += 1;
                if (redPoseCount > Green2Red)
                {
                    state = (int)State.red;
                    redPoseCount = 0;
                }
            }
            else if(poseState == (int)State.green && state == (int)State.green)
            {
                redPoseCount = 0;
            }
            else if (poseState == (int)State.red && state == (int)State.red)
            {
                greenPoseCount = 0;
            }

            change.ChangeMatColor(state);
        }

        private int GetNormScore(float poseScore)
        {
            return poseScore >= GreenScore ? 1 : 0;
        }

        public void ResetState()
        {
            state = (int)State.none;
            greenPoseCount = 0;
            redPoseCount = 0;
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

    /// <summary>
    /// 计算单帧pose之间的差异值
    /// </summary>
    /// <param name="mode">关节表示模式</param>
    /// <returns>差异值</returns>
    private float CalSingleFrameDif(DTWAlgorithm.JointCalculationMode mode)
    {
        float[,] masterPose = new float[4, 3];
        float[,] userPose = new float[4, 3];

        float difVal = 0.0f;

        switch (mode)
        {
            case DTWAlgorithm.JointCalculationMode.AngleMode:
                masterPose = GetAnglePose(true);
                userPose = GetAnglePose(false);
                difVal = PoseDiffCalculator.CalDiffOf2AnglePoses(
                                        masterPose, userPose);
                break;
            case DTWAlgorithm.JointCalculationMode.VectorMode:
                masterPose = GetVectorPose(true);
                userPose = GetVectorPose(false);
                difVal = PoseDiffCalculator.CalDiffOf2VecPoses(
                            masterPose, userPose, 2);
                break;
        }

        return difVal;
    }


    /// <summary>
    /// 获得关节欧拉角组成的pose
    /// </summary>
    /// <param name="isMaster">是否是标准模型</param>
    /// <returns></returns>
    private float[,] GetAnglePose(bool isMaster)
    {
        float[,] pose = new float[masterJoints.Length, numOfJointsDataDim];

        int numOfJoints = masterJoints.Length;

        for (int i = 0; i < numOfJoints; i++)
        {
            float xa;
            float ya;
            float za;

            if (isMaster)
            {
                //Quaternion jointRotation = joints[i].localRotation;
                xa = GetJointLocalEulerAngle(masterJoints, i, 0);
                ya = GetJointLocalEulerAngle(masterJoints, i, 1);
                za = GetJointLocalEulerAngle(masterJoints, i, 2);
            }
            else
            {
                //Quaternion userJointRotation = userJoints[i].localRotation;
                xa = GetJointLocalEulerAngle(userJoints, i, 0);
                ya = GetJointLocalEulerAngle(userJoints, i, 1);
                za = GetJointLocalEulerAngle(userJoints, i, 2);
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
            _joints = masterJoints;
        }
        else
        {
            rootJoint = rootJointUser;
            _joints = userJoints;
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
    /// 用DTW计算用户动作得分
    /// </summary>
    /// <param name="masterAction"></param>
    /// <param name="userAction"></param>
    /// <returns></returns>
    public float CalDifOfSingleAnimInMainTh(
        List<float[,]> masterAction,
        List<float[,]> userAction)
    {
        float distance = DTWAlgorithm.CalculateDistance(masterAction, userAction, normalize: true, mode: jointCalculationMode);

        print("anim distance is: " + distance.ToString());
        print("masterAction length: " + masterAction.Count.ToString());
        print("userAction length: " + userAction.Count.ToString());

        // for debug: 保存action到txt文件中
        //SaveActionToFile();

        // 清除action序列
        RestAngleList();

        return distance;
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

    // 关卡结束调用，计算所有animclip的得分均值
    public void CalScoreOfGameLevel()
    {
        float sum = 0;
        foreach (float cost in animsCost)
        {
            sum += cost;
        }

        float averageCost = sum / animsCost.Count;
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
    /// animClip结束时调用，计算单个动作得分并加到关卡总分上
    /// </summary>
    /// <returns></returns>
    public float GetTotalScore()
    {
        float animDif = CalDifOfSingleAnimInMainTh(masterAction, userAction);

        int animScore = ScoreCalculator.ConvertDifToScore(animDif);

        animScores.Add(animScore);

        sumOfAnimScores += (int)(animScore*20*multiplier);

        addnormalFB.PlayFeedbacks();

        print("总等级为: " + ConvertScore2Level(animScore));

        GetNextAnim();

        // 重置太极环颜色控制器状态
        circleColorController.ResetState();

        return animScore;
    }

    public void GetNextAnim()
    {
        ProcessSlider.Instance.AddAction();
        poseScores.Clear();
        if (indexOfAnim < totalAnim)
        {
            string tempanim = actionDB.taichiActions[indexOfAnim].animName;
            Debug.Log(tempanim);
            SetAnimName(actionDB.taichiActions[indexOfAnim].nameText);
            anim.Play(tempanim);
        }
        else
        {
            completeFB.PlayFeedbacks();
        }
        indexOfAnim++;
    }

    public void SetAnimName(string animnam)
    {
        //actionNameText1.text=animnam;
        //nextAnimFB.PlayFeedbacks();
        actionNameText.text=animnam;

    }
    
    public void EndGame()
    {
        print("taichi endgame");
        CountTime.Instance.SetScore(sumOfAnimScores);
        CountTime.Instance.SetRank(GetRank());
        totalActiontext.text = totalAnim.ToString();
        correctActionText.text=GetNumOfGoodActions().ToString();
        wrongActionText.text=GetNumOfBadActions().ToString();

        SetfinalscoreText();
        CountTime.Instance.EndCountTime();

    }
    public void SetfinalscoreText()
    {
        if (finalscoretext != null){
            finalscoretext.text = sumOfAnimScores.ToString();
            print("finalScoreText: " + finalscoretext.text);
        }
    }

    public int GetRank()
    {       
        if (sumOfAnimScores > totalAnim * 90)//一个动作最高100分
        {
            return 5;
        }
        else if (sumOfAnimScores > totalAnim *80)
        {
            return 4;
        }
        else if (sumOfAnimScores > totalAnim * 60)
        {
            return 3;
        }
        else if (sumOfAnimScores > totalAnim * 40)
        {
            return 2;
        }
        else if (sumOfAnimScores > totalAnim * 20)
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

    public int GetNumOfGoodActions()
    {
        int numOfActions = animScores.Count;

        if (numOfActions == 0)
        {
            return 0;
        }

        int numOfGoodActions = 0;

        foreach (var score in animScores)
        {
            if (score >= 3)
            {
                numOfGoodActions += 1;
            }
        }

        return numOfGoodActions;
    }

    public int GetNumOfBadActions()
    {
        int numOfActions = animScores.Count;

        if (numOfActions == 0)
        {
            return 0;
        }

        int numOfBadActions = 0;

        foreach (var score in animScores)
        {
            if (score < 3)
            {
                numOfBadActions += 1;
            }
        }

        return numOfBadActions;
    }


}


