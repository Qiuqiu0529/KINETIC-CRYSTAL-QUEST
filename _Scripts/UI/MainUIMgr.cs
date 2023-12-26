using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using Nova.TMP;
using NovaSamples.UIControls;
using MoreMountains.Feedbacks;

using System;


public class MainUIMgr : Singleton<MainUIMgr>//选关、选主题界面切换&场景跳转
{
    //public MyInputMgr inputmgr;
    public GameMode gamename;
    public int chaterid;//0 1
    public int chapterID;//1001 
    public TextMeshProTextBlock chapterTextBlock;

    public MMF_Player closeModePanel, openModePanel, selectModeUI, selectModeEnv;
    public MMF_DestinationTransform mMF_DestinationTransform;
    public MMF_Player closeChapterPanel, openChapterPanel;
    public MMF_Player openSettingPanel, closeSettingPanel;

    public List<MyModeButton> modeButtons;
    public MyModeButton selectButton;
    public GameObject selectModeParticle;

    public event Action startTransing;
    public event Action endTransing;

    public ChapterList chapterList;

    public bool isConversation;
    public GameObject converSationBlock;

    // void Update()
    // {
    //     if (TutorialUIController.Instance.IsTutorialPanelActive()&&selectButton!=null)
    //     {
    //         selectButton.selectParticle.SetActive(false);
    //     }
    //     else if(selectButton!=null){
    //         selectButton.selectParticle.SetActive(true);
    //     }
    // }
    public void SetParticleDisActive()
    {
        if (state.GetType() == typeof(ChapterSelectState))
            selectButton.selectParticle.SetActive(false);
    }
    public void SetParticleActive()
    {
        if (state.GetType() == typeof(ChapterSelectState))
            selectButton.selectParticle.SetActive(true);
    }


    public void SetSelectParticleDisActive()
    {
        if (selectModeParticle != null)
        {
            selectModeParticle.SetActive(false);

        }
    }

    public void SetSelectParticleActive()
    {
        if (selectModeParticle != null)
        {
            selectModeParticle.SetActive(true);

        }
    }

    public Transform modesScrollTargetPos;

    SelectUIState state;
    public SelectUIState prestate;
    bool chosenChapter;
    bool isTransing;

    public void SetSelectUIState(SelectUIState newState)//切换时暂停输入，避免出错？
    {
        //inputmgr.BlockInput();
        prestate = state;
        state.LeaveSelectMode();
        state = newState;
        state.EnterSelectMode();
    }

    public void SetToSettinngMode()
    {
        SetSelectUIState(new SettingSelectState());
    }
    public void ReSetTransing()
    {
        endTransing?.Invoke();
        isTransing = false;
    }
    public void SetInConversation()
    {
        converSationBlock.SetActive(true);
        isConversation = true;
    }

    public void ResetIsConversation()
    {
        converSationBlock.SetActive(false);
        isConversation = false;
    }

    public void SetToChapterMode()
    {

        if (gamename == GameMode.none)
        {
            return;
        }

        if (isTransing)
        {
            return;
        }
        isTransing = true;

        startTransing?.Invoke();
        ModeMgr.Instance.ChangeMode(gamename);
        chosenChapter = false;
        //hintObj.SetActive(true);
        //desTextBlock.text = ModeMgr.Instance.ModeDescription();
        SetSelectUIState(new ChapterSelectState());
    }
    public void SetSelectButtonHighLight()
    {
        if (selectButton != null)
        {
            selectButton.SetHighLighted();
        }

    }
    public void SetToModeMode()
    {
        if (isTransing)
        {
            return;
        }
        isTransing = true;
        startTransing?.Invoke();
        ModeMgr.Instance.ToDefaultMode();


        //hintObj.SetActive(false);
        SetSelectUIState(new ModeSelectState());
    }


    private void Start()
    {
        mMF_DestinationTransform = selectModeEnv.GetFeedbackOfType<MMF_DestinationTransform>();
        // if (ModeMgr.Instance.UseModeSelectPanel())
        // {
        //     state = new ModeSelectState();
        // }
        // else
        // {
        //     gamename=ModeMgr.Instance.gamename;
        //     state = new ChapterSelectState();
        //     StartCoroutine(SetDefaultMode());
        // }
        state = new ModeSelectState();
        state.EnterSelectMode();
    }

    public IEnumerator SetDefaultMode()
    {
        yield return new WaitForSeconds(0.5f);
        if (selectButton == null)
        {
            selectButton = modeButtons.Find(i => i.mode == gamename);
            SelectModeEffect();
            mMF_DestinationTransform.Destination = selectButton.rotTrans;
            selectModeEnv.PlayFeedbacks();
        }
    }

