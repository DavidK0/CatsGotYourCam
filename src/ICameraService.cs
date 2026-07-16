namespace CatsGotYourCam;

public interface ICameraService {
    ICameraSession Push(
        ICameraSource source,
        CameraRequest? request = null);

    ICameraSession? ActiveSession { get; }

    event EventHandler<CameraSessionChangedEventArgs>?
        ActiveSessionChanged;
}