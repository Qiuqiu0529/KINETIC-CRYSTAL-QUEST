using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using MoreMountains.Feedbacks;
using BestHTTP;
using System;
using Newtonsoft.Json;


public class Register : MonoBehaviour
{
    public Login login;
    public Slider weightSlider;//
    public Slider heightSlider;//
    public Dropdown genderDropdown;//
    public TextBlock nameblock;
    public TextBlock registerPwd;
    public TextBlock veriPwd;
    public TextBlock nameblockPlaceHolder;
    public TextBlock registerPwdPlaceHolder;
    public TextBlock veriPwdPlaceHolder;

    public MMF_Player loginFB;
    int genderIndex = 0, weight = 60, height = 160;
    // Start is called before the first frame update
    void Start()
    {
        weightSlider.Value = PlayerPrefs.GetInt(Global.weight, 60);
        heightSlider.Value = PlayerPrefs.GetInt(Global.height, 160);
        genderDropdown.DropdownOptions.SelectedIndex = PlayerPrefs.GetInt(Global.gender, 0);
    }

    public void RegisterTest()
    {
        if (CheckInput())
        {
            string md5pwd = NetWorkUtil.Md5Sum(veriPwd.Text);
            HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "completeUserInfo?email=" + login.tempUser
             + "&nickName=" + nameblock.Text + "&password=" + md5pwd
             + "&height=" + height.ToString() + "&weight=" + weight.ToString()
             + "&sex=" + genderIndex.ToString()), HTTPMethods.Post, OnRequestFinished);
            request.Send();

        }
    }
    void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        try
        {
            if (resp.IsSuccess)
            {
                NullDatadInfo info = JsonConvert.DeserializeObject<NullDatadInfo>(resp.DataAsText);
                LoggerInfo.Instance.PrintInfo(info.info);
                if (info.code == 200)
                {
                    ReturnToLogin();
                    LoggerInfo.Instance.PrintInfo("注册成功，请登录！");

                }
            }
        }
        catch
        {
            LoggerInfo.Instance.PrintInfo("输入用户信息失败！");
        }

    }

    public void ReturnToLogin()
    {
        nameblock.Text = "";
        registerPwd.Text = "";
        veriPwd.Text = "";
        nameblockPlaceHolder.Visible = true;
        registerPwdPlaceHolder.Visible = true;
        veriPwdPlaceHolder.Visible = true;

        weightSlider.Value = PlayerPrefs.GetInt(Global.weight, 60);
        heightSlider.Value = PlayerPrefs.GetInt(Global.height, 160);
        genderDropdown.DropdownOptions.SelectedIndex = PlayerPrefs.GetInt(Global.gender, 0);
        login.GoToLogin();
    }


    public bool CheckInput()
    {
        if (nameblock.Text.Length < 3)
        {
            LoggerInfo.Instance.PrintInfo("名字应大于等于3位");
            return false;
        }
        if (registerPwd.Text.Length < 8)
        {
            LoggerInfo.Instance.PrintInfo("密码应大于等于8位");
            return false;
        }
        foreach (var num in registerPwd.Text)
        {
            if (!((num <= '9' && num >= '0') || (num <= 'Z' && num >= 'A') || (num <= 'z' && num >= 'a')))
            {
                LoggerInfo.Instance.PrintInfo("密码应由大小写字母、数字构成");
                return false;
            }
        }
        if (!(registerPwd.Text == veriPwd.Text))
        {
            LoggerInfo.Instance.PrintInfo("确认密码与初次填写密码不同");
            return false;
        }
        return true;

    }

    public void ChangeGenderDropdown(int index)
    {
        genderIndex = index;
    }
    public void ChangeWeightSlider(float amount)
    {
        weight = (int)amount;
        // PlayerPrefs.SetFloat(Global.weight, amount);
    }

    public void ChangeHeightSlider(float amount)
    {
        height = (int)amount;
        // PlayerPrefs.SetFloat(Global.height, amount);
    }

}
