using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class StarNote : CreateNoteEvent
{
    [SerializeField] float starDelay = 0;
    [SerializeField] float gridx = 0.5f, gridy = 0.5f;
    public override void CreateNote(KoreographyEvent evt)
    {
        string info = evt.GetTextValue();//分解text
        string[] notes = info.Split('/');
        for (int i = 0; i < notes.Length; ++i)
        {
            PooledObject pooledObject = objectPools[notes[i][0] - 'A'].GetPoolObj();
            string[] notepos = notes[i].Split(',');
            float.TryParse(notepos[0].Substring(1), out float numx);
            float.TryParse(notepos[1], out float numy);
            pooledObject.transform.position = new Vector3(transform.position.x + gridx * numx, transform.position.y + gridy * numy, transform.position.z);
            pooledObject.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-180, 180));
        }
    }
    public override void SetPendingEvt()
    {
        customDelayTime = PlayerPrefs.GetFloat(Global.starDelay, 0f);
        toTargetTime = starDelay - delayTime - customDelayTime;
        base.SetPendingEvt();
        // Debug.Log(toTargetTime);
        // Debug.Log("SetPendingEvt()  rawEvents.Count"+rawEvents.Count+"   pendingEvt   "+pendingEvt);
        // Debug.Log(rawEvents[pendingEvt].StartSample);
    }
    public new void TestAllEvt()//测试所有音符是否能正常生成
    {
        base.TestAllEvt();
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
            string comatest = info;
            comatest = comatest.Replace('，', ',');
            if (comatest != info)
            {
                info = comatest;
                TextPayload textPayload = new TextPayload();//自己增加事件的示例
                textPayload.TextVal = info;
                rawEvents[i].Payload = textPayload;
                Debug.Log("第 " + i + "个事件逗号为中文,自动改了，startsample为 " + rawEvents[i].StartSample);
            }
            string[] notes = info.Split('/');
            for (int j = 0; j < notes.Length; ++j)
            {
                if (!(notes[j][0] >= 'A' && notes[j][0] <= 'D'))
                {
                    if ((notes[j][0] >= 'a' || notes[j][0] <= 'd'))
                    {
                        string correctnote = notes[j];
                        correctnote = correctnote.Replace('a', 'A');
                        correctnote = correctnote.Replace('b', 'B');
                        correctnote = correctnote.Replace('c', 'C');
                        correctnote = correctnote.Replace('d', 'D');
                        TextPayload textPayload = new TextPayload();//自己增加事件的示例
                        info = info.Replace(notes[j], correctnote);
                        textPayload.TextVal = info;
                        rawEvents[i].Payload = textPayload;
                        Debug.Log("第 " + i + "个事件的第" + j + "个音符种类为小写，已改正,startsample为 " + rawEvents[i].StartSample);

                    }
                    else
                    {
                        Debug.Log("第 " + i + "个事件的第" + j + "个音符种类有问题，非A、B、C、D,startsample为 " + rawEvents[i].StartSample);
                    }
                }

                string[] notepos = notes[j].Split(',');
                if (!float.TryParse(notepos[0].Substring(1), out float numx))
                {

                    Debug.Log("第 " + i + "个事件的第" + j + "个音符位置有问题，无法分解出float型参数,startsample为 " + rawEvents[i].StartSample);
                }

                if (!float.TryParse(notepos[1], out float numy))
                {
                    Debug.Log("第 " + i + "个事件的第" + j + "个音符位置有问题，无法分解出float型参数,startsample为 " + rawEvents[i].StartSample);
                }
                // numx += Random.Range(-0.3f, 0.3f);

                // numy += Random.Range(-0.3f, 0.3f);

                // string tempnote = notes[j][0] + numx.ToString("#0.00") + "," + numy.ToString("#0.00");
                // Debug.Log(tempnote);
                // notes[j]=tempnote;
            }

            // string changetemp = "";
            // for (int j = 0; j < notes.Length; ++j)
            // {
            //     changetemp += notes[j];
            //     if (j < notes.Length - 1)
            //     {
            //         changetemp += "/";
            //     }
            // }
            // Debug.Log(changetemp);
            // TextPayload textPayload1 = new TextPayload();//自己增加事件的示例
            // textPayload1.TextVal = changetemp;
            // rawEvents[i].Payload = textPayload1;

        }
    }


}

