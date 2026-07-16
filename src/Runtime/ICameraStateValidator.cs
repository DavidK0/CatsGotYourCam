namespace CatsGotYourCam;

internal interface ICameraStateValidator {
    bool TryValidate(
        in CameraState state,
        out CameraState validated);
}
