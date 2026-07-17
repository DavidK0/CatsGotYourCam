namespace CatsGotYourCam;

public static class CameraSources {
    public static ICameraSource FromPartModule(CameraModule module) {
        ArgumentNullException.ThrowIfNull(module);

        return FromPartModule(
            module,
            $"catsgotyourcam.part.{module.InstanceId}");
    }

    public static ICameraSource FromPartModule(
        CameraModule module,
        string id) {
        ArgumentNullException.ThrowIfNull(module);

        if(string.IsNullOrWhiteSpace(id)) {
            throw new ArgumentException(
                "Camera source id must not be empty.",
                nameof(id));
        }

        return new PartCameraSource(module, id);
    }
}
