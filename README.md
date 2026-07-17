# CatsGotYourCam

CatsGotYourCam is a high-level camera control framework for Kitten Space
Agency mods.

It does not add cameras, parts, UI, replay tools, or cinematic behavior by
itself. Instead, it gives other mods a shared way to take temporary ownership of
the in-game camera, evaluate camera transforms every frame, and hand control
back cleanly when the camera is no longer available.

Use this mod when you are building something like:

- Hull, docking, landing, or probe camera parts
- Cinematic or free-fly camera tools
- Replay, spectator, or mission-scripted cameras
- Any feature that needs to drive the game camera without directly fighting
  other camera mods

## Basic Model

Consumer mods provide an `ICameraSource`. CatsGotYourCam owns the camera
session stack and asks the active source for a `CameraState` each frame.

The important types are in the `CatsGotYourCam` namespace:

- `ICameraService`: entry point for requesting camera control.
- `ICameraSource`: implemented by your mod to produce camera state.
- `ICameraSession`: handle returned when your source is pushed.
- `CatsGotYourCamApi`: static access to the shared camera service.
- `CameraSources`: helpers for creating framework-provided camera sources.
- `CameraRequest`: options for how your session behaves when displaced.
- `CameraState`: position, rotation, and field of view to apply.
- `CameraEvaluationContext`: frame data passed into your source.

The flow is:

1. Your mod creates an `ICameraSource`.
2. Your mod calls `ICameraService.Push(source, request)`.
3. CatsGotYourCam makes that source the active session.
4. Each frame, CatsGotYourCam calls `source.TryEvaluate(...)`.
5. Your source returns a valid `CameraState`.
6. When finished, your mod calls `session.Release()` or disposes the session.

## Referencing the Framework

Add a reference to `CatsGotYourCam.dll` from your mod project and import the
namespace:

```csharp
using CatsGotYourCam;
```

Your StarMap `mod.toml` should declare CatsGotYourCam as a dependency and
import the `CatsGotYourCam` assembly:

```toml
[[StarMap.ModDependencies]]
ModId = "CatsGotYourCam"
Optional = false
ImportedAssemblies = [
    "CatsGotYourCam"
]
```

At runtime, get the shared camera service from `CatsGotYourCamApi`:

```csharp
ICameraService cameras = CatsGotYourCamApi.CameraService;
```

If your mod supports CatsGotYourCam as an optional dependency, use
`TryGetCameraService` instead:

```csharp
if(!CatsGotYourCamApi.TryGetCameraService(out ICameraService? cameras))
    return;
```

The public API is intentionally small. Most camera mods only need to implement
`ICameraSource` and hold on to the `ICameraSession` returned from
`ICameraService.Push`.

## Implementing a Camera Source

An `ICameraSource` is the object CatsGotYourCam evaluates while your session is
active.

```csharp
using Brutal.Numerics;
using CatsGotYourCam;

public sealed class CinematicCameraSource : ICameraSource {
    public string Id => "example.cinematic.follow";

    public string DisplayName => "Cinematic Follow Camera";

    public bool IsAvailable => _targetStillExists;

    private bool _targetStillExists = true;
    private double _time;

    public bool TryEvaluate(
        in CameraEvaluationContext context,
        out CameraState state) {

        _time += context.DeltaTime;

        // Replace this with your own target, orbit, spline, or scripted logic.
        double3 position = new double3(
            Math.Cos(_time) * 25.0,
            Math.Sin(_time) * 25.0,
            10.0);

        doubleQuat rotation = doubleQuat.Identity;

        state = new CameraState(
            PositionEgo: position,
            CameraToEgo: rotation,
            FieldOfView: 60.0);

        return true;
    }
}
```

`TryEvaluate` should return `false` when the source can no longer produce a
usable camera state. CatsGotYourCam will release the session and move to the
next available session, if any.

## Requesting Camera Control

Push your source through `ICameraService` when the player activates your camera.
Keep the returned session so you can release it later.

```csharp
private ICameraSession? _session;

public void EnableCamera(ICameraService cameras) {
    if(_session is not null &&
        _session.State != CameraSessionState.Released) {
        _session.TryBringToFront();
        return;
    }

    var source = new CinematicCameraSource();

    _session = cameras.Push(
        source,
        new CameraRequest {
            WhenDisplaced = CameraDisplacementBehavior.Suspend,
            ReleaseOnSceneChange = true
        });
}

public void DisableCamera() {
    _session?.Release();
    _session = null;
}
```

Always release the session when your feature is turned off, your part is
destroyed, your UI closes, or your mod unloads.

## Session Behavior

CatsGotYourCam uses a stack of camera sessions. The most recently pushed session
is active.

When another mod pushes a new source, your session is displaced. The behavior is
controlled by `CameraRequest.WhenDisplaced`:

- `CameraDisplacementBehavior.Suspend`: keep the session in the stack. It can
  become active again when sessions above it release.
- `CameraDisplacementBehavior.Release`: release the session immediately when
  another source takes over.

Use `Suspend` for cameras that should resume naturally, such as a part camera
that the player expects to return to. Use `Release` for one-shot or modal
cameras where being interrupted means the feature is done.

`ICameraSession.State` can be:

- `Active`: this source is currently driving the game camera.
- `Suspended`: another source is above this one in the stack.
- `Released`: the session is finished and cannot be reused.

