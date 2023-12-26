using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SelectUIState
{
    public void EnterSelectMode();
    public void LeaveSelectMode();
}

public class ModeSelectState : SelectUIState
{
    public void EnterSelectMode()
    {
        MainUIMgr.Instance.OpenModePanel();
    }
    public void LeaveSelectMode()
    {
        MainUIMgr.Instance.CloseModePanel();
    }
}

public class ChapterSelectState : SelectUIState
{
    public void EnterSelectMode()
    {
        MainUIMgr.Instance.OpenChapterPanel();
    }
    public void LeaveSelectMode()
    {
        MainUIMgr.Instance.CloseChapterPanel();
    }
}

public class SettingSelectState : SelectUIState
{
    public void EnterSelectMode()
    {
        MainUIMgr.Instance.OpenSettingPanel();
    }
    public void LeaveSelectMode()
    {
    }
}