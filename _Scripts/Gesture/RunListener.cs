using UnityEngine;

public class RunListener : Singleton<RunListener>, GestureRecogManager.IGestureListenerInterface
{
    // whether the needed gesture has been detected or not
    public bool running;

    public bool IsRunning()
    {
        if (running)
        {            
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
        manager.DetectGesture(GestureRecogManager.Gestures.Run);
    }

    public void GestureInProgress(GestureRecogManager.Gestures gesture, float progress, PoseIndex joint)
    {
        if (gesture == GestureRecogManager.Gestures.Run && progress > 0.5f)
        {
            running = true;
        }
    }

    public bool GestureCompleted(GestureRecogManager.Gestures gesture, PoseIndex joint)
    {
        return true;
    }

    public bool GestureCancelled(GestureRecogManager.Gestures gesture, PoseIndex joint)
    {
        running = false;
        return true;
    }
}
