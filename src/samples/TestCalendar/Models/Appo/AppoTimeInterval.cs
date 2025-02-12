using System.Globalization;
using Newtonsoft.Json;

namespace AppoMobi.Models;

public class AppoTimeInterval
{
    [JsonIgnore]
    public TimeSpan? Duration
        
    {
        get
        {
            DateTime? min = TimeStart;
            DateTime? max = TimeEnd;
            return min.HasValue && max.HasValue ? max.Value - min.Value : (TimeSpan?)null;
        }
    }

    public string Tag { get; set; }

        
    public AppoTimeInterval(DateTime? start, DateTime? end)
        
    {
        TimeStart = start;
        TimeEnd = end;
    }

        
    public AppoTimeInterval()
        
    {
    }
    public DateTime? TimeStart { get; set; }
    public DateTime? TimeEnd { get; set; }


    [JsonIgnore]
        
    public string DowDesc
        
    {
        get
        {
            if (TimeStart != null)
            {
                var t = DateTimeFormatInfo.CurrentInfo;
                if (t != null)
                {
                    return t.GetDayName(TimeStart.Value.Date.DayOfWeek);
                }
                else
                {
                    return "???";
                }
            }

            return "";
        }
    }

 
        [JsonIgnore]
        public string TimeDescAt
        {
            get
            {
                try
                {
                    var ret = string.Format(
                        "{0}",
                        //ResStrings.BookingTimeDescAt, 
                        TimeStart.Value.ToShortTimeString());
                    return ret;
                }
                catch (Exception e)
                {
                    return "ERROR";
                }
            }
        }
 
        
  
        [JsonIgnore]
        public string DateDesc
        
        {
            get
            {
                if (TimeStart != null)
                {
                    var date = TimeStart.Value.Date.ToString("d MMMM yyyy");
                    return $"{date}";
                }

                return "Oops";//ResStrings.Oops;
            }
        }
   

    [JsonIgnore]
        
    public string DayDesc
        
    {
        get
        {
            if (TimeStart != null)
            {
                return TimeStart.Value.Day.ToString();
            }
            return "";
        }
    }

    static string CorrectStringUponNumber(int num, string zero, string one, string with_one, string with_two, string other)
    {
        string ret = "";
        var lastDigit = "";
        int iDigit = 0;
        if (num > 0)
        {
            lastDigit = num.ToString().Substring(num.ToString().Length - 1);
            iDigit = int.Parse(lastDigit);
        }
        if (num < 1)
        {
            ret = zero;
        }
        else
        {
            if (num == 1)
            {
                ret = string.Format(one, num);
            }
            else
            if (iDigit == 1 && !(num >= 10 && num <= 20))
            {
                ret = string.Format(with_one, num);
            }
            else
            {
                if ((iDigit >= 2 && iDigit <= 4) && !(num >= 10 && num <= 20))
                {
                    ret = string.Format(with_two, num);
                }
                else
                {
                    ret = string.Format(other, num);
                }
            }

        }

        return ret;
    }

 
        [JsonIgnore]
        public string ExplainDate
        {
            get
            {
                if (TimeStart == null) return "";

                var todayDate = DateTime.Now;
                var today = todayDate.Date;
                var when = TimeStart.Value.Date;
                var days = (when - today).Days;

                var interval = $"{days}";
                //var interval = CorrectStringUponNumber(days,
                //    ResStrings.ExplainDate_Today,
                //    ResStrings.ExplainDate_Tomm,
                //    ResStrings.ExplainDate_X1,
                //    ResStrings.ExplainDate_X2,
                //    ResStrings.ExplainDate_X);

                var ret = string.Format(
                    "{0}",
                    //ResStrings.ExplainDateWithInterval, 
                    interval.ToLower());

                return ret;
            }
        }
    


   
        [JsonIgnore]
        public string ExplainWhen
        {
            get
            {
                if (TimeStart == null) return "";

                var todayDate = DateTime.Now;

                var today = todayDate.Date;
                var when = TimeStart.Value.Date;
                var delta = when - today;
                var days = delta.Days;

                var interval = $"{days}";
                //var interval = CorrectStringUponNumber(days,
                //    ResStrings.ExplainDate_Today,
                //    ResStrings.ExplainDate_Tomm,
                //    ResStrings.ExplainDate_X1,
                //    ResStrings.ExplainDate_X2,
                //    ResStrings.ExplainDate_X);

                var ret = interval;

                return ret;
            }
        }
    



}