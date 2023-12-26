using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Community : Singleton<Community>
{
    UniWebView webView;
    bool isopen;
    private void OnEnable()
    {
        UniWebView.SetCookie(NetWorkUtil.webUrl, PlayerPrefs.GetString(Global.cookieName, "") + "=" + PlayerPrefs.GetString(Global.cookieValue, ""));
    }

    public void LoadHttp()
    {
        // LoggerInfo.Instance.PrintInfo("网站公安局备案中，暂无法访问");
        // return;

        // if(isopen)
        // {
        //     return;
        // }
        //  isopen=true;
        // webView = gameObject.AddComponent<UniWebView>();
      
        // webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        // webView.Load(ModeMgr.Instance.Gameurl());

        // webView.Show();
        // webView.EmbeddedToolbar.Show();
        // MainUIMgr.Instance.OpenCommunity();
        // webView.OnShouldClose += (view) =>
        // {
        //     MainUIMgr.Instance.CloseCommunity(); 
        //    // LoggerInfo.Instance.AddInfo("return from web");
        //     isopen=false;
        //     webView = null;
        //     return true;
        // };
       
    }

}
