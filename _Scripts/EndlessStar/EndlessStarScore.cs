using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;
using Nova.TMP;


public class EndlessStarScore : Singleton<EndlessStarScore>
{

    [SerializeField] float score;


    [SerializeField] TMP_Text scoretext;

    [SerializeField] TextMeshProTextBlock finalscoretext;


    public MMF_Player addnormalFB;

    float multiplier=1;
    DetectNotice detectNotice;
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


    private void Start()
    {
        score = 0;
    }

    public int AddScore(int addscore)
    {
        score += addscore*multiplier;
        addnormalFB.PlayFeedbacks();
        return addscore;
    }


    public void SetScoreText()
    {
        scoretext.text = score.ToString();
    }

   

    public void SetfinalscoreText()
    {
        finalscoretext.text = score.ToString();
    }

    public void EndGame()
    {
        PoseTrack.instance.ClearObserver();
        CountTime.Instance.SetScore((int)score);
        CountTime.Instance.SetRank(GetRank());
        Debug.Log("rank"+GetRank());
        SetfinalscoreText();
        CountTime.Instance.EndCountTime();
    }

    public int GetRank()
    {
        int duration = (int)(Time.time - CountTime.Instance.startTime);
        Debug.Log(duration);
        if (duration > 300)
        {
            return 5;
        }
        else if (duration > 210)
        {
            return 4;
        }
        else if (duration > 150)
        {
            return 3;
        }
        else if (duration > 90)
        {
            return 2;
        }
        else if (duration > 30)
        {
            return 1;
        }
        return 0;

    }

}
