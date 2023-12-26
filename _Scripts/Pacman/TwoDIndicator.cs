using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDIndicator : MonoBehaviour
{
    public Transform startpos;
    public float x, y;
    public string temp;
    public string digit="#0.00";
    [SerializeField] float gridx = 0.5f, gridy = 0.5f;
    private void Start()
    {
        Destroy(this.gameObject);
    }

    public void CalXY()
    {
        x = (transform.position.x - startpos.position.x) / gridx;
        y = (transform.position.y - startpos.position.y) / gridy;
       
        temp=x.ToString(digit)+","+y.ToString(digit);
    }
}
