

namespace DrawnUi.Maui.Draw;

public static partial class ColorExtensions
{

    public static Color ColorFromHex(this string hex)
    {
        if (!hex.HasContent())
            return Colors.Transparent;

        if (hex.Left() != "#")
        {
            hex = "#" + hex;
        }

        return Color.Parse(hex);
    }

    public static string SetHexAlpha(this string hex, int percent)
    {
        //todo check if alpha exists!!!!!!

        // strip the leading # if it's there
        hex = hex.Replace("#", "");

        ushort a = 255;
        var aa = (a * percent / 100).ToString("X2");

        return '#' + aa + hex;
    }


    public static Color MakeDarker(this Color color, double percent)

    {
        var rc = color.Red - color.Red / 100 * percent;
        if (rc > 1) rc = 1;
        var gc = color.Green - color.Green / 100 * percent;
        if (gc > 1) gc = 1;
        var bc = color.Blue - color.Blue / 100 * percent;
        if (bc > 1) bc = 1;
        var ret = new Color((float)rc, (float)gc, (float)bc, color.Alpha);
        return ret;
    }


    public static Color MakeLighter(this Color color, double percent)

    {
        return color.AddLuminosity((float)(color.GetLuminosity() / 100 * percent));

        var rc = color.Red + color.Red / 100 * percent;
        if (rc > 1) rc = 1;
        var gc = color.Green + color.Green / 100 * percent;
        if (gc > 1) gc = 1;
        var bc = color.Blue + color.Blue / 100 * percent;
        if (bc > 1) bc = 1;
        var ret = new Color((float)rc, (float)gc, (float)bc, color.Alpha);
        return ret;
    }

    /*
                
                public static Color MakeLighter(this Xamarin.Forms.Color color, double percent)
                
                {
                    // strip the leading # if it's there
                    var hex = color.ToHex();
                    hex = hex.Replace("#", "");

                    var r = Convert.ToUInt16(hex.Substring(0, 2), 16);
                    var g = Convert.ToUInt16(hex.Substring(2, 2), 16);
                    var b = Convert.ToUInt16(hex.Substring(4, 2), 16);

                    var rr = ((0 | (1 << 8) + r + (int)((256 - r) * percent / 100)).ToString("X")).Substring(1);
                    var gg = ((0 | (1 << 8) + g + (int)((256 - g) * percent / 100)).ToString("X")).Substring(1);
                    var bb = ((0 | (1 << 8) + b + (int)((256 - b) * percent / 100)).ToString("X")).Substring(1);

                    var new_hex = '#' + rr + gg + bb;

                    return ToColorFromHex(new_hex); //889fao

                }
        */

    public static string ToHex(this Color color)

    {
        var r = (ushort)(color.Red * 255);
        var g = (ushort)(color.Green * 255);
        var b = (ushort)(color.Blue * 255);
        var a = (ushort)(color.Alpha * 255);

        var rr = r.ToString("X2");
        var gg = g.ToString("X2");
        var bb = b.ToString("X2");
        var aa = a.ToString("X2");

        if (a < 255) return '#' + aa + rr + gg + bb;
        return '#' + rr + gg + bb;
    }
    //
    /**
         * ('#000000', 50) --> #808080
         * ('#EEEEEE', 25) --> #F2F2F2
         * ('EEE     , 25) --> #F2F2F2
         **/

    public static Color ToColorFromHex(this string color)
    {
        return Color.Parse(color);
    }


    public static string GetHexString(this Color color)

    {
        var red = (int)(color.Red * 255);
        var green = (int)(color.Green * 255);
        var blue = (int)(color.Blue * 255);
        var alpha = (int)(color.Alpha * 255);
        var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

        return hex;
    }

    public static string GetHexDesc(this Color color)

    {
        var red = (int)(color.Red * 255);
        var green = (int)(color.Green * 255);
        var blue = (int)(color.Blue * 255);
        var alpha = (int)(color.Alpha * 255);
        var hex = $"#{red:X2}{green:X2}{blue:X2}";

        return hex;
    }

