using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;
using MoreMountains.Feedbacks;
using Newtonsoft.Json;
using BestHTTP.Cookies;


public class NetWorkUtil
{
    public static string webUrl = "http://aidong.fun";
    public static string baseUrl = "http://aidong.fun/api/";
    public static string Md5Sum(string input)
    {
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("x2"));//小写 
        }
        return sb.ToString();
    }

    public static string StringToBase64(string input)
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        string result = System.Convert.ToBase64String(inputBytes);

        return result;
    }
    public static void LogOut()
    {
        PlayerPrefs.SetInt(Global.weight, 60);
        PlayerPrefs.SetInt(Global.height, 160);
        PlayerPrefs.SetInt(Global.gender, 0);
        PlayerPrefs.SetString(Global.cookieName, "");
        PlayerPrefs.SetString(Global.cookieValue, "");
        PlayerPrefs.SetString(Global.nickName, "");
        PlayerPrefs.SetString(Global.userId, "");
        PlayerPrefs.SetString(Global.userPwd, "");
        PlayerPrefs.SetString(Global.friendCode, "");
        PlayerPrefs.SetInt(Global.experience, 0);
        PlayerPrefs.SetString(Global.userPhone, "");
        PlayerPrefs.SetInt(Global.experience, 0);
        HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "logout"), HTTPMethods.Post);
        Debug.Log(request.CurrentUri);
        request.Send();
    }

}
