using Brutal.Numerics;
using KSA;

namespace CatsGotYourCam;

public sealed class GameCameraAdapter
    : IDisposable, IGameCameraAdapter {
    private static Viewport? _mainViewport;
    private static bool _isControllingMainViewport;

    private CameraState _activeState;
    private bool _hasActiveState;

    private CameraMode _previousCameraMode;
    private bool _hasSavedCameraMode;
    private bool _disposed;

    public bool IsActive => _hasActiveState;

    public void Apply(in CameraState state) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        Viewport? viewport = GetMainViewport();

        if(viewport is null)
            return;

        if(!_hasSavedCameraMode) {
            _previousCameraMode = viewport.Mode;
            _hasSavedCameraMode = true;
        }

        _activeState = state;
        _hasActiveState = true;
        _isControllingMainViewport = true;

        PrepareFixedCamera(viewport);
        ApplyActiveState(viewport);
    }

    public void RestoreDefaultCamera() {
        _hasActiveState = false;
        _isControllingMainViewport = false;

        Viewport? viewport = GetMainViewport();

        if(viewport is not null && _hasSavedCameraMode) {
            if(viewport.Mode != _previousCameraMode)
                viewport.SetCameraMode(_previousCameraMode);
        }

        _hasSavedCameraMode = false;
    }

    public void Dispose() {
        if(_disposed)
            return;

        RestoreDefaultCamera();
        _disposed = true;
    }

    private void ApplyActiveState(Viewport viewport) {
        if(!_hasActiveState)
            return;

        Camera? camera = viewport.GetCamera();

        if(camera is null)
            return;

        CameraState state = _activeState;

        camera.PositionEcl = state.PositionEgo;
        camera.LocalRotation = state.CameraToEgo;

        ApplyFieldOfView(camera, state.FieldOfView);
    }

    private static Viewport? GetMainViewport() {
        return _mainViewport;
    }

    internal static Viewport? MainViewport =>
        _mainViewport;

    internal static void SetMainViewport(Viewport viewport) {
        if(viewport.Index != 0)
            return;

        _mainViewport = viewport;
    }

    internal static void ClearMainViewport() {
        _mainViewport = null;
        _isControllingMainViewport = false;
    }

    internal static bool ShouldSuppressFixedController(
        Viewport viewport) {
        return _isControllingMainViewport &&
            viewport.Index == 0;
    }

    private static void PrepareFixedCamera(Viewport viewport) {
        if(viewport.Mode != CameraMode.Fixed)
            viewport.SetCameraMode(CameraMode.Fixed);
    }

    private static void ApplyFieldOfView(
        Camera camera,
        double fieldOfView) {
        camera.SetFieldOfView((float)fieldOfView);
    }
}

public static class VectorMath {
    public static bool IsZero(double3 v) {
        return Math.Abs(v.X) <= 1e-6 &&
               Math.Abs(v.Y) <= 1e-6 &&
               Math.Abs(v.Z) <= 1e-6;
    }
}
