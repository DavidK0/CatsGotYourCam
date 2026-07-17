namespace CatsGotYourCam;

public static class CatsGotYourCamApi {
    public static bool IsAvailable =>
        CameraHost.IsInitialized;

    public static ICameraService CameraService =>
        CameraHost.Service;

    public static bool TryGetCameraService(
        out ICameraService? cameraService) {
        if(!CameraHost.IsInitialized) {
            cameraService = null;
            return false;
        }

        cameraService = CameraHost.Service;
        return true;
    }
}
