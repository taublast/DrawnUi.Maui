using AppoMobi.Models;

namespace TestCalendar.Views;

public class MainElementDto
{
    public string Id { get; set; }

    public string Title { get; set; }
    public string TitleLocal { get; set; }

    public string GalleryId { get; set; } //UID

    public string Desc { get; set; }
    public string Conditions { get; set; }
    public string LinkMoreInfo { get; set; }

    public string ImageId { get; set; }
    public string ImageColor { get; set; }

    public string Category { get; set; } //UID

    public int Priority { get; set; }

    public int MaxSeats { get; set; }

    public decimal Price { get; set; }

    public string PriceConverted { get; set; }

    public string PaymentMethods { get; set; }

    public List<BookableIntervalDto> Schedules { get; set; }

}