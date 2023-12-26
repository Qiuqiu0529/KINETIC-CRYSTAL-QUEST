using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Blobcreate.ProjectileToolkit;
using Language.Lua;

public class PizzaCreator : Singleton<PizzaCreator>
{
    public Rigidbody[] meat;
    public Rigidbody[] cheese;
    public Rigidbody[] vegetable;
    [SerializeField] TrajectoryPredictor tp;
    [SerializeField] Transform launchPoint;
    [SerializeField] Transform cannonBody;

    [SerializeField] Transform targetPoint;
    [SerializeField] Transform maxHeightPoint;

    // [SerializeField] float timeOfFlight;
    // [SerializeField] float gravityScale;

    // Vector3 gravity;
    public Rigidbody test;

    public float acce;


    public float waitTime = 10f;

    public MMF_Player endPlayFB;
    public MMF_Player buttonDownFB, buttonUpFB;

    public float angle;

    public bool showTp;

    Vector3 launchVelocity;

    void Start()
    {
        // origGravity=Physics.gravity;
        // gravity = Physics.gravity*gravityScale;

    }

    public void ReSetCreate()
    {
        buttonUpFB.PlayFeedbacks();
        showTp = true;
    }

    public void CreatePizzaPart()
    {
        if (showTp)
        {
            showTp = false;
            var pos = launchPoint.position;
            buttonDownFB.PlayFeedbacks();
            pos.z = 0;

            int temp = Random.Range(0, 5);
            if (temp < 1)
            {
                var myRigid = Instantiate(meat[Random.Range(0,meat.Length)], pos, launchPoint.rotation);
                myRigid.gameObject.transform.localScale = new Vector3(3f, 3f, 3f);
                myRigid.AddForce(launchVelocity, ForceMode.VelocityChange);

            }
            else if(temp<3)
            {
                var myRigid = Instantiate(cheese[Random.Range(0,cheese.Length)], pos, launchPoint.rotation);
                myRigid.gameObject.transform.localScale = new Vector3(3f, 3f, 3f);
                myRigid.AddForce(launchVelocity, ForceMode.VelocityChange);

            }
            else
            {
                var myRigid = Instantiate(vegetable[Random.Range(0,vegetable.Length)], pos, launchPoint.rotation);
                myRigid.gameObject.transform.localScale = new Vector3(3f, 3f, 3f);
                myRigid.AddForce(launchVelocity, ForceMode.VelocityChange);

            }

        }
    }


    void Update()
    {

        // var vHeight=Projectile.VelocityByAngle(launchPoint.position, targetPoint.position,angle);
        // var vHeight=Projectile.VelocityByHeight(launchPoint.position, targetPoint.position,maxHeightPoint.position.y-targetPoint.position.y);
        //var v = Projectile.VelocityByTime(launchPoint.position, targetPoint.position, timeOfFlight);
        if (showTp)
        {
            launchVelocity = Projectile.VelocityByA(launchPoint.position, targetPoint.position, acce);
            float degree = (Mathf.Atan(launchVelocity.y / launchVelocity.x)) * Mathf.Rad2Deg;
           // Debug.Log(degree);
            cannonBody.localEulerAngles = new Vector3(0, degree - 20, 0);

            tp.Render(launchPoint.position, launchVelocity, targetPoint.position.x - launchPoint.position.x);
        }

    }


    // public void EndDiaplay()
    // {
    //     foreach (Transform child in this.transform)
    //     {
    //         Rigidbody rb = child.GetComponent<Rigidbody>();
    //         if (rb != null)
    //         {
    //             Destroy(rb);
    //         }
    //     }
    //     endPlayFB.PlayFeedbacks();
    // }
}
