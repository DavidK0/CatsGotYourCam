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

        IParentBody parentBody = vehicle.Parent;

        doubleQuat partToVehicle =
            cameraPart.Asmb2VehicleAsmb;

        double3 positionVehicle =
            cameraPart.PositionVehicleAsmb +
            partToVehicle * _module.LocalOffset;

        double3 forwardVehicle =
            partToVehicle * _module.LocalForward;

        double3 upVehicle =
            partToVehicle * _module.LocalUp;

        doubleQuat bodyToCci =
            vehicle.GetBody2Cci();

        doubleQuat cciToCce =
            parentBody.GetCci2Cce();

        double3 positionEcl =
            vehicle.GetPositionEcl() +
            cciToCce * (bodyToCci * positionVehicle);

        double3 forwardEcl =
            cciToCce * (bodyToCci * forwardVehicle);

        double3 upEcl =
            cciToCce * (bodyToCci * upVehicle);

        if(!TryCreateOrientation(
            forwardEcl,
            upEcl,
            out doubleQuat cameraToEcl)) {
            state = default;
            return false;
        }

        state = new CameraState(
            PositionEgo: positionEcl,
            CameraToEgo: cameraToEcl,
            FieldOfView: _module.FieldOfView);

        return true;
    }

    private static bool BelongsToVehicle(
        Part part,
        Vehicle vehicle) {
        return ReferenceEquals(
            part.Tree,
            vehicle.Parts);
    }

    private static bool TryCreateOrientation(
        double3 forward,
        double3 up,
        out doubleQuat orientation) {

        return TryCreateLookTo(forward, up, out orientation);
    }

    private static bool TryCreateLookTo(
        double3 forward,
        double3 up,
        out doubleQuat orientation) {

        const double epsilon = 1e-12;

        double forwardLengthSquared =
            forward.X * forward.X +
            forward.Y * forward.Y +
            forward.Z * forward.Z;

        double upLengthSquared =
            up.X * up.X +
            up.Y * up.Y +
            up.Z * up.Z;

        if(forwardLengthSquared <= epsilon ||
           upLengthSquared <= epsilon) {
            orientation = default;
            return false;
        }

        double inverseForwardLength = 1.0 / Math.Sqrt(forwardLengthSquared);

        double3 zAxis = new double3(
            forward.X * inverseForwardLength,
            forward.Y * inverseForwardLength,
            forward.Z * inverseForwardLength);

        // right = normalize(cross(up, forward))
        double3 xAxis = Cross(up, zAxis);

        double rightLengthSquared =
            xAxis.X * xAxis.X +
            xAxis.Y * xAxis.Y +
            xAxis.Z * xAxis.Z;

        // forward and up are parallel or nearly parallel.
        if(rightLengthSquared <= epsilon) {
            orientation = default;
            return false;
        }

        double inverseRightLength = 1.0 / Math.Sqrt(rightLengthSquared);

        xAxis = new double3(
            xAxis.X * inverseRightLength,
            xAxis.Y * inverseRightLength,
            xAxis.Z * inverseRightLength);

        // Rebuild up so the basis is orthonormal.
        double3 yAxis = Cross(zAxis, xAxis);

        double4x4 rotationMatrix = new double4x4(
            xAxis.X, xAxis.Y, xAxis.Z, 0.0,
            yAxis.X, yAxis.Y, yAxis.Z, 0.0,
            zAxis.X, zAxis.Y, zAxis.Z, 0.0,
            0.0, 0.0, 0.0, 1.0);

        orientation = doubleQuat.Normalize(
            doubleQuat.CreateFromRotationMatrix(rotationMatrix));

        return true;
    }

    private static double3 Cross(double3 left, double3 right) {
        return new double3(
            left.Y * right.Z - left.Z * right.Y,
            left.Z * right.X - left.X * right.Z,
            left.X * right.Y - left.Y * right.X);
    }
}
