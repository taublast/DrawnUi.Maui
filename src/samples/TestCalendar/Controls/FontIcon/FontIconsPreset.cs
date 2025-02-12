using System;

namespace AppoMobi.Xam
{
    
    public class FontIconsPreset
    
    {
        public string icon { get; set; }
        public string FontOverride { get; set; }
        public double scale { get; set; }
        public double TranslationY { get; set; }

        public string Id { get; set; }

        public FontIconsPreset(string strIcon, double dScale = 1, string fontOverride = null, double translationY = 0.0)
        {
            Create();
            icon = strIcon;
            scale = dScale;
            FontOverride = fontOverride;
            TranslationY = translationY;
        }

        public string GetName()
        {
            return FontIcons.GetPresetName(Id);
        }

        void Create()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
