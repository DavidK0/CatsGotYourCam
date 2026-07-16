namespace CatsGotYourCam;

public enum CameraSessionChangeReason {
    Pushed,
    Released,
    BroughtToFront,
    SourceUnavailable,
    SceneChanged,
    FrameworkDisposed
}