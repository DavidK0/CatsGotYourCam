
using Brutal.Numerics;

namespace CatsGotYourCam;

public readonly record struct CameraState(
    double3 PositionEgo,
    doubleQuat CameraToEgo,
    double FieldOfView,
    double NearClip,
    double FarClip);