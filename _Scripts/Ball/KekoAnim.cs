using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KekoAnim : MonoBehaviour
{
    [SerializeField]Animator keko;
    public void Start() {
        keko.Play("Run");
    }

    public void Jump()
    {
        keko.SetTrigger("Jump");
    }

    public void Land()
    {
        keko.SetTrigger("Run");
    }
}
