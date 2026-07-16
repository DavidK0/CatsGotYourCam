using KSA;

namespace CatsGotYourCam;

public readonly struct CameraEvaluationContext {
    public double DeltaTime { get; init; }

    public double UnscaledDeltaTime { get; init; }

    public Vehicle? ControlledVehicle { get; init; }

    public required Viewport Viewport { get; init; }

    public long FrameIndex { get; init; }
}