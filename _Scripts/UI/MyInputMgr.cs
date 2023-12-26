using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
#if !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif


public class MyInputMgr : Singleton<MyInputMgr>
{
    public static System.Action<UIBlock> OnPostClick;
    private Camera cam = null;

    public GameObject motionControl;
    public MotionUIControl motionControl1;

    public GameObject instantMotionUI;

    public bool blockInput = false;

    [Header("Mouse")]
    [SerializeField]
    [Tooltip("Handle mouse input events.")]
    private bool mouseEnabled = true;
    [SerializeField]
    [Tooltip("Inverts the mouse wheel scroll direction.")]
    private bool invertScrolling = true;


    [Header("Touch")]
    [SerializeField]
    [Tooltip("Handle touch input events.")]
    private bool touchEnabled = true;

    [Header("Navigation")]
    [SerializeField]
    [Tooltip("Handle navigation input events.")]
    private bool navigationEnabled = false;
    [SerializeField]
    [Tooltip("The starting point for navigation focus.")]
    private GestureRecognizer startNavigationFrom = null;
    [Tooltip("The UIBlock to move around as navigation focus changes. Will be created if null.")]
    public UIBlock NavigationIndicator = null;
    [SerializeField]
    [Tooltip("Move navigation focus at this rate (in seconds) while its corresponding key is pressed.")]
    private float updateNavigationEveryXSeconds = 0.25f;


    public Camera Cam
    {
        get => cam;
        set
        {
            if (value == null)
            {
                Debug.LogError("Provided Camera is null. Please assign a valid Camera.", this);
                return;
            }

            cam = value;
        }
    }

    public bool TouchEnabled
    {
        get => touchEnabled;
        set => touchEnabled = value;
    }

    public bool NavigationEnabled
    {
        get => navigationEnabled;
        set => navigationEnabled = value;
    }

    private void OnEnable()
    {
        if (cam == null)
        {
            cam = Camera.current;
        }

        if (cam == null)
        {
            cam = Camera.main;
        }

        Navigation.OnNavigationFocusChanged += HandleNavigationFocusChanged;



#if !ENABLE_LEGACY_INPUT_MANAGER
            EnhancedTouchSupport.Enable();
#endif
    }
    public void OpenCommunity()
    {
         blockInput = true;
         
         if(navigationEnabled&&motionControl1!=null)
         {
            //motionControl1.Pause();
         }
    }
    public void CloseCommunity()
    {
        
         blockInput = false;
         if(navigationEnabled&&motionControl1!=null)
         {
            //motionControl1.Resume();
         }
        
    }

    public void BlockInput()
    {
        blockInput = true;

    }

    public void RecoverInput()
    {

        blockInput = false;
    }
    private void Start()
    {
        navigationEnabled=PlayerPrefs.GetInt(Global.useMotionUI,1)>0?true:false;//默认开启手势控制UI
        InstantiateMotionUI();
    }

    public void InstantiateMotionUI()
    {
       StartCoroutine(MotionUIInstantiate());
    }
    IEnumerator MotionUIInstantiate()
    {
        yield return new WaitForSeconds(1);
         if (navigationEnabled&&instantMotionUI==null)
        {
            instantMotionUI=Instantiate(motionControl);
            motionControl1=instantMotionUI.GetComponent<MotionUIControl>();
        }

    }

    public void ChangeMotionControl(bool change)
    {
        
        if(!change)//关闭
        {
            if(instantMotionUI!=null)
            {
                Destroy(instantMotionUI);
                motionControl1=null;
            }
        }
        else
        {
            if(instantMotionUI==null)
            {
                instantMotionUI=Instantiate(motionControl);
                motionControl1=instantMotionUI.GetComponent<MotionUIControl>();
            }
        }
        navigationEnabled=change;
    }


    private void OnDisable()
    {
        Navigation.OnNavigationFocusChanged -= HandleNavigationFocusChanged;
    }

    private void Update()
    {
        if (blockInput)
        {
            return;
        }
        UpdateMouse();
        UpdateTouch();
        UpdateNavigation();
    }
    #region Mouse
    private const uint MousePointerControlID = 1;
    private const uint ScrollWheelControlID = 2;

#if ENABLE_LEGACY_INPUT_MANAGER
    private bool MousePresent => Input.mousePresent;
    private Vector2 MousePosition => Input.mousePosition;
    private Vector2 MouseScrollDelta => Input.mouseScrollDelta;
    private bool LeftMouseButtonValue => Input.GetMouseButton(0);
    private bool LeftMouseButtonUp => Input.GetMouseButtonUp(0);
#else
        private bool MousePresent => Mouse.current != null;
        private Vector2 MousePosition => Mouse.current.position.ReadValue();
        private Vector2 MouseScrollDelta => Mouse.current.scroll.ReadValue().normalized;
        private bool LeftMouseButtonValue => Mouse.current.leftButton.isPressed;
        private bool LeftMouseButtonUp => Mouse.current.leftButton.wasReleasedThisFrame;
#endif

