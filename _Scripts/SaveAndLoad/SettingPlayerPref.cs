using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using BestHTTP;
using System;
using MoreMountains.Feedbacks;
using Newtonsoft.Json;
using BestHTTP.Cookies;

public class SettingPlayerPref : MonoBehaviour
{
    bool change = false;
    bool changeWeight, changeHeight, changeGender;

    public MySlider weightSlider;//
    public MySlider heightSlider;//
    public MyDropdown genderDropdown;//


    public MyToggle cameraToggle;//√ WebCamSource WebCamInput  Start() 默认开启

    public MyDropdown modelDropdown;//PoseTrackingGraph    HolisticTrackingGraph \Awake 

    //public Toggle BGMToggle;//待定

    public MyDropdown languageDropdown;

    public MyToggle motionUIToggle;//√ myinputmgr  Start() 默认关闭
                                 // public Slider autoPauseTimeSlider;//ballnote SetPendingEvt()  
    
    public MySlider autoReturnTimeSlider;//ballnote SetPendingEvt()

    public MySlider squashGestureLengthSlider;
    public MySlider ballRunKneeDisSlider;
    public MySlider jumpDisSlider;

    public MyToggle blendShapeToggle;//motionface awake
    public MyToggle handTrackToggle;//motionLhand 、motionRHand awake
    public MyToggle fixedHipToggle;//motionpose

    
    public MyToggle ballChannelmixer;//motionpose



    public MySlider ballDelaySlider;//ballnote SetPendingEvt()


    public float ballspeed;//2-7(),待定

    public Dropdown ballTargetDropdown;//√ BallTarget Awake

    public MySlider starDelaySlider;//starnote  SetPendingEvt()

    public MySlider pacmanDelaySlider;//beannote  SetPendingEvt()

    public Dropdown pacmanTargetDropdown;//√ PacmanTarget Awake



    public MySlider taichiDelaySlider;

    private void Awake()
    {
        weightSlider.Value = PlayerPrefs.GetInt(Global.weight, 60);
        heightSlider.Value = PlayerPrefs.GetInt(Global.height, 160);
        genderDropdown.DropdownOptions.SelectedIndex = PlayerPrefs.GetInt(Global.gender, 0);

        cameraToggle.toggledOn = PlayerPrefs.GetInt(Global.frontCamera, 1) > 0 ;//默认前置摄像头
        //BGMToggle.toggledOn = PlayerPrefs.GetInt(Global.useBGM, 1) > 0 ? true : false;
        motionUIToggle.toggledOn = PlayerPrefs.GetInt(Global.useMotionUI, 1) > 0 ;//默认true
        //
        
        ballChannelmixer.toggledOn=PlayerPrefs.GetInt(Global.ballChanelMixer, 1) > 0 ;                                                                                         //headDecToggle.toggledOn=PlayerPrefs.GetInt(Global.useHeadDec, 0) > 0 ? true : false;

        // //autoPauseTimeSlider.Value = PlayerPrefs.GetFloat(Global.autoPauseTime, 10f);
        autoReturnTimeSlider.Value = PlayerPrefs.GetFloat(Global.autoReturnTime, 10f);
        
        squashGestureLengthSlider.Value=PlayerPrefs.GetFloat(Global.squashGestureLength, 0.35f);
        ballRunKneeDisSlider.Value=PlayerPrefs.GetFloat(Global.ballRunKneeDis, 0.03f);
        jumpDisSlider.Value=PlayerPrefs.GetFloat(Global.jumpDis, 0.15f);

        blendShapeToggle.toggledOn = PlayerPrefs.GetInt(Global.useBlendShape, 1) > 0;//默认true
        handTrackToggle.toggledOn = PlayerPrefs.GetInt(Global.useHandTrack, 1) > 0 ;//默认true
        fixedHipToggle.toggledOn = PlayerPrefs.GetInt(Global.fixedHip, 0) > 0 ;//默认false


        // starDelaySlider.Value = PlayerPrefs.GetFloat(Global.starDelay, 0f);
        // pacmanDelaySlider.Value = PlayerPrefs.GetFloat(Global.pacmanDelay, 0f);
        // taichiDelaySlider.Value = PlayerPrefs.GetFloat(Global.taichiDelay, 0f);
        // ballDelaySlider.Value = PlayerPrefs.GetFloat(Global.ballDelay, 0f);

        modelDropdown.DropdownOptions.SelectedIndex = PlayerPrefs.GetInt(Global.modelComplexity, 1);//默认full,中
        languageDropdown.DropdownOptions.SelectedIndex=PlayerPrefs.GetInt(Global.languageSetIndex, 0);

        // ballTargetDropdown.DropdownOptions.SelectedIndex = PlayerPrefs.GetInt(Global.ballTarget, 1);//默认脖子
        // pacmanTargetDropdown.DropdownOptions.SelectedIndex = PlayerPrefs.GetInt(Global.pacmanTarget, 1);

    }

    #region  dropdown
    public void ChangeGenderDropdown(int index)
    {
        changeGender = true;
        PlayerPrefs.SetInt(Global.gender, index);
    }

    public void ChangeLanguageDropdown(int index)
    {
        PlayerPrefs.SetInt(Global.languageSetIndex, index);
        if(index>0)
        {
            TextMgr.Instance.SetEnglish();
        }
        else
        {
            TextMgr.Instance.SetChinese();
        }
    }

