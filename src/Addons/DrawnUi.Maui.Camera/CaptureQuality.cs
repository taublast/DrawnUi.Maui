namespace DrawnUi.Maui.Camera;

public enum CaptureQuality
{
	/// <summary>
	/// Max possible photo size
	/// </summary>
	Max,

	/// <summary>
	/// Medium photo size
	/// </summary>
	Medium,

	/// <summary>
	/// Small photo size
	/// </summary>
	Low,

	/// <summary>
	/// Will be sending image from preview as still capture without taking a real still picture, make sure you have preview enabled
	/// </summary>
	Preview
}