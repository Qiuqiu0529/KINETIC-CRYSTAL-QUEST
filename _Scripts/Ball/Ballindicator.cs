using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballindicator : MonoBehaviour
{
    public Transform startpos;
    public float x;
    [SerializeField] float gridx = 0.5f;

  private void Start() {
    Destroy(this.gameObject);
  }
    

    public void CalX()
    {
        x=(transform.position.x-startpos.position.x)/gridx;
    }
}
