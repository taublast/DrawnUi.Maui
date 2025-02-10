#define TMP

namespace DrawnUi.Maui.Draw
{

    public class Plane
    {
        public float OffsetY;
        public float OffsetX;

        public SKColor BackgroundColor { get; set; } = SKColors.Transparent;
        public RenderObject RenderObject { get; set; }
        public SKRect Source { get; set; }
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
            Source = source;
            Invalidate();
        }

        public void Invalidate()
        {
            IsReady = false;
            LastDrawnAt = SKRect.Empty;
        }
    }

    public partial class SkiaScroll
    {
        protected Plane PlaneCurrent { get; set; }
        protected Plane PlaneForward { get; set; }
        protected Plane PlaneBackward { get; set; }
        protected int _planeWidth;
        protected int _planeHeight;
        protected int _planePrepareThreshold;

        // create 2 planes for rendering inside scroll. one plane is around 2 sizes of viewport.
        // when scrolling we will draw 2 objects with scroll offset max:plane A and plane B.
        // at start we draw children on plane A then we prepare plane B in background.
        // when scrolling we drawn the visible part of plane A nd maybe the plane B if itis around to appear.
        // when plane A is out of view we move it to the end of plane B and prepare it for next drawing.

        /// <summary>
        /// Prepare the plane for virtual scroll
        /// </summary>
        /// <param name="context"></param>
        /// <param name="plane"></param>
        /// <param name="destination"></param>
        /// <param name="scale"></param>
        /// <param name="zoomedScale"></param>
        /// <param name="arguments"></param>
        protected virtual void PreparePlane(
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


        protected float PlanesPreviousScrollOffset;

        public virtual void InitializePlanes()
        {
            PlanesPreviousScrollOffset = 0;

            var viewportWidth = Viewport.Pixels.Width;
            var viewportHeight = Viewport.Pixels.Height;

            // Ensure the planes cover twice the viewport area
            _planeWidth = (int)(viewportWidth); //for vertical, todo all orientations
            _planeHeight = (int)(viewportHeight * 2);
            _planePrepareThreshold = (int)(_planeHeight / 2);

            PlaneCurrent = new Plane
            {
                Id="Current",
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                BackgroundColor = SKColors.Coral,
            };

            PlaneForward = new Plane
            {
                Id = "Forward",
                OffsetY = _planeHeight,
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                BackgroundColor = SKColors.DarkKhaki,
            };

            PlaneBackward = new Plane
            {
                Id = "Backward",
                OffsetY = -_planeHeight,
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                BackgroundColor = SKColors.DarkCyan,
            };

        }

        //protected float _baseOffsetY;
        protected bool _planesInverted;
        protected void SwapPlanes()
        {
            // Swap the planes and mark the new PlaneB as not ready.
            (PlaneCurrent, PlaneForward) = (PlaneForward, PlaneCurrent);
            if (_planesInverted)
            {
                PlaneCurrent.Invalidate();
            }
            else
            {
                PlaneForward.Invalidate();
            }
            _planesInverted = !_planesInverted;
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
            return ScaledRect.FromPixels(PlaneCurrent.Destination, RenderingScale);
        }

        public override void UpdateByChild(SkiaControl control)
        {
            base.UpdateByChild(control);

            //todo somehow detect which plane is invalidated upon child on it

            PlaneCurrent?.Invalidate();
            PlaneForward?.Invalidate();
        }

        private int visibleAreaCaller = 0;
        protected bool _buildingPlaneB;
        protected bool _buildingPlaneC;
        private bool _availablePlaneC;
        private bool _availablePlaneB;
        private SemaphoreSlim _lockPlanesWorker = new(1);

        private readonly Dictionary<string, PlaneBuildState> _planeBuildStates
            = new Dictionary<string, PlaneBuildState>
            {
                { "Current",  new PlaneBuildState() },
                { "Forward",  new PlaneBuildState() },
                { "Backward", new PlaneBuildState() }
            };

        private class PlaneBuildState
        {
            public bool IsBuilding;                
            public CancellationTokenSource Cts;    
        }

        protected void TriggerPreparePlane(
            string planeId,
            SkiaDrawingContext context,
            SKRect destination,
            float scale,
            float zoomedScale,
            object arguments)
        {
            if (!_planeBuildStates.TryGetValue(planeId, out var state))
            {
                Debug.WriteLine($"Unknown planeId: {planeId}");
                return;
            }

            // If this plane is already building, cancel the previous job
            if (state.IsBuilding && state.Cts != null)
            {
                state.Cts.Cancel();  // signal old task to stop
            }

            // Create a fresh CTS and mark building
            state.Cts = new CancellationTokenSource();
            state.IsBuilding = true;
            var token = state.Cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await _planeLocks[planeId].WaitAsync(token);

                    if (token.IsCancellationRequested)
                        return; // canceled before starting

                    // Now do the actual PreparePlane
                    var plane = GetPlaneById(planeId);
                    Debug.WriteLine($"Run prepare plane {plane?.Id}");

                    PreparePlane(context, plane, destination, scale, zoomedScale, arguments);
                }
                catch (OperationCanceledException)
                {
                    // Normal if we got canceled
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error building plane {planeId}: {ex}");
                }
                finally
                {
                    state.IsBuilding = false;
                    _planeLocks[planeId].Release();
                }
            }, token).ConfigureAwait(false);
        }





        protected virtual Plane GetPlaneById(string planeId)
        {
            return planeId switch
            {
                "Forward" => PlaneForward,
                "Backward" => PlaneBackward,
                "Current" => PlaneCurrent,
                _ => throw new ArgumentException("Invalid plane ID", nameof(planeId))
            };
        }

        private readonly Dictionary<string, SemaphoreSlim> _planeLocks
            = new Dictionary<string, SemaphoreSlim>
            {
                { "Current",  new SemaphoreSlim(1,1) },
                { "Forward",  new SemaphoreSlim(1,1) },
                { "Backward", new SemaphoreSlim(1,1) }
            };


        protected void TriggerPreparePlaneC(
            SkiaDrawingContext context,
            SKRect destination,
            float scale, float zoomedScale,
            object arguments)
        {
            if (_buildingPlaneC)
                return;

            _buildingPlaneC = true;

            async Task DoMeasure()
            {
                await _lockPlanesWorker.WaitAsync();

                Debug.WriteLine($"Run repare {PlaneForward.Id}");
                PreparePlane(context, PlaneBackward, destination, scale, zoomedScale, arguments);

                _lockPlanesWorker.Release();

                _buildingPlaneC = false;
            }

            Task.Run(DoMeasure).ConfigureAwait(false);
        }


        /// <summary>
        /// Viewport scrolled
        /// </summary>
        protected virtual void OnScrolledForPlanes()
        {
            _availablePlaneB = true;

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

        protected void OrderToPreparePlaneForwardInBackground(
            SkiaDrawingContext context, SKRect destination, float scale, float zoomedScale, object arguments)
        {
            if (_planeBuildStates["Forward"].IsBuilding
                //|| !_availablePlaneB
                || PlaneForward == null
                || PlaneForward.IsReady
                || ViewportOffsetY == 0)
            {
                return;
            }

            Debug.WriteLine("Preparing PLANE B..");

            var source = PlaneCurrent.Source;
            if (Orientation == ScrollOrientation.Vertical)
            {
                source.Offset(0, -_planeHeight);
            }
            else if (Orientation == ScrollOrientation.Horizontal)
            {
                source.Offset(-_planeWidth, 0);
            }

            TriggerPreparePlane("Forward", context, destination, scale, zoomedScale, arguments);

            //_availablePlaneB = false;
        }

        protected void OrderToPreparePlaneBackwardInBackground(
            SkiaDrawingContext context, SKRect destination, float scale, float zoomedScale, object arguments)
        {
            if (_planeBuildStates["Backward"].IsBuilding        
                //|| !_availablePlaneB                          
                || PlaneBackward == null
                || PlaneBackward.IsReady
                || ViewportOffsetY >= 0)
            {
                return;
            }
 
            Debug.WriteLine("Preparing PLANE C..");

            var source = PlaneCurrent.Source;
            if (Orientation == ScrollOrientation.Vertical)
            {
                source.Offset(0, -_planeHeight);
            }
            else if (Orientation == ScrollOrientation.Horizontal)
            {
                source.Offset(-_planeWidth, 0);
            }

            TriggerPreparePlane("Backward", context, destination, scale, zoomedScale, arguments);

            //_availablePlaneC = false;
        }



#if !TMP

        public virtual bool UsePlanes
        {
            get
            {
                return Content != null && Content.IsTemplated && Orientation != ScrollOrientation.Both;
            }
        }

#else

        public virtual bool UsePlanes
        {
            get
            {
                return false;
            }
        }

#endif


        public virtual void DrawPlanes(
            SkiaDrawingContext context,
            SKRect destination, float scale, float zoomedScale,
            object arguments)
        {
            if (Content == null)
            {
                return;
            }

            if (PlaneCurrent == null || PlaneForward == null)
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
                if (PlaneCurrent == null || PlaneForward == null)
                {
                    Super.Log("Failed to create planes");
                    return;
                }
            }

            var displayRectA = new SKRect(
                ContentRectWithOffset.Pixels.Left,
                ContentRectWithOffset.Pixels.Top,
                ContentRectWithOffset.Pixels.Left + _planeWidth,
                ContentRectWithOffset.Pixels.Top + _planeHeight
            );

            if (ScrollingDirection == LinearDirectionType.Forward && PlaneCurrent.Destination.Bottom + InternalViewportOffset.Pixels.Y <= ContentViewport.Pixels.Top)
            {
                SwapPlanes();
            }
            // Similarly for scrolling up:
            else
            if (ScrollingDirection == LinearDirectionType.Backward && PlaneCurrent.Destination.Top + InternalViewportOffset.Pixels.Y >= ContentViewport.Pixels.Bottom)
            {
                SwapPlanes();
            }


            if (!PlaneCurrent.IsReady)
            {
                Debug.WriteLine("Preparing PLANE A..");
                PreparePlane(context, PlaneCurrent, displayRectA, scale, zoomedScale, arguments);
            }

            // Draw the planes

            // A

            var showA = PlaneCurrent.Source;
            showA.Offset(0, InternalViewportOffset.Pixels.Y);

            if (ContentViewport.Pixels.IntersectsWithInclusive(showA))
            {
                PlaneCurrent.CachedObject.Draw(context.Canvas, showA.Left, showA.Top, null);
            }

            // B

            if (PlaneForward.IsReady)
            {
                var showB = showA;
                showB.Offset(0, _planeHeight);

                if (ContentViewport.Pixels.IntersectsWithInclusive(showB))
                {
                    PlaneForward.CachedObject.Draw(context.Canvas, showB.Left, showB.Top, null);
                }
            }
            else
            {
                OrderToPreparePlaneForwardInBackground(context, destination, scale, zoomedScale, arguments);
            }
        }

        
        protected virtual void PaintOnPlane(Plane plane, SKRect destination, float scale, float zoomedScale, object arguments,
            SkiaDrawingContext recordingContext)
        {
            PaintViews(recordingContext, destination, destination, scale, zoomedScale, arguments);
        }

    }
}
