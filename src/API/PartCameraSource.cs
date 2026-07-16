using Brutal.Numerics;
using KSA;

namespace CatsGotYourCam;

internal sealed class PartCameraSource : ICameraSource {
    private readonly CameraModule _module;

    public string Id { get; }

    public string DisplayName =>
        _module.DisplayName;

    public bool IsAvailable {
        get {
            Part part = _module.Parent;

            return part.Tree is not null;
        }
    }

    public PartCameraSource(
        CameraModule module,
        string id) {
        _module = module ??
            throw new ArgumentNullException(nameof(module));

        Id = id ??
            throw new ArgumentNullException(nameof(id));
    }

    public bool TryEvaluate(
        in CameraEvaluationContext context,
        out CameraState state) {
        Vehicle? vehicle = context.ControlledVehicle;
        Part cameraPart = _module.Parent;

        if(vehicle is null ||
            vehicle.Parent is null ||
            cameraPart.Tree is null ||
            !BelongsToVehicle(cameraPart, vehicle)) {
            state = default;
            return false;
        }

        doubleQuat partToVehicle =
            cameraPart.Asmb2VehicleAsmb;

        double3 positionVehicle =
            cameraPart.PositionVehicleAsmb +
            partToVehicle * _module.LocalOffset;

        double3 forwardVehicle =
            partToVehicle * _module.LocalForward;

        double3 upVehicle =
            partToVehicle * _module.LocalUp;

        doubleQuat vehicleToEgo =
            vehicle.Asmb2Ego;

        double3 positionEgo =
            vehicle.PositionEgo +
            vehicleToEgo * positionVehicle;

        double3 forwardEgo =
            vehicleToEgo * forwardVehicle;

        double3 upEgo =
            vehicleToEgo * upVehicle;

        if(!TryCreateOrientation(
            forwardEgo,
            upEgo,
            out doubleQuat cameraToEgo)) {
            state = default;
            return false;
        }

        state = new CameraState(
            PositionEgo: positionEgo,
            CameraToEgo: cameraToEgo,
            FieldOfView: _module.FieldOfView,
            NearClip: _module.NearClip,
            FarClip: _module.FarClip);

        return true;
    }

    private static bool BelongsToVehicle(
        Part part,
        Vehicle vehicle) {
        return ReferenceEquals(
            part.Tree,
            vehicle.PartTree);
    }

    private static bool TryCreateOrientation(
        double3 forward,
        double3 up,
        out doubleQuat orientation) {
        if(VectorMath.IsZero(forward) ||
            VectorMath.IsZero(up)) {
            orientation = default;
            return false;
        }

        orientation = doubleQuat.CreateLookTo(
            forward,
            up);

        return true;
    }
}
