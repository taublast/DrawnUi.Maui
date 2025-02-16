using AppoMobi.Models;
using AppoMobi.Specials;

namespace TestCalendar.Drawn;

public interface IDrawnDaysController
{

	ObservableRangeCollection<AppoMonth> Months { get; }

	AppoMonth? CurrentMonth { get; }

	bool RangeSelected { get; set; }

	bool IsSelecting { get; }
	
	bool CanSelectNextMonth { get; }
	
	bool CanSelectPrevMonth { get; }
	
	string CurrentMonthDesc { get;  }

	void SelectEnd(AppoDay? day);

	void SelectStart(AppoDay? day);
	
	void SelectNextMonth();
	
	void SelectPrevMonth();

	List<string> GetWeekDaysShortNames();

	event EventHandler<(DateTime? Start, DateTime? End)>? SelectionDatesChanged;

	event EventHandler<IEnumerable<AppoDay>>? SelectionChanged;

}