    public void ChangeModelDropdown(int index)
    {
        PlayerPrefs.SetInt(Global.modelComplexity, index);//012
    }

    public void ChangeBallTargetDropdown(int index)
    {
        PlayerPrefs.SetInt(Global.ballTarget, index);
    }
    public void ChangePacmanTargetDropdown(int index)
    {
        PlayerPrefs.SetInt(Global.pacmanTarget, index);
    }

    public void ChangeQuestion1TargetDropdown(int index)
    {
        PlayerPrefs.SetInt(Global.question1Target, index);
    }

    public void ChangeQuestion2TargetDropdown(int index)
    {
        PlayerPrefs.SetInt(Global.question2Target, index);
    }

    #endregion


    #region toggle

    public void ChangeBallChannelMixer(bool toggled)
    {
        PlayerPrefs.SetInt(Global.ballChanelMixer, toggled ? 1 : 0);//true -1 ,false-0
    }


    public void ChangeCameraToggle(bool toggled)
    {
        PlayerPrefs.SetInt(Global.frontCamera, toggled ? 1 : 0);//true -1 ,false-0
    }

    public void ChangeMotionUIToggle(bool toggled)
    {
        PlayerPrefs.SetInt(Global.useMotionUI, toggled ? 1 : 0);//true -1 ,false-0
    }
    public void ChangeHeadDecToggle(bool toggled)
    {
        PlayerPrefs.SetInt(Global.useHeadDec, toggled ? 1 : 0);//true -1 ,false-0
    }
    public void ChangeBlendShapeToggle(bool toggled)
    {
        PlayerPrefs.SetInt(Global.useBlendShape, toggled ? 1 : 0);//true -1 ,false-0
    }
    public void ChangeHandTrackToggle(bool toggled)
    {
        PlayerPrefs.SetInt(Global.useHandTrack, toggled ? 1 : 0);//true -1 ,false-0
    }
    public void ChangeFixedHipToggle(bool toggled)
    {
        PlayerPrefs.SetInt(Global.fixedHip, toggled ? 1 : 0);//true -1 ,false-0
    }

    #endregion


    #region  slider

    public void ChangeSquashGestureLength(float amount)
    {
        
        PlayerPrefs.SetFloat(Global.squashGestureLength, amount);
    }

    public void ChangeBallKneeDis(float amount)
    {
        
        PlayerPrefs.SetFloat(Global.ballRunKneeDis, amount);
    }
     public void ChangeJumpDis(float amount)
    {
        
        PlayerPrefs.SetFloat(Global.jumpDis, amount);
    }

    public void ChangeWeightSlider(float amount)
    {
        changeWeight = true;
        PlayerPrefs.SetInt(Global.weight, (int)amount);
    }

    public void ChangeHeightSlider(float amount)
    {
        changeHeight = true;
        PlayerPrefs.SetInt(Global.height, (int)amount);
    }
    public void ChangeAutoPauseTimeSlider(float amount)
    {
        PlayerPrefs.SetFloat(Global.autoPauseTime, amount);
    }

    public void ChangeAutoReturnTimeSlider(float amount)
    {
        PlayerPrefs.SetFloat(Global.autoReturnTime, amount);
    }

    public void ChangeBallDelaySlider(float amount)
    {
        PlayerPrefs.SetFloat(Global.ballDelay, amount);
    }

    public void ChangeStarDelaySlider(float amount)
    {
        PlayerPrefs.SetFloat(Global.starDelay, amount);
    }
    public void ChangeTaichiDelaySlider(float amount)
    {
        PlayerPrefs.SetFloat(Global.taichiDelay, amount);
    }

    public void ChangePacmanDelaySlider(float amount)
    {
        PlayerPrefs.SetFloat(Global.pacmanDelay, amount);
    }

    #endregion

    public void Close()//关闭setting界面，这里加上些检测，
    {  //比如之前没有开启手势控制UI（看myinputmgr是否navigateenable）这里开启（先看识别再），
        change = true;//点击setting panel back按钮时调
        MyInputMgr.Instance.ChangeMotionControl(PlayerPrefs.GetInt(Global.useMotionUI, 0) > 0 ? true : false);
    }

    private void OnDestroy()
    {
        if (changeHeight || changeWeight || changeGender)//服务器修改数据,待改
        {
            // HTTPRequest request = new HTTPRequest(new Uri(NetWorkUtil.baseUrl + "updateUserInfo?"  
            // + "height=" + PlayerPrefs.GetInt(Global.height, 160).ToString() 
            // + "&weight=" + PlayerPrefs.GetInt(Global.weight, 60).ToString() 
            //  + "&sex=" + PlayerPrefs.GetInt(Global.gender,0).ToString()), HTTPMethods.Post, OnRequestFinished);
            // // Debug.Log(request.CurrentUri);
            // request.Cookies.Add(new Cookie(PlayerPrefs.GetString("cookieName", ""),
            // PlayerPrefs.GetString("cookieValue", "")));
            // //Debug.Log(PlayerPrefs.GetString("cookieName", "") + PlayerPrefs.GetString("cookieValue", ""));
            // request.Send();
        }
    }

    public void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
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


    public void Save()
    {
        PlayerPrefs.Save();
    }


}
