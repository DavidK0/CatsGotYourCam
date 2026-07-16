using Brutal.Numerics;

namespace CatsGotYourCam;

internal sealed class CameraStateValidator : ICameraStateValidator {
    public bool TryValidate(
        in CameraState state,
        out CameraState validated) {
        if(!IsFinite(state.PositionEgo) ||
            !IsFinite(state.CameraToEgo) ||
            !double.IsFinite(state.FieldOfView) ||
            !double.IsFinite(state.NearClip) ||
            !double.IsFinite(state.FarClip) ||
            state.FieldOfView <= 0 ||
            state.NearClip <= 0 ||
            state.FarClip <= state.NearClip) {
            validated = default;
            return false;
        }

        validated = state;
        return true;
    }

    private static bool IsFinite(double3 value) {
        return double.IsFinite(value.X) &&
            double.IsFinite(value.Y) &&
            double.IsFinite(value.Z);
    }

    private static bool IsFinite(doubleQuat value) {
        return double.IsFinite(value.X) &&
            double.IsFinite(value.Y) &&
            double.IsFinite(value.Z) &&
            double.IsFinite(value.W);
    }
}
