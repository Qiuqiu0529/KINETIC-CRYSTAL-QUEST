using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] protected Collider mcollider;
    public Transform childobj;
    public PooledObject pooledObject;
    public float inittime;
    public float zspeed;
    public float yspeed;
    public float yspeedmax;
    public AnimationCurve yspeedCurve;
    protected bool hit;

   public void SetZspeed(float speed)
   {
      zspeed=-speed;
   }
}
