namespace CatsGotYourCam;

public sealed class CameraService
    : ICameraService, ICameraSessionManager, IDisposable {
    private readonly List<CameraSession> _sessions = new();

    private bool _disposed;

    private CameraSession? ActiveSessionInternal =>
        _sessions.Count == 0
            ? null
            : _sessions[^1];

    public ICameraSession? ActiveSession =>
        ActiveSessionInternal;

    CameraSession? ICameraSessionManager.ActiveSession =>
        ActiveSessionInternal;

    public event EventHandler<CameraSessionChangedEventArgs>?
        ActiveSessionChanged;

    public ICameraSession Push(
        ICameraSource source,
        CameraRequest? request = null) {
        ArgumentNullException.ThrowIfNull(source);
        ObjectDisposedException.ThrowIf(_disposed, this);

        if(!source.IsAvailable) {
            throw new InvalidOperationException(
                $"Camera source '{source.Id}' is not available.");
        }

        request ??= new CameraRequest();

        var session = new CameraSession(
            this,
            source,
            request);

        CameraSession? previous = ActiveSessionInternal;

        if(previous is not null) {
            Displace(previous);
        }

        _sessions.Add(session);
        session.State = CameraSessionState.Active;

        RaiseActiveSessionChanged(
            previous,
            session,
            CameraSessionChangeReason.Pushed);

        return session;
    }

    internal void Release(
        CameraSession session,
        CameraSessionChangeReason reason) {
        ArgumentNullException.ThrowIfNull(session);

        int index = _sessions.IndexOf(session);

        if(index < 0)
            return;

        CameraSession? previousActive =
            ActiveSessionInternal;

        bool wasActive =
            ReferenceEquals(session, previousActive);

        _sessions.RemoveAt(index);
        session.MarkReleased();

        if(!wasActive)
            return;

        CameraSession? resumed =
            ActiveSessionInternal;

        if(resumed is not null)
            resumed.State = CameraSessionState.Active;

        RaiseActiveSessionChanged(
            previousActive,
            resumed,
            reason);
    }

    void ICameraSessionManager.Release(
        CameraSession session,
        CameraSessionChangeReason reason) {
        Release(session, reason);
    }

    internal bool TryBringToFront(CameraSession session) {
        ArgumentNullException.ThrowIfNull(session);
        ObjectDisposedException.ThrowIf(_disposed, this);

        int index = _sessions.IndexOf(session);

        if(index < 0 ||
            session.State == CameraSessionState.Released) {
            return false;
        }

        CameraSession? previous = ActiveSessionInternal;

        if(ReferenceEquals(previous, session))
            return true;

        if(previous is not null)
            Displace(previous);

        // Displace may release entries, so locate the requested session
        // again before moving it.
        index = _sessions.IndexOf(session);

        if(index < 0)
            return false;

        _sessions.RemoveAt(index);
        _sessions.Add(session);
        session.State = CameraSessionState.Active;

        RaiseActiveSessionChanged(
            previous,
            session,
            CameraSessionChangeReason.BroughtToFront);

        return true;
    }

    private void Displace(CameraSession session) {
        switch(session.Request.WhenDisplaced) {
            case CameraDisplacementBehavior.Suspend:
                session.State = CameraSessionState.Suspended;
                break;

            case CameraDisplacementBehavior.Release:
                _sessions.Remove(session);
                session.MarkReleased();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ReleaseForSceneChange() {
        ObjectDisposedException.ThrowIf(_disposed, this);

        CameraSession? previous =
            ActiveSessionInternal;

        for(int index = _sessions.Count - 1;
            index >= 0;
            index--) {
            CameraSession session = _sessions[index];

            if(!session.Request.ReleaseOnSceneChange)
                continue;

            _sessions.RemoveAt(index);
            session.MarkReleased();
        }

        CameraSession? current =
            ActiveSessionInternal;

        if(current is not null)
            current.State = CameraSessionState.Active;

        if(!ReferenceEquals(previous, current)) {
            RaiseActiveSessionChanged(
                previous,
                current,
                CameraSessionChangeReason.SceneChanged);
        }
    }

    private void RaiseActiveSessionChanged(
        CameraSession? previous,
        CameraSession? current,
        CameraSessionChangeReason reason) {
        ActiveSessionChanged?.Invoke(
            this,
            new CameraSessionChangedEventArgs(
                previous,
                current,
                reason));
    }

    public void Dispose() {
        if(_disposed)
            return;

        CameraSession? previous =
            ActiveSessionInternal;

        foreach(CameraSession session in _sessions)
            session.MarkReleased();

        _sessions.Clear();
        _disposed = true;

        if(previous is not null) {
            RaiseActiveSessionChanged(
                previous,
                null,
                CameraSessionChangeReason.FrameworkDisposed);
        }
    }
}
