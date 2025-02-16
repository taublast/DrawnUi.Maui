using AppoMobi.Models;

namespace TestCalendar.Views;

public class DaysGenerator()
{
	#region DAYS CONTAINER

	(List<AppoDay> Days, List<MonthInsideDaysIndex> Indexes) CreateDays(int yearStart, int monthStart, int yearEnd, int monthEnd)
	{
		var year = yearStart;
		var month = monthStart;

		List<AppoDay> ret = new();
		List<MonthInsideDaysIndex> indexes = new();

		while (year <= yearEnd || month <= monthEnd)
		{
			var days = GenerateMonth(year, month);
			indexes.Add(new()
			{
				Id = AppoMonth.EncodeMonth(year, month),
				Month = month,
				Year = year,
				DayIndexStart = ret.Count,
				DayIndexEnd = ret.Count + days.Count - 1
			});
			ret.AddRange(days);

			month++;
			if (month > 12)
			{
				month = 1;
				year++;
			}
		}

		return (ret, indexes);
	}

	public DaysContainer DaysContainer { get; protected set; }

	public void SetupDays(int yearStart, int monthStart, int yearEnd, int monthEnd)
	{
		var data = CreateDays(yearStart, monthStart, yearEnd, monthEnd);
		DaysContainer = new DaysContainer(data.Days, data.Indexes);
	}

	public List<AppoDay> GenerateMonth(int year, int month)
	{
		var days = new List<AppoDay>();
		var maxday = DateTime.DaysInMonth(year, month);
		for (int a = 1; a < maxday + 1; a++)
		{
			var day = new AppoDay();
			day.Date = new DateTime(year, month, a);

			//day.Id = $"{day.Month}-{day.Day}"; legacy appo, will not work with multi-months engine
			day.Id = $"{a:00}.{month:00}.{year}";

			//todo disabled logic
			// day.Disabled = true;

			days.Add(day);
		}
		return days;
	}

	#endregion

}