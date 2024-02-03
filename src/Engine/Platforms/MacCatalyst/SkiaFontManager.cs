using System.Diagnostics;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaFontManager
    {

        public async Task<SKTypeface> GetFont(string alias)
        {
            if (Fonts.TryGetValue(alias, out var existing))
            {
                return existing;
            }

            var font = SKTypeface.FromFamilyName(alias);
            if (font == null || font.FamilyName != alias)
            {
                try
                {
                    var instance = FontRegistrar as FontRegistrar;
                    var type = instance.GetType();
                    var fields = type.GetAllHiddenFields();
                    var field = fields.First(x => x.Name == "_nativeFonts");
                    var fonts = (Dictionary<string, (string Filename, string? Alias)>)field.GetValue(instance);

                    var registered = fonts.Values.FirstOrDefault(x =>
                        x.Filename == alias
                        || x.Alias == alias);

                    using (Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(registered.Filename))
                    {
                        font = SKTypeface.FromStream(fileStream);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }


            if (font == null)
            {
                if (ThrowIfFailedToCreateFont)
                {
                    throw new Exception($"[SKIA] Couldn't create font {alias}");
                }
                font = SKTypeface.CreateDefault();
                Trace.WriteLine($"[SKIA] Couldn't create font {alias}");
            }
            else
            {
                if (!string.IsNullOrEmpty(alias))
                    Fonts[alias] = font;
            }

            return font;
        }

    }
}