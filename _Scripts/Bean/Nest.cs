using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class Nest : Singleton<Nest>
{
    // Start is called before the first frame update
    public MMF_Player moveBean;
    public MMF_DestinationTransform mMF_DestinationTransform;
    void Start()
    {
        mMF_DestinationTransform=moveBean.GetFeedbackOfType<MMF_DestinationTransform>();

    }

    public void MoveBean(Transform mtransform)
    {
        mMF_DestinationTransform.TargetTransform=mtransform;
        moveBean.PlayFeedbacks();
    }
}
