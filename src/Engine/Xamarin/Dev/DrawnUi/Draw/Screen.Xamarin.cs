using Xamarin.Essentials;

namespace DrawnUi.Models;

public partial class Screen : BindableObject
{
    public static DisplayInfo DisplayInfo => DeviceDisplay.MainDisplayInfo;

    public static bool KeepScreenOn => DeviceDisplay.KeepScreenOn;

}
