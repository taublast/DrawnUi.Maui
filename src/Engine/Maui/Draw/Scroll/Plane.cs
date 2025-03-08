namespace DrawnUi.Maui.Draw
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class Plane
    {
        public override string ToString()
        {
            return $"Plane {Id}, offset {OffsetX},{OffsetY}, destination {Destination}";
        }

        //sources offsets
        public float OffsetY;
        public float OffsetX;

        public SKColor BackgroundColor { get; set; } = SKColors.Transparent;
        public RenderObject RenderObject { get; set; }
        public SKRect Destination { get; set; }
        public SKRect LastDrawnAt { get; set; }
        public CachedObject CachedObject { get; set; }
        public SKSurface Surface { get; set; }
        public bool IsReady { get; set; } = false;
        public string Id { get; set; }

        public void Reset(SKSurface surface, SKRect source)
        {
            OffsetX = 0;
            OffsetY = 0;
            Surface = surface;
            Invalidate();
        }

        public void Invalidate()
        {
            IsReady = false;
            LastDrawnAt = SKRect.Empty;
        }
    }
}
