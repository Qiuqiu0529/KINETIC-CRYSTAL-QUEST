using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using System;
using BestHTTP;
using BestHTTP.Cookies;
using Newtonsoft.Json;
using TMPro;
using Nova;
public class ChapterMgr : Singleton<ChapterMgr>
{
    public TextMeshPro modeNameBlock;
    public TextMeshPro trackmodeBlock;
    public TextMeshPro trackmodeInfoBlock;
    public GameObject fullPic,upPic;


    public TextMgr textMgr;
    
    void OnEnable()
    {
         textMgr=TextMgr.Instance;
        ChangeText();
        
        
        textMgr.changeText += ChangeText;
    }

    void OnDisable()
    {
        textMgr.changeText -= ChangeText;
    }
    public void ChangeText()
    {
        modeNameBlock.text = ModeMgr.Instance.ModeNameText(ModeMgr.Instance.gamename);
        trackmodeBlock.text= textMgr.GetText("trackmode");
        // trackmodeInfoBlock.text= 
        // textMgr.GetText(ModeMgr.Instance.GetCapMode().ToString());
        fullPic.SetActive(ModeMgr.Instance.GetCapMode()==CapMode.full);
        upPic.SetActive(ModeMgr.Instance.GetCapMode()==CapMode.up);


        //textBlock.text = TextMgr.Instance.GetText(textkey);
    }
    // public TextBlock[] scoreBlocks;
    // public TextBlock myScoreBlock;
    // public GameObject scorePanel;
    // public GameObject multiPlayerButton;
    // public bool allScore = true;

    // public void OnEnable()
    // {
    //     LoggerInfo.Instance.PrintInfo("");
    //     scorePanel.SetActive(ModeMgr.Instance.EnableScore());
    //     multiPlayerButton.SetActive(ModeMgr.Instance.EnableMultiPlayer());
    //     if (ModeMgr.Instance.EnableScore())
    //     {
    //         myScoreBlock.Text = "";
    //         ClearScoreInfo();
    //     }
    // }



    public void ChangeChapter(int chater)
    {
        // if (ModeMgr.Instance.EnableScore())
        // {
        //     myScoreBlock.Text = "";
        //     ClearScoreInfo();
        //     if (allScore)
        //     {
        //         AllScoreRank();
        //     }
        //     else
        //     {
        //         FriendScoreRank();
        //     }

        //     HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "recordInfo/getHighestRecord?chapterId=" + GetChapterID()), HTTPMethods.Post, OnRequestMyBestScoreFinished);
        //     request.Cookies.Add(new Cookie(PlayerPrefs.GetString("cookieName", ""),
        //     PlayerPrefs.GetString("cookieValue", "")));
        //     request.Send();
        //     //Debug.Log(request.CurrentUri);
        // }
    }

    // void OnRequestMyBestScoreFinished(HTTPRequest req, HTTPResponse resp)
    // {
    //     try
    //     {
    //         if (resp.IsSuccess)
    //         {
    //             Debug.Log(resp.DataAsText);
    //             BestScoreInfo bestScoreInfo = JsonConvert.DeserializeObject<BestScoreInfo>(resp.DataAsText);
    //             if (bestScoreInfo.code == 200)
    //             {
    //                 myScoreBlock.Text = bestScoreInfo.data.ToString();
    //                 return;
    //             }
    //             LoggerInfo.Instance.PrintInfo(bestScoreInfo.info);
    //         }
    //         else
    //         {
    //             LoggerInfo.Instance.PrintInfo("获取最高分失败");
    //         }
    //     }
    //     catch
    //     {
    //         LoggerInfo.Instance.PrintInfo("获取最高分失败");
    //     }

    // }


    // public void ChangeRankMode(bool all)
    // {

    //     if (all == allScore)
    //     {
    //         return;
    //     }
    //     allScore = all;
    //     if (all)
    //     {
    //         AllScoreRank();
    //     }
    //     else
    //     {
    //         FriendScoreRank();
    //     }

    // }

    // public void ClearScoreInfo()
    // {
    //     foreach (var block in scoreBlocks)
    //     {
    //         block.Text = "";
    //     }
    // }
    // public string GetChapterID()
    // {
    //     return MainUIMgr.Instance.chapterID.ToString();
    // }
    // public void AllScoreRank()
    // {
    //     ClearScoreInfo();
    //     HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "recordInfo/rankingList?chapterId=" + GetChapterID() + "&listSize=10"), HTTPMethods.Post, OnRequestFinished);
    //     request.Cookies.Add(new Cookie(PlayerPrefs.GetString("cookieName", ""),
    //     PlayerPrefs.GetString("cookieValue", "")));

    //     //Debug.Log(request.Uri);
    //     request.Send();
    // }

    // void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    // {
    //     try
    //     {
    //         if (resp.IsSuccess)
    //         {

    //             ScoreInfo scoreInfo = JsonConvert.DeserializeObject<ScoreInfo>(resp.DataAsText);
    //             if (scoreInfo.code == 200)
    //             {
    //                 for (int i = 0; i < scoreInfo.data.Count; ++i)
    //                 {
    //                     SetScoreBlock(i + 1, scoreInfo.data[i].nickName, scoreInfo.data[i].score);
    //                 }
    //                 return;
    //             }
    //             LoggerInfo.Instance.PrintInfo(scoreInfo.info);
    //         }
    //         else
    //         {
    //             LoggerInfo.Instance.PrintInfo("获取排行榜失败");
    //         }
    //     }
    //     catch
    //     {
    //         LoggerInfo.Instance.PrintInfo("获取排行榜失败");
    //     }

    // }
    // public void FriendScoreRank()
    // {
    //     ClearScoreInfo();
    //     HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "recordInfo/friendRankingList?chapterId=" + GetChapterID() + "&listSize=10"), HTTPMethods.Post, OnFriendRequestFinished);
    //     request.Cookies.Add(new Cookie(PlayerPrefs.GetString("cookieName", ""),
    //      PlayerPrefs.GetString("cookieValue", "")));

    //     request.Send();
    // }

    // void OnFriendRequestFinished(HTTPRequest req, HTTPResponse resp)
    // {
    //     try
    //     {
    //         if (resp.IsSuccess)
    //         {
    //             ScoreInfo scoreInfo = JsonConvert.DeserializeObject<ScoreInfo>(resp.DataAsText);
    //             if (scoreInfo.code == 200)
    //             {
    //                 for (int i = 0; i < scoreInfo.data.Count; ++i)
    //                 {
    //                     SetScoreBlock(i + 1, scoreInfo.data[i].nickName, scoreInfo.data[i].score);
    //                 }
    //                 return;
    //             }
    //             LoggerInfo.Instance.PrintInfo(scoreInfo.info);

    //         }
    //         else
    //         {
    //             LoggerInfo.Instance.PrintInfo("获取好友排行榜失败");
    //         }
    //     }
    //     catch
    //     {
    //         LoggerInfo.Instance.PrintInfo("获取好友排行榜失败");
    //     }
    // }

    // public void SetMyScore(int score)
    // {
    //     myScoreBlock.Text = score.ToString();
    // }

    // public bool SetScoreBlock(int index, string name, int score)//index 1-10
    // {
    //     if (index >= 1 && index <= scoreBlocks.Length)
    //     {
    //         scoreBlocks[index - 1].Text = index.ToString() + "  " + name + "  " + score;
    //         return true;
    //     }
    //     return false;
    // }
}
