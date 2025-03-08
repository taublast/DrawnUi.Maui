#define TMP

using System.Collections.Immutable;
using System.Numerics;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaScroll
    {
        //todo complete and move

        public override void UpdateByChild(SkiaControl control)
        {
            if (UseVirtual)
            {
                //todo somehow detect which plane is invalidated upon child on it
                return;

                PlaneCurrent?.Invalidate();
                PlaneForward?.Invalidate();
            }

            base.UpdateByChild(control);
        }

        //todo use when context size is bigger than 2 viewports?
        public virtual bool UseVirtual
        {
            get
            {
                return Content != null
                       && Orientation != ScrollOrientation.Both && Content is SkiaLayout layout && layout.Virtualisation == VirtualisationType.Managed;
            }
        }


        protected Plane PlaneCurrent { get; set; }
        protected Plane PlaneForward { get; set; }
        protected Plane PlaneBackward { get; set; }
        protected int _planeWidth;
        protected int _planeHeight;
        protected int _planePrepareThreshold;
        private float swappedDownAt;
        private float swappedUpAt;

        public override ScaledRect GetOnScreenVisibleArea(DrawingContext context, Vector2 inflateByPixels = default)
        {

            if (UseVirtual)
            {
                //todo
                if (context.GetArgument(ContextArguments.Viewport.ToString()) is SKRect insideViewport)
                {
                    //yay!!!!!
                    //we can return the plane rect!!!!
                    Debug.WriteLine($"UsePlanes area: {insideViewport}");

                    return ScaledRect.FromPixels(insideViewport, _zoomedScale);
                }

                return ScaledRect.FromPixels(context.Destination, _zoomedScale);
            }

            if (Virtualisation != VirtualisationType.Disabled) //true by default
            {
                //passing visible area to be rendered
                //when scrolling we will pass changed area to be rendered
                //most suitable for large content
                var inflated = ContentViewport.Pixels;
                inflated.Inflate(inflateByPixels.X, inflateByPixels.Y);
                return ScaledRect.FromPixels(inflated, RenderingScale);
            }
            else
            {
                //passing the whole area to be rendered.
                //when scrolling we will just translate it
                //most suitable for small content
                return ContentRectWithOffset;

                //absoluteViewPort = new SKRect(Viewport.Pixels.Left, Viewport.Pixels.Top,
                //    Viewport.Pixels.Left + ContentSize.Pixels.Width, Viewport.Pixels.Top + ContentSize.Pixels.Height);
            }
        }


        public virtual void InitializePlanes()
        {
            var viewportWidth = Viewport.Pixels.Width;
            var viewportHeight = Viewport.Pixels.Height;

            // Ensure the planes cover twice the viewport area
            _planeWidth = (int)(viewportWidth); //for vertical, todo all orientations
            _planeHeight = (int)(viewportHeight * 2);
            _planePrepareThreshold = (int)(_planeHeight / 2);

            float offsetX = 0, offsetY = 0;

            if (Orientation == ScrollOrientation.Vertical)
            {
                offsetY = _planeHeight;
            }
            else if (Orientation == ScrollOrientation.Horizontal)
            {
                offsetX = _planeWidth;
            }

            PlaneCurrent = new Plane
            {
                Id="Current",
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                BackgroundColor = SKColors.Coral,
                Destination = new (0,0,_planeWidth, _planeHeight)
            };

            PlaneForward = new Plane
            {
                Id = "Forward",
                OffsetX = offsetX,
                OffsetY = offsetY,
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                Destination = new(0, 0, _planeWidth, _planeHeight),
                BackgroundColor = SKColors.DarkKhaki,
            };

            PlaneBackward = new Plane
            {
                Id = "Backward",
                OffsetX = -offsetX,
                OffsetY = -offsetY,
                Surface = SKSurface.Create(new SKImageInfo(_planeWidth, _planeHeight)),
                Destination = new(0, 0, _planeWidth, _planeHeight),
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

        private ScaledRect ReportVisibleAreToContent(Vector2 arg)
        {
            return ScaledRect.FromPixels(PlaneCurrent.Destination, RenderingScale);
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

        protected void TriggerPreparePlane(DrawingContext context, string planeId)
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

            var clone = context; //always clone struct from arguments for another thread!
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

                    PreparePlane(clone.WithArgument(new("BThread", true)), plane);
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

        protected void OrderToPreparePlaneForwardInBackground(DrawingContext context)
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
            TriggerPreparePlane(context, "Forward");

            //_availablePlaneB = false;
        }

        protected void OrderToPreparePlaneBackwardInBackground(DrawingContext context)
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
            TriggerPreparePlane(context,"Backward");
        }

        /// <summary>
        /// This is called when scrolling changes when in UseVirtual mode, override this to draw custom content
        /// </summary>
        /// <param name="context"></param>
        public virtual void DrawVirtual(DrawingContext context)
        {
            if (PlaneCurrent == null)
            {
                InitializePlanes();
                if (PlaneCurrent == null || PlaneForward == null || PlaneBackward == null)
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

            if (!PlaneCurrent.IsReady)
            {
                Debug.WriteLine("Preparing PLANE A..");
                PreparePlane(context.WithDestination(displayRectA), PlaneCurrent);
            }

            // Draw the planes
            var currentScroll = InternalViewportOffset.Pixels.Y;

            var rectBase = new SKRect(0, 0, _planeWidth, _planeHeight);
            rectBase.Offset(DrawingRect.Left, DrawingRect.Top);

            var rectCurrent = rectBase;
            var rectForward = rectBase;
            var rectBackward = rectBase;

            // Apply vertical offsets
            rectCurrent.Offset(0, currentScroll + PlaneCurrent.OffsetY);
            rectForward.Offset(0, currentScroll + PlaneForward.OffsetY);
            rectBackward.Offset(0, currentScroll + PlaneBackward.OffsetY);

            //  if we've moved enough can re-allow a swap
            if (swappedDownAt != 0 && Math.Abs(currentScroll - swappedDownAt) > _planeHeight / 2f)
            {
                swappedDownAt = 0;
            }
            if (swappedUpAt != 0 && Math.Abs(currentScroll - swappedUpAt) > _planeHeight / 2f)
            {
                swappedUpAt = 0;
            }

            // Draw Backward
            if (PlaneBackward.IsReady)
            {
                if (ContentViewport.Pixels.IntersectsWithInclusive(rectBackward))
                {
                    PlaneBackward.CachedObject.Draw(context.Context.Canvas, rectBackward.Left, rectBackward.Top, null);
                    PlaneBackward.LastDrawnAt = rectBackward;
                }
            }
            else
            {
                OrderToPreparePlaneBackwardInBackground(context);
                PlaneBackward.CachedObject?.Draw(context.Context.Canvas, rectBackward.Left, rectBackward.Top, null);
            }

            // Draw Current
            if (ContentViewport.Pixels.IntersectsWith(rectCurrent))
            {
                PlaneCurrent.CachedObject.Draw(context.Context.Canvas, rectCurrent.Left, rectCurrent.Top, null);
                PlaneCurrent.LastDrawnAt = rectCurrent;
            }

            // Draw Forward
            if (PlaneForward.IsReady)
            {
                if (ContentViewport.Pixels.IntersectsWith(rectForward))
                {
                    PlaneForward.CachedObject.Draw(context.Context.Canvas, rectForward.Left, rectForward.Top, null);
                    PlaneForward.LastDrawnAt = rectForward;
                }
            }
            else
            {
                OrderToPreparePlaneForwardInBackground(context);
                PlaneForward.CachedObject?.Draw(context.Context.Canvas, rectForward.Left, rectForward.Top, null); //repeat last image for fast scrolling
            }

            // --------------------------------------------------------------------
            // Multiple-swap logic to handle fast scrolling
            // --------------------------------------------------------------------

            int swaps = 0;
            bool swappedSomething = false;
            while (!swappedSomething)
            {
                // ------------------------------------------------------
                // then swap down as many times as needed
                // ------------------------------------------------------
                var topDown = -1f; // break when same 
                while (topDown != rectForward.Top && rectForward.MidY <= (Viewport.Pixels.Height / 2))
                {
                    topDown = rectForward.Top;
                    //if (swappedDownAt != 0)
                    //    break;

                    SwapDown();
                    swaps++;
                    swappedDownAt = currentScroll;
                    swappedSomething = true;

                    rectForward = rectBase;
                    rectForward.Offset(0, currentScroll + PlaneForward.OffsetY);
                }

                // ------------------------------------------------------
                // swap up as many times as needed
                // ------------------------------------------------------
                var topUp = -1f;
                while (topUp != rectBackward.Top && rectBackward.MidY > Viewport.Pixels.Height / 2)
                {
                    topUp = rectBackward.Top;
                    //if (swappedUpAt != 0)
                    //    break;

                    SwapUp();
                    swaps++;
                    swappedUpAt = currentScroll;
                    swappedSomething = true;

                    rectBackward = rectBase;
                    rectBackward.Offset(0, currentScroll - rectBackward.Top);
                }

                if (!swappedSomething)
                    break;
            }


        }



        // -----------------------------------------------------------
        // SWAP LOGIC
        // -----------------------------------------------------------
        private void SwapDown()
        {
            // forward ↑ current
            // current ↑ backward
            // backward ↓ forward + invalidate
            var temp = PlaneBackward;
            PlaneBackward = PlaneCurrent;
            PlaneCurrent = PlaneForward;
            PlaneForward = temp;
            PlaneForward.OffsetY = PlaneCurrent.OffsetY + _planeHeight;
            PlaneBackward.OffsetY = PlaneCurrent.OffsetY - _planeHeight;
            PlaneForward.Invalidate();
        }

        private void SwapUp()
        {
            var temp = PlaneForward;
            PlaneForward = PlaneCurrent;
            PlaneCurrent = PlaneBackward;
            PlaneBackward = temp;
            PlaneBackward.OffsetY = PlaneCurrent.OffsetY - _planeHeight;
            PlaneForward.OffsetY = PlaneCurrent.OffsetY + _planeHeight;
            PlaneBackward.Invalidate();
        }

        protected virtual void PreparePlane(DrawingContext context, Plane plane)
        {
            var destination = plane.Destination;
           
            var recordingContext = context.CreateForRecordingImage(plane.Surface, destination.Size);

            var viewport = plane.Destination;
            viewport.Offset(InternalViewportOffset.Pixels.X, InternalViewportOffset.Pixels.Y);
            viewport.Offset(DrawingRect.Left, DrawingRect.Top);
            viewport.Offset(plane.OffsetX, plane.OffsetY);

            var c = recordingContext.Context.Canvas.Save();
            recordingContext.Context.Canvas.Translate(-viewport.Left, -viewport.Top);
            recordingContext.Context.Canvas.Clear(plane.BackgroundColor);

            PaintOnPlane(recordingContext
                .WithDestination(viewport)
                .WithArguments(
                    new(ContextArguments.Plane.ToString(), plane.Id),
                    new(ContextArguments.Viewport.ToString(), viewport)), plane);

            recordingContext.Context.Canvas.RestoreToCount(c);

            recordingContext.Context.Canvas.Flush();
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
        


        protected virtual void PaintOnPlane(DrawingContext context, Plane plane)
        {
            PaintViews(context);
        }

    }
}
