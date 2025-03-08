namespace DrawnUi.Maui.Models;

public partial class Screen : BindableObject
{
    public static IDeviceDisplay DisplayInfo => Microsoft.Maui.Devices.DeviceDisplay.Current;//.Essentials.DeviceDisplay.MainDisplayInfo;

    public static bool KeepScreenOn => DeviceDisplay.Current.KeepScreenOn;





}
