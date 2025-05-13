namespace DrawnUi.Camera;

public enum StretchModes
{
	/// <summary>
	/// Whatever
	/// </summary>
	Default,
	/// <summary>
	/// Fit inside screen bounds
	/// </summary>
	Fit,
	/// <summary>
	/// Fullscreen
	/// </summary>
	Fill
}

public class ExposureResult
{
    public bool Success { get; set; }
    public double ExposureValue { get; set; } // EV value
    public double Brightness { get; set; } // Current scene brightness
    public string ErrorMessage { get; set; }
    public double SuggestedShutterSpeed { get; set; }
    public double SuggestedIso { get; set; }
    public double SuggestedAperture { get; set; }
}

public enum MeteringMode
{
    Spot,
    CenterWeighted
}
