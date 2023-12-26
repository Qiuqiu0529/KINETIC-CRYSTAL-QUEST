using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class FinishTutorial : MonoBehaviour
{
    // Start is called before the first frame update
     public MMF_Player finishDiaFB;
     DetectNotice detectNotice;
     public bool hastrigger;
    void Start()
    {
        detectNotice=DetectNotice.Instance;
        detectNotice.normalCap+=NormalCap;
    }
    void OnDisable()
    {
        detectNotice.normalCap-=NormalCap;
    }
    public void NormalCap()
    {
        // if(hastrigger)
        // {
        //     return;
        // }
        // hastrigger=true;
        if(!finishDiaFB.IsPlaying)
        {
            finishDiaFB.PlayFeedbacks();
        }
        
    }

    public void Finish()
    {
        PlayerPrefs.SetInt(Global.begintutorial, 1);
    }
}
