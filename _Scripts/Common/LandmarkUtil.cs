using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using TensorFlowLite.MoveNet;
using Mediapipe;

public class LandmarkUtil 
{
    public static Vector3 Test(NormalizedLandmark landmark)
    {
        return new Vector3(1 - landmark.X, 1 - landmark.Y, landmark.Z);
    }
    public static Vector3 LandMarkToVectorMirror(NormalizedLandmark landmark)
    {
        return new Vector3(1 - landmark.X, 1 - landmark.Y, 1-landmark.Z);
    }
    public static Vector3 LandMarkToVector(NormalizedLandmark landmark)
    {
        return new Vector3(1 - landmark.X, 1 - landmark.Y, landmark.Z);
    }

    // public static Vector3 MoveNetJointToVector(MoveNetPose.Joint joint)
    // {
    //     return new Vector3(1 - joint.x, 1 - joint.y, 0);
    // }

    public static Vector3 LandMarkToVector(Landmark landmark)
    {
        return new Vector3(-landmark.X, -landmark.Y, landmark.Z);
    }

    public static Vector3 LandMarkToVector(Landmark landmark, float zScale)
    {
        return new Vector3(-landmark.X,- landmark.Y, landmark.Z* zScale);
    }



    public static Vector3 LandMarkToVectorMirror(NormalizedLandmark landmark, float zScale)
    {
        return new Vector3(1-landmark.X, 1-landmark.Y, (1-landmark.Z)* zScale);
    }
    public static Vector3 LandMarkToVector(NormalizedLandmark landmark, float zScale)
    {
        return new Vector3(1 - landmark.X, 1 - landmark.Y, landmark.Z * zScale);
    }
    public static Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)//dd垂直于a，b，c三点组成的平面
    {
        Vector3 dd = Vector3.Cross(a - b, a - c);
        dd.Normalize();
        return dd;
    }

    
    public static Quaternion GetInverse(JointPoint p1, JointPoint p2, Vector3 forward)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position, forward));
    }
}
