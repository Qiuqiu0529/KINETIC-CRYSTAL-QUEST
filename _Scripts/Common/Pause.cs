using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;
public class Pause : Singleton<Pause>
{

    bool autoReturn, autoPause;
    public float autoPauseTime, autoReturnTime;

    public MMF_Player pauseFB, returnFB;

    public float lastTrackTime ;



    void Start()
    {
       // autoPauseTime = PlayerPrefs.GetFloat(Global.autoPauseTime, 10f);
        autoReturnTime = PlayerPrefs.GetFloat(Global.autoReturnTime, 10f);
        Debug.Log(autoReturnTime);
       // autoPause = autoPauseTime < 100 ? true : false;
        autoReturn = autoReturnTime < 100 ? true : false;
    }



    public void AutoReturn()
    {
        StartCoroutine(AutoReturnIEnum());
    }



    public IEnumerator AutoReturnIEnum()
    {
        if (autoReturn)
        {
            yield return new WaitForSeconds(autoReturnTime);
            returnFB.PlayFeedbacks();
        }
        yield return 0;
    }

}
