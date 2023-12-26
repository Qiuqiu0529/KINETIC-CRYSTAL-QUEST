using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpectrumLight : MonoBehaviour {
	public int audioChannel = 4;
	public float audioSensibility = 0.15f;
	public float intensity = 3.0f;
	public float lerpTime = 2.0f;

	private Light lt;
	private float oldIntensity;

	void Start(){
		lt = GetComponent<Light>();
		oldIntensity = lt.intensity;
	}

	void Update () {

		// If i find the beat
		if (SpectrumKernel.Instance.spects [audioChannel] * SpectrumKernel.Instance.threshold >= audioSensibility) {
			lt.intensity = SpectrumKernel.Instance.spects [audioChannel] * (intensity * SpectrumKernel.Instance.threshold);
		}else{
			// Retrieve the old intensity
			oldIntensity = Mathf.Lerp(lt.intensity, 1.0f, lerpTime * Time.deltaTime);	
			lt.intensity = oldIntensity;			
		}

	}
}
