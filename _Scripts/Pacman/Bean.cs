using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class Bean : MonoBehaviour
{
    [SerializeField] protected int score = 10;
    [SerializeField] protected float survialTime = 5f;
    [SerializeField] Collider mcollider;
    public Transform childobj;
    public float inittime;
    bool hit;
    public MMF_Player hitFB;
    public MMF_Player failFB;
    MMF_FloatingText mMF_FloatingText;

    private void Awake()
    {
        mcollider = GetComponent<Collider>();
        mMF_FloatingText=hitFB.GetFeedbackOfType<MMF_FloatingText>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            hit = true;
            Normal();
        }
    }

    private void OnEnable()
    {
        mcollider.enabled = true;
        inittime = Time.time;

        hit = false;
        childobj.eulerAngles = new Vector3(0, 0, 0);
    }
    private void FixedUpdate()
    {
        if (Time.time - inittime >= survialTime && !hit)
        {
            hit = true;
            Fail();
        }
    }


    protected void Normal()
    {
        int temp=PacmanCoin.Instance.AddScore(score);
        mMF_FloatingText.Value=temp.ToString();
        hitFB.PlayFeedbacks();
    }
    protected void Fail()
    {
        PacmanCoin.Instance.FailNote();
        failFB.PlayFeedbacks();
    }

}
