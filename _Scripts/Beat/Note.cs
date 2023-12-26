using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class Note : MonoBehaviour
{
    [SerializeField] protected float holdTime;
    [SerializeField] protected int perfectScore = 100;
    [SerializeField] protected int greatScore = 70;
    [SerializeField] protected int normalScore = 50;
    [SerializeField] protected float survialTime = 5f;
    public float inittime;
    protected Collider mcollider;

    public Transform childobj;

    public float entertime;
    public bool enter;
    public bool finish;
    public MMF_Player hitFB;
    public MMF_Player failFB;

    MMF_FloatingText mMF_FloatingText;

    public MMF_Player enterFB;

    public void Awake()
    {
        mcollider = GetComponent<Collider>();
        mMF_FloatingText = hitFB.GetFeedbackOfType<MMF_FloatingText>();
    }

    public void OnEnable()
    {
        inittime = Time.time;
        mcollider.enabled = true;
        childobj.localScale = new Vector3(1, 1, 1);
    }

    public void Hit()
    {
        var time = Time.time - inittime - holdTime;
        if (time <= 1)
        {
            Perfect();
        }
        else if (time <= 1.5f)
        {
            Great();
        }
        else
        {
            Normal();
        }
        hitFB.PlayFeedbacks();
    }

    public void FixedUpdate()
    {
        if (!finish && Time.time - inittime - holdTime >= survialTime)
        {
            finish = true;
            Fail();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!enter && (other.CompareTag("lhand") || other.CompareTag("rhand")))
        {
            enter = true;
            entertime = Time.time;
            enterFB.PlayFeedbacks();

        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!finish && (other.CompareTag("lhand") || other.CompareTag("rhand")))
        {
            if (Time.time - entertime > holdTime)
            {

                finish = true;
                enterFB.StopFeedbacks();
                Hit();
            }

        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (enter && (other.CompareTag("lhand") || other.CompareTag("rhand")))
        {
            enter = false;
            //finish=true;
        }

    }

    public void OnDisable()
    {
        enter = false;
        finish = false;
        mcollider.enabled = true;
    }


    protected void Perfect()
    {
        int temp = StarCoin.Instance.AddPerfect(perfectScore);
        mMF_FloatingText.Value = temp.ToString();
    }
    protected void Great()
    {
        int temp = StarCoin.Instance.AddGreat(greatScore);
        mMF_FloatingText.Value = temp.ToString();

    }
    protected void Normal()
    {
        int temp = StarCoin.Instance.AddNormal(normalScore);
        mMF_FloatingText.Value = temp.ToString();

    }
    protected void Fail()
    {
        if (enterFB != null)
        {
            enterFB.StopFeedbacks();
        }
        StarCoin.Instance.FailNote();
        failFB.PlayFeedbacks();
    }


}