    private void UpdateMouse()
    {
        if (cam == null || !mouseEnabled || !MousePresent)
        {
            return;
        }

        // Get the current world-space ray of the mouse
        Ray mouseRay = cam.ScreenPointToRay(MousePosition);

        // Get the current scroll wheel delta
        Vector2 mouseScrollDelta = MouseScrollDelta;

        if (mouseScrollDelta != Vector2.zero)
        {
            // Invert scrolling for a mouse-type experience,
            // otherwise will scroll track-pad style.
            if (invertScrolling)
            {
                mouseScrollDelta.y *= -1f;
            }

            // Create a new Interaction.Update from the mouse ray and scroll wheel control id
            Interaction.Update scrollInteraction = new Interaction.Update(mouseRay, ScrollWheelControlID);

            // Feed the scroll update and scroll delta into Nova's Interaction APIs
            Interaction.Scroll(scrollInteraction, mouseScrollDelta);
        }

        // Create a new Interaction.Update from the mouse ray and pointer control id
        Interaction.Update pointInteraction = new Interaction.Update(mouseRay, MousePointerControlID);

        // Feed the pointer update and pressed state to Nova's Interaction APIs
        Interaction.Point(pointInteraction, LeftMouseButtonValue);

        if (LeftMouseButtonUp)
        {
            // If the mouse button was released this frame, fire the OnPostClick
            // event with the hit UIBlock (or null if there wasn't one)
            Interaction.TryGetActiveReceiver(MousePointerControlID, out UIBlockHit hit);
            OnPostClick?.Invoke(hit.UIBlock);
        }
    }
    #endregion


    #region Touch

#if ENABLE_LEGACY_INPUT_MANAGER
    private bool TouchSupported => Input.touchSupported;
    private int TouchCount => Input.touchCount;
    private Touch GetTouch(int index) => Input.GetTouch(index);
    private Vector2 GetTouchPosition(Touch touch) => touch.position;
    private uint GetTouchID(Touch touch) => (uint)touch.fingerId;
#else
        private bool TouchSupported => Touchscreen.current != null;
        private int TouchCount => Touch.activeTouches.Count;
        private Touch GetTouch(int index) => Touch.activeTouches[index];
        private Vector2 GetTouchPosition(Touch touch) => touch.screenPosition;
        private uint GetTouchID(Touch touch) => (uint)touch.finger.index;
#endif

    private void UpdateTouch()
    {
        if (cam == null || !touchEnabled || !TouchSupported)
        {
            return;
        }

        for (int i = 0; i < TouchCount; i++)
        {
            Touch touch = GetTouch(i);

            // Convert the touch point to a world-space ray.
            Ray ray = cam.ScreenPointToRay(GetTouchPosition(touch));

            // Create a new Interaction from the ray and the finger's ID
            Interaction.Update update = new Interaction.Update(ray, GetTouchID(touch));

            // Get the current touch phase
            TouchPhase touchPhase = touch.phase;

            // If the touch phase hasn't ended and hasn't been canceled, then pointerDown == true.
            bool pointerDown = touchPhase != TouchPhase.Canceled && touchPhase != TouchPhase.Ended;

            // Feed the update and pressed state to Nova's Interaction APIs
            Interaction.Point(update, pointerDown);

            if (!pointerDown)
            {
                // If the finger is off the screen, send a follow up hover that
                // won't hit anything and cancel to indicate the interaction is over.
                // This is to assist with cross platform compatibility for hover events,
                // since touch events are driven by press/release and don't provide the same
                // kind of "pointer exit" data as an unpressed mouse pointer
                update.Ray = default;
                Interaction.Point(update, false);
                Interaction.Cancel(update);
            }
        }
    }
    #endregion
    ////待改，导航输入
    #region Navigation     
    private bool GestureReady => TestListener.Instance != null;
#if ENABLE_LEGACY_INPUT_MANAGER

