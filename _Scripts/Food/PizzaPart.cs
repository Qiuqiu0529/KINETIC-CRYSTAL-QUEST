using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
public class PizzaPart : MonoBehaviour
{
    public Transform targetTransform;
    public MMAutoRotate mMAutoRotate;
    public int minNum = 3, maxNum = 8;
    public float radis = 0.8f;
    public MeshCollider meshCollider;
    public int score=20;

    bool canMove = true;
    float inity;
    public float duration = 7f;
    public GameObject pizzaPartPrefab;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Awake()
    {
        mMAutoRotate = GetComponent<MMAutoRotate>();
    }
    void OnEnable()
    {
        canMove = true;
        mMAutoRotate.enabled = true;
        startTime = Time.time;
        inity = transform.position.y;
        meshCollider = GetComponent<MeshCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (canMove)
        {
            canMove = false;
            mMAutoRotate.enabled = false;
            if (other.CompareTag("target"))
            {
                targetTransform = other.transform;
                PizzaScore.Instance.SetFloatingText(transform.position,score);
                SuccessEat();
            }
            else if (other.CompareTag("ground"))
            {
                PizzaCreator.Instance.ReSetCreate();
                PizzaScore.Instance.FailNote();
                Debug.Log("Destroy");
                Destroy(gameObject);
            }
        }
        
    }

    public void SuccessEat()
    {

        int numPizzaParts = Random.Range(minNum, maxNum); // 随机生成3-7个披萨组件

        for (int i = 0; i < numPizzaParts; i++)
        {
            GeneratePizzaPartOnTarget();
        }
        PizzaCreator.Instance.ReSetCreate();
        Destroy(gameObject);

    }
    private void OnCollisionEnter(Collision other)
    {
        if (canMove)
        {
            canMove = false;
            mMAutoRotate.enabled = false;
            Debug.Log(other.collider.tag);
            if (other.collider.CompareTag("target"))
            {
                targetTransform = other.transform;
                PizzaScore.Instance.SetFloatingText(transform.position,score);
                SuccessEat();
            }
            else if (other.collider.CompareTag("ground"))
            {
                PizzaCreator.Instance.ReSetCreate();
                Debug.Log("Destroy");
                PizzaScore.Instance.FailNote();
                Destroy(gameObject);
            }
        }

    }


    private void GeneratePizzaPartOnTarget()
    {
        Vector3 spawnPosition = targetTransform.transform.position; // 生成位置在饼底上        
        // 生成在圆的表面上
        float angle = Random.Range(0f, 360f);
        float radius = 0.8f; // 饼底的半径
        spawnPosition += new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), Random.Range(0.1f, 0.5f), radius * Mathf.Sin(angle * Mathf.Deg2Rad));

        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f); // 随机旋转

        GameObject pizzaPart = Instantiate(pizzaPartPrefab, spawnPosition, randomRotation);
        pizzaPart.transform.SetParent(targetTransform);
        this.transform.SetParent(targetTransform);
        this.enabled = false;
    }


    private float startTime;
    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }


    protected virtual void Update()
    {
        // if (canMove)
        // {
        //     if (targetTransform != null)
        //     {
        //         float elapsedTime = Time.time - startTime;
        //         float t = Mathf.Clamp01(elapsedTime / duration);

        //         float yProgress = MMTweenDefinitions.EaseOut_Quadratic(t);

        //         float newY = Mathf.Lerp(inity, targetTransform.position.y, yProgress);
        //         Debug.Log(newY);
        //         if(newY<=targetTransform.position.y)
        //         {
        //             mMAutoRotate.enabled = false;
        //             meshCollider.enabled=false;

        //             canMove = false;
        //             Destroy(gameObject,5f);
        //         }

        //         transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        //     }
        // }
    }
}
