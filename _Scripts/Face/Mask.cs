using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : TrackTarget//加口罩，暂时不做
{
    float width, height;

    [SerializeField] Transform startpos;
    [SerializeField] Transform endpos;


    public Transform nose;

    public Transform headtrans;
    JointPoint head;


    // private void Start()
    // {
    //     if (PlayerPrefs.GetInt(Global.useHeadDec, 0) > 0 ? false : true)//不装饰，销毁
    //     {
    //         Destroy(this.gameObject);
    //         return;

    //     }
    //     startpos = InitPos.Instance.startpos;
    //     endpos = InitPos.Instance.endpos;
    //     width = endpos.position.x - startpos.position.x;
    //     height = endpos.position.y - startpos.position.y;
    //     if (useHolistic)//也许后面会改？
    //     {
    //         HolisticMotion.instance.AddPoseObserver(this);
    //     }
    //     else
    //     {
    //         PoseTrack.instance.AddPoseObserver(this);
    //     }
    //     head = new JointPoint();
    //     head.Transform = headtrans;
    //     head.InitRotation = headtrans.rotation;
    //     var gaze = nose.position - headtrans.position;
    //     head.Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));
    //     head.InverseRotation = head.Inverse * head.InitRotation;

    // }


    // private void Update()
    // {
    //     if (stable)
    //     {
    //         MoveMask();
    //         stable = false;
    //     }
    // }
    public void MoveMask()
    {
        // var nose = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.nose)]);
        // var lear = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lear)]);
        // var rear = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rear)]);
        //  // var earCenter = (lear + rear) / 2;

        // var hv = earCenter - neck;
        // var nhv = Vector3.Normalize(hv);
        // var nv = nose - neck;

        // head.Pos3D = neck + nhv * Vector3.Dot(nhv, nv);
        // var gaze = nose - head.Pos3D;
        // var f = LandmarkUtil.TriangleNormal(nose, lear, rear);
        // head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.InverseRotation;

        var nose = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.nose)]);
        var lear = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lear)]);
        var rear = LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rear)]);
        var mouth = (LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.lmouth)]) + LandmarkUtil.LandMarkToVector(landmarks[((int)PoseIndex.rmouth)])) / 2;
        var gaze = nose - (lear + rear) / 2;

        var f = LandmarkUtil.TriangleNormal(nose, lear, rear);
        head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.InverseRotation;

        transform.position=new Vector3(startpos.position.x+mouth.x*width,startpos.position.y+mouth.y*height,transform.position.z);

    }
}