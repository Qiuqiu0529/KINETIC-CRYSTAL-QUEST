using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanTarget : OneTarget
{
    [SerializeField] bool faceRight;
    [SerializeField] Transform visualObj;

    private void Awake()
    {
        choosetarget = (Choosetarget)PlayerPrefs.GetInt(Global.pacmanTarget, 1);
    }

    public new void Start()
    {
        base.Start();
        if (useHolistic)//也许后面会改？
        {
            HolisticMotion.instance.AddPoseObserver(this);
        }
        else
        {
            PoseTrack.instance.AddPoseObserver(this);
        }
    }

    public void Flip()
    {
        float x;
        if (faceRight)
            x = Mathf.Abs(visualObj.transform.localScale.x) * 1;
        else
            x = Mathf.Abs(visualObj.transform.localScale.x) * -1;
        Vector3 scal = new Vector3(x,
           visualObj.transform.localScale.y, visualObj.transform.localScale.z);
        visualObj.transform.localScale = scal;
    }
    public override void ChangeTransform()
    {
        var targetPos = new Vector3(startpos.position.x + targetJP.Pos3D.x * width, startpos.position.y + targetJP.Pos3D.y * height, 0);
        // if (targetobj.position.x <= targetPos.x)
        // {
        //     if (!faceRight)
        //     {
        //         faceRight = true;
        //         Flip();
        //     }
        // }
        // else
        // {
        //     if (faceRight)
        //     {
        //         faceRight = false;
        //         Flip();
        //     }
        // }

        // visualObj.rotation=Quaternion.Euler(0,0,Vector3.SignedAngle(Vector3.up,targetPos-targetobj.position,Vector3.forward));
        // if (Vector3.Distance(targetPos, targetobj.position) > 0.02f)
        // {

        //     visualObj.rotation = Quaternion.FromToRotation(Vector3.up, targetPos - targetobj.position);
        // }

        targetobj.position = targetPos;
    }
}