    private bool NavigateUp => TestListener.Instance.IsSwipeUp();
    private bool NavigateDown => TestListener.Instance.IsSwipeDown();
    private bool NavigateLeft => TestListener.Instance.IsSwipeLeft();
    private bool NavigateRight => TestListener.Instance.IsSwipeRight();
    private bool Select => TestListener.Instance.IsSquash();
    private bool Deselect => TestListener.Instance.IsExtend();
#else
       private bool NavigateUp => TestListener.Instance.IsSwipeUp();
    private bool NavigateDown => TestListener.Instance.IsSwipeDown();
    private bool NavigateLeft => TestListener.Instance.IsSwipeLeft();
    private bool NavigateRight => TestListener.Instance.IsSwipeRight();
        private bool Select => TestListener.Instance.IsSquash();
        private bool Deselect => TestListener.Instance.IsExtend();
#endif
    private const uint NavigationID = 3;
    private float prevNavTime = 0;

    public void ChangeNavigationTo(UIBlock uIBlock)
    {
        EnsureNavigationIndicator();
        Navigation.Focus(uIBlock, NavigationID);
        NavigationIndicator.gameObject.SetActive(false);

    }

    private void UpdateNavigation()
    {
        bool canNavigate = navigationEnabled && GestureReady;
        bool navigating = Navigation.TryGetFocusedUIBlock(NavigationID, out UIBlock focused);


        if (canNavigate && !navigating)
        {
            // Initialize
            EnsureNavigationIndicator();
            Navigation.Focus(startNavigationFrom.UIBlock, NavigationID);
        }

        if (NavigationIndicator != null)
        {
            // Ensure visibility matches navigable status
            NavigationIndicator.gameObject.SetActive(canNavigate && navigating);
        }

        if (!canNavigate || !navigating)
        {
            // Not navigable
            return;
        }

        Vector3 navDirection = Vector3.zero;

        float timeSinceNav = Time.unscaledTime - prevNavTime;

        if (NavigateDown)
        {
            navDirection += Vector3.down;
        }

        if (NavigateUp)
        {
            navDirection += Vector3.up;
        }

        if (NavigateRight)
        {
            navDirection += Vector3.right;
        }

        if (NavigateLeft)
        {
            navDirection += Vector3.left;
        }

        if (navDirection == Vector3.zero)
        {
            // if no navigation key is pressed,
            // "reset" prevNavTime.
            prevNavTime = Time.unscaledTime - updateNavigationEveryXSeconds;
        }

        if (navDirection != Vector3.zero)
        {
            if (timeSinceNav > updateNavigationEveryXSeconds)
            {
                prevNavTime = Time.unscaledTime;
                Navigation.Move(navDirection, NavigationID);
            }
        }
        else if (Deselect)
        {
            Navigation.Deselect(NavigationID);
        }
        else if (Select)
        {
            Navigation.Select(NavigationID);
        }

        if (focused == null)
        {
            // Nothing to update
            return;
        }

        // Match the world scale of the focused element
        Vector3 parentScale = NavigationIndicator.transform.parent == null ? Vector3.one : NavigationIndicator.transform.parent.lossyScale;
        Vector3 focusedScale = focused.transform.lossyScale;
        NavigationIndicator.transform.localScale = new Vector3(focusedScale.x / parentScale.x, focusedScale.y / parentScale.y, focusedScale.z / parentScale.z);

        // Update size and position to match whatever's focused
        NavigationIndicator.Size = focused.CalculatedSize.Value + NavigationIndicator.CalculatedPadding.Size;
        NavigationIndicator.TrySetWorldPosition(focused.transform.position);
        NavigationIndicator.transform.rotation = focused.transform.rotation;
    }

    private void HandleNavigationFocusChanged(uint controlID, UIBlock focused)
    {
        // When navigation focus changes, treat that
        // as though something new was clicked, since
        // we're no longer "hovering" on the previously
        // focused element.
        OnPostClick?.Invoke(focused);
    }

    private void EnsureNavigationIndicator()
    {
        if (NavigationIndicator != null)
        {
            return;
        }

        // Create a new game object to be our focus indicator
        UIBlock2D indicator = new GameObject("Navigation Indicator").AddComponent<UIBlock2D>();

        // Hide the body and enable the border
        indicator.BodyEnabled = false;
        indicator.Border = new Border()
        {
            Enabled = true,
            Width = 12,
            Direction = BorderDirection.Out,
            Color = new Color(1, 0, 1, 1),
        };

        // Round the cornes a bit
        indicator.CornerRadius = 2;

        // Add some padding
        indicator.Padding.XY = 5;

        // Make sure the indicator will render over the other content
        SortGroup sortGroup = indicator.gameObject.AddComponent<SortGroup>();
        sortGroup.RenderQueue = 4001;
        sortGroup.RenderOverOpaqueGeometry = true;

        // assign the focus indicator
        NavigationIndicator = indicator;
    }

    #endregion
}
