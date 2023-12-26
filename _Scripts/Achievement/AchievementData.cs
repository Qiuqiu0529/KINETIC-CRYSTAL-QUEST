using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "new AchievementData", menuName = "_Scripts/AchievementData")]
public class AchievementData : SerializedScriptableObject
{
    public int totalCompletedLevels;
    

    public List<AchievementInfo> achievementInfos;
}
