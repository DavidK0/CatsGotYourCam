using Brutal.Numerics;
using KSA;

namespace CatsGotYourCam;

public sealed class CameraModule
    : Module<CameraModule>, IDisposable {
    public double3 LocalOffset { get; set; } =
        double3.Zero;

    public double3 LocalForward { get; set; } =
        double3.UnitX;

    public double3 LocalUp { get; set; } =
        double3.UnitZ;

    public double FieldOfView { get; set; } = 60.0;

    public string DisplayName { get; set; } =
        "Vehicle Camera";

    public CameraModule() {
    }

    public override void Dispose() {
    }
}