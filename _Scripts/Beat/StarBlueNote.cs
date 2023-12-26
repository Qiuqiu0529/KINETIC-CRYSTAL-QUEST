using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class StarBlueNote : Note
{
    private void OnTriggerEnter(Collider other)
    {
        if (!enter && (other.CompareTag("lhand")))
        {
            enter = true;
            entertime = Time.time;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!finish && (other.CompareTag("lhand")))
        {
            if (Time.time - entertime > holdTime)
            {
                finish = true;
                Hit();
            }

        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (enter && other.CompareTag("lhand"))
        {
            enter = false;
            //finish=true;
        }

    }


}
