using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using System;
using BestHTTP;
using BestHTTP.Cookies;
using Newtonsoft.Json;
public class Friend : MonoBehaviour
{
    public TextBlock friendCode;
    public TextBlock myFriendCode;

    public void AddFriend()
    {
        string code = friendCode.Text;
        HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "friendshipInfo/addFriend?friendCode=" + code), HTTPMethods.Post, OnRequestFinished);
        // Debug.Log(request.CurrentUri);
        request.Cookies.Add(new Cookie(PlayerPrefs.GetString("cookieName", ""),
        PlayerPrefs.GetString("cookieValue", "")));
        //Debug.Log(PlayerPrefs.GetString("cookieName", "") + PlayerPrefs.GetString("cookieValue", ""));
        request.Send();
    }

    void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        try
        {
            if (resp.IsSuccess)
            {
                NullDatadInfo friendInfo = JsonConvert.DeserializeObject<NullDatadInfo>(resp.DataAsText);
                LoggerInfo.Instance.PrintInfo(friendInfo.info);
            }
        }
        catch
        {
            LoggerInfo.Instance.PrintInfo("添加失败！");
        }

    }

    private void Start()
    {

        string temp = PlayerPrefs.GetString(Global.friendCode, "");
        if (temp.Length > 0)
        {
            myFriendCode.Text = temp;
        }
        else
        {
            string userId = PlayerPrefs.GetString(Global.userId, "");
            string result = NetWorkUtil.StringToBase64(userId);
            PlayerPrefs.SetString(Global.friendCode, result);
            myFriendCode.Text = result;

        }

    }

}
