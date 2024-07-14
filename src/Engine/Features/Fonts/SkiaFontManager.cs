using System.Reflection;

namespace DrawnUi.Maui.Draw;

public partial class SkiaFontManager
{
    public static bool ThrowIfFailedToCreateFont = true;

    static SKFontManager _SKFontManager;
    public static SKFontManager Manager
    {
        get
        {
            if (_SKFontManager == null)
            {
                _SKFontManager = SKFontManager.CreateDefault();
            }
            return _SKFontManager;
        }
    }

    public bool CanRender(SKTypeface typeface, int character)
    {
        return typeface.GetGlyphs(new string((char)character, 1))[0] != 0;
    }

    public static List<int> StringToUnicodeValues(string text)
    {
        List<int> codePoints = new List<int>();
        for (int i = 0; i < text.Length; i++)
        {
            int codePoint = char.ConvertToUtf32(text, i);
            codePoints.Add(codePoint);

            // If it's a surrogate pair, skip the next char
            if (char.IsSurrogatePair(text, i))
            {
                i++;
            }
        }
        return codePoints;
    }

#if (!ANDROID && !IOS && !MACCATALYST && !WINDOWS && !TIZEN)

    public async Task<SKTypeface> GetFont(string alias)
    {
        throw new NotImplementedException();
    }

#endif

    // Change the type of RegisteredWeights
    static Dictionary<string, List<int>> RegisteredWeights = new(16);

    public static void RegisterWeight(string alias, FontWeight weight)
    {
        // Get the list for the alias (or a new list if it doesn't exist yet)
        List<int> list;
        if (!RegisteredWeights.TryGetValue(alias, out list))
        {
            list = new();
            RegisteredWeights[alias] = list;
        }
        list.Add((int)weight);
    }

    public static string GetRegisteredAlias(string alias, int weight)
    {
        // Check if any weights have been registered for this alias
        if (RegisteredWeights.TryGetValue(alias, out var registeredWeights))
        {
            // Find the closest registered weight
            var closestRegisteredWeight = registeredWeights.OrderBy(w => Math.Abs(w - weight)).First();

            // Get the enum equivalent of the closest registered weight
            var closestWeight = GetWeightEnum(closestRegisteredWeight);

            return GetAlias(alias, closestWeight);
        }

        // If no weights have been registered under this alias, return the alias itself
        return alias;
    }

    public async Task<SKTypeface> GetFont(string fontFamily, int fontWeight)
    {
        if (string.IsNullOrEmpty(fontFamily))
        {
            return SKTypeface.Default;
        }
        var alias = GetRegisteredAlias(fontFamily, fontWeight);
        var font = await GetFont(alias);

        //safety check to avoid any chance of crash split_config.arm64_v8a.apk!libSkiaSharp.so (sk_font_set_typeface+60)
        if (font == null)
        {
            return SKTypeface.Default;
        }
        return font;
    }

    /// <summary>
    /// Gets the closest enum value to the given weight. Like 590 would return Semibold.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static FontWeight GetWeightEnum(int weight)
    {
        FontWeight[] fontWeights = (FontWeight[])Enum.GetValues(typeof(FontWeight));
        var closest = fontWeights.Select(f => new { Value = f, Difference = Math.Abs((int)f - weight) })
            .OrderBy(item => item.Difference)
            .First().Value;

        return closest;
    }


    public static string GetAlias(string alias, FontWeight weight)
    {
        if (!string.IsNullOrEmpty(alias))
            return $"{alias}{weight}";

        return alias;
    }

    public static string GetAlias(string alias, int weight)
    {
        var e = GetWeightEnum(weight);
        return GetAlias(alias, e);
    }


