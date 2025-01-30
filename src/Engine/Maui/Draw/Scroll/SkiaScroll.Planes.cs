#define TMP

namespace DrawnUi.Maui.Draw
{


    public class Plane
    {
        public SKColor BackgroundColor { get; set; } = SKColors.Transparent;
        public RenderObject RenderObject { get; set; }
        public SKRect Source { get; set; }
        public SKRect Destination { get; set; }
        public CachedObject CachedObject { get; set; }
        public SKSurface Surface { get; set; }
        public bool IsReady { get; set; } = false;

        public void Reset(SKSurface surface, SKRect source)
        {
            Surface = surface;
            Source = source;
            IsReady = false;
        }

        public void Invalidate()
        {
            IsReady = false;
        }

    }

    public partial class SkiaScroll
    {

        private Plane PlaneA { get; set; }
        private Plane PlaneB { get; set; }

        private int _planeWidth;
        private int _planeHeight;

        // create 2 planes for rendering inside scroll. one plane is around 2 sizes of viewport.
        // when scrolling we will draw 2 objects with scroll offset max:plane A and plane B.
        // at start we draw children on plane A then we prepare plane B in background.
        // when scrolling we drawn the visible part of plane A nd maybe the plane B if itis around to appear.
        // when plane A is out of view we move it to the end of plane B and prepare it for next drawing.

        private void PreparePlane(
            SkiaDrawingContext context,
            Plane plane, SKRect destination,
            float scale, float zoomedScale,
            object arguments)
        {
            plane.Source = destination;
            plane.Destination = destination;

            var recordingContext = context.CreateForRecordingImage(plane.Surface, destination.Size);
            recordingContext.Canvas.Translate(-destination.Left, -destination.Top);

            recordingContext.Canvas.Clear(plane.BackgroundColor);

            PaintViews(recordingContext, destination, destination, scale, zoomedScale, arguments);

            recordingContext.Canvas.Flush();

            DisposeObject(plane.CachedObject);

            plane.CachedObject = new CachedObject(
                SkiaCacheType.Image,
                plane.Surface,
                new SKRect(0, 0, _planeWidth, _planeHeight),
                destination)
            {
                PreserveSourceFromDispose = true
            };

            plane.IsReady = true;
        }

        //private void PositionPlane(Plane targetPlane, Plane referencePlane, SKRect destination)
        //{
        //    targetPlane.Source = referencePlane.Source;
        //    if (Orientation == ScrollOrientation.Vertical)
        //    {
        //        targetPlane.Source.Offset(0, _planeHeight);
        //    }
        //    else if (Orientation == ScrollOrientation.Horizontal)
        //    {
        //        targetPlane.Source.Offset(_planeWidth, 0);
        //    }

        //    targetPlane.Destination = targetPlane.Source;
        //}


        public virtual void InitializePlanes()
        {
            var viewportWidth = Viewport.Pixels.Width;
            var viewportHeight = Viewport.Pixels.Height;

            // Ensure the planes cover twice the viewport area
            _planeWidth = (int)(viewportWidth); //for vertical, todo all orientations
            _planeHeight = (int)(viewportHeight * 2);

            PlaneA = new Plane
            {
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                BackgroundColor = SKColors.Coral,
            };

            PlaneB = new Plane
            {
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                BackgroundColor = SKColors.DarkKhaki,
            };

            Debug.WriteLine($"Planes initialized: {PlaneA} and {PlaneB}");
        }

        protected void SwapPlanes()
        {
            (PlaneA, PlaneB) = (PlaneB, PlaneA);
            PlaneB.IsReady = false; //todo why?
        }

        void SetContentVisibleDelegate()
        {
            if (Content != null && Content.DelegateGetOnScreenVisibleArea == null)
            {
                Content.DelegateGetOnScreenVisibleArea = ReportVisibleAreToContent;
            }
        }

        private ScaledRect ReportVisibleAreToContent(float arg)
        {
            return ScaledRect.FromPixels(PlaneA.Destination, RenderingScale);
        }

        public override void UpdateByChild(SkiaControl control)
        {
            base.UpdateByChild(control);

            //todo somehow detect which plane is invalidated upon child on it

            PlaneA?.Invalidate();
            PlaneB?.Invalidate();
        }

        private int visibleAreaCaller = 0;
        private bool _planesWorkingInBackground = false;
        private SemaphoreSlim _lockPlanesWorker = new(1);

