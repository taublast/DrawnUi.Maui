using AppoMobi.Specials;

namespace AppoMobi.Models;

public class AppoMonth : BindableObject
{
	public AppoMonth()
	{
		Month = DateTime.Now.Month;
		Year = DateTime.Now.Year;
		Id = EncodeMonth(Year, Month);
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

	public AppoMonth(int year, int month)
	{
		Month = month;
		Year = year;
		Id = EncodeMonth(Year, Month);
	}

	public float Id { get; set; }
	public int Month { get; set; }
	public int Year { get; set; }

	public string Name
	{
		get
		{
			var time = new DateTime(Year, Month, 1);
			var desc = time.ToString("MMMM yyyy").ToTitleCase();
			return desc;
		}
		set
		{

		}
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