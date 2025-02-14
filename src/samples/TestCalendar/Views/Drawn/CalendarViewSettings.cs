namespace TestCalendar.Drawn;

public class CalendarViewSettings
{

	public static Color ColorTextActive = Colors.White;
	public static Color ColorText = Colors.DimGrey;
	public static Color ColorGrid = Colors.Gainsboro;
	public static Color ColorBackground = Colors.White;
	//public static Color ColorLines = Colors.DarkGrey;
	public static Color ColorCellBackground = Colors.White;


	// DAYS
	public static float DayHeight = 44f;
	public static float DayTextSize = 15f;
	public static Color DayText = Color.Parse("#444444");
	public static FontAttributes DayTextAttributes = FontAttributes.Bold;
	public static Color DayDisabledText = Colors.DarkGray;
	public static FontAttributes DayDisabledTextAttributes = FontAttributes.None;
	public static Color DayTodayStroke = Colors.Crimson;
	public static Color DayTodayBackground = Colors.Transparent;
	public static Color DaySelectionBackground = Colors.Red;
	public static Color DaySelectionStroke = Colors.Crimson;
	public static Color DayRangeSelectionBackground = Color.Parse("#22FF0000");
	public static Color DaySelectionText = Colors.White;
	public static float SelectionRadius = 16f;
}