        /// <summary>
        /// Launches PreparePlaneB in background
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destination"></param>
        /// <param name="scale"></param>
        /// <param name="zoomedScale"></param>
        /// <param name="arguments"></param>
        protected void TriggerPreparePlaneB(
            SkiaDrawingContext context,
            SKRect destination,
            float scale, float zoomedScale,
            object arguments)
        {
            if (_planesWorkingInBackground)
                return;

            _planesWorkingInBackground = true;

            async Task DoMeasure()
            {
                await _lockPlanesWorker.WaitAsync();

                PreparePlane(context, PlaneB, destination, scale, zoomedScale, arguments);

                _lockPlanesWorker.Release();

                _planesWorkingInBackground = false;
            }

            Task.Run(DoMeasure);
        }


        public virtual void DrawPlanes(
            SkiaDrawingContext context,
            SKRect destination, float scale, float zoomedScale,
            object arguments)
        {
            if (Content == null)
            {
                return;
            }

            if (PlaneA == null || PlaneB == null)
            {
                if (Content is SkiaLayout layout)
                {
                    layout.Virtualisation = VirtualisationType.Managed;
                }
                else
                {
                    return;
                }
                InitializePlanes();
            }

            var displayRectA = new SKRect(
                ContentRectWithOffset.Pixels.Left,
                ContentRectWithOffset.Pixels.Top,
                ContentRectWithOffset.Pixels.Left + _planeWidth,
                ContentRectWithOffset.Pixels.Top + _planeHeight
            );

            if (!PlaneA.IsReady)
            {
                Debug.WriteLine("Preparing PLANE A..");
                PreparePlane(context, PlaneA, displayRectA, scale, zoomedScale, arguments);
            }

            // Draw the planes

            // A

            var showA = PlaneA.Source;
            showA.Offset(0, InternalViewportOffset.Pixels.Y);

            if (ContentViewport.Pixels.IntersectsWithInclusive(showA))
            {
                PlaneA.CachedObject.Draw(context.Canvas, showA.Left, showA.Top, null);
            }

            // B

            if (PlaneB.IsReady)
            {
                var showB = showA;
                showB.Offset(0, _planeHeight);

                if (ContentViewport.Pixels.IntersectsWithInclusive(showB))
                {
                    PlaneB.CachedObject.Draw(context.Canvas, showB.Left, showB.Top, null);
                }
            }
            else
            {
                PreparePlaneBIfNeeded(context, destination, scale, zoomedScale, arguments);
            }
        }


        /// <summary>
        /// Viewport scrolled
        /// </summary>
        protected virtual void OnScrolledForPlanes()
        {
            _planesCanPrepareB = true;

            if (Content is SkiaLayout layout && layout.IsTemplated
                                             && layout.MeasureItemsStrategy == MeasuringStrategy.MeasureVisible
                                             && layout.LastMeasuredIndex < layout.ItemsSource.Count)
            {
                var measuredEnd = layout.GetMeasuredContentEnd();

                double currentOffset = Orientation == ScrollOrientation.Vertical
                    ? -ViewportOffsetY
                    : -ViewportOffsetX;

                if (measuredEnd - currentOffset < 0)
                {
                    TriggerIncrementalMeasurement(layout);
                }

            }
        }

        protected void PreparePlaneBIfNeeded(
            SkiaDrawingContext context, SKRect destination, float scale, float zoomedScale, object arguments)
        {
            if (_planesWorkingInBackground || !_planesCanPrepareB || PlaneB == null || PlaneB.IsReady
                || ViewportOffsetY == 0)
                return;

            Debug.WriteLine("Preparing PLANE B..");

            var source = PlaneA.Source;
            if (Orientation == ScrollOrientation.Vertical)
            {
                source.Offset(0, -_planeHeight);
            }
            else if (Orientation == ScrollOrientation.Horizontal)
            {
                source.Offset(-_planeWidth, 0);
            }

            TriggerPreparePlaneB(context, source, scale, zoomedScale, arguments);

            _planesCanPrepareB = false;
        }


        private bool _planesCanPrepareB;

#if !TMP

        public bool UsePlanes
        {
            get
            {
                return Content != null && Content.IsTemplated && Orientation != ScrollOrientation.Both;
            }
        }

#else

        public bool UsePlanes
        {
            get
            {
                return false;
            }
        }

#endif
    }


 


}
