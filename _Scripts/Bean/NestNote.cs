using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using Blobcreate.ProjectileToolkit;

public class NestNote : CreateNoteEvent
{

    [SerializeField] Transform launchPoint;

    [SerializeField] Transform targetPoint;
    
    [SerializeField] Transform launchPoint1;

    [SerializeField] Transform targetPoint1;

    Vector3 launchVelocity;

    public float acce;

    public void CreateTest()
    {
        PooledObject pooledObject = objectPools[0].GetPoolObj();
        pooledObject.transform.position = launchPoint.position;
        launchVelocity = Projectile.VelocityByA(launchPoint.position, targetPoint.position, acce);
        var myRigid = pooledObject.GetComponent<Rigidbody>();
        myRigid.AddForce(launchVelocity, ForceMode.VelocityChange);
        pooledObject.transform.localScale=new Vector3(1,1,1);


        PooledObject pooledObject1 = objectPools[0].GetPoolObj();
        launchVelocity = Projectile.VelocityByA(launchPoint1.position, targetPoint1.position, acce);
        pooledObject1.transform.position = launchPoint1.position;
        var myRigid1 = pooledObject1.GetComponent<Rigidbody>();
        myRigid1.AddForce(launchVelocity, ForceMode.VelocityChange);
        pooledObject1.transform.localScale=new Vector3(-1,1,1);

    }

    public override void CreateNote(KoreographyEvent evt)
    {
        CreateTest();

        // string info = evt.GetTextValue();//分解text
        // string[] notes = info.Split('/');
        // for (int i = 0; i < notes.Length; ++i)
        // {
        //     PooledObject pooledObject = objectPools[notes[i][0]-'A'].GetPoolObj();
        //     float.TryParse(notes[i].Substring(1), out float num);
        //     // Debug.Log(num);
        //     pooledObject.GetComponent<Ball>().SetZspeed(speed);
        //     pooledObject.transform.position = new Vector3(transform.position.x + gridx * num, transform.position.y, transform.position.z);
        // }
    }

    public override void SetPendingEvt()
    {
        toTargetTime = 1.25f;
        //     toTargetTime = (distance / speed)-delayTime-customDelayTime;
        //     base.SetPendingEvt();
    }


}
