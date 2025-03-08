using System.Runtime.InteropServices;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaLayout
    {

        /// <summary>
        /// TODO for templated measure only visible?! and just reserve predicted scroll amount for scrolling
        /// </summary>
        /// <param name="rectForChildrenPixels"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public virtual ScaledSize MeasureWrap(SKRect rectForChildrenPixels, float scale)
        {
            var layout = new BuildWrapLayout(this);

            var measuredLayout = layout.Build(rectForChildrenPixels, scale);
            return measuredLayout;
        }

        protected override void OnFirstDrawn()
        {
            if (IsTemplated && MeasureItemsStrategy == MeasuringStrategy.MeasureFirst &&
                RecyclingTemplate != RecyclingTemplate.Disabled)
            {
                //avoid lag-spike of first scrolling
                Task.Run(() =>
                {
                    ChildrenFactory.FillPool(ChildrenFactory.PoolSize + 2);
                }).ConfigureAwait(false);
            }
            base.OnFirstDrawn();
        }

        /// <summary>
        /// Renders stack/wrap layout.
        /// Returns number of drawn children.
        /// </summary>
        protected virtual int DrawStack(DrawingContext ctx, LayoutStructure structure)
        {
            var drawn = 0;
            //StackStructure was creating inside Measure.
            //While scrolling templated its not called again (checked).

            List<SkiaControlWithRect> tree = new();

            var needrebuild = templatesInvalidated;
            List<ControlInStack> visibleElements = new();

            if (structure != null)
            {
                //draw children manually
                var inflate = (float)(this.VirtualisationInflated * ctx.Scale);
                var visibleArea = GetOnScreenVisibleArea(ctx, new (inflate, inflate));

                //PASS 1 - VISIBILITY
                //we need this pass before drawing to recycle views that became hidden
                var currentIndex = -1;
                foreach (var cell in structure.GetChildrenAsSpans())
                {
                    if (!cell.WasMeasured)
                    {
                        continue;
                    }

                    currentIndex++;

                    if (cell.Destination == SKRect.Empty || cell.Measured.Pixels.IsEmpty)
                    {
                        cell.IsVisible = false;
                    }
                    else
                    {
                        var x = ctx.Destination.Left + cell.Destination.Left;
                        var y = ctx.Destination.Top + cell.Destination.Top;

                        cell.Drawn.Set(x, y, x + cell.Destination.Width, y + cell.Destination.Height);

                        if (Virtualisation != VirtualisationType.Disabled)
                        {
                            if (needrebuild && UsingCacheType == SkiaCacheType.None &&
                                Virtualisation == VirtualisationType.Smart
                                && !(IsTemplated && RecyclingTemplate == RecyclingTemplate.Enabled))
                            {
                                cell.IsVisible = true;
                            }
                            else
                            {
                                cell.IsVisible = cell.Drawn.IntersectsWith(visibleArea.Pixels);
                            }
                        }
                        else
                        {
                            cell.IsVisible = true;
                        }
                    }

                    if (!cell.IsVisible)
                    {
                        ChildrenFactory.MarkViewAsHidden(cell.ControlIndex);
                    }
                    else
                    {
                        visibleElements.Add(cell);
                    }
                }

                if (OutputDebug)
                {
                    Super.Log($"[SkiaLayout] visible area {visibleArea}, visible items: {visibleElements.Count}");
                };

                //PASS 2 DRAW VISIBLE
                //using precalculated rects
                bool wasVisible = false;
                var index = -1;
                //SkiaControl[] nonTemplated = null;

                int countRendered = 0;

                foreach (var cell in CollectionsMarshal.AsSpan(visibleElements))
                {
                    if (!cell.WasMeasured)
                    {
                        continue;
                    }

                    index++;

                    SkiaControl child = null;
                    if (IsTemplated)
                    {
                        if (!ChildrenFactory.TemplatesAvailable && InitializeTemplatesInBackgroundDelay > 0)
                        {
                            break; //itemssource was changed by other thread
                        }
                        child = ChildrenFactory.GetViewForIndex(cell.ControlIndex, null, GetSizeKey(cell.Measured.Pixels));
                        if (child == null)
                        {
                            return countRendered;
                        }
                    }
                    else
                    {
                        child = cell.View;
                    }

                    if (child is SkiaControl control && child.IsVisible)
                    {
                        if (child.NeedMeasure)
                        {
                            if (!child.WasMeasured || GetSizeKey(child.MeasuredSize.Pixels) != GetSizeKey(cell.Measured.Pixels))
                            {
                                child.Measure((float)cell.Area.Width, (float)cell.Area.Height, ctx.Scale);
                            }
                        }

                        SKRect destinationRect;
                        if (IsTemplated && RecyclingTemplate != RecyclingTemplate.Disabled)
                        {
                            //when context changes we need all available space for remeasuring cell
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top,
                                cell.Drawn.Left + cell.Area.Width, cell.Drawn.Top + cell.Area.Bottom);
                        }
                        else
                        {
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top, cell.Drawn.Right,
                                cell.Drawn.Bottom);
                        }

                        if (IsRenderingWithComposition)
                        {
                            if (DirtyChildrenInternal.Contains(child))
                            {
                                DrawChild(ctx.WithDestination(destinationRect), child);
                                countRendered++;
                            }
                            else
                            {
                                //skip drawing but need arrange :(
                                //todo set virtual offset between drawnrect and the new
                                child.Arrange(destinationRect, child.SizeRequest.Width, child.SizeRequest.Height, ctx.Scale);
                            }
                        }
                        else
                        {
                            DrawChild(ctx.WithDestination(destinationRect), child);
                            countRendered++;
                        }

                        drawn++;

                        //gonna use that for gestures and for item inside viewport detection and for hotreload children tree
                        tree.Add(new SkiaControlWithRect(control,
                            destinationRect,
                            control.LastDrawnAt,
                            index));
                    }
                }

            }

            //_stopwatchRender.Restart();

            if (needrebuild && visibleElements.Count > 0)
            {
                templatesInvalidated = false;
            }

            RenderTree = tree;
            _builtRenderTreeStamp = _measuredStamp;

            if (Parent is IDefinesViewport viewport &&
                viewport.TrackIndexPosition != RelativePositionType.None)
            {
                viewport.UpdateVisibleIndex();
            }

            OnPropertyChanged(nameof(DebugString));

            //if (IsTemplated)
            //{
            //    Trace.WriteLine(ChildrenFactory.GetDebugInfo());
            //}

            return drawn;
        }
    }
}


