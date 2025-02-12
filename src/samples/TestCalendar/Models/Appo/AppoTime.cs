namespace AppoMobi.Models;

public class AppoTime
    
{
        
    public AppoTime(TimeSpan start, TimeSpan end)
        
    {
        TimeStart = start;
        TimeEnd = end;
    }
        
    public AppoTime()
        
    {
    }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
}