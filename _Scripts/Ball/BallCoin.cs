using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;
using Nova.TMP;

public class BallCoin : Singleton<BallCoin>
{
    // [SerializeField] BallChapter chapter;

    [SerializeField] float score;
    [SerializeField] int combo;

    [SerializeField] int maxcombo;
    [SerializeField] int totalNote;
    [SerializeField] int blackNote;
    [SerializeField] int whiteNote;
    [SerializeField] int failNote;

    float multiplier=1;
    
    public float multiplier1=1;
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


    [SerializeField] TMP_Text scoretext;
    [SerializeField] TMP_Text combotext;


    [SerializeField] TextMeshProTextBlock maxcombotext;
    [SerializeField] TextMeshProTextBlock totalNotetext;
    [SerializeField] TextMeshProTextBlock blackNotetext;
    [SerializeField] TextMeshProTextBlock whiteNotetext;
    [SerializeField] TextMeshProTextBlock failNotetext;
    [SerializeField] TextMeshProTextBlock finalscoretext;


    public MMF_Player addnormalFB;
    public MMF_Player failFB;
    public MMF_Player addcomboFB;


    private void Start()
    {
        score = 0;
        combo = 0;
        maxcombo = 0;
        failNote = 0;
        totalNote = 0;
    }

    public int AddBlack(int addscore)
    {
        ++blackNote;
        return AddScore(addscore);
    }
    public int AddWhite(int addscore)
    {
        ++whiteNote;
        return AddScore(addscore);
    }

    public int AddScore(int addscore)
    {
        ++combo;
        ++totalNote;
        addcomboFB.PlayFeedbacks();
        int temp=0;
        if (combo >= 50)
        {
            temp=(int)(addscore * 1.5f*multiplier*multiplier1);
            
        }
        else
        {
            temp=(int)(addscore * (1 + (combo / 10) * 0.1f)*multiplier*multiplier1);
        }
        score += temp;
        addnormalFB.PlayFeedbacks();
        return temp;
    }


    public void FailNote()
    {
        if (maxcombo < combo)
        {
            maxcombo = combo;
        }
        ++failNote;
        ++totalNote;
        combo = 0;
        failFB.PlayFeedbacks();
    }

    public void SetScoreText()
    {
        scoretext.text = score.ToString();
    }

    public void SetComboText()
    {
        combotext.text = combo.ToString();
    }

    public void SetmaxcomboText()
    {
        if (combo > maxcombo)
        {
            maxcombo = combo;
        }
        maxcombotext.text = maxcombo.ToString();
    }

    public void SettotalNoteText()
    {
        totalNotetext.text = totalNote.ToString();
    }
    public void SetblackNoteText()
    {
        blackNotetext.text = blackNote.ToString();
    }

    public void SetwhiteNoteText()
    {
        whiteNotetext.text = whiteNote.ToString();
    }

    public void SetfailNoteText()
    {
        failNotetext.text = failNote.ToString();
    }

    public void SetfinalscoreText()
    {
        finalscoretext.text = score.ToString() ;
    }

    public void EndGame()
    {
        PoseTrack.instance.ClearObserver();
        CountTime.Instance.SetScore((int)score);
        CountTime.Instance.SetRank(GetRank());
        
        SettotalNoteText();
        SetblackNoteText();
        SetwhiteNoteText();
        SetfailNoteText();
        SetmaxcomboText();
        SetfinalscoreText();
        CountTime.Instance.EndCountTime();
    }

    public int GetRank()
    {
        if (score > totalNote * 1 * 25)
        {
            return 5;
        }
        else if (score > totalNote * 0.8 * 25)
        {
            return 4;
        }
        else if (score > totalNote * 0.6 * 25)
        {
            return 3;
        }
        else if (score > totalNote * 0.4 * 25)
        {
            return 2;
        }
        else if (score > totalNote * 0.2 * 25)
        {
            return 1;
        }

        return 0;

    }

}
