using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.Holistic;
public class HolisticMotion : ImageSourceSolution<HolisticTrackingGraph>
{
    public static HolisticMotion instance;
    public bool useScreen=true;
    List<ILandmarkObserver> poseLandmarkObservers = new List<ILandmarkObserver>();
    List<ILandmarkObserver> faceLandmarkObservers = new List<ILandmarkObserver>();
    List<ILandmarkObserver> leftHandLandmarkObservers = new List<ILandmarkObserver>();
    List<ILandmarkObserver> rightHandLandmarkObservers = new List<ILandmarkObserver>();
    
    List<IWorldLandmarkObserver > worldPoseLandmarkObservers = new List<IWorldLandmarkObserver >();
    public void AddWorldPoseObserver(IWorldLandmarkObserver  observer)
    {
        worldPoseLandmarkObservers.Add(observer);
    }

    public void RemoveWorldPoseObserver(IWorldLandmarkObserver  observer)
    {
        worldPoseLandmarkObservers.Remove(observer);
    }

    public void AddPoseObserver(ILandmarkObserver observer)
    {
        poseLandmarkObservers.Add(observer);
    }

    public void RemovePoseObserver(ILandmarkObserver observer)
    {
        poseLandmarkObservers.Remove(observer);
    }
    public void AddFaceObserver(ILandmarkObserver observer)
    {
        faceLandmarkObservers.Add(observer);
    }
    public void RemoveFaceObserver(ILandmarkObserver observer)
    {
        faceLandmarkObservers.Remove(observer);
    }
    public void AddLhandObserver(ILandmarkObserver observer)
    {
        leftHandLandmarkObservers.Add(observer);
    }
     public void RemoveLhandObserver(ILandmarkObserver observer)
    {
        leftHandLandmarkObservers.Remove(observer);
    }
    public void AddRhandObserver(ILandmarkObserver observer)
    {
        rightHandLandmarkObservers.Add(observer);
    }

     public void RemoveRhandObserver(ILandmarkObserver observer)
    {
        rightHandLandmarkObservers.Remove(observer);
    }


    private void OnDestroy()
    {
        poseLandmarkObservers.Clear();
        faceLandmarkObservers.Clear();
        leftHandLandmarkObservers.Clear();
        rightHandLandmarkObservers.Clear();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private HolisticLandmarkListAnnotationController _holisticAnnotationController;
    Detection poseDetection = null;
    public NormalizedLandmarkList faceLandmarks = null;
    public NormalizedLandmarkList poseLandmarks = null;
    public NormalizedLandmarkList leftHandLandmarks = null;
    public NormalizedLandmarkList rightHandLandmarks = null;
    public LandmarkList poseWorldLandmarks = null;
    public ImageFrame segmentationMask = null;
    public NormalizedRect poseRoi = null;

    public HolisticTrackingGraph.ModelComplexity modelComplexity
    {
        get => graphRunner.modelComplexity;
        set => graphRunner.modelComplexity = value;
    }

    public bool smoothLandmarks
    {
        get => graphRunner.smoothLandmarks;
        set => graphRunner.smoothLandmarks = value;
    }

    public bool refineFaceLandmarks
    {
        get => graphRunner.refineFaceLandmarks;
        set => graphRunner.refineFaceLandmarks = value;
    }

    public bool enableSegmentation
    {
        get => graphRunner.enableSegmentation;
        set => graphRunner.enableSegmentation = value;
    }

    public bool smoothSegmentation
    {
        get => graphRunner.smoothSegmentation;
        set => graphRunner.smoothSegmentation = value;
    }

    public float minDetectionConfidence
    {
        get => graphRunner.minDetectionConfidence;
        set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
        get => graphRunner.minTrackingConfidence;
        set => graphRunner.minTrackingConfidence = value;
    }

    protected override void SetupScreen(ImageSource imageSource)
    {
        if(useScreen)
        {
            base.SetupScreen(imageSource);
        }
        
    }

    protected override void OnStartRun()
    {
    
        if (!runningMode.IsSynchronous())
        {
            graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
            graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
            graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
            graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
            graphRunner.OnPoseWorldLandmarksOutput+=OnPoseWorldLandmarksOutput;
        }
        if(useScreen)
        {
            var imageSource = ImageSourceProvider.ImageSource;
            SetupAnnotationController(_holisticAnnotationController, imageSource);
        }
        
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
        graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {


        if (runningMode == RunningMode.Sync)
        {
            var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, true);
        }
        else if (runningMode == RunningMode.NonBlockingSync)
        {
            yield return new WaitUntil(() =>
              graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, false));
        }
       
    }

    private void OnFaceLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
        foreach (var observer in faceLandmarkObservers)
        {
            observer.UpdateLandmarks(eventArgs.value);
        }
       //_holisticAnnotationController.DrawFaceLandmarkListLater(eventArgs.value);
    }

    private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
    {
        foreach (var observer in worldPoseLandmarkObservers)
        {
            observer.UpdateWorldLandmarks(eventArgs.value);
        }
    }

    private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
       
        //_holisticAnnotationController.DrawPoseLandmarkListLater(eventArgs.value);
        foreach (var observer in poseLandmarkObservers)
        {
            observer.UpdateLandmarks(eventArgs.value);
        }
    }

    private void OnLeftHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
        //_holisticAnnotationController.DrawLeftHandLandmarkListLater(eventArgs.value);
        foreach (var observer in leftHandLandmarkObservers)
        {
            observer.UpdateLandmarks(eventArgs.value);
        }
    }

    private void OnRightHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
        //_holisticAnnotationController.DrawRightHandLandmarkListLater(eventArgs.value);
        foreach (var observer in rightHandLandmarkObservers)
        {
            observer.UpdateLandmarks(eventArgs.value);
        }
    }

}