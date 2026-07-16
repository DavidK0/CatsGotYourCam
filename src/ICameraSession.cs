namespace CatsGotYourCam;

public interface ICameraSession : IDisposable {
    ICameraSource Source { get; }

    CameraSessionState State { get; }

    bool TryBringToFront();

    void Release();
}