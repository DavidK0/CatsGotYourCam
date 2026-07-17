namespace CatsGotYourCam;

internal sealed class CameraRuntime : IDisposable {
    private const int MaxContextFailureFrames = 3;

    private readonly ICameraSessionManager _cameraService;
    private readonly IGameCameraAdapter _gameCamera;
    private readonly ICameraContextProvider _contextProvider;
    private readonly ICameraStateValidator _validator;

    private int _contextFailureFrames;
    private long _frameIndex;
    private bool _disposed;

    public CameraRuntime(
        ICameraSessionManager cameraService,
        IGameCameraAdapter gameCamera,
        ICameraContextProvider contextProvider,
        ICameraStateValidator validator) {
        _cameraService = cameraService
            ?? throw new ArgumentNullException(
                nameof(cameraService));

        _gameCamera = gameCamera
            ?? throw new ArgumentNullException(
                nameof(gameCamera));

        _contextProvider = contextProvider
            ?? throw new ArgumentNullException(
                nameof(contextProvider));

        _validator = validator
            ?? throw new ArgumentNullException(
                nameof(validator));
    }

    public void Update(double deltaTime) {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);

        _frameIndex++;

        while(true) {
            CameraSession? session =
                _cameraService.ActiveSession;

            if(session is null) {
                _contextFailureFrames = 0;
                _gameCamera.RestoreDefaultCamera();
                return;
            }

            ICameraSource source =
                session.Source;

            if(!source.IsAvailable) {
                _cameraService.Release(
                    session,
                    CameraSessionChangeReason.SourceUnavailable);

                continue;
            }

            if(TryApplySource(
                session,
                source,
                deltaTime))
                return;
        }
    }

    private bool TryApplySource(
        CameraSession session,
        ICameraSource source,
        double deltaTime) {
        if(!_contextProvider.TryGetContext(
            deltaTime,
            _frameIndex,
            out CameraEvaluationContext context)) {
            _contextFailureFrames++;

            if(_contextFailureFrames > MaxContextFailureFrames)
                _gameCamera.RestoreDefaultCamera();

            return true;
        }

        _contextFailureFrames = 0;

        if(!source.TryEvaluate(context, out CameraState state)) {
            _cameraService.Release(
                session,
                CameraSessionChangeReason.SourceUnavailable);

            return false;
        }

        if(!_validator.TryValidate(state, out CameraState validated)) {
            _cameraService.Release(
                session,
                CameraSessionChangeReason.InvalidState);

            return false;
        }

        _gameCamera.Apply(validated);
        return true;
    }

    public void Dispose() {
        if(_disposed)
            return;

        _gameCamera.RestoreDefaultCamera();
        _gameCamera.Dispose();

        _disposed = true;
    }
}
