using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class ModeMgr : Singleton<ModeMgr>
{
    public int chapter = 0;//加载不同关卡
    public ModeData data;
    public GameMode gamename;
    public MMF_LoadScene loadScene;
    public MMF_Player changeScne;

    public int chapterID = 0;
    public bool UseModeSelectPanel()
    {
        if (gamename == GameMode.none)
        {
            return true;
        }
        return false;
    }

    public string GetChapterID()
    {
        // int temp = (int)gamename;
        // int chapterinfo = temp * 1000 + chapter + 1;
        return chapterID.ToString();
    }

    public string GetUnlockStringName()
    {
        return data.GetModeInfo(gamename).unlockStringName;
    }

    public string GetUnlockStringName(GameMode gameMode)
    {
        return data.GetModeInfo(gameMode).unlockStringName;
    }




    private void Start()
    {
        loadScene = changeScne.GetFeedbackOfType<MMF_LoadScene>();

        DontDestroyOnLoad(this.gameObject);
    }

    public void GoToTargetScene(int chaterid, bool multiPlyayer = false)
    {
        chapter = chaterid;
        switch (gamename)
        {
            case GameMode.motioncap:
                loadScene.DestinationSceneName = SceneNameBase() + chapter.ToString();
                break;
            case GameMode.ball:
            case GameMode.note:
            case GameMode.pacman:
            case GameMode.taichi:
            case GameMode.food:
            case GameMode.bean:
            case GameMode.endlessplanet:
                loadScene.DestinationSceneName = SceneNameBase();
                break;
            // case GameMode.math:
            //     loadScene.DestinationSceneName = SceneNameBase();
            //     break;
            // case GameMode.question:
            //     if (multiPlyayer)
            //     {
            //         loadScene.DestinationSceneName = SceneNameBase() + "M";
            //     }
            //     else
            //     {
            //         loadScene.DestinationSceneName = SceneNameBase();
            //     }
            //     break;
            default:
                break;
        }
        changeScne.PlayFeedbacks();
    }

    public void GoToLoginScene()
    {
        ToDefaultMode();
        loadScene.DestinationSceneName = "BlobLogin";
        changeScne.PlayFeedbacks();
    }

    public void ChangeMode(GameMode mode)
    {
        gamename = mode;
        chapter = 0;
    }
    public void ToDefaultMode()
    {
        gamename = GameMode.none;
    }
    public List<ChapterInfo> GetChapterInfos()
    {

        return data.GetModeInfo(gamename).chapterData.chapterInfos;
    }
    public List<ChapterInfo> GetChapterInfos(GameMode mode)
    {
        return data.GetModeInfo(mode).chapterData.chapterInfos;
    }

    public string ModeNameText()
    {
        return TextMgr.Instance.GetText(data.GetModeInfo(gamename).Name);

    }

    public string ModeNameText(GameMode mode)
    {
        return TextMgr.Instance.GetText(data.GetModeInfo(mode).Name);

    }

    public bool GetUnLockInfo(GameMode mode)
    {
        return data.GetModeInfo(mode).unlock;
    }

    public CapMode GetCapMode()
    {
        return data.GetModeInfo(gamename).capMode;
    }

    public bool EnableScore()
    {
        return data.GetModeInfo(gamename).enableScore;
    }


    public bool EnableMultiPlayer()
    {
        return data.GetModeInfo(gamename).multiPlayer;
    }

    public string SceneNameBase()
    {
        return data.GetModeInfo(gamename).sceneNameBase;
    }

    public string ModeDescription()
    {

        return data.GetModeInfo(gamename).description;
    }


    public string Gameurl()
    {
        return data.GetModeInfo(gamename).gameUrl;
    }

}
