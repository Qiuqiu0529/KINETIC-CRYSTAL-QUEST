using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaButton : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
         if (other.CompareTag("hand"))
        {
            PizzaCreator.Instance.CreatePizzaPart();
        }
    }
   
}
