using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitScreen : MonoBehaviour
{
    [SerializeField] private RawImage _screen;

    public Texture texture
    {
        private get => _screen.texture;
        set => _screen.texture = value;
    }
    // Start is called before the first frame update
    void Start()
    {
        float w = texture.width;
        float h = texture.height;
        float screenRatio = (float)UnityEngine.Screen.width / UnityEngine.Screen.height;

        if (screenRatio > 1) // width > height
        {
            if (screenRatio > (w / h))
            {
                float scaleRatio = screenRatio * h / w;
                transform.localScale = new Vector3(transform.localScale.x * scaleRatio, transform.localScale.y * scaleRatio, 1);
            }
            else
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
        }
        else
        {
            if (screenRatio > (h / w))
            {
                float scaleRatio = screenRatio * w / h;
                transform.localScale = new Vector3(transform.localScale.x * scaleRatio, transform.localScale.y * scaleRatio, 1);
            }
            else
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
        }

    }
}
