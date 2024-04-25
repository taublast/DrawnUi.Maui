using System.Runtime.CompilerServices;

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
        public virtual ScaledSize MeasureStackPanel(SKRect rectForChildrenPixels, float scale)
        {
            var layout = new BuildColumnLayout(this);

            var measuredLayout = layout.Build(rectForChildrenPixels, scale);
            return measuredLayout;
        }


        /// <summary>
        /// Renders stack layout.
        /// Returns number of drawn children.
        /// </summary>
        protected virtual int DrawStack(
            LayoutStructure structure,
            SkiaDrawingContext context, SKRect destination, float scale)
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

                var visibleArea = GetOnScreenVisibleArea();

                //PASS 1 - VISIBILITY
                //we need this pass before drawing to recycle views that became hidden
                var viewsTotal = 0;
                foreach (var cell in structure.GetChildren())
                {
                    viewsTotal++;
                    viewsTotal++;
                    if (cell.Destination == SKRect.Empty)
                    {
                        cell.IsVisible = false;
                    }
                    else
                    {
                        var x = destination.Left + cell.Destination.Left;
                        var y = destination.Top + cell.Destination.Top;

                        cell.Drawn.Set(x, y, x + cell.Destination.Width, y + cell.Destination.Height);

                        if (Virtualisation != VirtualisationType.Disabled)
                        {
                            if (needrebuild && UseCache == SkiaCacheType.None &&
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

                //PASS 2 DRAW VISIBLE
                //using precalculated rects
                bool wasVisible = false;
                var index = -1;
                //SkiaControl[] nonTemplated = null;

                int countRendered = 0;

                //todo modify structure upon ZIndex or something... do not order on every frame!!!!!
                foreach (var cell in visibleElements)//.OrderBy(x => x.ZIndex))
                {
                    index++;

                    SkiaControl child = null;
                    if (IsTemplated)
                    {
                        if (!ChildrenFactory.TemplatesAvailable && InitializeTemplatesInBackgroundDelay > 0)
                        {
                            break; //itemssource was changed by other thread
                        }
                        child = ChildrenFactory.GetChildAt(cell.ControlIndex, null);
                    }
                    else
                    {
                        child = cell.View;
                    }

                    if (child == null) //ChildrenFactory.GetChildAt was unable to return child?..
                    {
                        //NeedMeasure = true;
                        return drawn;
                    }

                    if (child is SkiaControl control && child.IsVisible)
                    {
                        SKRect destinationRect;
                        if (IsTemplated && ItemSizingStrategy == ItemSizingStrategy.MeasureAllItems)
                        {
                            //when context changes we need all available space for remeasuring cell
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top, cell.Drawn.Left + cell.Area.Width, cell.Drawn.Top + cell.Area.Bottom);
                        }
                        else
                        {
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top, cell.Drawn.Right, cell.Drawn.Bottom);
                        }

                        //fixes case we changed size of columns/cells and there where already measured..
                        /*
                        if (IsTemplated
                            && (DynamicColumns || ItemSizingStrategy == Microsoft.Maui.Controls.ItemSizingStrategy.MeasureAllItems)
                            && RecyclingTemplate == RecyclingTemplate.Enabled
                            && child.RenderedAtDestination != SKRect.Empty
                            && (destinationRect.Width != child.RenderedAtDestination.Width
                                || destinationRect.Height != child.RenderedAtDestination.Height))
                        {
                            //size is different but template is the same - need to invalidate!
                            //for example same template rendering on 2 columns in one row and 1 column on the last one
                            InvalidateChildrenTree(control);
                        }
                        */

                        if (IsRenderingWithComposition)
                        {
                            if (DirtyChildrenInternal.Contains(child))
                            {
                                DrawChild(context, destinationRect, child, scale);
                                countRendered++;
                            }
                            else
                            {
                                //skip drawing but need arrange :(
                                //todo set virtual offset between drawnrect and the new
                                child.Arrange(destinationRect, child.SizeRequest.Width, child.SizeRequest.Height, scale);
                            }
                        }
                        else
                        {
                            DrawChild(context, destinationRect, child, scale);
                            countRendered++;
                        }

                        drawn++;

                        //gonna use that for gestures and for item inside viewport detection and for hotreload children tree
                        tree.Add(new SkiaControlWithRect(control, destinationRect, index));
                    }
                }

                //if (Tag == "Controls")
                //{
                //    Debug.WriteLine($"[?] {countRendered}");
                //}
            }

            //_stopwatchRender.Restart();

            if (needrebuild && visibleElements.Count > 0)
            {
                //reserve for one row above and one row below
                var row = Split;
                if (row < 1)
                    row = 1;
                var reserve = row * 3;
                if (IsTemplated
                    && RecyclingTemplate == RecyclingTemplate.Enabled
                    && ChildrenFactory.AddedMore < reserve)
                {
                    //basically we have to do this here becase now we know the quantity
                    //of visible cells onscreen. so we can oversize the pool a bit to avoid
                    //a lag spike when scrolling would start.
                    Task.Run(async () =>
                    {

                        ChildrenFactory.AddMoreToPool(reserve);

                    }).ConfigureAwait(false);
                }

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


