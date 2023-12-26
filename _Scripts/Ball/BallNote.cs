using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class BallNote : CreateNoteEvent
{
    public float speed = 3f;
    public float distance = 15f;
    [SerializeField] float gridx = 0.5f;
    public override void CreateNote(KoreographyEvent evt)
    {
        string info = evt.GetTextValue();//分解text
        string[] notes = info.Split('/');
        for (int i = 0; i < notes.Length; ++i)
        {
            PooledObject pooledObject = objectPools[notes[i][0]-'A'].GetPoolObj();
            float.TryParse(notes[i].Substring(1), out float num);
            // Debug.Log(num);
            pooledObject.GetComponent<Ball>().SetZspeed(speed);
            pooledObject.transform.position = new Vector3(transform.position.x + gridx * num, transform.position.y, transform.position.z);
        }
    }

    public override void SetPendingEvt()
    {
      
        customDelayTime=PlayerPrefs.GetFloat(Global.ballDelay, 0f);
       
        //  Debug.Log( customDelayTime);
        toTargetTime = (distance / speed)-delayTime-customDelayTime;
    //    Debug.Log(toTargetTime);
        base.SetPendingEvt();
        // Debug.Log("SetPendingEvt()  rawEvents.Count"+rawEvents.Count+"   pendingEvt   "+pendingEvt);
        // Debug.Log(rawEvents[pendingEvt].StartSample);
    }


    public new void TestAllEvt()//测试所有音符是否能正常生成
    {
        base.TestAllEvt();
        //     TextPayload textPayload=new TextPayload();//自己增加事件的示例
        //    textPayload.TextVal="test";
        //    KoreographyEvent testevt=new KoreographyEvent();
        //    testevt.Payload=textPayload;
        //    testevt.StartSample=110000;
        //    KoreographyTrackBase rhythmTrack =playingKoreo.GetTrackByID(eventID);
        //    rhythmTrack.AddEvent(testevt);
        for (int i = 0; i < rawEvents.Count; ++i)
        {
            if (!rawEvents[i].HasTextPayload())
            {
                Debug.Log("第 " + i + "个事件不是text型参数,startsample为 " + rawEvents[i].StartSample);
                continue;
            }
            string rawinfo = rawEvents[i].GetTextValue();//分解text
            if (rawinfo.Length == 0)
            {
                Debug.Log("第 " + i + "个事件text型参数为空,startsample为 " + rawEvents[i].StartSample);
                continue;
            }
            string info = rawinfo.Trim();
            if (info.Length < rawinfo.Length)
            {
                TextPayload textPayload = new TextPayload();//自己增加事件的示例
                textPayload.TextVal = info;
                rawEvents[i].Payload = textPayload;
                Debug.Log("第 " + i + "个事件存在空格,自动改了，startsample为 " + rawEvents[i].StartSample);
            }

            string[] notes = info.Split('/');
            for (int j = 0; j < notes.Length; ++j)
            {
                if (!(notes[j][0] >= 'A' && notes[j][0] <= 'B'))
                {
                    if ((notes[j][0] >= 'a' || notes[j][0] <= 'b'))
                    {
                        string correctnote=notes[j];
                        correctnote=correctnote.Replace('a','A');
                        correctnote=correctnote.Replace('b','B');

                        TextPayload textPayload = new TextPayload();//自己增加事件的示例
                        info=info.Replace(notes[j],correctnote);
                        textPayload.TextVal = info;
                        rawEvents[i].Payload = textPayload;
                        Debug.Log("第 " + i + "个事件的第" + j + "个音符种类为小写，已改正,startsample为 " + rawEvents[i].StartSample);

                    }
                    else
                    {
                        Debug.Log("第 " + i + "个事件的第" + j + "个音符种类有问题，非A或B,startsample为 " + rawEvents[i].StartSample);
                    }

                }

                if (!float.TryParse(notes[j].Substring(1), out float num))
                {
                    Debug.Log("第 " + i + "个事件的第" + j + "个音符位置有问题，无法分解出float型参数,startsample为 " + rawEvents[i].StartSample);
                }
            }
        }
    }


}

