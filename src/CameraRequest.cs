namespace CatsGotYourCam;

public sealed record CameraRequest {
    public CameraDisplacementBehavior WhenDisplaced { get; init; }
        = CameraDisplacementBehavior.Suspend;

    /// <summary>
    /// Transition used when this session becomes active by being pushed
    /// or brought to the front.
    /// </summary>
    public CameraTransition BlendIn { get; init; }
        = CameraTransition.None;

    /// <summary>
    /// Transition used when this active session is released and control
    /// returns to the session below it or to the game camera.
    /// </summary>
    public CameraTransition BlendOut { get; init; }
        = CameraTransition.None;

    public bool ReleaseOnSceneChange { get; init; }
        = true;
}