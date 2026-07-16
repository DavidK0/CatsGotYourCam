namespace CatsGotYourCam;

internal interface ICameraContextProvider {
    bool TryGetContext(out CameraEvaluationContext context);
}
