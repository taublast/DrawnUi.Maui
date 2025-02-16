using AppoMobi.Models;
using AppoMobi.Xam;

namespace TestCalendar.Drawn;

public class DaysGrid : DrawnDataGrid
{
	public DaysGrid()
	{
		OffsetDaysUponContext();
	}

	void OffsetDaysUponContext()
	{
		if (BindingContext is AppoMonth month)
		{
			var startingdow = (int)month.Days.First().Date.DayOfWeek;
			OffsetDays(startingdow);
		}
	}

	protected override void OnBindingContextChanged()
	{
		OffsetDaysUponContext();

		base.OnBindingContextChanged();
	}

	void OffsetDays(int startingdow)
	{
		if (startingdow == 0)
			this.StartColumn = 6;
		else
		if (startingdow > 1)
			this.StartColumn = startingdow - 1;
	}
}