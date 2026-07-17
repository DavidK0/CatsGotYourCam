namespace CatsGotYourCam;

internal interface ICameraContextProvider {
    bool TryGetContext(
        double deltaTime,
        long frameIndex,
        out CameraEvaluationContext context);
}
