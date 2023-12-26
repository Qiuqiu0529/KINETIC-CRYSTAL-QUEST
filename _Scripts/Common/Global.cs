using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    public static int poseLandmarks = 33;
    public static int posenum = 38;
    public static int faceusekeypoints = 9;
    public static int faceleyeinner = 10;

    public static int handnum = 21;


    public static string cookieName = "cookieName";
    public static string cookieValue = "cookieValue";
    public static string nickName = "nickName";
    public static string calNow = "calNow";

    public static string calRead = "calRead";

    public static string useOppoWatch = "useOppoWatch";

    public static string userId = "userId";
    public static string userPhone = "userPhone";
    public static string userPwd = "userPwd";

    public static string friendCode = "friendCode";

    public static string weight = "weight";
    public static string height = "height";

    public static string gender = "gender";
    public static string experience = "experience";
    public static string level = "level";

    public static string frontCamera = "frontCamera";
    public static string modelComplexity = "modelComplexity";
    public static string useBGM = "useBGM";
    public static string useMotionUI = "useMotionUI";
    public static string useHeadDec = "useHeadDec";

    public static string autoPauseTime = "autoPauseTime";
    public static string autoReturnTime = "autoReturnTime";

    public static string useBlendShape = "useBlendShape";
    public static string useHandTrack = "useHandTrack";
    public static string fixedHip = "useFixedHip";

    public static string ballDelay = "ballDelay";
    public static string ballTarget = "ballTarget";
    public static string starDelay = "starDelay";
    public static string pacmanDelay = "pacmanDelay";
    public static string pacmanTarget = "pacmanTarget";

    public static string taichiDelay = "taichiDelay";
    public static string question1Target = "question1Target";
    public static string question2Target = "question2Target";

    public static string languageSet = "languageSet";
    public static string languageSetIndex = "languageSetIndex";
    public static string chinese = "ZH";
    public static string english = "EN";


    public static string ballChanelMixer = "ballChanelMixer";
    public static string squashGestureLength = "squashGestureLength";
    public static string ballRunKneeDis = "ballRunKneeDis";
    public static string jumpDis = "jumpDis";

    public static string begintutorial = "begintutorial";

    public static string unlockStar = "unlockStar";

    public static string unlockBean = "unlockBean";

    public static string unlockBall = "unlockBall";
    public static string unlockPizza= "unlockPizza";
    public static string unlockEndlessStar = "unlockEndlessStar";
    public static string unlockPacman = "unlockPacman";   
    public static string unlockTaichi = "unlockTaichi";
    public static string unlockNone = "unlockNone";
    public static string unlockMocap = "unlockMocap";



    /// <summary>
    /// Constants used by this class and other K2-components
    /// </summary>
    public static class Constants
    {
        public const int MaxBodyCount = 6;
        public const int MaxJointCount = 38;

        public const float MinTimeBetweenSameGestures = 0.0f;
        public const float PoseCompleteDuration = 1.0f;
        public const float ClickMaxDistance = 0.05f;
        public const float ClickStayDuration = 2.0f;
    }

    /// <summary>
	/// Container for the sensor data, including color, depth, ir and body frames.
	/// </summary>
	public class SensorData
    {
        public int bodyCount;
        public int jointCount = 38;

        public float depthCameraOffset;
        public float depthCameraFOV;
        public float colorCameraFOV;
        public float faceOverlayOffset;

        public Vector3 colorImageScale = Vector3.one;
        public Vector3 depthImageScale = Vector3.one;

        public int colorImageWidth;
        public int colorImageHeight;

        public byte[] colorImage;
        public long lastColorFrameTime = 0;

        public int depthImageWidth;
        public int depthImageHeight;

        public ushort[] depthImage;
        public long lastDepthFrameTime = 0;

        public ushort[] infraredImage;
        public long lastInfraredFrameTime = 0;

        public byte[] bodyIndexImage;
        public long lastBodyIndexFrameTime = 0;

        public byte selectedBodyIndex = 255;
        public byte[] trackedBodyIndices;

        public bool hintHeightAngle = false;
        public Quaternion sensorRotDetected = Quaternion.identity;
        public float sensorHgtDetected = 0f;

        public RenderTexture bodyIndexTexture;
        public Material bodyIndexMaterial;
        public ComputeBuffer bodyIndexBuffer;

        public float[] bodyIndexBufferData = null;
        public bool bodyIndexBufferReady = false;
        public object bodyIndexBufferLock = new object();

        public RenderTexture depthImageTexture;
        public Material depthImageMaterial;
        public ComputeBuffer depthImageBuffer;
        public ComputeBuffer depthHistBuffer;

        public float[] depthImageBufferData = null;
        public int[] depthHistBufferData = null;
        public float[] equalHistBufferData = null;
        public int depthHistTotalPoints = 0;
        public int firstUserIndex = -1;

        public bool depthImageBufferReady = false;
        public object depthImageBufferLock = new object();
        public bool depthCoordsBufferReady = false;
        public object depthCoordsBufferLock = new object();
        public bool newDepthImage = false;

        public Texture colorImageTexture = null;
        public Texture2D colorImageTexture2D = null;

        public bool colorImageBufferReady = false;
        public object colorImageBufferLock = new object();
        public bool newColorImage = false;

        public RenderTexture depth2ColorTexture;
        public Material depth2ColorMaterial;
        public ComputeBuffer depth2ColorBuffer;
        public Vector2[] depth2ColorCoords;
        public long lastDepth2ColorCoordsTime = 0;

        public Vector3[] depth2SpaceCoords;
        public bool spaceCoordsBufferReady = false;
        public object spaceCoordsBufferLock = new object();
        public long lastDepth2SpaceCoordsTime = 0;

        public bool backgroundRemovalInited = false;
        public bool backgroundRemovalHiRes = false;
        public bool invertAlphaColorMask = false;

        public RenderTexture color2DepthTexture;
        public Material color2DepthMaterial;
        public ComputeBuffer color2DepthBuffer;
        public Vector2[] color2DepthCoords;
        public long lastColor2DepthCoordsTime = 0;

        public RenderTexture alphaBodyTexture;
        public Material alphaBodyMaterial;
        public Material erodeBodyMaterial, dilateBodyMaterial, gradientBodyMaterial;
        public Material medianBodyMaterial, blurBodyMaterial;

        public int erodeIterations0;
        public int dilateIterations1;
        public int erodeIterations2;
        public Color bodyContourColor = Color.green;

        public RenderTexture colorBackgroundTexture;
        public Material colorBackgroundMaterial;

        public bool newInfraredImage = false;

        public bool bodyFrameReady = false;
        public object bodyFrameLock = new object();
        public bool newBodyFrame = false;

        public bool isPlayModeEnabled;
        public string playModeData;
        public string playModeHandData;
    }
}

