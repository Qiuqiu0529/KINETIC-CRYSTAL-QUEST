using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class BGMDuration : MonoBehaviour
{
    public MMF_Player BGMFB;
    public AudioClip audioClip;
    public MMF_Player calcScoreFB;
    public void PlayMusic()
    {
        float duration=audioClip.length;
        BGMFB.PlayFeedbacks();
        ProcessSlider.Instance.SetTotal(duration);
        calcScoreFB.GetFeedbackOfType<MMF_HoldingPause>().PauseDuration = duration;
        ProcessSlider.Instance.SetTotal(duration);
        calcScoreFB.PlayFeedbacks();
    }
}