    public static readonly Dictionary<string, Color> ColorLookup = new Dictionary<string, Color>
    {
        {"aliceblue", Color.FromUint(0xFFF0F8FF)},
        {"antiquewhite", Color.FromUint(0xFFFAEBD7)},
        {"aqua", Color.FromUint(0xFF00FFFF)},
        {"aquamarine", Color.FromUint(0xFF7FFFD4)},
        {"azure", Color.FromUint(0xFFF0FFFF)},
        {"beige", Color.FromUint(0xFFF5F5DC)},
        {"bisque", Color.FromUint(0xFFFFE4C4)},
        {"black", Color.FromUint(0xFF000000)},
        {"blanchedalmond", Color.FromUint(0xFFFFEBCD)},
        {"blue", Color.FromUint(0xFF0000FF)},
        {"blueviolet", Color.FromUint(0xFF8A2BE2)},
        {"brown", Color.FromUint(0xFFA52A2A)},
        {"burlywood", Color.FromUint(0xFFDEB887)},
        {"cadetblue", Color.FromUint(0xFF5F9EA0)},
        {"chartreuse", Color.FromUint(0xFF7FFF00)},
        {"chocolate", Color.FromUint(0xFFD2691E)},
        {"copper", Color.FromUint(0xFFB87333)},
        {"coral", Color.FromUint(0xFFFF7F50)},
        {"cornflowerblue", Color.FromUint(0xFF6495ED)},
        {"cornsilk", Color.FromUint(0xFFFFF8DC)},
        {"crimson", Color.FromUint(0xFFDC143C)},
        {"cyan", Color.FromUint(0xFF00FFFF)},
        {"darkblue", Color.FromUint(0xFF00008B)},
        {"darkcyan", Color.FromUint(0xFF008B8B)},
        {"darkgoldenrod", Color.FromUint(0xFFB8860B)},
        {"darkgray", Color.FromUint(0xFFA9A9A9)},
        {"darkgreen", Color.FromUint(0xFF006400)},
        {"darkkhaki", Color.FromUint(0xFFBDB76B)},
        {"darkmagenta", Color.FromUint(0xFF8B008B)},
        {"darkolivegreen", Color.FromUint(0xFF556B2F)},
        {"darkorange", Color.FromUint(0xFFFF8C00)},
        {"darkorchid", Color.FromUint(0xFF9932CC)},
        {"darkred", Color.FromUint(0xFF8B0000)},
        {"darksalmon", Color.FromUint(0xFFE9967A)},
        {"darkseagreen", Color.FromUint(0xFF8FBC8F)},
        {"darkslateblue", Color.FromUint(0xFF483D8B)},
        {"darkslategray", Color.FromUint(0xFF2F4F4F)},
        {"darkturquoise", Color.FromUint(0xFF00CED1)},
        {"darkviolet", Color.FromUint(0xFF9400D3)},
        {"deeppink", Color.FromUint(0xFFFF1493)},
        {"deepskyblue", Color.FromUint(0xFF00BFFF)},
        {"dimgray", Color.FromUint(0xFF696969)},
        {"dodgerblue", Color.FromUint(0xFF1E90FF)},
        {"firebrick", Color.FromUint(0xFFB22222)},
        {"floralwhite", Color.FromUint(0xFFFFFAF0)},
        {"forestgreen", Color.FromUint(0xFF228B22)},
        {"fuchsia", Color.FromUint(0xFFFF00FF)},
        {"gainsboro", Color.FromUint(0xFFDCDCDC)},
        {"ghostwhite", Color.FromUint(0xFFF8F8FF)},
        {"gold", Color.FromUint(0xFFFFD700)},
        {"goldenrod", Color.FromUint(0xFFDAA520)},
        {"gray", Color.FromUint(0xFF808080)},
        {"green", Color.FromUint(0xFF008000)},
        {"greenyellow", Color.FromUint(0xFFADFF2F)},
        {"honeydew", Color.FromUint(0xFFF0FFF0)},
        {"hotpink", Color.FromUint(0xFFFF69B4)},
        {"indianred", Color.FromUint(0xFFCD5C5C)},
        {"indigo", Color.FromUint(0xFF4B0082)},
        {"ivory", Color.FromUint(0xFFFFFFF0)},
        {"khaki", Color.FromUint(0xFFF0E68C)},
        {"lavender", Color.FromUint(0xFFE6E6FA)},
        {"lavenderblush", Color.FromUint(0xFFFFF0F5)},
        {"lawngreen", Color.FromUint(0xFF7CFC00)},
        {"lemonchiffon", Color.FromUint(0xFFFFFACD)},
        {"lightblue", Color.FromUint(0xFFADD8E6)},
        {"lightcoral", Color.FromUint(0xFFF08080)},
        {"lightcyan", Color.FromUint(0xFFE0FFFF)},
        {"lightgoldenrodyellow", Color.FromUint(0xFFFAFAD2)},
        {"lightgray", Color.FromUint(0xFFD3D3D3)},
        {"lightgreen", Color.FromUint(0xFF90EE90)},
        {"lightpink", Color.FromUint(0xFFFFB6C1)},
        {"lightsalmon", Color.FromUint(0xFFFFA07A)},
        {"lightseagreen", Color.FromUint(0xFF20B2AA)},
        {"lightskyblue", Color.FromUint(0xFF87CEFA)},
        {"lightslategray", Color.FromUint(0xFF778899)},
        {"lightsteelblue", Color.FromUint(0xFFB0C4DE)},
        {"lightyellow", Color.FromUint(0xFFFFFFE0)},
        {"lime", Color.FromUint(0xFF00FF00)},
        {"limegreen", Color.FromUint(0xFF32CD32)},
        {"linen", Color.FromUint(0xFFFAF0E6)},
        {"magenta", Color.FromUint(0xFFFF00FF)},
        {"maroon", Color.FromUint(0xFF800000)},
        {"mediumaquamarine", Color.FromUint(0xFF66CDAA)},
        {"mediumblue", Color.FromUint(0xFF0000CD)},
        {"mediumorchid", Color.FromUint(0xFFBA55D3)},
        {"mediumpurple", Color.FromUint(0xFF9370DB)},
        {"mediumseagreen", Color.FromUint(0xFF3CB371)},
        {"mediumslateblue", Color.FromUint(0xFF7B68EE)},
        {"mediumspringgreen", Color.FromUint(0xFF00FA9A)},
        {"mediumturquoise", Color.FromUint(0xFF48D1CC)},
        {"mediumvioletred", Color.FromUint(0xFFC71585)},
        {"metallic", Color.FromUint(0xFFC0C0C0)},
        {"midnightblue", Color.FromUint(0xFF191970)},
        {"mintcream", Color.FromUint(0xFFF5FFFA)},
        {"mistyrose", Color.FromUint(0xFFFFE4E1)},
        {"moccasin", Color.FromUint(0xFFFFE4B5)},
        {"navajowhite", Color.FromUint(0xFFFFDEAD)},
        {"navy", Color.FromUint(0xFF000080)},
        {"oldlace", Color.FromUint(0xFFFDF5E6)},
        {"olive", Color.FromUint(0xFF808000)},
        {"olivedrab", Color.FromUint(0xFF6B8E23)},
        {"orange", Color.FromUint(0xFFFFA500)},
        {"orangered", Color.FromUint(0xFFFF4500)},
        {"orchid", Color.FromUint(0xFFDA70D6)},
        {"palegoldenrod", Color.FromUint(0xFFEEE8AA)},
        {"palegreen", Color.FromUint(0xFF98FB98)},
        {"paleturquoise", Color.FromUint(0xFFAFEEEE)},
        {"palevioletred", Color.FromUint(0xFFDB7093)},
        {"papayawhip", Color.FromUint(0xFFFFEFD5)},
        {"peachpuff", Color.FromUint(0xFFFFDAB9)},
        {"peru", Color.FromUint(0xFFCD853F)},
        {"pink", Color.FromUint(0xFFFFC0CB)},
        {"plum", Color.FromUint(0xFFDDA0DD)},
        {"powderblue", Color.FromUint(0xFFB0E0E6)},
        {"purple", Color.FromUint(0xFF800080)},
        {"rebeccapurple", Color.FromUint(0xFF663399)},
        {"red", Color.FromUint(0xFFFF0000)},
        {"rosybrown", Color.FromUint(0xFFBC8F8F)},
        {"royalblue", Color.FromUint(0xFF4169E1)},
        {"saddlebrown", Color.FromUint(0xFF8B4513)},
        {"salmon", Color.FromUint(0xFFFA8072)},
        {"sandybrown", Color.FromUint(0xFFF4A460)},
        {"seagreen", Color.FromUint(0xFF2E8B57)},
        {"seashell", Color.FromUint(0xFFFFF5EE)},
        {"sienna", Color.FromUint(0xFFA0522D)},
        {"silver", Color.FromUint(0xFFC0C0C0)},
        {"skyblue", Color.FromUint(0xFF87CEEB)},
        {"slateblue", Color.FromUint(0xFF6A5ACD)},
        {"slategray", Color.FromUint(0xFF708090)},
        {"snow", Color.FromUint(0xFFFFFAFA)},
        {"springgreen", Color.FromUint(0xFF00FF7F)},
        {"steelblue", Color.FromUint(0xFF4682B4)},
        {"tan", Color.FromUint(0xFFD2B48C)},
        {"teal", Color.FromUint(0xFF008080)},
        {"thistle", Color.FromUint(0xFFD8BFD8)},
        {"tomato", Color.FromUint(0xFFFF6347)},
        {"turquoise", Color.FromUint(0xFF40E0D0)},
        {"violet", Color.FromUint(0xFFEE82EE)},
        {"wheat", Color.FromUint(0xFFF5DEB3)},
        {"white", Color.FromUint(0xFFFFFFFF)},
        {"whitesmoke", Color.FromUint(0xFFF5F5F5)},
        {"yellow", Color.FromUint(0xFFFFFF00)},
        {"yellowgreen", Color.FromUint(0xFF9ACD32)},
    };

}
