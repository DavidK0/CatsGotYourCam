namespace CatsGotYourCam;

public sealed class CameraSessionChangedEventArgs : EventArgs {
    public ICameraSession? Previous { get; }

    public ICameraSession? Current { get; }

    public CameraSessionChangeReason Reason { get; }

    public CameraSessionChangedEventArgs(
        ICameraSession? previous,
        ICameraSession? current,
        CameraSessionChangeReason reason) {
        Previous = previous;
        Current = current;
        Reason = reason;
    }
}