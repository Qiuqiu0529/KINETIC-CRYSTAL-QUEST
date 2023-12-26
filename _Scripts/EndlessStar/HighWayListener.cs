using UnityEngine;

public class HighWayListener : Singleton<HighWayListener>, GestureRecogManager.IGestureListenerInterface
{
    // whether the needed gesture has been detected or not
    private bool jump = false;

    public bool IsJump()
    {
        //Debug.Log("highway"+jump);
        if (jump)
        {
            jump = false;
            return true;
        }

        return false;
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
        manager.DetectGesture(GestureRecogManager.Gestures.Jump);
    }

    public void GestureInProgress(GestureRecogManager.Gestures gesture, float progress, PoseIndex joint)
    {
        return;
    }

    public bool GestureCompleted(GestureRecogManager.Gestures gesture, PoseIndex joint)
    {
        switch (gesture)
        {
            case GestureRecogManager.Gestures.Jump:
                jump = true;
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
}
