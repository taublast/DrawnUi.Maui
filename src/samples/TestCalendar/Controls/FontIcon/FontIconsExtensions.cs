

namespace AppoMobi.Xam
{
    
    public static class FontIconsExtensions
    
    {
        
        public static void SetIcon(this FontIconLabel label, string icon, double scale = 1, string fontOverride = null)
        
        {

            label.Scale = scale;
            label.Text = icon;
            label.Font = fontOverride;
            label.UpdateSkin();

        }

        
        public static void SetIcon(this FontIconLabel label, FontIconsPreset value, string fontOverride)
        {
            if (!string.IsNullOrEmpty(fontOverride))
            {
                label.FontFamily = fontOverride;
            }
            else
            {
                label.FontFamily = "Fa";
            }

            if (DeviceInfo.Platform == DevicePlatform.Android)
                label.TranslationY = value.TranslationY;
            else
                label.TranslationY = -value.TranslationY;

            label.Scale = value.scale;

            label.Text = value.icon;

            label.Font = fontOverride;

            label.UpdateSkin();
        }

        /*
        
        public static void SetIcon(this FontIconLabel label, FontIconsPreset value, double scale = -1, string fontOverride = null)
        
        {
            //   label.Preset = value;

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                label.TranslationY = value.TranslationY;
            else
                label.TranslationY = -value.TranslationY;

            if (scale < 0)
                label.Scale = value.scale;
            else
                label.Scale = scale;

            label.Text = value.icon;

            if (fontOverride == null)
                label.Font = value.FontOverride;
            else
            {
                label.Font = fontOverride;
            }

            label.UpdateSkin();
            //label.TranslationY = value.TranslationY;
            //label.VerticalOptions = LayoutOptions.Center;
        }
        */
        
        public static void Update(this FontIconLabel label)
        
        {
            label.UpdateSkin();
        }






    }

}
