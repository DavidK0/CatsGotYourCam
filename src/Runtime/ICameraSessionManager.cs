namespace CatsGotYourCam;

internal interface ICameraSessionManager : ICameraService {
    new CameraSession? ActiveSession { get; }

    void Release(
        CameraSession session,
        CameraSessionChangeReason reason);
}
