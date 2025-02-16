using System.Globalization;
using AppoMobi.Models;

namespace TestCalendar.Drawn;

public class DynamicOptions
{
	public CultureInfo Culture = CultureInfo.CurrentCulture;

	public string MonthFormat = "MMMM yyyy";

	public  Color ColorGrid = Colors.Gainsboro;

	public  Color ColorBackground = Colors.White;
	public Color ColorDaysBackground = Colors.Transparent;

	public Color ColorMonthName = Colors.DimGrey;
	public float MonthTextSize = 15;

	public Color ColorWeekDay = Colors.DimGrey;
	public float WeekDayTextSize = 13;

	public string MonthNameFont = string.Empty;
	public string WeekDayFont = string.Empty;
	public string DayFont = string.Empty;

	// DAYS
	public float DayHeight = 44f;
	public float SelectionRadius = 16f;

	public float DayTextSize = 15f;
	public FontAttributes DayTextAttributes = FontAttributes.Bold;
	public FontAttributes DayDisabledTextAttributes = FontAttributes.None;

	public Color DayText = Color.Parse("#555555");
	public Color DayDisabledText = Color.Parse("#999999");
	
	public  Color DayTodayStroke = Colors.LightGray;
	public  Color DayTodayBackground = Colors.LightGray;

	public  Color DaySelectionBackground = Colors.Red;
	public  Color DaySelectionStroke = Colors.Crimson;
	public  Color DayRangeSelectionBackground = Color.Parse("#22FF0000");
	public  Color DaySelectionText = Colors.White;

	public RangeSelectionEdgesStyle StyleRangeSelection = RangeSelectionEdgesStyle.Round;

	public Func<AppoDay, bool> CheckDayIsDisabled = (item) =>
	{
		return item.Date.Date < DateTime.Now.Date;
	};
}