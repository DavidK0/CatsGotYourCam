namespace CatsGotYourCam;

public readonly record struct CameraTransition {
    public static CameraTransition None { get; } =
        new(TimeSpan.Zero);

    public TimeSpan Duration { get; }

    public bool IsEnabled =>
        Duration > TimeSpan.Zero;

    public CameraTransition(TimeSpan duration) {
        if(duration < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(
                nameof(duration),
                duration,
                "Camera transition duration cannot be negative.");
        }

        Duration = duration;
    }
}