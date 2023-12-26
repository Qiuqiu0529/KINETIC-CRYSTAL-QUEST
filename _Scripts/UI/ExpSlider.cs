using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
public class ExpSlider : MonoBehaviour
{
    public TextBlock level;
    public CountTime countTime;

    public UIBlock2D processSlider;
    public UIBlock2D processBlock;
    public float duration = 0.5f;
    public float startTime;
    int expNow;
    int preExp;
    float preScale;
    float addScale;
    float finalScale;
    int levelt;

    public MMF_Player addExpFB,levelUpFB;

    // Start is called before the first frame update
    void Start()
    {
        //原始
        countTime = CountTime.Instance;

        int preExp = countTime.preExp;
        expNow = PlayerPrefs.GetInt(Global.experience, 0);
        Debug.Log(preExp);
        Debug.Log(expNow);

        // int preExp = 150;
        // expNow = 350;

        float scaleX = (preExp % 100);
        scaleX /= 100;
        processSlider.Size.X.Percent = scaleX;
        processBlock.Position.X.Percent = scaleX - 0.02f;


        levelt = preExp / 100;
        levelt += 1;//最低一级
        level.Text = levelt.ToString();

        startTime=Time.time;

        preScale=scaleX;
        addScale= expNow-preExp;
        addScale/=100;

        finalScale=preScale+addScale;
        preScale*=100;//0.01->1
        finalScale*=100;
        addExpFB.PlayFeedbacks();

        Debug.Log(preScale);
        Debug.Log(finalScale);
    }

    public void SetLevelNum()
    {
        level.Text = levelt.ToString();
    }

    public void ReSet()
    {
        startTime=Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        float xProgress = MMTweenDefinitions.EaseOut_Quadratic(t);
        //Debug.Log("xProgress"+xProgress);
        float tempScaleX=processSlider.Size.X.Percent;

        float newX = Mathf.Lerp(preScale, finalScale, xProgress);
        newX %=100;
        newX /=100;
        //Debug.Log("newX"+newX);
        processSlider.Size.X.Percent = newX ;
        if(tempScaleX>newX)
        {
            levelt++;
            Debug.Log("levelUP"+levelt);
            SetLevelNum();
            levelUpFB.PlayFeedbacks();
        }
        processBlock.Position.X.Percent = newX - 0.02f;

    }
}
