using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using BestHTTP;
using BestHTTP.Cookies;
using Newtonsoft.Json;
using Nova;
using Nova.TMP;
using UnityEngine.Events;
public class CountTime : Singleton<CountTime>
{
    public float startTime;
    public int score;
    public int addexperience;
    public int preExp;
    public int rank;
    public int baseExperience;

    public bool useOppoWatch;

    public int startCal, endCal;

    //时间、卡路里和经验为通用

    [SerializeField] TextMeshProTextBlock timeUI;
    [SerializeField] TextMeshProTextBlock calorieUI;
    [SerializeField] TextMeshProTextBlock experience;

    int duration;
    int cal;

    public UnityEvent getCal;

    void Start()
    {
        startTime = Time.time;
        rank = -1;
        useOppoWatch = PlayerPrefs.GetInt(Global.useOppoWatch, 0) > 0 ? true : false;
        if (useOppoWatch)
        {
            SetStartCal();
        }
        preExp= PlayerPrefs.GetInt(Global.experience, 0);

    }
    public void SetScore(int amout)
    {
        score = amout;
    }

    public void SetRank(int amout)
    {
        rank = amout;
    }

    public void SetStartCal()
    {
        getCal?.Invoke();
        StartCoroutine(WaitStartCal());

    }

    public IEnumerator WaitStartCal()
    {
        yield return new WaitForSeconds(1);
        int calNow = PlayerPrefs.GetInt(Global.calNow, 0);
        if (PlayerPrefs.GetInt(Global.calRead, 0) == 1 &&
       calNow > 0)
        {
            PlayerPrefs.SetInt(Global.calRead, 0);
            startCal = calNow;
        }
        else
        {
            useOppoWatch = false;
        }
    }

    public void SetEndCal()
    {
        getCal?.Invoke();
        StartCoroutine(WaitEndCal());
    }

    public IEnumerator WaitEndCal()
    {
        yield return new WaitForSeconds(1);
        int calNow = PlayerPrefs.GetInt(Global.calNow, 0);
        if (PlayerPrefs.GetInt(Global.calRead, 0) == 1 &&
       calNow > 0)
        {
            PlayerPrefs.SetInt(Global.calRead, 0);
            endCal = calNow;
            cal = endCal - startCal;
            calorieUI.text = cal.ToString() + " kcal";
            //SendRecord();
        }
        else
        {
            useOppoWatch = false;
            DefaultCal();
        }
    }

    public void DefaultCal()
    {
        cal = Mathf.FloorToInt(CalorieUtil.CalCalorie(duration, rank));
        calorieUI.text = cal.ToString() + " kcal";//*千卡为单位
        //SendRecord();

    }


    public void EndCountTime()
    {
        duration = (int)(Time.time - startTime);
        Debug.Log("endCountTime");

        Pause.Instance.AutoReturn();//自动返回

        int second = Mathf.FloorToInt(duration % 60);
        int minute = Mathf.FloorToInt(duration / 60);
        timeUI.text = minute.ToString() + " : " + second.ToString();
        if (rank != -1)
        {
            addexperience = (1 + minute / 2) * (rank * 20);
        }
        else
        {
            addexperience = (minute / 2) * (baseExperience);//自由行动
        }

        //addexperience=120;//待注释


        experience.text = addexperience.ToString();

        

        PlayerPrefs.SetInt(Global.experience, preExp + addexperience);

        cal = 0;
        if (!useOppoWatch)
        {
            DefaultCal();
        }
        else
        {
            SetEndCal();

        }

    }
    public void SendRecord()
    {
        HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "recordInfo/addRecord?chapterId=" + ModeMgr.Instance.GetChapterID() +
      "&score=" + score + "&calories=" + cal + "&experience=" + addexperience + "&duration=" + duration), HTTPMethods.Post, OnRequestFinished);
        request.Cookies.Add(new Cookie(PlayerPrefs.GetString("cookieName", ""),
        PlayerPrefs.GetString("cookieValue", "")));
        Debug.Log(request.Uri);
        request.Send();
    }


    void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        if (resp.IsSuccess)
        {
            LoggerInfo.Instance.PrintInfo("上传运动记录成功");

        }
        else
        {
            LoggerInfo.Instance.PrintInfo("上传运动记录失败");
        }

    }

}
