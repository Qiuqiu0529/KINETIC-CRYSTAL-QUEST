using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
public class WorldTrackTarget : MonoBehaviour,IWorldLandmarkObserver
{
    public bool useHolistic;
    [SerializeField]protected bool stable = false;
    protected IList<Landmark> landmarks;

    public float kalmanParamQ=0.001f;
    public float kalmanParamR=0.0015f;
    public float lowPassParam=0.1f;

    public void UpdateWorldLandmarks(LandmarkList landmarkList)
    {
        if (landmarkList == null)
        {
            print("landmarkList is null!!!");
            return;
        }
        stable = true;
        landmarks = landmarkList.Landmark;
    }
}
