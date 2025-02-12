using System.Data.Common;
using AppoMobi.Specials;

namespace AppoMobi.Models;

public class AppoConstraints
{
	public IEnumerable<DayOfWeek> ClosedDaysOfWeek { get; set; }
	public IEnumerable<DayOfWeek> OpenDaysOfWeek { get; set; }
	public IEnumerable<DaylyHours> BusinessHours { get; set; }

	public TimeSpan MinHour { get; set; }
	public TimeSpan MaxHour { get; set; }

	/// <summary>
	/// Whether bookable time should be indicated in bookable time for each object to be available
	/// </summary>
	public bool ExplicitBookable { get; set; }

	public AppoTime GetConstraintsForDay(DateTime date)
	{
		var constraints = new AppoTime(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59));

		var thisDow = date.DayOfWeek;
		var found = BusinessHours.FirstOrDefault(x => x.DaysOfWeek.Contains(thisDow.ToString()));

		if (found == null)
			return constraints;

		constraints.TimeStart = found.Start;
		constraints.TimeEnd = found.End;
		return constraints;
	}
}

public class AppoHelper
{
	 

	public static DateTime UtcToZoneTime(DateTime timeUtc, string zone)
	{
		var cstTime = timeUtc;
		try
		{
			var cstZone = TimeZoneInfo.FindSystemTimeZoneById(zone); //"Central Standard Time"
			cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
			Console.WriteLine("The date and time are {0} {1}.",
				cstTime,
				cstZone.IsDaylightSavingTime(cstTime) ? cstZone.DaylightName : cstZone.StandardName);
		}
		catch (TimeZoneNotFoundException)
		{
			Console.WriteLine("The registry does not define the Central Standard Time zone.");
		}
		catch (InvalidTimeZoneException)
		{
			Console.WriteLine("Registry data on the Central Standard Time zone has been corrupted.");
		}

		return cstTime;
	}


	public static AppoConstraints GetDefaultConstraints()
	{

		var constraints = new AppoConstraints();
 
		{
			
			constraints.ExplicitBookable = false;

			// general constraints
			constraints.MinHour = TimeSpan.FromHours(10);
			constraints.MaxHour = TimeSpan.FromHours(23);

			var listOpenHours = new List<DaylyHours>();
			//if (co.DaylyHours.Any())
			//{
			//	//have specific dayly hours
			//	// 1 day specific constraints
			//	foreach (var subTime in co.DaylyHours)
			//	{
			//		var businessHours = new DaylyHours
			//		{
			//			DaysOfWeek = subTime.DaysOfWeek,
			//			Start = subTime.Start,
			//			End = subTime.End
			//		};
			//		listOpenHours.Add(businessHours);
			//	}
			//}
			//else
			{
				//user have set have general business hours ONLY
				//create list from general constraints
				var businessHours = new DaylyHours
				{
					DaysOfWeek = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday",
					Start = constraints.MinHour,
					End = constraints.MaxHour
				};
				listOpenHours.Add(businessHours);
			}

			constraints.BusinessHours = listOpenHours;

			// 2 general constraints
			var workingDays = "Monday,Tuesday,Wednesday,Thursday,Friday";
			var open = true;
			var listOpenDays = workingDays.TagsToList();
			if (listOpenDays == null || listOpenDays[0] == "")
				//totally closed;
				open = false;

			if (open)
			{
				var dayOpen = new bool[7];
				foreach (var day in listOpenDays)
				{
					var dday = Enum.Parse(typeof(DayOfWeek), day);
					var indexOpen = (int)dday;
					dayOpen[indexOpen] = true;
				}

				var listCalendarClosed = new List<DayOfWeek>();
				var listCalendarOpen = new List<DayOfWeek>();

				var countDays = 0;
				foreach (var day in dayOpen)
				{
					if (!day)
						listCalendarClosed.Add((DayOfWeek)countDays);
					else
						listCalendarOpen.Add((DayOfWeek)countDays);

					countDays++;
				}

				constraints.OpenDaysOfWeek = listCalendarOpen.ToArray();
				constraints.ClosedDaysOfWeek = listCalendarClosed.ToArray();
			}
		} //end of using db

		return constraints;
	}

