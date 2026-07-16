namespace CatsGotYourCam;

internal interface IGameCameraAdapter : IDisposable {
    void Apply(in CameraState state);

    void RestoreDefaultCamera();
}