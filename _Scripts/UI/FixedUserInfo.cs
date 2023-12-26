using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using PixelCrushers.DialogueSystem;

public class FixedUserInfo : MonoBehaviour
{
    public TextBlock nickName;
    public TextBlock level;
    public UIBlock2D experienceBlock;
    // Start is called before the first frame update
    void Start()
    {
       nickName.Text=PlayerPrefs.GetString(Global.nickName,"湫湫湫");
       DialogueLua.SetVariable("playerName", nickName.Text);
      
        
       PlayerPrefs.SetInt(Global.experience,1560);
       PlayerPrefs.SetInt(Global.level,15);

        int levelt=PlayerPrefs.GetInt(Global.experience,0);
        float scaleX=(levelt%100);
        scaleX/=100;
        
        experienceBlock.Size.X.Percent=scaleX;
        levelt/=100;
        levelt+=1;//最低一级
        level.Text=levelt.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