	public static IEnumerable<AppoTimeInterval> GetAvailable(DateTime? start, DateTime? end, AppoConstraints constraints,
   TimeSpan minInterval)
	{
		if (start == null || end == null) return null;

		var periods = new List<AppoTimeInterval>();
		var totalDays = (end.Value - start.Value).TotalDays;
		var _start = start.Value;
		var _end = end.Value;

		if (end.Value - start.Value < minInterval) end = start + minInterval;

		for (var dc = 0; dc < totalDays + 1; dc++)
		{
			_start = start.Value.AddDays(dc);
			_end = _start;
			_end = _end.Date.Add(end.Value.TimeOfDay);

			//check if we are globally closed today
			var aa = constraints.ClosedDaysOfWeek;
			var bb = _start.DayOfWeek;
			var cc = aa.Contains(bb);

			if (cc) continue; //we are just fricking closed

			var cs = constraints.GetConstraintsForDay(_start);
			//apply general contraints
			if (cs.TimeStart < constraints.MinHour)
				cs.TimeStart = constraints.MinHour;
			if (cs.TimeEnd > constraints.MaxHour)
				cs.TimeEnd = constraints.MaxHour;


			var multiDays = _start.Date != end.Value.Date;

			//set end constraint
			if (multiDays && dc != totalDays - 1) //different dates 
				_end = _end.Date.Add(new TimeSpan(23, 59, 59));

			//set end constraint
			if (multiDays && dc != 0) //different dates and not first day
				_start = _start.Date.Add(new TimeSpan(0, 0, 0));

			//set starting constraint
			if (_start.TimeOfDay < cs.TimeStart)
				_start = _start.Date.Add(cs.TimeStart);

			//set starting constraint
			if (_start.TimeOfDay >= cs.TimeEnd)
				_start = _start.Date.Add(cs.TimeEnd);


			//set end constraint
			if (_end.TimeOfDay > cs.TimeEnd) //or.. out of possible
				_end = _end.Date + cs.TimeEnd;

			var thisDay = new AppoTimeInterval(_start, _end);

			var delta = thisDay.TimeEnd - thisDay.TimeStart;

			if (delta.Value >= minInterval)
				periods.Add(thisDay);

			//                _end.Add(end.Value.TimeOfDay); //set time only
			//                _start.Add(start.Value.TimeOfDay); //set time only
		}

		return periods;
	}

