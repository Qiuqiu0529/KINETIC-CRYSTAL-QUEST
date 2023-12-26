using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class EnterScene : MonoBehaviour
{
  public MMF_Player mainFB, introFB;

  void Awake()
  {
    PlayerPrefs.SetInt(Global.unlockNone, 1) ;
    PlayerPrefs.SetInt(Global.unlockMocap, 1) ;

    // PlayerPrefs.SetInt(Global.unlockStar, 0) ;
    // PlayerPrefs.SetInt(Global.unlockTaichi, 0) ;

    if (PlayerPrefs.GetInt(Global.begintutorial, 0) > 0 )
    {
      mainFB.PlayFeedbacks();
      
    }
    else
    {
      //PlayerPrefs.SetInt(Global.begintutorial, 1);
      introFB.PlayFeedbacks();
    }
  }
}
