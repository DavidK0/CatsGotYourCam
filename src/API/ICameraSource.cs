namespace CatsGotYourCam;

public interface ICameraSource {
    string Id { get; }

    string DisplayName { get; }

    bool IsAvailable { get; }

    bool TryEvaluate(
        in CameraEvaluationContext context,
        out CameraState state);
}