    /// <summary>
    /// Takes the full name of a resource and loads it in to a stream.
    /// </summary>
    /// <param name="resourceName">Assuming an embedded resource is a file
    /// called info.png and is located in a folder called Resources, it
    /// will be compiled in to the assembly with this fully qualified
    /// name: Full.Assembly.Name.Resources.info.png. That is the string
    /// that you should pass to this method.</param>
    /// <returns></returns>
    public static Stream GetEmbeddedResourceStream(string resourceName)
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
    }

    public static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
    {
        var resourceNames = assembly.GetManifestResourceNames();

        var resourcePaths = resourceNames
            .Where(x => x.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();

        if (!resourcePaths.Any())
        {
            throw new Exception(string.Format("Resource ending with {0} not found.", resourceFileName));
        }
        if (resourcePaths.Length > 1)
        {
            resourcePaths = resourcePaths.Where(x => IsFile(x, resourceFileName)).ToArray();
        }

        return assembly.GetManifestResourceStream(resourcePaths.FirstOrDefault());
    }

    static bool IsFile(string path, string file)
    {
        if (!path.EndsWith(file, StringComparison.Ordinal))
            return false;
        return path.Replace(file, "").EndsWith(".", StringComparison.Ordinal);
    }

    /// <summary>
    /// Get the list of all emdedded resources in the assembly.
    /// </summary>
    /// <returns>An array of fully qualified resource names</returns>
    public static string[] GetEmbeddedResourceNames()
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceNames();
    }

    private static SkiaFontManager _instance;
    public static SkiaFontManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = new SkiaFontManager();
            }
            return _instance;
        }
    }


    public Dictionary<string, SKTypeface> Fonts { get; set; } = new(128);

    private static IFontRegistrar _registrar;
    public static IFontRegistrar FontRegistrar
    {
        get
        {
            if (_registrar == null)
            {
                _registrar = Super.Services.GetService<IFontRegistrar>();
            }
            return _registrar;
        }
    }

    public async Task<SKTypeface> GetRegisteredFont2(string alias)
    {
        if (Fonts.TryGetValue(alias, out var existing))
        {
            return existing;
        }

        //if (string.IsNullOrEmpty(FontFamily))
        //{
        //    font = SKTypeface.CreateDefault();
        //}
        //else
        //{

        //    font = SKTypeface.FromFamilyName(FontFamily);
        //    if (font == null || font.FamilyName != alias)
        //    {
        //        var registrar = FontRegistrar;
        //        var realName = registrar.GetFont(FontFamily);
        //        using (Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(realName))
        //        {
        //            font = SKTypeface.FromStream(fileStream);

        //            if (font == null)
        //                font = SKTypeface.CreateDefault();
        //        }
        //    }

        //}


        //LEGACY
        if (!string.IsNullOrEmpty(alias))
        {
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
                    if (fonts.TryGetValue(alias, out (string Filename, string? Alias) value))
                    {
                        //font = GetEmbeededFont(value.Item1.FontFileName, value.Item2, alias);

                        using (Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(value.Filename))
                        {
                            font = SKTypeface.FromStream(fileStream);

                            if (font == null)
                                font = SKTypeface.CreateDefault();
                        }


                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                //var family = CleanseFontName(alias);
                //font = SKTypeface.FromFamilyName(info.fontPath);
                //if (font.FamilyName != family)
                //{
                //    Trace.WriteLine($"[SKIA] Couldn't create font {family}");
                //    font = null;
                //}
            }

            if (font == null)
                Trace.WriteLine($"[SKIA] Couldn't create font {alias}");
            else
            {
                if (!string.IsNullOrEmpty(alias))
                    Fonts[alias] = font;
            }

            return font;
        }

        return SKTypeface.Default;
    }

    public SKTypeface GetEmbeededFont(string filename, Assembly assembly, string alias = null)
    {
        if (string.IsNullOrEmpty(alias))
            alias = filename;

        SKTypeface font = null;
        try
        {
            font = Fonts[alias];
        }
        catch (Exception e)
        {
        }

        if (font == null)
        {

            using (var stream = GetEmbeddedResourceStream(assembly, filename))
            {

                if (stream == null)
                    return SKTypeface.Default;

                font = SKTypeface.FromStream(stream);
                if (font != null)
                {
                    if (!string.IsNullOrEmpty(alias))
                        Fonts[alias] = font;
                }
            }

        }

        if (font == null)
            font = SKTypeface.Default;

        return font;

    }



}