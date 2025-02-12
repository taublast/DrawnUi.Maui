using Microsoft.Maui.Controls.Internals;
using System;
using System.Runtime.CompilerServices;

namespace AppoMobi.Xam
{

    [Preserve(AllMembers = true)]
    public class FontIconLabel : Label
    {

        // Font
        const string nameFont = "Font";
        public static readonly BindableProperty FontProperty = BindableProperty.Create(nameFont, typeof(string),
            typeof(FontIconLabel), "FaSolid");
        public new string Font
        {
            get { return (string)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        public EventHandler RendererNeedUpdate { get; set; }


        // Tag
        const string nameTag = "Tag";
        public static readonly BindableProperty TagProperty = BindableProperty.Create(nameTag, typeof(string), typeof(FontIconLabel), string.Empty); //, BindingMode.TwoWay
        public string Tag
        {
            get { return (string)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }


        public FontIconLabel()
        {
            //if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            //{
            //    TranslationY = 1;
            //}
        }



        // IconName
        const string nameIconName = "IconName";
        public static readonly BindableProperty IconNameProperty = BindableProperty.Create(nameIconName, typeof(string), typeof(FontIconLabel), string.Empty); //, BindingMode.TwoWay
        public string IconName
        {
            get { return (string)GetValue(IconNameProperty); }
            set { SetValue(IconNameProperty, value); }
        }



        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)

        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {

            case nameof(Text):
            case nameFont:
            UpdateSkin();
            break;

            case nameIconName:
            var maybePreset = FontIcons.GetPresetByName(IconName);
            Preset = maybePreset;
            UpdateSkin();
            break;

            //property changed
            case namePreset:
            if (Preset != null)
            {
                SetValue(IconNameProperty, Preset.GetName());
                SetIcon(Preset);
                //if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                //    TranslationY = Preset.TranslationY;
                //else
                //    TranslationY = -Preset.TranslationY;
                //Scale = Preset.scale;
                //Text = Preset.icon;
                //DefaultFontOverride = Preset.FontOverride;
            }
            else
            {
                IconName = "";
                //    if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                TranslationY = 0.0;
                UpdateSkin();
            }
            break;


            }
        }


        public void SetIcon(FontIconsPreset value, double scale = -1, string fontOverride = null)

        {
            //   label.Preset = value;
            if (value == null)
            {
                Text = null;
            }
            else
            {
                if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                    TranslationY = value.TranslationY;
                else
                    TranslationY = -value.TranslationY;

                if (scale < 0)
                    Scale = value.scale;
                else
                    Scale = scale;

                Text = value.icon;

                if (!string.IsNullOrEmpty(fontOverride))
                    Font = value.FontOverride;
            }

            UpdateSkin();
            //label.TranslationY = value.TranslationY;
            //label.VerticalOptions = LayoutOptions.Center;
        }


        // Preset
        const string namePreset = "Preset";
        public static readonly BindableProperty PresetProperty = BindableProperty.Create(namePreset, typeof(FontIconsPreset), typeof(FontIconLabel), null); //, BindingMode.TwoWay
        public FontIconsPreset Preset
        {
            get { return (FontIconsPreset)GetValue(PresetProperty); }
            set { SetValue(PresetProperty, value); }
        }

        public void UpdateSkin()
        {
            this.FontFamily = Font;
            RendererNeedUpdate?.Invoke(this, null);
        }
    }
}
