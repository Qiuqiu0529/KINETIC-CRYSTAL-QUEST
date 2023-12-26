using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System;

public class TextMgr : Singleton<TextMgr>
{
    public TextTable textTable;
    public string language;
    public event Action changeText;


    private void Start()
    {
        language=PlayerPrefs.GetString(Global.languageSet,Global.chinese);
        DialogueManager.SetLanguage( language);
        changeText?.Invoke();
    }

    public void SetChinese()
    {
        
        language=Global.chinese;
        PlayerPrefs.SetString(Global.languageSet,language);
        DialogueManager.SetLanguage( language);
        changeText?.Invoke();
        
    }
    public void SetEnglish()
    {
        language=Global.english;
        PlayerPrefs.SetString(Global.languageSet,language);
        DialogueManager.SetLanguage( language);
        changeText?.Invoke();
    }

    public string GetText(string textID)
    {
        return textTable.GetFieldTextForLanguage(textID, language);
    }
}
