using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "new ChapterData", menuName = "_Scripts/ChapterData")]
public class ChapterData : SerializedScriptableObject
{
    public List<ChapterInfo> chapterInfos;
}


[System.Serializable]
public class ChapterInfo
{
    public int chapterID;
    public string nameText;
    public int maxScore;//本地云端都存，if本地==0，再请求
    [TextArea]
    public string author;
    public AudioClip audioClip;
}


