using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;
using Nova;
using MoreMountains.Feedbacks;
using Newtonsoft.Json;
using BestHTTP.Cookies;
using BestHTTP.Authentication;

public class Login : MonoBehaviour
{

    public TextBlock user;
    public TextBlock pwd;
    public TextBlock userPlaceHolder;
    public TextBlock pwdPlaceHolder;
    public GameObject loginObj;
    public GameObject registerObj;
    public MMF_Player loginFB;
    string md5pwd;
    public string tempUser;

    float lastVeritime;
    bool pressVeriButton;

    private void Start()
    {
        TestGetUserInfo();
    }
    public void TestGetUserInfo()
    {
        if (PlayerPrefs.GetString("cookieValue", "") == "")
        {
            return;
        }
        string temp = NetWorkUtil.baseUrl + "getUserInfo";
        HTTPRequest request = new HTTPRequest(new Uri(temp), HTTPMethods.Post, OnGetUserInfoRequestFinished);
        request.Cookies.Add(new Cookie(PlayerPrefs.GetString("cookieName", ""),
        PlayerPrefs.GetString("cookieValue", "")));
        request.Send();
    }

    void OnGetUserInfoRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        try
        {
            if (resp.IsSuccess)
            {
                LoginInfo logininfo = JsonConvert.DeserializeObject<LoginInfo>(resp.DataAsText);
                if (logininfo == null)
                {
                    return;
                }
                if (logininfo.code == 200)
                {
                    if (logininfo.data == null)
                    {
                        return;
                    }
                    if (logininfo.data.nickName != null)
                    {
                        PlayerPrefs.SetString(Global.nickName, logininfo.data.nickName);
                        PlayerPrefs.SetString(Global.userId, logininfo.data.userId);
                        PlayerPrefs.SetInt(Global.gender, logininfo.data.sex);
                        PlayerPrefs.SetInt(Global.experience, logininfo.data.experience);
                        PlayerPrefs.SetInt(Global.weight, logininfo.data.weight);
                        PlayerPrefs.SetInt(Global.height, logininfo.data.height);
                        loginFB.PlayFeedbacks();
                    }
                }

            }
        }
        catch
        {
            //LoggerInfo.Instance.PrintInfo("发送失败！");
        }
    }



    public bool CheckPhoneNum()
    {
        if (user.Text.Length != 11)
        {
            LoggerInfo.Instance.PrintInfo("请输入11位手机号");
            return false;
        }
        if (user.Text[0] != '1')
        {
            LoggerInfo.Instance.PrintInfo("请输入有效手机号");
            return false;
        }
        foreach (var num in user.Text)
        {
            if (!(num <= '9' && num >= '0'))
            {
                LoggerInfo.Instance.PrintInfo("请输入有效手机号");
                return false;
            }
        }
        return true;
    }



    public void Verify()
    {
        // if (!CheckPhoneNum())
        // {
        //     return;
        // }

        if (!pressVeriButton)
        {
            pressVeriButton = true;
            lastVeritime = Time.time;
            RequestVerify();
        }
        else
        {
            if (Time.time - lastVeritime > 60f)
            {
                lastVeritime = Time.time;
                RequestVerify();
            }
            else
            {
                LoggerInfo.Instance.PrintInfo("60s内只能发送一次验证码");
            }
        }
    }

    public void RequestVerify()
    {
        HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "sendEmailCode?email=" + user.Text), HTTPMethods.Post, OnVerifyRequestFinished);
        // Debug.Log(request.CurrentUri);
        request.Send();
    }

    void OnVerifyRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        try
        {
            if (resp.IsSuccess)
            {
                NullDatadInfo info = JsonConvert.DeserializeObject<NullDatadInfo>(resp.DataAsText);
                LoggerInfo.Instance.PrintInfo(info.info);
                return;
            }
        }
        catch
        {
            LoggerInfo.Instance.PrintInfo("发送失败！");
        }
    }

    public void GotoRegister()
    {
        tempUser = user.Text;
        user.Text = "";
        pwd.Text = "";
        userPlaceHolder.Visible = true;
        pwdPlaceHolder.Visible = true;
        loginObj.SetActive(false);
        registerObj.SetActive(true);
        LoggerInfo.Instance.PrintInfo("");
    }

    public void GoToLogin()
    {
        loginObj.SetActive(true);
        registerObj.SetActive(false);
        LoggerInfo.Instance.PrintInfo("");
    }



    public void TestLogin()
    {
        if (pwd.Text.Length < 5)
        {
            LoggerInfo.Instance.PrintInfo("验证码应等于5位，密码应大于等于8位");
            return;
        }
        // if (!HTTPManager.IsCookiesEnabled)
        // {
        //     Debug.Log("IsCookiesEnabled");
        //     HTTPManager.IsCookiesEnabled = true;
        // }

        if (pwd.Text.Length == 5)//验证码
        {
            foreach (var num in pwd.Text)
            {
                if (!(num <= '9' && num >= '0'))
                {
                    LoggerInfo.Instance.PrintInfo("请输入有效验证码");
                    return;
                }
            }

            HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "registerOrLoginWithCode?email=" + user.Text + "&code=" + pwd.Text)
            , HTTPMethods.Post, OnRegisterOrLoginWithCodeRequestFinished);
            request.IsCookiesEnabled=true;
            BestHTTP.Cookies.CookieJar.Clear();

            request.Send();
            return;
        }

        if (pwd.Text.Length < 8)
        {
            LoggerInfo.Instance.PrintInfo("密码应大于等于8位");
            return;
        }
        else
        {
            md5pwd = NetWorkUtil.Md5Sum(pwd.Text);
            HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "loginWithPassword?email=" + user.Text + "&password=" + md5pwd), HTTPMethods.Post, OnRequestFinished);
            Debug.Log(request.CurrentUri);
            request.Send();
        }
        // GotoRegister();

    }
    void OnRegisterOrLoginWithCodeRequestFinished(HTTPRequest req, HTTPResponse resp)
    {

        if (resp.IsSuccess)
        {

            LoginInfo logininfo = JsonConvert.DeserializeObject<LoginInfo>(resp.DataAsText);
            if (logininfo == null)
            {
                return;
            }
            if (logininfo.code == 200)
            {
                if (logininfo.data == null)
                {
                    return;
                }

                if (logininfo.data.nickName == null)
                {
                    PlayerPrefs.SetString(Global.userPhone, user.Text);
                    GotoRegister();
                }
                else
                {
                    PlayerPrefs.SetString(Global.userPhone, user.Text);
                    PlayerPrefs.SetString(Global.nickName, logininfo.data.nickName);
                    PlayerPrefs.SetString(Global.userId, logininfo.data.userId);
                    PlayerPrefs.SetString(Global.userPwd, "");
                    PlayerPrefs.SetInt(Global.gender, logininfo.data.sex);
                    PlayerPrefs.SetInt(Global.experience, logininfo.data.experience);
                    PlayerPrefs.SetInt(Global.weight, logininfo.data.weight);
                    PlayerPrefs.SetInt(Global.height, logininfo.data.height);
                    loginFB.PlayFeedbacks();


                    string temp = resp.Cookies[0].Name;
                    PlayerPrefs.SetString(Global.cookieName, temp);
                    string temp1 = resp.Cookies[0].Value;
                    PlayerPrefs.SetString(Global.cookieValue, temp1);
                    
                    Debug.Log(temp+temp1);

                }
            }
            else
            {
                LoggerInfo.Instance.PrintInfo(logininfo.info);
            }
        }


    }

    void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    {

        if (resp.IsSuccess)
        {
            LoginInfo logininfo = JsonConvert.DeserializeObject<LoginInfo>(resp.DataAsText);
            if (logininfo == null)
            {
                return;
            }
            if (logininfo.code == 200)
            {


                PlayerPrefs.SetString(Global.userPhone, user.Text);
                PlayerPrefs.SetString(Global.nickName, logininfo.data.nickName);
                PlayerPrefs.SetString(Global.userId, logininfo.data.userId);
                PlayerPrefs.SetString(Global.userPwd, md5pwd);
                PlayerPrefs.SetInt(Global.gender, logininfo.data.sex);
                PlayerPrefs.SetInt(Global.experience, logininfo.data.experience);
                PlayerPrefs.SetInt(Global.weight, logininfo.data.weight);
                PlayerPrefs.SetInt(Global.height, logininfo.data.height);

                loginFB.PlayFeedbacks();
               

                //Debug.Log( BestHTTP.HTTPManager.IsCookiesEnabled);


                string temp = resp.Cookies[0].Name;
                Debug.Log(temp);
                PlayerPrefs.SetString(Global.cookieName, temp);
                string temp1 = resp.Cookies[0].Value;

                Debug.Log(temp1);
                PlayerPrefs.SetString(Global.cookieValue, temp1);

                // PlayerPrefs.SetString(Global.cookieName, resp.Cookies[0].Name);
                // PlayerPrefs.SetString(Global.cookieValue, resp.Cookies[0].Value);

            }
            else
            {
                LoggerInfo.Instance.PrintInfo(logininfo.info);
            }
        }


    }
}
