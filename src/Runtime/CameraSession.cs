namespace CatsGotYourCam;

internal sealed class CameraSession : ICameraSession {
    private CameraService? _owner;

    public ICameraSource Source { get; }

    internal CameraRequest Request { get; }

    public CameraSessionState State { get; internal set; }

    public bool IsActive =>
        State == CameraSessionState.Active;

    public bool IsSuspended =>
        State == CameraSessionState.Suspended;

    public bool IsReleased =>
        State == CameraSessionState.Released;

    internal CameraSession(
        CameraService owner,
        ICameraSource source,
        CameraRequest request) {
        _owner = owner
            ?? throw new ArgumentNullException(nameof(owner));

        Source = source
            ?? throw new ArgumentNullException(nameof(source));

        Request = request
            ?? throw new ArgumentNullException(nameof(request));

        State = CameraSessionState.Suspended;
    }

    public bool TryBringToFront() {
        CameraService? owner = _owner;

        return owner is not null &&
            owner.TryBringToFront(this);
    }

    public void Release() {
        Release(CameraSessionChangeReason.Released);
    }

    internal void Release(CameraSessionChangeReason reason) {
        CameraService? owner = _owner;

        if(owner is null)
            return;

        owner.Release(this, reason);
    }

    internal void MarkReleased() {
        State = CameraSessionState.Released;
        _owner = null;
    }

    public void Dispose() {
        Release();
    }
}
