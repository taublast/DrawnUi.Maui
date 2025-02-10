
using Microsoft.Maui.Controls;

namespace DrawnUi.Maui.Draw
{
    /// <summary>
    /// Provides the ability to create/draw views directly while scrolling.
    /// Content will be generated dynamically, instead of the usual way.
    /// Override `GetMeasuredView` to provide views upon passed index.
    /// TODO: for horizonal
    /// </summary>
    public class VirtualScroll : SkiaScroll
    {
        private float swappedDownAt;
        private float swappedUpAt;
        protected Dictionary<Plane, List<ViewLayoutInfo>> _planeLayoutData = new();

        /// <summary>
        /// Holds layout information for a rendered month cell.
        /// </summary>
        public struct ViewLayoutInfo
        {
            /// <summary>
            /// Relative month index (0 for current, negative for past, positive for future).
            /// </summary>
            public int Index;

 
            public SKRect DrawingRect;
        }

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

        /// <summary>
        /// Returns a view for a specific index. Actually used for virtual scroll.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="destination"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected virtual SkiaLayout GetMeasuredView(int index, SKRect destination, float scale)
        {
            //todo
 
            return null;
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
            if (ContentViewport.Pixels.IntersectsWith(rectCurrent))
            {
                PlaneCurrent.CachedObject.Draw(context.Canvas, rectCurrent.Left, rectCurrent.Top, null);
            }
            PlaneCurrent.LastDrawnAt = rectCurrent;

            // Draw Forward
            if (PlaneForward.IsReady && ContentViewport.Pixels.IntersectsWith(rectForward))
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
                    //if (swappedDownAt != 0)
                    //    break;

                    SwapDown();
                    swappedDownAt = currentScroll;
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

                while (rectBackward.MidY > Viewport.Pixels.Height / 2)//rectBackward.MidY >= _planePrepareThreshold)
                {
                    //if (swappedUpAt != 0)
                    //    break;

                    SwapUp();
                    swappedUpAt = currentScroll;
                    swappedSomething = true;

                    // Recompute backwardTop, rectBackward after swapping
                    backwardTop = ContentRectWithOffset.Pixels.Top
                                  + InternalViewportOffset.Pixels.Y
                                  + PlaneBackward.OffsetY;

                    rectBackward = PlaneBackward.Source;
                    rectBackward.Offset(0, backwardTop - rectBackward.Top);
                    //break;
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
            recordingContext.Canvas.Clear(plane.BackgroundColor);

            var fromZero = destination;
            fromZero.Offset(-DrawingRect.Left, -DrawingRect.Top);
            PaintOnPlane(plane, fromZero, scale, zoomedScale, arguments, recordingContext);

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

        Plane? GetNeighborPlaneId(Plane plane, bool isScrollingDown)
        {
            Plane? ret = null;
            if (isScrollingDown)
            {
                ret = _planeLayoutData.Keys
                    .Where(x => x.IsReady && x.OffsetY < plane.OffsetY)
                    .OrderByDescending(x => x.OffsetY)
                    .FirstOrDefault();
            }
            else
            {
                ret = _planeLayoutData.Keys
                    .Where(x => x.IsReady && x.OffsetY > plane.OffsetY)
                    .OrderBy(x => x.OffsetY)
                    .FirstOrDefault();
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousPlaneLayout"></param>
        /// <param name="planeHeight"></param>
        /// <param name="isScrollingDown"></param>
        /// <returns></returns>
        (int startingMonthIndex, float startingOffset) GetNextPlaneStartingLayout(IList<ViewLayoutInfo> previousPlaneLayout, float planeHeight, bool isScrollingDown)
        {
            if (previousPlaneLayout == null || previousPlaneLayout.Count == 0)
            {
                return (0, 0);
            }

            if (isScrollingDown)
            {
                var lastInfo = previousPlaneLayout.Last();
                float totalContentHeight = lastInfo.DrawingRect.Top + lastInfo.DrawingRect.Height;
                float overflow = totalContentHeight - planeHeight;
                overflow = overflow < 0 ? 0 : overflow;
                if (overflow > 0)
                {
                    return (lastInfo.Index, -lastInfo.DrawingRect.Height + overflow);
                }
                int startingMonthIndex = lastInfo.Index + 1;
                return (startingMonthIndex, overflow);
            }
            else
            {
                var firstInfo = previousPlaneLayout.First();
                float underflow = -firstInfo.DrawingRect.Top; 
                underflow = underflow < 0 ? 0 : underflow;
                if (underflow > 0)
                {
                    return (firstInfo.Index, firstInfo.DrawingRect.Height - underflow);
                }
                int startingMonthIndex = firstInfo.Index - 1;
                return (startingMonthIndex, -underflow);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalOffset"></param>
        /// <param name="top"></param>
        /// <param name="spacingPixels"></param>
        /// <param name="destination"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        (int, float) ComputeDefaultOffset(
            float globalOffset,
            float top,
            float spacingPixels,
            SKRect destination,
            float scale)
        {
            var tempDefaultMonthView = GetMeasuredView(0, destination, scale);
            float defaultCellHeight = tempDefaultMonthView.MeasuredSize.Pixels.Height;
            float estimatedCellTotalHeight = defaultCellHeight + spacingPixels;
            int firstMonthIndex = (int)Math.Floor(globalOffset / estimatedCellTotalHeight);
            float offsetWithinCell = globalOffset - (firstMonthIndex * estimatedCellTotalHeight);
            float startingOffset = top - offsetWithinCell;

            return (firstMonthIndex, startingOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="destination"></param>
        /// <param name="scale"></param>
        /// <param name="zoomedScale"></param>
        /// <param name="arguments"></param>
        /// <param name="recordingContext"></param>
        protected override void PaintOnPlane(
            Plane plane,
            SKRect destination,
            float scale,
            float zoomedScale,
            object arguments,
            SkiaDrawingContext recordingContext)
        {
            bool isScrollingDown = (this.ScrollingDirection != LinearDirectionType.Backward);

            float planeTop = destination.Top;
            float planeHeight = _planeHeight; 
            float spacingPixels = 0f;          // vertical spacing between months

            float globalOffset = InternalViewportOffset.Pixels.Y;

            int firstMonthIndex;
            float startingOffset;

            //glue to neighbor
            var neighbor = GetNeighborPlaneId(plane, isScrollingDown);
            if (neighbor != null && _planeLayoutData.TryGetValue(neighbor, out var previousLayout)
                && previousLayout.Count > 0)
            {
                (firstMonthIndex, startingOffset)
                    = GetNextPlaneStartingLayout(previousLayout, planeHeight, isScrollingDown);
            }
            else
            {
                // no neighbor data
                (firstMonthIndex, startingOffset)
                    = ComputeDefaultOffset(globalOffset, planeTop, spacingPixels, destination, scale);
            }

            // will draw with direction: +1 for down, -1 for up
            int direction = isScrollingDown ? +1 : -1;
            float currentPos = isScrollingDown
                ? (planeTop + startingOffset)
                : (planeTop + planeHeight + startingOffset);

            int monthIndex = firstMonthIndex;
            var layoutInfoList = new List<ViewLayoutInfo>();

            while (true)
            {
                bool outOfPlane = isScrollingDown
                    ? (currentPos >= (planeTop + planeHeight))
                    : (currentPos <= planeTop);

                if (outOfPlane)
                    break; // we've filled this plane


                var monthView = GetMeasuredView(monthIndex, destination, scale);
                float cellHeight = monthView.MeasuredSize.Pixels.Height;

                // C) Calculate rect
                SKRect rect;
                if (isScrollingDown)
                {
                    // top->bottom
                    rect = new SKRect(
                        destination.Left,
                        currentPos,
                        destination.Right,
                        currentPos + cellHeight
                    );

                    // Move downward
                    currentPos += (cellHeight + spacingPixels);
                }
                else
                {
                    // bottom->top
                    float cellTop = currentPos - cellHeight;
                    rect = new SKRect(
                        destination.Left,
                        cellTop,
                        destination.Right,
                        currentPos
                    );

                    // Move upward
                    currentPos -= (cellHeight + spacingPixels);
                }

                //rect.Offset(DrawingRect.Location);

                monthView.Arrange(rect, monthView.SizeRequest.Width, monthView.SizeRequest.Height, scale);
                monthView.Render(recordingContext, rect, scale);

                var info = new ViewLayoutInfo
                {
                    Index = monthIndex,
                    DrawingRect = rect,
                };

                if (isScrollingDown)
                {
                    layoutInfoList.Add(info);
                }
                else
                {
                    layoutInfoList.Insert(0, info); // is drawing bottom->top
                }

                monthIndex += direction;
            }

            // 5) Save the layout data for continuity
            _planeLayoutData[plane] = layoutInfoList;

        }


    }
}
