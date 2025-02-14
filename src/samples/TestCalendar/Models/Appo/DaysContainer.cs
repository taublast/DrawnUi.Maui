using AppoMobi.Models;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;

namespace TestCalendar.Views;

public class DaysContainer
{

	public DaysContainer(List<AppoDay> days, List<MonthInsideDaysIndex> indexes)
	{
		_indexes = indexes;
		Days = days;
	}

	public List<AppoDay> Days { get; private set; } = new List<AppoDay>();
	
	List<MonthInsideDaysIndex> _indexes = new List<MonthInsideDaysIndex>();
	public IReadOnlyList<MonthInsideDaysIndex> Indexes => _indexes.AsReadOnly();

	public List<AppoDay> GetDaysForMonth(int year, int month)
	{
		var index = _indexes.FirstOrDefault(i => i.Year == year && i.Month == month);

		if (index == null)
		{
			return new List<AppoDay>();  
		}

		return Days.GetRange(index.DayIndexStart, index.DayIndexEnd - index.DayIndexStart + 1);
	}

	public List<AppoDay> GetDaysForMonth(float monthId)
	{
		var index = _indexes.FirstOrDefault(i => i.Id == monthId);

		if (index == null)
		{
			return new List<AppoDay>(); 
		}

		return Days.GetRange(index.DayIndexStart, index.DayIndexEnd - index.DayIndexStart + 1);
	}

	public bool ContainsMonth(int year, int month)
	{
		var id = AppoMonth.EncodeMonth(year, month);
		return _indexes.Any(x => x.Id == id);
	}

	public bool ContainsMonth(float id)
	{
		return _indexes.Any(x => x.Id == id);
	}

	/// <summary>
	/// Select a single day
	/// </summary>
	/// <param name="selected"></param>
	public void SelectDay(ISelectableRangeOption? selected)
	{
		IEnumerable<ISelectableRangeOption> list = Days;

		foreach (var canBeSelected in list)
		{
			canBeSelected.SelectionStart = false;
			canBeSelected.SelectionEnd = false;

			if (selected != null && selected.Id == canBeSelected.Id)
			{
				canBeSelected.Selected = true;
			}
			else
			{
				canBeSelected.Selected = false;
			}
		}
	}

	/// <summary>
	/// Set range selection. As the selection could be inverted that returns the adjusted start and end IDs.
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <returns></returns>
	public (int Start, int End) SelectDays(ISelectableRangeOption? start, ISelectableRangeOption? end)
	{
		IEnumerable<ISelectableRangeOption> list = Days;

		var startIndex = -1;
		var endIndex = -1;
		var iteration = -1;
		var direction = -1;
		foreach (var canBeSelected in list)
		{
			iteration++;
			if (end != null && canBeSelected.Id == end.Id)
			{
				endIndex = iteration;
			}
			if (start != null && start.Id == canBeSelected.Id)
			{
				canBeSelected.Selected = true;
				canBeSelected.SelectionStart = true;
				canBeSelected.SelectionEnd = false;
				startIndex = iteration;
				if (endIndex < 0)
				{
					direction = 1;
				}
			}
			else
			{
				canBeSelected.Selected = false;
				canBeSelected.SelectionEnd = false;
				canBeSelected.SelectionStart = false;
			}
		}

		if (endIndex >= 0)
		{
			iteration = -1;
			bool insideRange = false;
			if (direction < 0)
			{
				(endIndex, startIndex) = (startIndex, endIndex);
			}
			foreach (var canBeSelected in list)
			{
				iteration++;
				if (iteration > endIndex)
				{
					insideRange = false;
				}
				if (iteration == startIndex)
				{
					canBeSelected.Selected = true;
					canBeSelected.SelectionStart = true;
					canBeSelected.SelectionEnd = false;
					insideRange = true;
				}
				else
				if (iteration == endIndex)
				{
					canBeSelected.Selected = true;
					canBeSelected.SelectionStart = false;
					canBeSelected.SelectionEnd = true;
				}
				else
				if (insideRange)
				{
					canBeSelected.Selected = true;
					canBeSelected.SelectionEnd = false;
					canBeSelected.SelectionStart = false;
				}
			}


		}

		System.Diagnostics.Debug.WriteLine($"Selected: {list.Where(x=>x.Selected).Select(x=>$"{x.Id:0.00}").ToList().ToTags()}");

		return (startIndex, endIndex);
	}


}