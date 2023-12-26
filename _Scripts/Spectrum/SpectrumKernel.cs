using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumKernel : Singleton<SpectrumKernel>
{
    public float[] spects = new float[1024];
    public float[] prevSpectrum = new float[1024];

    public float threshold = 3.0f;
    private void Start()
    {
        Cursor.visible = false;
    }

    // Allow to check only one time the channels
    void Update()
    {
        spects.CopyTo (prevSpectrum, 0);
        AudioListener.GetSpectrumData(spects, 0, FFTWindow.BlackmanHarris);
    }
}
