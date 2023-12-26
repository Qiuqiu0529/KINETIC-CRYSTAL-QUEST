using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public interface ILandmarkObserver 
{
    public void UpdateLandmarks(NormalizedLandmarkList landmarkList);
}
public interface IWorldLandmarkObserver 
{
    public void UpdateWorldLandmarks(LandmarkList landmarkList);
}
