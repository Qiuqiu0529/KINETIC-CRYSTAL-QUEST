using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
public class MyPanel : MonoBehaviour
{
    public MyInputMgr myInputMgr;
    public UIBlock firstNavi;
    
    public UIBlock closeNav;

    public void SetInitNavUI()
    {
        if (myInputMgr.NavigationEnabled)//支持导航
        {
            myInputMgr.ChangeNavigationTo(firstNavi);
        }

    }

    private void OnEnable()
    {
        myInputMgr = MyInputMgr.Instance;
        StartCoroutine(RecoverInput());
        SetInitNavUI();
    }
    public void OnDisable()
    {
        if(closeNav!=null)
        {
            if (myInputMgr.NavigationEnabled)//支持导航
        {
            myInputMgr.ChangeNavigationTo(closeNav);
        }

        }
    }

    IEnumerator RecoverInput()
    {
        yield return new WaitForSeconds(1f);
        myInputMgr.RecoverInput();
    }
}
