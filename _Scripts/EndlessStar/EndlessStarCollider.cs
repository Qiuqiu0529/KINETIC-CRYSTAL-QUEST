using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessStarCollider : MonoBehaviour
{
   
    public EndlessStarPlayer endlessStarPlayer;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("trap"))
        {
            endlessStarPlayer.EncounterTrap();
        }
    }

    private void ontrigg(Collision other) {
        Debug.Log("collider");
        if(other.collider.CompareTag("trap"))
        {
            endlessStarPlayer.EncounterTrap();
        }
    }
}