public enum RunningState : int
{
    normal = 0,
    pause = 1,

}

public enum ModelComplexity
{
    Lite = 0,
    Full = 1,
    Heavy = 2,
}

public enum PoseIndex //33keypoints+1none+1head+1hip+1neck+1spine
{
    nose,//0鼻子
    leyeinner,//左眼内部（靠鼻子
    leye,//左眼中
    leyeouter,//左眼外部，靠耳朵
    reyeinner,//右眼内部（靠鼻子
    reye,//右眼中
    reyeouter,//右眼外部，靠耳朵
    lear,//左耳
    rear,//右耳朵
    lmouth,//嘴左角
    rmouth,//10，嘴右角
    lshoulder,//左肩
    rshoulder,//右肩
    lelbow,//左手肘
    relbow,//右手肘
    lwrist,//左手腕
    rwrist,//右手腕
    lpinkey,//左小指
    rpinkey,//右小指
    lindex,//左食指
    rindex,//20，右食指
    lthumb,//左大拇指
    rthumb,//右大拇指
    lhip,//左屁股
    rhip,//右屁股
    lknee,//左膝盖
    rknee,//右膝盖
    lankle,//左脚踝
    rankle,//右脚踝
    lheel,//左脚跟
    rheel,//右脚跟
    lfoot,//左脚前
    rfoot,//左脚后
    neck,
    head,
    spine,
    hip,
    none,
}



