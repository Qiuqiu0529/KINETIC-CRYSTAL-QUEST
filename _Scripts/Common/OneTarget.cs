using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OneTarget : TrackTarget
{
   
    [SerializeField] protected Transform targetobj;
    protected float width, height;

    [SerializeField] protected Transform startpos;
    [SerializeField] protected Transform endpos;

    public Choosetarget choosetarget;
    protected Action targetAction;


    public void TrackNose()
    {
        targetJP.Pos3D = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.nose)]);
    }
    public void TrackNeck()
    {
        targetJP.Pos3D = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lshoulder)]) +
           LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rshoulder)])) / 2;
    }
    public void TrackCenter()
    {
        var neck = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lshoulder)]) +
           LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rshoulder)])) / 2;
        var hip = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lhip)]) +
        LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rhip)])) / 2;//中心
        var center = hip + (neck - hip) * 0.5f;
        targetJP.Pos3D = center;
    }
    public void TrackHip()
    {
        targetJP.Pos3D = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lhip)]) +
           LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rhip)])) / 2;
    }
    private void SetTargetAction()
    {
        switch (choosetarget)
        {
            case Choosetarget.nose:
                targetAction = TrackNose;
                break;
            case Choosetarget.neck:
                targetAction = TrackNeck;
                break;
            case Choosetarget.center:
                targetAction = TrackCenter;
                break;
            case Choosetarget.hip:
                targetAction = TrackHip;
                break;
            default:
                Debug.LogError("Invalid target value: " + choosetarget);
                break;
        }
    }
   
   
    protected JointPoint targetJP = new JointPoint();

    protected void Start()
    {
        stable = false;
        startpos = InitPos.Instance.startpos;
        endpos = InitPos.Instance.endpos;
        width = endpos.position.x - startpos.position.x;
        height = endpos.position.y - startpos.position.y;
        SetTargetAction();


    }
    protected void Update()
    {
        if (stable)
        {
            Move();
            //stable = false;
        }
    }

    public void Move()
    {
        targetAction?.Invoke();
        targetJP.Kalman(kalmanParamR,kalmanParamQ);
        targetJP.Smooth(lowPassParam);
        ChangeTransform();
    }

    public virtual void ChangeTransform()
    {
        // Debug.Log(" virtual void ChangeTransform()");

    }
}
