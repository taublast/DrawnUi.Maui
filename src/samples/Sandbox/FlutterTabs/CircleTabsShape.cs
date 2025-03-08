namespace Sandbox
{
    public class CircleTabsShape : SkiaShape
    {
        public static float SidePadding = 12;
        public static float SmoothBezier = 12;
        public static float  cornerRadius = 12;

        private float _btnPaddingPts;
        private float _btnSizePts;
        private float _fabX;
        private bool _wasBuild;

        public CircleTabsShape(float btnSizePts, float btnPaddingPts)
        {
            Type = ShapeType.Path;

            _btnSizePts = btnSizePts;
            _btnPaddingPts = btnPaddingPts;
        }

        public void Setup(float btnSize, float btnPadding)
        {
            _btnSizePts = btnSize;
            _btnPaddingPts = btnPadding;
        }

        public static string GenerateClippedRectPath(
            double rectWidth = 200,      // Total width of rectangle
            double rectHeight = 56,      // Height from top of rect to bottom (120-64=56 in original)
            double cornerRadius = 16,    // Radius of rectangle corners
            double circleWidth = 100,     // Width of circle cutout (roughly 140-60=80 in original)
            double circleHeight = 100,    // Height/radius of circle cutout
            double circleOffsetX = 100,  // X position of circle center (100 is middle of 200)
            double circleOffsetY = 0,    // Y offset of circle from top
            double transitionRadius = 4  // Radius of transition curves (controls smoothness where circle meets rect)
        )
        {

            // Rectangle top and bottom edges
            double topY = 0;
            double bottomY = rectHeight;

            // Calculate circle horizontal bounds based on its center (circleOffsetX) and width
            double circleStartX = circleOffsetX - (circleWidth / 2);
            double circleEndX = circleOffsetX + (circleWidth / 2);

            // Extend transition points beyond circle edges by transitionRadius for smooth blending
            double transitionStartX = circleStartX - transitionRadius;
            double transitionEndX = circleEndX + transitionRadius;

            // Calculate control points for Bezier curves
            double curveOffset = transitionRadius * 0.8;        // How far curve peaks vertically
            double midPointOffset = curveOffset * 0.5;          // Horizontal position of middle control point

            return $"M0,{cornerRadius} " +                      // Move to start at bottom left with corner radius
                   $"A{cornerRadius},{cornerRadius} 0 0,1 {cornerRadius},{topY} " +   // Draw top left corner arc
                   $"L{transitionStartX},{topY} " +             // Line to where circle blend starts
                   // Enhanced entrance curve with intermediate control point
                   $"C{circleStartX},{topY} {circleStartX + midPointOffset},{topY} {circleStartX + curveOffset},{topY + curveOffset} " +  // Bezier curve into circle
                   $"A{circleHeight / 2},{circleHeight / 2} 0 0,0 {circleEndX - curveOffset},{topY + curveOffset} " +  // Draw half circle cutout
                   // Enhanced exit curve with intermediate control point
                   $"C{circleEndX - midPointOffset},{topY} {circleEndX},{topY} {transitionEndX},{topY} " +  // Bezier curve out of circle
                   $"L{rectWidth - cornerRadius},{topY} " +     // Line to top right corner
                   $"A{cornerRadius},{cornerRadius} 0 0,1 {rectWidth},{cornerRadius} " +  // Draw top right corner arc
                   $"L{rectWidth},{bottomY} " +                 // Line down right side
                   $"L0,{bottomY} " +                          // Line across bottom
                   $"Z";                                       // Close path

        }

        public void Build()
        {
            var circleD = _btnSizePts * RenderingScale + _btnPaddingPts * 2 * RenderingScale + SmoothBezier *  RenderingScale;

            //var fabCenterX = _fabX + (_btnSize / 2) + _btnPadding * 2;

            string pathData = GenerateClippedRectPath(
                rectWidth: DrawingRect.Width,
                rectHeight: DrawingRect.Height,
                cornerRadius: cornerRadius * RenderingScale,
                circleWidth: circleD,
                circleHeight: circleD / 3,
                circleOffsetX: _fabX, //pixels
                //circleOffsetY: -20,
                transitionRadius: SmoothBezier * RenderingScale
            );

            this.PathData = pathData;
        }

        public void SetCutoutMiddleX(float fabX)
        {
            _fabX = fabX;
            Build();
        }

        protected override void OnLayoutReady()
        {
            base.OnLayoutReady();

           Build();
        }

    }
}
