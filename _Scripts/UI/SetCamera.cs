using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetCamera : MonoBehaviour
{
    public Canvas canvas;
    private void OnEnable()
    {
        canvas.worldCamera = Camera.main;
    }
}