	/*

	public IEnumerable<AppoTimeInterval> GetBlockedIntervals(DateTime start, DateTime end,
		string appoObjectKey = "blocked")
	{
		var booked = dbContent.AppoBookableIntervals
			.Where(s => s.TimeStart >= start && s.TimeEnd <= end &&
						s.DbAppoObject.Key == appoObjectKey) //todo add objectId
			.Select(x => new AppoTimeInterval
			{
				TimeStart = x.TimeStart,
				TimeEnd = x.TimeEnd
			}).ToList();
		return booked;
	}

	public IEnumerable<AppoTimeInterval> GetBookableIntervals(string appoObjectKey, DateTime start, DateTime end)
	{
		var ret = dbContent.AppoBookableIntervals
			.Where(s => s.TimeStart >= start && s.TimeEnd <= end &&
						s.DbAppoObject.Key == appoObjectKey) //todo add objectId
			.Select(x => new AppoTimeInterval
			{
				TimeStart = x.TimeStart,
				TimeEnd = x.TimeEnd
			}).ToList();
		return ret;
	}

	public IEnumerable<AppoTimeInterval> GetBookedIntervalsForObject(string appoObjectId, DateTime start,
		DateTime end)
	{
		var booked = dbContent.AppoBookedIntervals
			.Where(s => s.TimeStart >= start && s.TimeEnd <= end && s.DbAppoObject.Key == appoObjectId &&
						(s.Status == "confirmed" || s.Status == "pending" || s.Status == "" ||
						 s.Status == null)) //todo add objectId
			.Select(x => new AppoTimeInterval
			{
				TimeStart = x.TimeStart,
				TimeEnd = x.TimeEnd
			}).ToList();
		return booked;
	}

	public IEnumerable<AppoTimeInterval> GetBookableTimeForObject(string appoObjectId, DateTime start, DateTime end)
	{
		var periods = new List<AppoTimeInterval>();

		//var constraints = GetConstraints(start, end);


		return periods;
	}

	/// <summary>
	/// Company general available time for booking
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <param name="constraints"></param>
	/// <param name="minInterval"></param>
	/// <returns></returns>
	public IEnumerable<AppoTimeInterval> GetAvailable(DateTime? start, DateTime? end, AppoConstraints constraints,
		TimeSpan minInterval)
	{
		if (start == null || end == null) return null;

		var periods = new List<AppoTimeInterval>();
		var totalDays = (end.Value - start.Value).TotalDays;
		var _start = start.Value;
		var _end = end.Value;

		if (end.Value - start.Value < minInterval) end = start + minInterval;

		for (var dc = 0; dc < totalDays + 1; dc++)
		{
			_start = start.Value.AddDays(dc);
			_end = _start;
			_end = _end.Date.Add(end.Value.TimeOfDay);

			//check if we are globally closed today
			var aa = constraints.ClosedDaysOfWeek;
			var bb = _start.DayOfWeek;
			var cc = aa.Contains(bb);

			if (cc) continue; //we are just fricking closed

			var cs = constraints.GetConstraintsForDay(_start);
			//apply general contraints
			if (cs.TimeStart < constraints.MinHour)
				cs.TimeStart = constraints.MinHour;
			if (cs.TimeEnd > constraints.MaxHour)
				cs.TimeEnd = constraints.MaxHour;


			var multiDays = _start.Date != end.Value.Date;

			//set end constraint
			if (multiDays && dc != totalDays - 1) //different dates 
				_end = _end.Date.Add(new TimeSpan(23, 59, 59));

			//set end constraint
			if (multiDays && dc != 0) //different dates and not first day
				_start = _start.Date.Add(new TimeSpan(0, 0, 0));

			//set starting constraint
			if (_start.TimeOfDay < cs.TimeStart)
				_start = _start.Date.Add(cs.TimeStart);

			//set starting constraint
			if (_start.TimeOfDay >= cs.TimeEnd)
				_start = _start.Date.Add(cs.TimeEnd);


			//set end constraint
			if (_end.TimeOfDay > cs.TimeEnd) //or.. out of possible
				_end = _end.Date + cs.TimeEnd;

			var thisDay = new AppoTimeInterval(_start, _end);

			var delta = thisDay.TimeEnd - thisDay.TimeStart;

			if (delta.Value >= minInterval)
				periods.Add(thisDay);

			//                _end.Add(end.Value.TimeOfDay); //set time only
			//                _start.Add(start.Value.TimeOfDay); //set time only
		}

		return periods;
	}

	public IEnumerable<AppoTimeInterval> IntersectSchedules(List<AppoTimeInterval> one, List<AppoTimeInterval> two)
	{
		//combine everything into 1 list
		var periods = new TimePeriodCollection();
		foreach (var item in one)
			try
			{
				periods.Add(new TimeRange(item.TimeStart.Value, item.TimeEnd.Value));
			}
			catch (Exception e)
			{
			}

		foreach (var item in two)
			try
			{
				periods.Add(new TimeRange(item.TimeStart.Value, item.TimeEnd.Value));
			}
			catch (Exception e)
			{
			}

		var periodIntersector =
			new TimePeriodIntersector<TimeRange>();
		var intersectedPeriods = periodIntersector.IntersectPeriods(periods);

		var ret = new List<AppoTimeInterval>();
		foreach (var item in intersectedPeriods)
			try
			{
				ret.Add(new AppoTimeInterval(item.Start, item.End));
			}
			catch (Exception e)
			{
			}

		return ret;
	}

	public IEnumerable<AppoTimeInterval> CombineSchedules(List<AppoTimeInterval> one, List<AppoTimeInterval> two)
	{
		//combine everything into 1 list
		var periods = new TimePeriodCollection(); //Itenso.TimePeriod nuget
		foreach (var item in one)
			try
			{
				periods.Add(new TimeRange(item.TimeStart.Value, item.TimeEnd.Value));
			}
			catch (Exception e)
			{
			}

		foreach (var item in two)
			try
			{
				periods.Add(new TimeRange(item.TimeStart.Value, item.TimeEnd.Value));
			}
			catch (Exception e)
			{
			}

		var periodCombiner = new TimePeriodCombiner<TimeRange>();
		var combinedPeriods = periodCombiner.CombinePeriods(periods);

		var ret = new List<AppoTimeInterval>();
		foreach (var item in combinedPeriods)
			try
			{
				ret.Add(new AppoTimeInterval(item.Start, item.End));
			}
			catch (Exception e)
			{
			}

		return ret;
	}

	public IEnumerable<AppoTimeInterval> ApplyConstraints(List<AppoTimeInterval> available,
		List<AppoTimeInterval> moreConstraints, TimeSpan minInterval)
	{
		var gapCalculator =
			new TimeGapCalculator<TimeRange>(new TimeCalendar());

		var periods = new List<AppoTimeInterval>();

		foreach (var current in available)
		{
			var searchLimits = new TimePeriodCollection();
			var constraints = new TimePeriodCollection();
			searchLimits.Add(new TimeInterval(current.TimeStart.Value, current.TimeEnd.Value));

			var found = moreConstraints.Where(x => x.TimeStart < current.TimeEnd && current.TimeStart < x.TimeEnd)
				.ToList(); //overlapping

			var nochange = true;
			foreach (var limits in found)
			{
				nochange = false;
				constraints.Add(new TimeInterval(limits.TimeStart.Value, limits.TimeEnd.Value));
			}

			if (nochange)
			{
				if (current.Duration >= minInterval) periods.Add(current);
			}
			else
			{
				var freeTimes = gapCalculator.GetGaps(constraints, searchLimits);
				foreach (var free in freeTimes)
					if (free.Duration >= minInterval)
					{
						var add = new AppoTimeInterval(free.Start, free.End);
						periods.Add(add);
					}
			}
		}

		return periods;
	}

	private IEnumerable<AppoTimeInterval> GetCutAvailable(AppoTimeInterval current,
		List<AppoTimeInterval> moreConstraints, TimeSpan minInterval)
	{
		var periods = new List<AppoTimeInterval>();
		var found = moreConstraints.Where(x => x.TimeStart < current.TimeEnd && current.TimeStart < x.TimeEnd)
			.ToList(); //overlapping
		var nochange = true;
		foreach (var period in found)
		{
			nochange = false;
			//create list of gaps between blocked peiods inside the day
			var left = new AppoTimeInterval(current.TimeStart, period.TimeStart.Value.AddSeconds(-1));
			var delta1 = left.TimeEnd - left.TimeStart;
			var right = new AppoTimeInterval(period.TimeEnd.Value, current.TimeEnd);
			var delta2 = right.TimeEnd - right.TimeStart;

			bool hasLeft = false, hasRight = false;
			IEnumerable<AppoTimeInterval> myLeft = null, myRight = null;

			if (delta1 >= minInterval)
			{
				hasLeft = true;
				myLeft = GetCutAvailable(left, moreConstraints, minInterval);
			}

			if (delta2 >= minInterval)
			{
				hasRight = true;
				myRight = GetCutAvailable(right, moreConstraints, minInterval);
			}

			if (hasRight && hasLeft)
			{
				//todo get intersection
				periods.AddRange(myLeft);
				periods.AddRange(myRight);
				//periods.AddRange(GetIntersections(myLeft, moreConstraints));
				//periods.AddRange(GetIntersections(myRight, moreConstraints));
			}
			else
			{
				if (hasLeft)
					periods.AddRange(myLeft);
				if (hasRight)
					periods.AddRange(myRight);
			}
		}

		if (nochange)
			periods.Add(current);

		return periods;
	}

	private static AppoTimeInterval GetIntersection(AppoTimeInterval range1, AppoTimeInterval range2)
	{
		var iRange = new AppoTimeInterval();
		iRange.TimeStart = range1.TimeStart < range2.TimeStart ? range2.TimeStart : range1.TimeStart;
		iRange.TimeEnd = range1.TimeEnd < range2.TimeEnd ? range1.TimeEnd : range2.TimeEnd;
		if (iRange.TimeStart > iRange.TimeEnd) iRange = null;
		return iRange;
	}

	public IEnumerable<AppoTimeInterval> GetIntersections(IEnumerable<AppoTimeInterval> myLeft,
		IEnumerable<AppoTimeInterval> myRight)
	{
		var list = new List<AppoTimeInterval>();
		foreach (var current in myLeft)
		{
			var found = myRight.Where(x => x.TimeStart < current.TimeEnd && current.TimeStart < x.TimeEnd)
				.ToList(); //overlapping
			foreach (var item in found)
			{
				var intersection = GetIntersection(item, current);
				list.Add(intersection);
			}
		}

		return list;
	}


	public void TimeGapCalculatorSample()
	{
		// simulation of some reservations
		var reservations = new TimePeriodCollection();
		reservations.Add(new Days(2011, 3, 7, 2));
		reservations.Add(new Days(2011, 3, 16, 2));

		// the overall search range
		var searchLimits = new CalendarTimeRange(
			new DateTime(2011, 3, 4), new DateTime(2011, 3, 21));

		// search the largest free time block
		var largestFreeTimeBlock = FindLargestFreeTimeBlock(reservations, searchLimits);
		Console.WriteLine("Largest free time: " + largestFreeTimeBlock);
		// > Largest free time: 09.03.2011 00:00:00 - 11.03.2011 23:59:59 | 2.23:59
	} // TimeGapCalculatorSample

	public ICalendarTimeRange FindLargestFreeTimeBlock(
		IEnumerable<ITimePeriod> reservations,
		ITimePeriod searchLimits = null, bool excludeWeekends = true)
	{
		var bookedPeriods = new TimePeriodCollection(reservations);

		if (searchLimits == null) searchLimits = bookedPeriods; // use boundary of reservations

		if (excludeWeekends)
		{
			var currentWeek = new Week(searchLimits.Start);
			var lastWeek = new Week(searchLimits.End);
			do
			{
				var days = currentWeek.GetDays();
				foreach (Day day in days)
				{
					if (!searchLimits.HasInside(day)) continue; // outside of the search scope
					if (day.DayOfWeek == DayOfWeek.Saturday ||
						day.DayOfWeek == DayOfWeek.Sunday)
						bookedPeriods.Add(day); // // exclude weekend day
				}

				currentWeek = currentWeek.GetNextWeek();
			} while (currentWeek.Start < lastWeek.Start);
		}

		// calculate the gaps using the time calendar as period mapper
		var gapCalculator =
			new TimeGapCalculator<TimeRange>(new TimeCalendar());
		var freeTimes = gapCalculator.GetGaps(bookedPeriods, searchLimits);
		if (freeTimes.Count == 0) return null;

		freeTimes.SortByDuration(); // move the largest gap to the start

		return new CalendarTimeRange(freeTimes[0]);
	} // FindLargestFreeTimeBlock

	*/
}
