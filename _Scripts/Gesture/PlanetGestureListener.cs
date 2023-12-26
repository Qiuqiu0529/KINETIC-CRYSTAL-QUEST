using UnityEngine;

/// <summary>
/// planet场景手势控制注册器，包括左右挥手、合掌、张开双手
/// </summary>
public class PlanetGestureListener : Singleton<PlanetGestureListener>, GestureRecogManager.IGestureListenerInterface
{
    // whether the needed gesture has been detected or not
    [SerializeField]
    private bool swipeLeft = false;
    [SerializeField]
    private bool swipeRight = false;
    [SerializeField]
    private bool extend = false;
    [SerializeField]
    private bool squash = false;

    public bool IsSwipeLeft()
    {
        if (swipeLeft)
        {
            swipeLeft = false;
            return true;
        }

        return false;
    }
   
    public bool IsSwipeRight()
    {
        if (swipeRight)
        {
            swipeRight = false;
            return true;
        }

        return false;
    }

    
    public bool IsExtend()
    {
        if (extend)
        {
            extend = false;
            return true;
        }

        return false;
    }

    public bool IsSquash()
    {
        if (squash)
        {
            squash = false;
            return true;
        }

        return false;
    }


    public void GestureInProgress(GestureRecogManager.Gestures gesture, float progress, PoseIndex joint)
    {
        return;
    }

    public bool GestureCompleted(GestureRecogManager.Gestures gesture, PoseIndex joint)
    {
        switch (gesture)
        {
            case GestureRecogManager.Gestures.SwipeLeft:
                swipeLeft = true;
                break;
            case GestureRecogManager.Gestures.SwipeRight:
                swipeRight = true;
                break;

            case GestureRecogManager.Gestures.Extend:
                extend = true;
                break;
            case GestureRecogManager.Gestures.Squash:
                squash = true;
                break;

            default:
                break;
                // Not supported anymore.
        }


        return true;
    }

    public bool GestureCancelled(GestureRecogManager.Gestures gesture, PoseIndex joint)
    {
        return true;
    }

    public void RegisterGestures()
    {
        GestureControlManager manager = GestureControlManager.Instance;
        if (!manager)
        {
            Debug.LogWarning("gestureControlManager is null!");
            return;
        }

        // detect these user specific gestures
        manager.DetectGesture(GestureRecogManager.Gestures.SwipeLeft);
        manager.DetectGesture(GestureRecogManager.Gestures.SwipeRight);

        manager.DetectGesture(GestureRecogManager.Gestures.Extend);
        manager.DetectGesture(GestureRecogManager.Gestures.Squash);
    }
}
