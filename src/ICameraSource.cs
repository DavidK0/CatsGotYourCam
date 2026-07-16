namespace CatsGotYourCam;

public interface ICameraSource {
    string Id { get; }

    string DisplayName { get; }

    bool IsAvailable { get; }

    CameraState Evaluate(in CameraEvaluationContext context);
}