using DrawnUi.Draw;

namespace DrawnUi.MapsUi
{
    public interface IMapPin
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ZIndex { get; set; }
        public bool IsVisible { get; set; }
        public SkiaControl Icon { get; set; }
        public event EventHandler<MapPin> Tapped;
        public event EventHandler<MapPin> InfoWindowTapped;
    }
}