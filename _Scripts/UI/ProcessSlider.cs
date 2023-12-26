using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;

public class ProcessSlider : Singleton<ProcessSlider>
{
    public float totalTime = 1;
    public float timeCountNow;
    public UIBlock2D processSlider;
    public UIBlock2D processBlock;
    public bool useProcessSlider = true;
    [SerializeField]bool timeProcessInit = false;
    
    public int totalActionCount=1;
    public int actionNow=0;
     [SerializeField]bool actionProcessInit=false;

    public void SetTotalAction(int totalA)
    {
        totalActionCount=totalA;
        actionProcessInit=true;
        actionNow=0;
    }

    public void AddAction()
    {
        actionNow++;
        float scale=(float)actionNow / totalActionCount;
        processSlider.Size.X.Percent = scale;
        processBlock.Position.X.Percent = scale - 0.02f;
    }

    public void SetTotal(float totalT)
    {
        totalTime = totalT;
        timeProcessInit = true;
    }


    private void Start()
    {
        if (!useProcessSlider)
        {
            this.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (!timeProcessInit)
            return;
        else
        {
            timeCountNow += Time.deltaTime;
            float scale = timeCountNow / totalTime;
            if (scale <= 1)
            {
                SetProcess(scale);
            }
        }

    }


    public void SetProcess(float scale)
    {
        processSlider.Size.X.Percent = scale;
        processBlock.Position.X.Percent = scale - 0.02f;
    }

}
