using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.PoseTracking;

public class PoseTrack : ImageSourceSolution<PoseTrackingGraph>
{
    public bool useScreen=true;
    public static PoseTrack instance;
    List<ILandmarkObserver> landmarkObservers = new List<ILandmarkObserver>();
    
    List<IWorldLandmarkObserver > worldPoseLandmarkObservers = new List<IWorldLandmarkObserver >();
    [SerializeField] private PoseLandmarkListAnnotationController _poseLandmarksAnnotationController;
    public Detection poseDetection = null;
    public NormalizedLandmarkList poseLandmarks = null;
    public LandmarkList poseWorldLandmarks = null;
    public ImageFrame segmentationMask = null;
    public NormalizedRect roiFromLandmarks = null;

    public void ClearObserver()
    {
        worldPoseLandmarkObservers.Clear();
        landmarkObservers.Clear();
    }

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
        landmarkObservers.Add(observer);
    }

    private void OnDestroy()
    {
        landmarkObservers.Clear();
    }

    public PoseTrackingGraph.ModelComplexity modelComplexity
    {
        get => graphRunner.modelComplexity;
        set => graphRunner.modelComplexity = value;
    }

    public bool smoothLandmarks
    {
        get => graphRunner.smoothLandmarks;
        set => graphRunner.smoothLandmarks = value;
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
            graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
            graphRunner.OnPoseWorldLandmarksOutput+=OnPoseWorldLandmarksOutput;
        }
        // var imageSource = ImageSourceProvider.ImageSource;
        
        // SetupAnnotationController(_poseLandmarksAnnotationController, imageSource);
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

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
        graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
        if (runningMode == RunningMode.Sync)
        {
            var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, true);
        }
        else if (runningMode == RunningMode.NonBlockingSync)
        {
            yield return new WaitUntil(() => graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, false));
        }
        // _poseLandmarksAnnotationController.DrawNow(poseLandmarks);
    }

    private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {

        foreach (var observer in landmarkObservers)
        {
            observer.UpdateLandmarks(eventArgs.value);
        }
        //_poseLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

        private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
    {
        foreach (var observer in worldPoseLandmarkObservers)
        {
            observer.UpdateWorldLandmarks(eventArgs.value);
        }
    }
}