Call `TryBringToFront()` on a suspended session if the player selects that
camera again.

## Scene Changes

By default, sessions are released on scene changes:

```csharp
new CameraRequest {
    ReleaseOnSceneChange = true
}
```

Keep this default unless your source is specifically designed to survive scene
transitions. A source that survives a scene change must not retain stale
scene-owned objects without revalidating them.

## Availability and Invalid States

CatsGotYourCam checks `ICameraSource.IsAvailable` before evaluating a source.
Return `false` when the source no longer has the objects it needs.

Examples:

- The camera part was destroyed or detached.
- The controlled vehicle changed and your source cannot follow it.
- The cinematic target no longer exists.
- Your UI/tool has been closed.

`CameraState` must contain finite values and a positive field of view. If the
state contains `NaN`, infinity, or `FieldOfView <= 0`, CatsGotYourCam releases
the session as invalid.

## Camera Coordinates

`CameraState` contains:

```csharp
public readonly record struct CameraState(
    double3 PositionEgo,
    doubleQuat CameraToEgo,
    double FieldOfView);
```

`PositionEgo` is the camera position in the game's ego/ecliptic camera space.
`CameraToEgo` is the camera orientation in that same space. `FieldOfView` is
passed to the game camera as the active lens field of view.

When CatsGotYourCam applies a state, it switches the main viewport to the
game's fixed camera mode, applies the state, and restores the previous camera
mode when no custom camera is active.

## Using Frame Context

`CameraEvaluationContext` gives your source the data it needs to evaluate the
current frame:

```csharp
public readonly struct CameraEvaluationContext {
    public double DeltaTime { get; init; }
    public double UnscaledDeltaTime { get; init; }
    public Vehicle? ControlledVehicle { get; init; }
    public required Viewport Viewport { get; init; }
    public long FrameIndex { get; init; }
}
```

Use `DeltaTime` for time-based camera motion, `ControlledVehicle` for vehicle
relative cameras, and `Viewport` when your camera needs viewport-specific data.

If `ControlledVehicle` is null and your source requires a vehicle, return
`false`.

## Part-Mounted Cameras

CatsGotYourCam includes a `CameraModule` type intended for part-camera mods:

```csharp
public sealed class CameraModule : Module<CameraModule>, IDisposable {
    public double3 LocalOffset { get; set; } = double3.Zero;
    public double3 LocalForward { get; set; } = double3.UnitX;
    public double3 LocalUp { get; set; } = double3.UnitZ;
    public double FieldOfView { get; set; } = 60.0;
    public string DisplayName { get; set; } = "Vehicle Camera";
}
```

A part camera should define the camera in the part's local assembly space:

- `LocalOffset`: where the camera sits relative to the part.
- `LocalForward`: the direction the camera looks.
- `LocalUp`: the camera's up direction.
- `FieldOfView`: the camera lens field of view.
- `DisplayName`: player-facing camera name.

Use `CameraSources.FromPartModule` to turn a `CameraModule` into an
`ICameraSource`:

```csharp
ICameraSource source =
    CameraSources.FromPartModule(cameraModule);

ICameraSession session =
    CatsGotYourCamApi.CameraService.Push(source);
```

You can also provide your own stable source ID:

```csharp
ICameraSource source =
    CameraSources.FromPartModule(
        cameraModule,
        "yourname.yourmod.forward-hull-camera");
```

The framework's part-camera source converts these local values through the
part, vehicle, body, and ecliptic transforms before producing a `CameraState`.
The concrete source type remains internal so its implementation details can
change without breaking consumer mods.

## Listening for Camera Changes

Use `ICameraService.ActiveSessionChanged` if your UI needs to react when camera
control moves between mods.

```csharp
cameras.ActiveSessionChanged += (_, args) => {
    switch(args.Reason) {
        case CameraSessionChangeReason.Pushed:
        case CameraSessionChangeReason.BroughtToFront:
            // args.Current is now active.
            break;

        case CameraSessionChangeReason.Released:
        case CameraSessionChangeReason.SourceUnavailable:
        case CameraSessionChangeReason.InvalidState:
        case CameraSessionChangeReason.SceneChanged:
        case CameraSessionChangeReason.FrameworkDisposed:
            // The previous active session ended or changed.
            break;
    }
};
```

The event provides:

- `Previous`: the previously active session, if any.
- `Current`: the newly active session, if any.
- `Reason`: why the active session changed.

## Recommended Patterns

- Use stable, globally unique source IDs, such as
  `yourname.yourmod.camera-name`.
- Keep `TryEvaluate` fast. It runs every frame while your session is active.
- Return `false` instead of throwing when your target disappears.
- Release sessions explicitly. Do not wait for scene cleanup.
- Prefer `ReleaseOnSceneChange = true` unless you have revalidation logic.
- Avoid writing directly to the game camera from your mod while a
  CatsGotYourCam session is active.
- Use `TryBringToFront()` when reselecting an existing camera instead of
  pushing duplicate sessions.

## Current Limitations

CatsGotYourCam exposes the shared camera service and a part-module source
factory. It does not yet provide a high-level browser or selector for all
`CameraModule` instances on the active vehicle. Mods that add camera parts
should discover their own modules, create sources for the cameras they expose,
and push the selected source through `ICameraService`.
