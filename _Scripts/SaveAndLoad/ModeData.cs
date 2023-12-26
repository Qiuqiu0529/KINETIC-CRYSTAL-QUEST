using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "new GameModeData", menuName = "_Scripts/GameModeData")]
public class ModeData : SerializedScriptableObject
{
    public List<ModeInfo> modeInfos;
    public bool GetHaveEnter(GameMode gameM)
    {
        if(!modeInfos.Find(i => i.gameMode== gameM).haveEnter)
        {
            modeInfos.Find(i => i.gameMode== gameM).haveEnter=true;
            return false;//没进入改进入
        }
        return true;
    }

    public ModeInfo GetModeInfo(GameMode gameM)
    {
        return modeInfos.Find(i => i.gameMode== gameM);
    }
}

[Serializable]
public class ModeInfo
{
    public bool unlock;
    public GameMode gameMode;
    public int modeID;
    public String Name;
    public bool haveEnter;
    public string unlockStringName;
    public bool enableScore;
    public string nameText;
    public bool multiPlayer;//单人or多人游戏
    public CapMode capMode;
    public string gameUrl;
    public string sceneNameBase;
    [TextArea]
    public string description;

    public ChapterData chapterData;
}


