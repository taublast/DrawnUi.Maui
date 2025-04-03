using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;

namespace DrawnUi.Draw;

public class SkiaFontManager
{
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

	static SkiaFontManager()
	{
		DefaultTypeface = SKTypeface.CreateDefault();
	}

	public bool Initialized { get; set; }

	public static SKTypeface DefaultTypeface { get; }

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


	public Dictionary<string, SKTypeface> Fonts { get; set; } = new Dictionary<string, SKTypeface>();

	public SKTypeface GetRegisteredFont(string alias)
	{
		if (Fonts.ContainsKey(alias))
			return Fonts[alias];

		var info = FontRegistrar.HasFont(alias);
		if (info.hasFont)
		{
			var font = SKTypeface.FromFile(info.fontPath);
			if (font == null)
			{
				try
				{
					Type type = typeof(FontRegistrar);
					FieldInfo fi = type.GetField("EmbeddedFonts", BindingFlags.NonPublic | BindingFlags.Static);
					var fonts = (Dictionary<string, (ExportFontAttribute attribute, Assembly assembly)>)fi.GetValue(null);
					if (fonts.TryGetValue(alias, out (ExportFontAttribute, Assembly) value))
					{
						font = GetEmbeededFont(value.Item1.FontFileName, value.Item2, alias);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}

				//var family = CleanseFontName(alias);
				//font = SKTypeface.FromFamilyName(info.fontPath);
				//if (font.FamilyName != family)
				//{
				//    Console.WriteLine($"[SKIA] Couldn't create font {family}");
				//    font = null;
				//}
			}
			else
			{
				Fonts[alias] = font;
			}

			if (font == null)
				Console.WriteLine($"[SKIA] Couldn't create font {alias}");
			else
				return font;

		}

		return SkiaFontManager.DefaultTypeface;
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
					return null;

				font = SKTypeface.FromStream(stream);
				if (font != null)
				{
					Fonts[alias] = font;
				}
			}

		}

		return font;

	}

	static string CleanseFontName(string fontName)
	{

		//First check Alias
		var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontName);
		if (hasFontAlias)
			return fontPostScriptName;

		var fontFile = FontFile.FromString(fontName);

		if (!string.IsNullOrWhiteSpace(fontFile.Extension))
		{
			var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
			if (hasFont)
				return filePath ?? fontFile.PostScriptName;
		}
		else
		{
			foreach (var ext in FontFile.Extensions)
			{

				var formated = fontFile.FileNameWithExtension(ext);
				var (hasFont, filePath) = FontRegistrar.HasFont(formated);
				if (hasFont)
					return filePath;
			}
		}
		return fontFile.PostScriptName;
	}


}