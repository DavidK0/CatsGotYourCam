namespace CatsGotYourCam;

public sealed record CameraRequest {
    public CameraDisplacementBehavior WhenDisplaced { get; init; }
        = CameraDisplacementBehavior.Suspend;

    /// <summary>
    /// Determines whether the session is automatically released when the
    /// framework detects a scene transition. Sources that survive a scene
    /// transition must not retain scene-owned objects without revalidating them.
    /// </summary>
    public bool ReleaseOnSceneChange { get; init; }
        = true;
}
