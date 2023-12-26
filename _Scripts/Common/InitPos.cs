using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPos : Singleton<InitPos>
{
    public Transform startpos, endpos;
    //public float width, height;

    private new void Awake()
    {
        base.Awake();
        float w = 1280;
        float h = 720;
        float screenRatio = (float)UnityEngine.Screen.width / UnityEngine.Screen.height;

        if (screenRatio > 1) // width > height
        {
            if (screenRatio > (w / h))
            {
                float scaleRatio = screenRatio * h / w;
                startpos.localPosition=new Vector3(startpos.localPosition.x * scaleRatio, startpos.localPosition.y * scaleRatio,startpos.localPosition.z);
                endpos.localPosition=new Vector3(endpos.localPosition.x * scaleRatio, endpos.localPosition.y * scaleRatio,endpos.localPosition.z);
            }
        }
        else
        {
            if (screenRatio > (h / w))
            {
                float scaleRatio = screenRatio * w / h;
                startpos.localPosition=new Vector3(startpos.localPosition.x * scaleRatio, startpos.localPosition.y * scaleRatio,startpos.localPosition.z);
                endpos.localPosition=new Vector3(endpos.localPosition.x * scaleRatio, endpos.localPosition.y * scaleRatio,endpos.localPosition.z);
            }
        }
    }
}
