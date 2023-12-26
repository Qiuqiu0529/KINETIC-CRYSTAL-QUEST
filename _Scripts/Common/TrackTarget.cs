using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class TrackTarget : MonoBehaviour,ILandmarkObserver
{
    public bool useHolistic;
    [SerializeField]protected bool stable = false;
    protected IList<NormalizedLandmark> landmarks;

    public float kalmanParamQ=0.001f;
    public float kalmanParamR=0.0015f;
    public float lowPassParam=0.1f;

    public void UpdateLandmarks(NormalizedLandmarkList landmarkList)
    {
        if (landmarkList == null)
            return;
        stable = true;
        landmarks = landmarkList.Landmark;
    }
}
