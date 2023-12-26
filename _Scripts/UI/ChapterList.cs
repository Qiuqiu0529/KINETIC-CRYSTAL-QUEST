using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using MoreMountains.Feedbacks;
public class ChapterList : MonoBehaviour
{
    public ListView listView;
    public ChapterVisual select;
    public List<ChapterVisual> chapters = new List<ChapterVisual>();

    public AudioClip uisound;

    public List<ChapterInfo> chapterInfos;

    public MMF_Player mMF_Player;
    public MMF_Player musicStop;
    public MMF_MMSoundManagerSound mMF_MMSoundManagerSound;


    int selectIndex = 0;
    private void Awake()
    {

        listView.AddDataBinder<ChapterInfo, ChapterVisual>(BindChapter);
        listView.AddGestureHandler<Gesture.OnClick, ChapterVisual>(HandleContactClicked);
        mMF_MMSoundManagerSound = mMF_Player.GetFeedbackOfType<MMF_MMSoundManagerSound>();


    }

    private void OnDestroy()
    {
        listView.RemoveDataBinder<ChapterInfo, ChapterVisual>(BindChapter);
        listView.RemoveGestureHandler<Gesture.OnClick, ChapterVisual>(HandleContactClicked);
    }
    private void HandleContactClicked(Gesture.OnClick evt, ChapterVisual target, int index)
    {
        selectIndex = index;
        ChangeChapterIndex(index);
        if (select != null)
        {
            select.UpdateVisualState(VisualState.Default);
        }
        select = target;
        target.UpdateVisualState(VisualState.Pressed);

    }

    public void ChangeChapterIndex(int index)
    {
        UISound.Instance.PlayUISound(uisound);
        MainUIMgr.Instance.ChangeChapter(index, chapterInfos[index].chapterID);
        if (chapterInfos[index].audioClip != null)
        {
            if (mMF_MMSoundManagerSound.Sfx != chapterInfos[index].audioClip)
            {
                mMF_Player.StopFeedbacks();
                musicStop.PlayFeedbacks();
                mMF_MMSoundManagerSound.Sfx = chapterInfos[index].audioClip;
                mMF_Player.PlayFeedbacks();
            }

        }
        else
        {
            mMF_Player.StopFeedbacks();
            musicStop.PlayFeedbacks();
            mMF_MMSoundManagerSound.Sfx = null;
        }

    }

    public void PreChapter()
    {
        if (select != null)
        {
            select.UpdateVisualState(VisualState.Default);
        }

        if (select == null)
        {
            selectIndex = 0;
        }
        else
        {
            selectIndex--;
            if (selectIndex < 0)
            {
                selectIndex = chapters.Count - 1;
            }
        }



        ChangeChapterIndex(selectIndex);

        select = chapters[selectIndex];
        listView.JumpToIndex(selectIndex);
        select.UpdateVisualState(VisualState.Pressed);
    }

    public void NextChapter()
    {
        if (select != null)
        {
            select.UpdateVisualState(VisualState.Default);
        }

        if (select == null)
        {
            selectIndex = chapterInfos.Count - 1;
        }
        else
        {
            selectIndex++;
            selectIndex %= chapters.Count;
        }
        ChangeChapterIndex(selectIndex);
        select = chapters[selectIndex];
        listView.JumpToIndex(selectIndex);
        select.UpdateVisualState(VisualState.Pressed);

    }


    private void OnEnable()
    {
        selectIndex = -1;
        chapters.Clear();
        //Debug.Log(MainUIMgr.Instance.gamename);
        chapterInfos = ModeMgr.Instance.GetChapterInfos(MainUIMgr.Instance.gamename);
        listView.SetDataSource(chapterInfos);
        StartCoroutine(TestSelect());

       
    }

    IEnumerator TestSelect()
    {
        yield return new WaitForSeconds(0.3f);
        PreChapter();
    }

    void OnDisable()
    {
        selectIndex = -1;
        select = null;
        //mMF_Player.StopFeedbacks();
        musicStop.PlayFeedbacks();

        mMF_MMSoundManagerSound.Sfx = null;
    }

    public void BindChapter(Nova.Data.OnBind<ChapterInfo> evt, ChapterVisual ChapterVisual, int index)
    {
        ChapterInfo ChapterInfo = evt.UserData;

        ChapterVisual.nameBlock.Text = ChapterInfo.nameText;
        ChapterVisual.authorBlock.Text = ChapterInfo.author;
        ChapterVisual.UpdateVisualState(VisualState.Default);

        chapters.Add(ChapterVisual);

    }

}
