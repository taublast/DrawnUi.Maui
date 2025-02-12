using Newtonsoft.Json;

namespace AppoMobi.Models;

public class AppoBookedInterval : AppoTimeInterval
    
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string ServiceId { get; set; }
    public string ObjectId { get; set; }

    [JsonIgnore]
    public string TimeDesc
        
    {
        get
        {
            if (TimeStart != null)
            {
                var time = TimeStart.Value.ToShortTimeString();
                if (Status == "" || Status == "pending" || Status == null)
                {
                    //confirmation pending
                    var ret = string.Format("{0} (confirmation pending)", time);
                    return ret;
                    //return string.Format(ResStrings.AppoTimeDescPending, time);
                }
                else
                {
                    //confirmed
                    return time;
                }
            }

            return "";
        }

    }
}