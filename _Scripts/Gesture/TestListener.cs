using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListener : Singleton<TestListener>, GestureRecogManager.IGestureListenerInterface
{
    // whether the needed gesture has been detected or not
    private bool swipeLeft = false;
    private bool swipeRight = false;
    private bool swipeDown = false;
    private bool swipeUp = false;
    private bool raiseRightHand = false;
    private bool zoomOut = false;

    private bool zoomIn = false;

    private bool extend = false;

    private bool squash = false;

    private bool jump = false;
    private bool squat = false;


    

    public bool IsSwipeLeft()
    {
        if (swipeLeft)
        {
            swipeLeft = false;
            return true;
        }

        return false;
    }

    
    public bool IsZoomOut()
    {
        if (zoomOut)
        {
            zoomOut = false;
            return true;
        }

        return false;
    }

    public bool IsZoomIn()
    {
        if (zoomIn)
        {
            zoomIn = false;
            return true;
        }

        return false;
    }

    public bool IsRaiseRightHand()
    {
        if (raiseRightHand)
        {
            raiseRightHand = false;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether swipe right is detected.
    /// </summary>
    /// <returns><c>true</c> if swipe right is detected; otherwise, <c>false</c>.</returns>
    public bool IsSwipeRight()
    {
        if (swipeRight)
        {
            swipeRight = false;
            return true;
        }

        return false;
    }

    public bool IsSwipeUp()
    {
        if (swipeUp)
        {
            swipeUp = false;
            return true;
        }

        return false;
    }

    public bool IsSwipeDown()
    {
        if (swipeDown)
        {
            swipeDown = false;
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
    public bool IsJump()
    {
        if (jump)
        {
            jump = false;
            return true;
        }

        return false;
    }
    public bool IsSquat()
    {
        if (squat)
        {
            squat = false;
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
        manager.DetectGesture(GestureRecogManager.Gestures.SwipeLeft);
        manager.DetectGesture(GestureRecogManager.Gestures.SwipeRight);

        manager.DetectGesture(GestureRecogManager.Gestures.Extend);
        manager.DetectGesture(GestureRecogManager.Gestures.Squash);
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
            case GestureRecogManager.Gestures.SwipeUp:
                swipeUp = true;
                break;
            case GestureRecogManager.Gestures.SwipeDown:
                swipeDown = true;
                break;
            case GestureRecogManager.Gestures.Extend:
                extend = true;
                break;
            case GestureRecogManager.Gestures.Squash:
                squash = true;
                break;
            case GestureRecogManager.Gestures.Jump:
                jump = true;
                break;
            case GestureRecogManager.Gestures.Squat:
                squat = true;
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
