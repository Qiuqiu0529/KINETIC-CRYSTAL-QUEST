using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class BeanAnim : MonoBehaviour
{
    public Animator animator;
    public MMF_Player dieFB, hitFB, endFB;
    public int score = 20;
    MMF_FloatingText mMF_FloatingText;
    public Rigidbody rb;

    [SerializeField] protected Collider mcollider;

    public Transform chaseTransform;
    bool canMove = true;
    bool canChase=true;



    void Start()
    {
        mMF_FloatingText=hitFB.GetFeedbackOfType<MMF_FloatingText>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            canMove = false;
            rb.velocity=Vector3.zero;
            rb.isKinematic=true;
            rb.useGravity=false;
            mcollider.enabled = false;
            
            //Nest.Instance.MoveBean(transform);
            Happy();
        }

        if (canChase && other.CompareTag("hand"))
        {
            canChase = false;
            rb.velocity=Vector3.zero;
            rb.isKinematic=true;
            rb.useGravity=false;
            chaseTransform = other.transform;
        }
    }
    private void OnCollisionEnter(Collision other)
    {

        if (other.collider.CompareTag("ground"))
        {
            canMove = false;
            rb.velocity=Vector3.zero;
            rb.isKinematic=true;
            rb.useGravity=false;

            mcollider.enabled = false;
        
            Die();
        }
    }

    void FixedUpdate()
    {
        if(canMove)
        {
            if(chaseTransform!=null)
            {
                Vector3 temp=chaseTransform.position;
                temp.z=transform.position.z;
                Vector3 slerp =  Vector3.Slerp(transform.position, temp, Time.deltaTime * 20);
                transform.position=slerp ;
            }
        }
    }





    void OnEnable()
    {
        rb.useGravity=true;
        rb.isKinematic=false;
        animator = GetComponent<Animator>();
        animator.SetTrigger("idle");
        int temp = Random.Range(0, 2);
        mcollider.enabled = true;
        chaseTransform = null;
        canMove = true;
        canChase=true;

        transform.localEulerAngles = new Vector3(0, 0, Random.Range(30, 70));
        Debug.Log(transform.localEulerAngles);
    }

    public void FaceLeft()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void FaceRight()
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }



    public void Die()
    {
        NestScore.Instance.FailNote();
        animator.SetTrigger("die");
        dieFB.PlayFeedbacks();
    }
    public void Happy()
    {
        int temp = Random.Range(0, 3);
        int tempscore=NestScore.Instance.AddScore(score);
        mMF_FloatingText.Value=tempscore.ToString();

        if (temp == 0)
        {
            animator.SetTrigger("joy");

        }
        else if (temp == 1)
        {
            animator.SetTrigger("dance1");
        }
        else if (temp == 2)
        {
            animator.SetTrigger("buffup");
        }
        hitFB.PlayFeedbacks();
    }

    public void Ending()
    {
        Debug.Log("ending");
        rb.useGravity=true;
        rb.isKinematic=false;
        endFB.PlayFeedbacks();
    }

}
