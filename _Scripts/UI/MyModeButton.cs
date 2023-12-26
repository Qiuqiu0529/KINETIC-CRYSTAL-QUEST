using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

public class MyModeButton : UIControl<ButtonVisuals>
{
    public Animator animator;
    public AudioClip uisound;
    public GameMode mode;
    public UnityEvent OnClicked = null;
    public GameObject lockMode;
    bool unlock;

    public MMF_Player canClickFB;
    public Transform rotTrans;
    public GameObject selectParticle;

    public UIBlock uIBlock => GetComponent<UIBlock>();

    public MMF_Player unlockDialogue;

    protected void OnEnable()
    {
        animator = GetComponent<Animator>();
        if (View.TryGetVisuals(out ButtonVisuals visuals))
        {
            visuals.UpdateVisualState(VisualState.Default);
        }
        unlock = ModeMgr.Instance.GetUnLockInfo(mode);
        lockMode.SetActive(!unlock);
        View.UIBlock.AddGestureHandler<Gesture.OnClick, ButtonVisuals>(HandleClicked);
        View.UIBlock.AddGestureHandler<Gesture.OnHover, ButtonVisuals>(ButtonVisuals.HandleHovered);
        View.UIBlock.AddGestureHandler<Gesture.OnUnhover, ButtonVisuals>(ButtonVisuals.HandleUnhovered);
        View.UIBlock.AddGestureHandler<Gesture.OnPress, ButtonVisuals>(ButtonVisuals.HandlePressed);
        View.UIBlock.AddGestureHandler<Gesture.OnRelease, ButtonVisuals>(ButtonVisuals.HandleReleased);
        View.UIBlock.AddGestureHandler<Gesture.OnCancel, ButtonVisuals>(ButtonVisuals.HandlePressCanceled);
    }

    protected void OnDisable()
    {
        View.UIBlock.RemoveGestureHandler<Gesture.OnClick, ButtonVisuals>(HandleClicked);
        View.UIBlock.RemoveGestureHandler<Gesture.OnHover, ButtonVisuals>(ButtonVisuals.HandleHovered);
        View.UIBlock.RemoveGestureHandler<Gesture.OnUnhover, ButtonVisuals>(ButtonVisuals.HandleUnhovered);
        View.UIBlock.RemoveGestureHandler<Gesture.OnPress, ButtonVisuals>(ButtonVisuals.HandlePressed);
        View.UIBlock.RemoveGestureHandler<Gesture.OnRelease, ButtonVisuals>(ButtonVisuals.HandleReleased);
        View.UIBlock.RemoveGestureHandler<Gesture.OnCancel, ButtonVisuals>(ButtonVisuals.HandlePressCanceled);
    }

    protected void HandleClicked(Gesture.OnClick evt, ButtonVisuals visuals)
    {

        UISound.Instance.PlayUISound(uisound);
        if (unlock)
        {
            MainUIMgr.Instance.ChangeMode(this);

            Debug.Log(PlayerPrefs.GetInt(ModeMgr.Instance.GetUnlockStringName(mode), 0));

            if (PlayerPrefs.GetInt(ModeMgr.Instance.GetUnlockStringName(mode), 0) == 0)//unlock 默认0，没解锁，播放对话
            {
                MainUIMgr.Instance.SetInConversation();

                unlockDialogue.PlayFeedbacks();
                PlayerPrefs.SetInt(ModeMgr.Instance.GetUnlockStringName(mode), 1);
            }

            OnClicked?.Invoke();
        }
        else
        {
            canClickFB.PlayFeedbacks();
        }
    }
    public void SetHighLighted()
    {
        animator.SetTrigger("Highlighted");
    }

    public void ResetAnim()
    {
        animator.SetTrigger("Normal");
    }
}
