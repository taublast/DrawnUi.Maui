using System.Globalization;

namespace TestCalendar.Drawn;

public class BlueCalendar : DrawnMultimonthsView
{
	protected override DynamicOptions CreateSettings()
	{
		return new DynamicOptions()
		{
			Culture = CultureInfo.CreateSpecificCulture("ja"),

			DayFont = "FontText",
			MonthNameFont = "FontText",
			WeekDayFont = "FontText",

			StyleRangeSelection = RangeSelectionEdgesStyle.Round,

			ColorBackground = Color.Parse("#FFFFFF"),
			DayTodayBackground = Color.Parse("#F1EFFF"),
			DayTodayStroke = Color.Parse("#E9E6FF"),

			ColorMonthName = Color.Parse("#1E1584"),
			ColorWeekDay = Color.Parse("#1E1584"),
			DayText = Color.Parse("#1E1584"),

			DayRangeSelectionBackground = Color.Parse("#E9E6FF"),
			DaySelectionBackground = Color.Parse("#6759FF"),
			DaySelectionStroke = Color.Parse("#6759FF"),
			DayDisabledText = Color.Parse("#524E7D"),
			DayTextSize = 12, 
			WeekDayTextSize = 12,
			MonthTextSize = 14,
		};
	}
}