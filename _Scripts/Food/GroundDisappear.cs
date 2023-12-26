using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDisappear : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {

        if (other.collider.CompareTag("ground"))
        {
            Debug.Log("Destroy");
            Destroy(gameObject);
        }
    }

}


