using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class BlackBall : Ball
{
    [SerializeField] protected int score = 20;

    public MMF_Player failFB;

    public void Awake()
    {
        mcollider = GetComponent<Collider>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            hit = true;
            Fail();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("line"))
        {
            Normal();
        }

    }
    public void OnEnable()
    {
        inittime = Time.time;
        mcollider.enabled = true;
        hit = false;
        childobj.localScale=new Vector3(0.3f,0.3f,0.3f);
    }

    public void FixedUpdate()
    {
        yspeed=-1*Mathf.Lerp(0, yspeedmax, yspeedCurve.Evaluate((Time.time-inittime)/5));
        transform.position += new Vector3(0, 0, zspeed * Time.fixedDeltaTime);
    }

    public void Normal()
    {
        mcollider.enabled = false;
        BallCoin.Instance.AddBlack(score);
        pooledObject.Release();
    }
    public void Fail()
    {
        mcollider.enabled = false;
        failFB.PlayFeedbacks();
        BallCoin.Instance.FailNote();
    }
}
