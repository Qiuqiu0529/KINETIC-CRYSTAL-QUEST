using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class BallTarget : OneTarget
{
    [SerializeField]Animator keko;
    protected JointPoint kneeLJP = new JointPoint();
    protected JointPoint kneeRJP = new JointPoint();
    public float runningThreshold = 0.1f; 

    public bool changeChannelMixer;

    public Volume volume;
    public ChannelMixer channelMixer;

    protected Action chanelChangeNormal,chanelChangeWrong;

    public void ChangeToBlue()
    {
        channelMixer.active=true;
    }

    public void ChangeToRed()
    {
        channelMixer.active=false;
    }



    private void Awake() {
        changeChannelMixer= PlayerPrefs.GetInt(Global.ballChanelMixer, 1) > 0 ? true : false;
        volume.profile.TryGet(out channelMixer);
        if(changeChannelMixer)
        {
            chanelChangeNormal+=ChangeToRed;
            chanelChangeWrong+=ChangeToBlue;
        }

    }

    public new void Start()
    {
        base.Start();
        
        if(useHolistic)//也许后面会改？
        {
            HolisticMotion.instance.AddPoseObserver(this);
        }
        else
        {
            PoseTrack.instance.AddPoseObserver(this);
        }

    }

    public override void ChangeTransform()
    {        
        // 判断是否在奔跑
       

        if (RunListener.Instance.running)
        {
            keko.SetTrigger("Run");
            chanelChangeNormal?.Invoke();
            
            //channelMixer.active=false;
            //keko.speed=1;
            Vector3 target = new Vector3(startpos.position.x + targetJP.Pos3D.x * width, 0, 0);// 正在跑步
            Vector3 lerp=Vector3.Slerp(targetobj.position, target, Time.deltaTime * 20);
            BallCoin.Instance.multiplier1=1;
            targetobj.position=lerp;
            
        }
        else
        {          
            
            keko.SetTrigger("Idle");
            chanelChangeWrong?.Invoke();
            BallCoin.Instance.multiplier1=0.1f;

            //keko.speed=(tempspeed/runningThreshold);
        }

        
    }
}
