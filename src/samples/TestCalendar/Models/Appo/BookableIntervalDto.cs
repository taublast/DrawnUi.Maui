namespace AppoMobi.Models;

public class BookableIntervalDto : AppoTimeInterval
{
    public string Id { get; set; }
    public int SeatsTaken { get; set; }
    public string PublicNotes { get; set; }
}