public enum HandIndex : int//21keypoints
{
    wrist,//手腕
    tcmc,//大拇指手掌连接点
    tmcp,//大拇指底
    tdip,//大拇指中
    ttip,//大拇指尖
    ifmcp,//食指手掌连接处
    ifpip,//食指底
    ifdip,//食指中
    iftip,//食指尖
    mfmcp,//中指手掌连接处
    mfpip,//中指底
    mfdip,//中指中
    mftip,//中指尖
    rfmcp,//无名指手掌连接处
    rfpip,//无名指底
    rfdip,//无名指中
    rftip,//无名指尖
    pmcp,//小指手掌连接处
    ppip,//小指底
    pdip,//小指中
    ptip,//小指尖

    none
}


public enum FaceIndex : int
{

    betweeneye = 6,
    betweenbrow = 9,
    noseup = 4,
    nosetip = 1,

    nosedown = 2,
    mouthoutterup = 0,
    mouthinnereup = 13,
    mouthuppter = 12,
    mouthoverupper = 164,
    mouthinnerdown = 14,
    mouthoutterdown = 17,
    mouthunderdown = 18,

    mouthleft = 291,
    mouthright = 61,
    lowchin = 152,

    mouthleftdown = 422,
    mouthrightdown = 202,
    mouthleftstretch = 287,
    mouthrightstretch = 57,

    leyeouter = 263,
    leyeinner = 362,
    leyedown = 374,
    leyeup = 386,
    lp2 = 385,
    lp3 = 387,
    lp5 = 380,
    lp6 = 373,

    lbrow = 282,
    rbrow = 52,

    reyeouter = 33,
    reyeinner = 133,
    reyedown = 145,
    reyeup = 159,
    rp2 = 160,
    rp3 = 158,
    rp5 = 144,
    rp6 = 153,

    lupperpress1 = 40,
    lupperpress2 = 80,
    llowerpress1 = 88,
    llowerpress2 = 91,

    rupperpress1 = 270,
    rupperpress2 = 310,
    rlowerpress1 = 318,
    rlowerpress2 = 321,

    lsquint1 = 253,
    lsquint2 = 450,
    rsquint1 = 23,
    rsquint2 = 230,

    lcheek_squint1 = 359,
    lcheek_squint2 = 342,
    rcheek_squint1 = 130,
    rcheek_squint2 = 113,

    lear = 323,
    rear = 93,
}

public enum ModelFaceIndex : int
{
    mouthopen = 0,
    reyeclose = 1,
    leyeclose = 2,

    rsquint = 3,
    lsquint = 4,

}


public enum EyebrowIndex : int
{
    browUpL = 1,
    browUpR = 0,
}


public enum MovenetIndex
{
    NOSE = 0,
    LEFT_EYE,
    RIGHT_EYE,
    LEFT_EAR,
    RIGHT_EAR,
    LEFT_SHOULDER,
    RIGHT_SHOULDER,
    LEFT_ELBOW,
    RIGHT_ELBOW,
    LEFT_WRIST,
    RIGHT_WRIST,
    LEFT_HIP,
    RIGHT_HIP,
    LEFT_KNEE,
    RIGHT_KNEE,
    LEFT_ANKLE,
    RIGHT_ANKLE
}


public enum Choosetarget : int
{
    nose,
    neck,
    center,
    hip,

}


public enum GameMode : int
{
    none,
    note,
    pacman,
    motioncap,
    ball,
    taichi,
    food,
    bean,
    endlessplanet,

}

public enum BGMMode : int
{
    randomMusic,
    selectMusic,
}

public enum CapMode : int
{
    none,
    full,
    up,
    down
}
