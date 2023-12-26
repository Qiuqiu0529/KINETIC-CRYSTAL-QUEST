using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using MoreMountains.Feedbacks;

public class CreateNoteEvent : MonoBehaviour
{
    [Tooltip("Koreography")]
    public Koreography[] koreographies;
    [Tooltip("koreographies[targetGraphy]为实际音乐，每首歌一个场景感觉有点浪费，改成一个场景，根据需求调用不同音乐事件会好点？")]
    public int targetGraphy;

    [Tooltip("实际的音乐audiosource ")]
    public AudioSource audioCom;//
    protected Koreography playingKoreo;
    protected List<KoreographyEvent> rawEvents = new List<KoreographyEvent>();

    [EventID]
    public string eventID;

    [Tooltip("多少种note，多少种pools")]
    public List<ObjectPool> objectPools = new List<ObjectPool>();

    [Space]
    [Header("时间参数")]

    [Tooltip("事件开始后多少时间播放音乐")]
    public float leadTime;

    [Tooltip("延迟时间")]
    public float delayTime;



    [Tooltip("用户设置的delaytime")]
    public float customDelayTime;

    protected float toTargetTime;//从生成到节奏点的时间（create比koreographyevt早多少），理论上是leadtime（整体提前）-delaytime（延后生成）？

    [Tooltip("从哪里开始播放，调试用，默认应该是0，seconds，float")]
    public float initPlayTime;



    [Space]

    public MMF_Player calcScoreFB;
    public MMF_Player countDownFB;

    // protected bool startplay;

    [Tooltip("当前事件编号，测试")]
    public int pendingEvt = 0;//处理的事件index

    public int DelayedSampleTime
    {
        get
        {
            // Offset the time reported by Koreographer by a possible leadInTime amount.
            return playingKoreo.GetLatestSampleTime();//- (int)(audioCom.pitch * leadInTimeLeft * SampleRate);
        }
    }

    public int SampleRate
    {
        get
        {
            return playingKoreo.SampleRate;
        }
    }
    protected void Awake()
    {
        targetGraphy=ModeMgr.Instance.chapter;

    }

    protected void Start()
    {
        Koreographer.Instance.LoadKoreography(koreographies[targetGraphy]);
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        // Grab all the events out of the Koreography.
        audioCom.clip = playingKoreo.SourceClip;
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID);

        rawEvents = rhythmTrack.GetAllEvents();
        InitializeLeadIn();
    }


    protected void Update()
    {
        CheckSpawnNext();
    }


    public void CheckSpawnNext()
    {
        while (pendingEvt < rawEvents.Count &&
               rawEvents[pendingEvt].StartSample < (int)((audioCom.time + toTargetTime) * SampleRate))
        {
            CreateNote(rawEvents[pendingEvt]);
            pendingEvt++;
        }
        
        //Debug.Log(toTargetTime);
    }

    public virtual void SetPendingEvt()
    {
        for (int i = 0; i < rawEvents.Count; ++i)
        {
            if (rawEvents[i].StartSample >= (int)((initPlayTime - leadTime) * SampleRate))
            {
                pendingEvt = i;
                break;
            }
        }
    }

    public virtual void TestEvtInfo(string info)
    {
    }

    public virtual void CreateNote(KoreographyEvent evt)
    {

    }

    public void TestAllEvt()
    {
        playingKoreo = koreographies[targetGraphy];
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID);
        rawEvents = rhythmTrack.GetAllEvents();
        Debug.Log("共有" + rawEvents.Count + "个事件");
    }


    protected void InitializeLeadIn()
    {
        // if (leadTime > 0f)
        // {
        //     StartCoroutine(StartDelay());
        // }
        // else
        // {
        //     PlayMusic();
        // }
        countDownFB.PlayFeedbacks();

        audioCom.time = initPlayTime;
        SetPendingEvt();
    }

    public void PlayMusic()
    {
        audioCom.Play();
        
        //startplay=true;
        calcScoreFB.StopFeedbacks();
        float duration=playingKoreo.SourceClip.length - initPlayTime;
        Debug.Log(duration);
        calcScoreFB.GetFeedbackOfType<MMF_HoldingPause>().PauseDuration = duration;
        ProcessSlider.Instance.SetTotal(duration);
        calcScoreFB.PlayFeedbacks();
    }


    public void Restart()
    {
        //startplay=false;
        audioCom.Stop();
        audioCom.time = 0f;
        Koreographer.Instance.FlushDelayQueue(playingKoreo);
        playingKoreo.ResetTimings();
        InitializeLeadIn();
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(leadTime);
        PlayMusic();
    }

}