    public void OpenCommunity()//开网页关闭main输入等，待改，如果有手势控制UI，关闭
    {
        //inputmgr.OpenCommunity();

    }
    public void CloseCommunity()//如果有手势控制UI，开启
    {
        //inputmgr.CloseCommunity();
    }

    public void SelectModeEffect()
    {
        selectModeParticle = selectButton.selectParticle;
        selectButton.SetHighLighted();
        chapterTextBlock.text = ModeMgr.Instance.ModeNameText(selectButton.mode);//选模式时不改modemgr，打开选关界面再打开mgr
        modesScrollTargetPos.position = selectButton.transform.position;
        mMF_DestinationTransform.Destination = selectButton.rotTrans;
        selectModeUI.PlayFeedbacks();
        selectModeEnv.PlayFeedbacks();
    }

    public void ChangeMode(MyModeButton nselected)
    {
        if (isTransing)
        {
            return;
        }
        // if(isConversation)
        // {
        //     return;
        // }

        if (gamename == nselected.mode)
        {
            return;
        }
        gamename = nselected.mode;
        if (selectButton != null)
        {
            selectButton.ResetAnim();
        }
        selectButton = nselected;
        SelectModeEffect();
    }

    public void ChangeToPreMode()
    {

        if (selectButton == null)
        {
            ChangeMode(modeButtons[0]);
        }
        else
        {
            int temp = (modeButtons.IndexOf(selectButton) - 1) % (modeButtons.Count);
            if (temp < 0)
            {
                temp = modeButtons.Count - 1;
            }
            ChangeMode(modeButtons[temp]);
        }

    }

    public void ChangeToNextMode()
    {
        if (selectButton == null)
        {
            int temp = modeButtons.Count - 1;
            ChangeMode(modeButtons[temp]);
        }
        else
        {
            int temp = (modeButtons.IndexOf(selectButton) + 1) % (modeButtons.Count);
            //Debug.Log(temp);
            if (temp < modeButtons.Count && temp >= 0)
            {
                ChangeMode(modeButtons[temp]);
            }
        }
    }

    public void MoveUILeft()
    {
        if (state.GetType() == typeof(ModeSelectState))
        {
            ChangeToPreMode();
        }
        else if (state.GetType() == typeof(ChapterSelectState))
        {
            if (isTransing)
            {
                return;
            }
            chapterList.PreChapter();

        }
    }

    public void MoveUIRight()
    {
        if (state.GetType() == typeof(ModeSelectState))
        {
            ChangeToNextMode();
        }
        else if (state.GetType() == typeof(ChapterSelectState))
        {
            if (isTransing)
            {
                return;
            }
            chapterList.NextChapter();
        }
    }

    public void SelectUIButton()
    {
        if (state.GetType() == typeof(ModeSelectState))
        {
            SetToChapterMode();
        }
        else if (state.GetType() == typeof(ChapterSelectState))
        {

            ToSingleScene();
        }
    }

    public void CancleUIButton()
    {
        if (state.GetType() == typeof(ModeSelectState))
        {

        }
        else if (state.GetType() == typeof(ChapterSelectState))
        {

            SetToModeMode();
        }
    }

    public void ChangeChapter(int chater, int chapterid = 0)
    {
        chosenChapter = true;
        chaterid = chater;
        chapterID = chapterid;
        ModeMgr.Instance.chapterID = chapterid;
    }

    public void ToSingleScene()
    {
        if (!chosenChapter)
            return;
        ModeMgr.Instance.GoToTargetScene(chaterid);
    }
    public void ToMultiScene()
    {
        if (!chosenChapter)
            return;
        ModeMgr.Instance.GoToTargetScene(chaterid, true);
    }

    public void CloseChapterPanel() => closeChapterPanel.PlayFeedbacks();

    public void OpenChapterPanel() => openChapterPanel.PlayFeedbacks();

    public void OpenModePanel() => openModePanel.PlayFeedbacks();

    public void CloseModePanel() => closeModePanel.PlayFeedbacks();

    public void OpenSettingPanel() => openSettingPanel.PlayFeedbacks();

    public void CloseSettingPanel()
    {
        SetSelectUIState(prestate);
        closeSettingPanel.PlayFeedbacks();
    }

    public void LogOut()
    {
        NetWorkUtil.LogOut();
        ModeMgr.Instance.GoToLoginScene();
    }


    public void GoToTargetScene()
    {
        //inputmgr.BlockInput();
        ModeMgr.Instance.GoToTargetScene(chaterid);
    }

}
