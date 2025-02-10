
namespace DrawnUi.Maui.Draw
{
    public class VirtualScroll : SkiaScroll
    {
        private float swappedDownAt;
        private float swappedUpAt;

        public VirtualScroll()
        {
            Content = new SkiaControl() // simulated
            {
                BackgroundColor = Colors.Red
            };
        }

        public override bool UsePlanes
        {
            get
            {
                return true && Orientation != ScrollOrientation.Both;
            }
        }

        protected override bool IsContentActive
        {
            get
            {
                return true;
            }
        }

        protected override void SetDetectIndexChildPoint(RelativePositionType option = RelativePositionType.Start)
        {
            return;
        }

        protected override void OnMeasured()
        {
            if (Orientation == ScrollOrientation.Vertical)
            {
                ContentSize = ScaledSize.FromPixels(new(MeasuredSize.Pixels.Width, 10000), MeasuredSize.Scale);
            }
            else if (Orientation == ScrollOrientation.Horizontal)
            {
                ContentSize = ScaledSize.FromPixels(new(10000, MeasuredSize.Pixels.Height), MeasuredSize.Scale);
            }

            base.OnMeasured();
        }
 

        protected override bool PositionViewport(SKRect destination, SKPoint offsetPixels, float viewportScale, float scale)
        {
            if (!IsSnapping)
                Snapped = false;

            var isScroling = _animatorFlingY.IsRunning || _animatorFlingX.IsRunning ||
                             _vectorAnimatorBounceY.IsRunning || _vectorAnimatorBounceX.IsRunning
                             || _scrollerX.IsRunning || _scrollerY.IsRunning || IsUserPanning;



            //todo do i need this here?
            ContentAvailableSpace = GetContentAvailableRect(destination);

            InternalViewportOffset = ScaledPoint.FromPixels(offsetPixels.X, offsetPixels.Y, scale); //removed pixel rounding

            var childRect = ContentAvailableSpace;
            childRect.Offset(InternalViewportOffset.Pixels.X, InternalViewportOffset.Pixels.Y);

            ContentRectWithOffset = ScaledRect.FromPixels(childRect, scale);

            //Debug.WriteLine($"VirtualScroll at {offsetPixels.Y}");

            //okay... hmm
            ContentViewport = ScaledRect.FromPixels(DrawingRect, scale);

            //POST EVENTS

            OverscrollDistance = CalculateOverscrollDistance(InternalViewportOffset.Units.X, InternalViewportOffset.Units.Y);

            SendScrolled();

            if (isScroling)
            {
                IsScrolling = true;
            }
            else
            {
                if (IsScrolling)
                {
                    SendScrollingEnded();
                }
                IsScrolling = false;
            }

            return true;
        }

        public override void DrawPlanes(
        SkiaDrawingContext context,
        SKRect destination,
        float scale,
        float zoomedScale,
        object arguments)
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
                PreparePlane(context, PlaneCurrent, displayRectA, scale, zoomedScale, arguments);
            }

            // Draw the planes
            var currentScroll = InternalViewportOffset.Pixels.Y;
            var offsetChanged = currentScroll - PlanesPreviousScrollOffset;
            PlanesPreviousScrollOffset = currentScroll;

            var baseTop = ContentRectWithOffset.Pixels.Top;
            var currentTop = baseTop + currentScroll + PlaneCurrent.OffsetY;
            var forwardTop = baseTop + currentScroll + PlaneForward.OffsetY;
            var backwardTop = baseTop + currentScroll + PlaneBackward.OffsetY;

            var rectCurrent = new SKRect(PlaneCurrent.Source.Left, PlaneCurrent.Source.Top,
                PlaneCurrent.Source.Left+ _planeWidth, PlaneCurrent.Source.Top + _planeHeight);

            var rectForward = new SKRect(PlaneForward.Source.Left, PlaneForward.Source.Top,
                PlaneForward.Source.Left + _planeWidth, PlaneForward.Source.Top + _planeHeight);

            var rectBackward = new SKRect(PlaneBackward.Source.Left, PlaneBackward.Source.Top,
                PlaneBackward.Source.Left + _planeWidth, PlaneBackward.Source.Top + _planeHeight);

