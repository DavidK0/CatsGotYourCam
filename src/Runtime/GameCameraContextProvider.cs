using KSA;

namespace CatsGotYourCam;

internal sealed class GameCameraContextProvider
    : ICameraContextProvider {
    public bool TryGetContext(
        double deltaTime,
        long frameIndex,
        out CameraEvaluationContext context) {
        Viewport? viewport =
            GameCameraAdapter.MainViewport;

        if(viewport is null) {
            context = default;
            return false;
        }

        context = new CameraEvaluationContext {
            DeltaTime = deltaTime,
            UnscaledDeltaTime = deltaTime,
            ControlledVehicle = Program.ControlledVehicle,
            Viewport = viewport,
            FrameIndex = frameIndex
        };

        return true;
    }
}
