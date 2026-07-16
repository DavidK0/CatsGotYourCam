namespace CatsGotYourCam;

public enum CameraSessionChangeReason {
    Pushed,
    Released,
    BroughtToFront,
    SourceUnavailable,
    InvalidState,
    SceneChanged,
    FrameworkDisposed
}