            // Apply vertical offsets
            rectCurrent.Offset(0, currentTop - rectCurrent.Top);
            rectForward.Offset(0, forwardTop - rectForward.Top);
            rectBackward.Offset(0, backwardTop - rectBackward.Top);

            // Draw Backward
            if (PlaneBackward.IsReady && ContentViewport.Pixels.IntersectsWithInclusive(rectBackward))
            {
                PlaneBackward.CachedObject.Draw(context.Canvas, rectBackward.Left, rectBackward.Top, null);
            }
            else
            {
                OrderToPreparePlaneBackwardInBackground(context, destination, scale, zoomedScale, arguments);
                PlaneBackward.CachedObject?.Draw(context.Canvas, rectBackward.Left, rectBackward.Top, null);
            }
            PlaneBackward.LastDrawnAt = rectBackward;

            // Draw Current
            if (ContentViewport.Pixels.IntersectsWithInclusive(rectCurrent))
            {
                PlaneCurrent.CachedObject.Draw(context.Canvas, rectCurrent.Left, rectCurrent.Top, null);
            }
            PlaneCurrent.LastDrawnAt = rectCurrent;

            // Draw Forward
            if (PlaneForward.IsReady && ContentViewport.Pixels.IntersectsWithInclusive(rectForward))
            {
                PlaneForward.CachedObject.Draw(context.Canvas, rectForward.Left, rectForward.Top, null);
            }
            else
            {
                OrderToPreparePlaneForwardInBackground(context, destination, scale, zoomedScale, arguments);
                PlaneForward.CachedObject?.Draw(context.Canvas, rectForward.Left, rectForward.Top, null); //repeat last image for fast scrolling
            }
            PlaneForward.LastDrawnAt = rectForward;

            // --------------------------------------------------------------------
            // Multiple-swap logic to handle fast scrolling
            // --------------------------------------------------------------------

            while (true)
            {
                bool swappedSomething = false;

                // ------------------------------------------------------
                // A) Recompute FORWARD plane’s positions, then swap down
                //    as many times as needed
                // ------------------------------------------------------
                forwardTop = ContentRectWithOffset.Pixels.Top
                             + InternalViewportOffset.Pixels.Y
                             + PlaneForward.OffsetY;

                rectForward = PlaneForward.Source;
                rectForward.Offset(0, forwardTop - rectForward.Top);

                while (rectForward.MidY <= (Viewport.Pixels.Height / 2))
                {
                    SwapDown();
                    swappedSomething = true;

                    // Recompute forwardTop, rectForward after swapping
                    forwardTop = ContentRectWithOffset.Pixels.Top
                                 + InternalViewportOffset.Pixels.Y
                                 + PlaneForward.OffsetY;

                    rectForward = PlaneForward.Source;
                    rectForward.Offset(0, forwardTop - rectForward.Top);
                }

                // ------------------------------------------------------
                // B) Recompute BACKWARD plane’s positions, then swap up
                //    as many times as needed
                // ------------------------------------------------------
                backwardTop = ContentRectWithOffset.Pixels.Top
                              + InternalViewportOffset.Pixels.Y
                              + PlaneBackward.OffsetY;

                rectBackward = PlaneBackward.Source;
                rectBackward.Offset(0, backwardTop - rectBackward.Top);

                while (rectBackward.MidY >= Viewport.Pixels.Height / 2)//rectBackward.MidY >= _planePrepareThreshold)
                {
                    SwapUp();
                    swappedSomething = true;

                    // Recompute backwardTop, rectBackward after swapping
                    backwardTop = ContentRectWithOffset.Pixels.Top
                                  + InternalViewportOffset.Pixels.Y
                                  + PlaneBackward.OffsetY;

                    rectBackward = PlaneBackward.Source;
                    rectBackward.Offset(0, backwardTop - rectBackward.Top);
                }

                // If we did no swaps this iteration, no need for more loops
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

        protected override void PreparePlane(
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

            PaintOnPlane(plane, destination, scale, zoomedScale, arguments, recordingContext);

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

        protected virtual void PaintOnPlane(Plane plane, SKRect destination, float scale, float zoomedScale, object arguments,
            SkiaDrawingContext recordingContext)
        {
            PaintViews(recordingContext, destination, destination, scale, zoomedScale, arguments);
        }
    }
}
