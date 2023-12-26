using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NovaSamples.UIControls;
using PixelCrushers.DialogueSystem;
using MoreMountains.Feedbacks;
public class NamePlayer : MonoBehaviour
{
    public TextMeshPro nameBlock,hintBlock;
    public TextField textField;
    public MMF_Player toXingxiangSettingScene;

    void Start()
    {
        textField.OnTextChanged+=ChangeName;
    }
    
    public void ChangeName()
    {
        if(nameBlock.text.Length>6)
        {
            hintBlock.text="名字太长了，记不住呀！";
        }
        else if(nameBlock.text.Length<=0)
        {
            hintBlock.text="还不知道你叫什么……";
        }
        else 
        {
            hintBlock.text="真是个好名字！";
        }
    }

    public bool CheckName()
    {
        if(nameBlock.text.Length>8)
        {
        }
        else if(nameBlock.text.Length<=0)
        {
        }
        else 
        {
            return true;
        }
        return false;
    }

    public void VerifyName()
    {
        if(CheckName())
        {
            Debug.Log("luatest");
            PlayerPrefs.SetString(Global.nickName, nameBlock.text);
            DialogueLua.SetVariable("playerName", nameBlock.text);
            
            Debug.Log(DialogueLua.GetVariable("playerName").AsString);
            toXingxiangSettingScene.PlayFeedbacks();
        }
    }
    
}
