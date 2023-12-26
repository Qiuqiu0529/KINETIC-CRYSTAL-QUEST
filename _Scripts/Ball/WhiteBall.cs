using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class WhiteBall : Ball
{
    [SerializeField] protected int score = 30;
   
    public MMF_Player hitFB;
    
    MMF_FloatingText mMF_FloatingText;

    public void Awake()
    {
        mcollider = GetComponent<Collider>();
        mMF_FloatingText=hitFB.GetFeedbackOfType<MMF_FloatingText>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            hit = true;
            Normal();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("line"))
        {
            Fail();
        }

    }
    public void FixedUpdate()
    {
        yspeed=-1*Mathf.Lerp(0, yspeedmax, yspeedCurve.Evaluate((Time.time-inittime)/10));
        transform.position += new Vector3(0, 0, zspeed * Time.fixedDeltaTime);
        // if(transform.position.z<=0)
        // {
        //     Debug.Log(Time.time-inittime);
        // }

    }

    public void OnEnable()
    {
        inittime = Time.time;
        mcollider.enabled = true;
        hit = false;
        childobj.localScale=new Vector3(0.3f,0.3f,0.3f);
    }

    protected void Normal()
    {
        mcollider.enabled = false;
        int temp=BallCoin.Instance.AddWhite(score);
        mMF_FloatingText.Value=temp.ToString();
        hitFB.PlayFeedbacks();
    }
    protected void Fail()
    {
        mcollider.enabled = false;
        BallCoin.Instance.FailNote(); 
        pooledObject.Release();
    }
}
