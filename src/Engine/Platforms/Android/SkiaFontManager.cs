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
            if (font == null)
            {
                try
                {

                    var registrar = FontRegistrar;
                    var realName = registrar.GetFont(alias);
                    if (!string.IsNullOrEmpty(realName))
                    {
                        using (Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(realName))
                        {
                            font = SKTypeface.FromStream(fileStream);
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
