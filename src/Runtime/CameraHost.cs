namespace CatsGotYourCam;

internal static class CameraHost {
    private static CameraService? _service;
    private static CameraRuntime? _runtime;

    public static bool IsInitialized =>
        _service is not null &&
        _runtime is not null;

    public static ICameraService Service =>
        _service ??
        throw new InvalidOperationException(
            "CatsGotYourCam has not initialized its camera service.");

    public static void Initialize() {
        if(IsInitialized)
            return;

        var service = new CameraService();

        _service = service;
        _runtime = new CameraRuntime(
            service,
            new GameCameraAdapter(),
            new GameCameraContextProvider(),
            new CameraStateValidator());
    }

    public static void Update(double deltaTime) {
        CameraRuntime? runtime = _runtime;

        if(runtime is null)
            return;

        runtime.Update(deltaTime);
    }

    public static void Dispose() {
        CameraRuntime? runtime = _runtime;
        CameraService? service = _service;

        _runtime = null;
        _service = null;

        runtime?.Dispose();
        service?.Dispose();
    }
}
