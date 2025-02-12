using AppoMobi.Specials;

namespace AppoMobi.Models;

public class DaylyHours  
{
	public IEnumerable<DayOfWeek> Dow
	{
		get
		{
			var output = new List<DayOfWeek>();
			var iterate = DaysOfWeek.TagsToList();
			foreach (var member in iterate)
			{
				var dday = Enum.Parse(typeof(DayOfWeek), member);
				output.Add((DayOfWeek)dday);
			}

			return output;
		}
		//set
		//{
		//    DaysOfWeek = String.Join(",", value);
		//}
	}
	public string DaysOfWeek { get; set; }

	public TimeSpan Start { get; set; }

	public TimeSpan End { get; set; }
}