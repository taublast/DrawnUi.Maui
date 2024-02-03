

namespace DrawnUi.Maui.Infrastructure.Extensions;

public static partial class ColorExtensions
{

	public static Color ColorFromHex(this string hex)

	{
		if (!hex.HasContent())
			return Colors.Transparent;

		return Color.FromHex(hex);
	}



	public static string SetAlpha(this string hex, int percent)

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
		return Color.FromHex(color);
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

}
