using System.Globalization;
using AppoMobi.Specials;
using TestCalendar.Drawn;

namespace AppoMobi.Models;

public class AppoMonth : BindableObject
{
	public float Id { get; set; }
	public int Month { get; set; }
	public int Year { get; set; }
	public DynamicOptions Settings { get; set; } = new();


	public AppoMonth()
	{
		Month = DateTime.Now.Month;
		Year = DateTime.Now.Year;
		Id = EncodeMonth(Year, Month);
	}


	public AppoMonth(int year, int month, DynamicOptions settings)
	{
		Month = month;
		Year = year;
		Id = EncodeMonth(Year, Month);
		Settings = settings;
	}


	public (int Year, int Month, float Id) GetNextMonthInfo()
	{
		var month = Month;
		var year = Year;

		month++;
		if (month > 12)
		{
			month = 1;
			year++;
		}

		return (year, month, EncodeMonth(year, month));
	}

	public (int Year, int Month, float Id) GetPreviousMonthInfo()
	{
		var month = Month;
		var year = Year;

		month--;
		if (month < 1)
		{
			month = 12;
			year--;
		}

		return (year, month, EncodeMonth(year, month));
	}

	public float GetNextMonthId()
	{
		var month = Month;
		var year = Year;

		month++;
		if (month > 12)
		{
			month = 1;
			year++;
		}

		return EncodeMonth(year, month);
	}

	public float GetPreviousMonthId()
	{
		var month = Month;
		var year = Year;

		month--;
		if (month < 1)
		{
			month = 12;
			year--;
		}

		return EncodeMonth(year, month);
	}

	public static float EncodeMonth(int year, int month)
	{
		return year + month / 100.0f;
	}

	public static (int Year, int Month) DecodeMonth(float value)
	{
		var year = (int)value;
		var month = (int)((value - year) * 100);
		return (year, month);
	}



	public string Name
	{
		get
		{
			try
			{
				var time = new DateTime(Year, Month, 1);
				var desc = time.ToString(Settings.MonthFormat, Settings.Culture).ToTitleCase();
				return desc;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return string.Empty;
			}
		}
		set {}
	}

	List<AppoDay> _Days = new ();
	public List<AppoDay> Days
	{
		get { return _Days; }
		set
		{
			if (_Days != value)
			{
				_Days = value;
				OnPropertyChanged();
			}
		}
	}
}