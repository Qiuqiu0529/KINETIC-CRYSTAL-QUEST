using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;
using Nova.TMP;

public class PizzaScore : Singleton<PizzaScore>
{

    [SerializeField] float score;
    [SerializeField] int combo;

    [SerializeField] int maxcombo;
    [SerializeField] int totalNote;
    [SerializeField] int ateNote;
    [SerializeField] int failNote;


    [SerializeField] TMP_Text scoretext;
    [SerializeField] TMP_Text combotext;


    [SerializeField] TextMeshProTextBlock maxcombotext;
    [SerializeField] TextMeshProTextBlock totalNotetext;
    [SerializeField] TextMeshProTextBlock ateNotetext;
    [SerializeField] TextMeshProTextBlock failNotetext;
    [SerializeField] TextMeshProTextBlock finalscoretext;


    public MMF_Player addnormalFB;
    public MMF_Player failFB;
    public MMF_Player addcomboFB;

    
    public MMF_Player eatFB;
    
    MMF_FloatingText mMF_FloatingText;

    float multiplier=1;
    DetectNotice detectNotice;
    int maxNote=0;
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
        mMF_FloatingText=eatFB.GetFeedbackOfType<MMF_FloatingText>();
        score = 0;
        combo = 0;
        maxcombo = 0;
        failNote = 0;
        totalNote = 0;
    }

    public void SetFloatingText(Vector3 position,int score)
    {
        eatFB.transform.position=position;
        mMF_FloatingText.Value=score.ToString();
        AddScore(score);
        eatFB.PlayFeedbacks();
    }

    public void AddScore(int addscore)
    {
        ++combo;
        ++totalNote;
        ++ateNote;

        addcomboFB.PlayFeedbacks();
        if (combo >= 50)
        {
            score += (int)addscore * 1.5f*multiplier;
        }
        else
        {
            score += (int)addscore * (1 + (combo / 10) * 0.1f*multiplier);
        }
        addnormalFB.PlayFeedbacks();
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
        if(combo>maxcombo)
        {
            maxcombo=combo;
        }
        maxcombotext.text = maxcombo.ToString();
    }

    public void SettotalNoteText()
    {
        totalNotetext.text = totalNote.ToString();
    }
    public void SetateNoteText()
    {
        ateNotetext.text = ateNote.ToString();
    }

    public void SetfailNoteText()
    {
        failNotetext.text = failNote.ToString();
    }

    public void SetfinalscoreText()
    {
        finalscoretext.text = score.ToString();
    }

    public void EndGame()
    {
        PoseTrack.instance.ClearObserver();
        int duration = (int)(Time.time - CountTime.Instance.startTime);
        maxNote=duration/7;//上限值
        CountTime.Instance.SetScore((int)score);
        CountTime.Instance.SetRank(GetRank());

        SettotalNoteText();
        SetateNoteText();
        SetfailNoteText();
        // SetmaxcomboText();
        SetfinalscoreText();
        CountTime.Instance.EndCountTime();
    }

    public int GetRank()
    {
        if (ateNote>=maxNote)
        {
            return 5;
        }
        else if (ateNote > maxNote * 0.8)
        {
            return 4;
        }
        else if (ateNote > maxNote * 0.6)
        {
            return 3;
        }
        else if (ateNote > maxNote * 0.4)
        {
            return 2;
        }
        else if (ateNote > maxNote * 0.3)
        {
            return 1;
        }
        return 0;

    }

}
