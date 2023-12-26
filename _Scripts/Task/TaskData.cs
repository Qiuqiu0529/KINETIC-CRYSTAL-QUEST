using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "new TaskData", menuName = "_Scripts/TaskData")]
public class TaskData : SerializedScriptableObject
{
    public int shareToday;
    public int completedLevelsToday;
    public bool enterGameToday;
    public bool addNewFriendToday;
    public int unlockLevelCountToday;

    public DateTime lastDateTime;

    public void ResetDailyTask()
    {
        shareToday=0;
        completedLevelsToday=0;
        enterGameToday=false;
        addNewFriendToday=false;
        unlockLevelCountToday=0;
        foreach(var task in taskInfos)
        {
            task.finished=false;
        }

    }

    public List<TaskInfo> taskInfos;

}
