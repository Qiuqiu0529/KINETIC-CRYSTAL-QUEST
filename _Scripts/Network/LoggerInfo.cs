using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using BestHTTP;
using BestHTTP.Cookies;
using System;

public class LoggerInfo : Singleton<LoggerInfo>
{
    public TextBlock loggerblock;
    public void PrintInfo(string info)
    {
        loggerblock.Text = info;
    }
    public void AddInfo(string info)
    {
        loggerblock.Text += info;
    }

    // private void Start()
    // {
    //     //TestGetUserInfo();
    // }

    
}
