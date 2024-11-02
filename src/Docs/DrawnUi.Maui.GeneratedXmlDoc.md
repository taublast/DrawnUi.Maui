# DrawnUi.Maui #

## Type Draw.SkiaButton

 Button-like control, can include any content inside. It's either you use default content (todo templates?..) or can include any content inside, and properties will by applied by convention to a SkiaLabel with Tag `MainLabel`, SkiaShape with Tag `MainFrame`. At the same time you can override ApplyProperties() and apply them to your content yourself. 

 Button-like control, can include any content inside. It's either you use default content (todo templates?..) or can include any content inside, and properties will by applied by convention to a SkiaLabel with Tag `MainLabel`, SkiaShape with Tag `MainFrame`. At the same time you can override ApplyProperties() and apply them to your content yourself. 



---
#### Method Draw.SkiaButton.CreateClip(System.Object,System.Boolean,SkiaSharp.SKPath)

 Clip effects with rounded rect of the frame inside 

**Returns**: 



---
#### Field Draw.SkiaButton.DelayCallbackMs

 You might want to pause to show effect before executing command. Default is 0. 



---
#### Property Draw.SkiaButton.Text

 Bind to your own content! 



---
## Type Draw.SkiaMarkdownLabel

 Will internally create spans from markdown. Spans property must not be set directly. 



---
#### Method Draw.SkiaMarkdownLabel.ProcessSpanData(System.Collections.Generic.List{System.ValueTuple{System.String,SkiaSharp.SKTypeface,System.Int32,System.Boolean}}@,SkiaSharp.SKTypeface)

 Do not let spans with non-default typeface end with standart symbols like ' ', move them to span with original typecase 

|Name | Description |
|-----|------|
|spanData: ||
|originalTypeFace: ||


---
#### Method Draw.SkiaMarkdownLabel.OnSpanTapped(DrawnUi.Maui.Draw.TextSpan)

 Url will be inside Tag 

|Name | Description |
|-----|------|
|span: ||
**Returns**: 



---
## Type Draw.SkiaHoverMask

 Paints the parent view with the background color with a clipped viewport oth this view size 



---
#### Field Draw.SkiaSlider.moreHotspotSize

 enlarge hotspot by pts 



---
#### Field Draw.SkiaSlider.touchArea

 track touched area type 



---
#### Property Draw.SkiaSlider.RespondsToGestures

 Can be open/closed by gestures along with code-behind, default is true 



---
#### Property Draw.SkiaSlider.Start

 Enabled for ranged 



---
#### Property Draw.SkiaSlider.End

 For non-ranged this is your main value 



---
#### Property Draw.SkiaSlider.Orientation

Gets or sets the orientation. This is a bindable property.





---
#### Property Draw.SkiaSlider.IgnoreWrongDirection

 Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity 



---
## Type Draw.SkiaCheckbox

 Switch-like control, can include any content inside. It's aither you use default content (todo templates?..) or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag `Frame`, SkiaShape with Tag `Thumb`. At the same time you can override ApplyProperties() and apply them to your content yourself. 



---
## Type Draw.SkiaSwitch

 Switch-like control, can include any content inside. It's aither you use default content (todo templates?..) or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag `Frame`, SkiaShape with Tag `Thumb`. At the same time you can override ApplyProperties() and apply them to your content yourself. 



---
## Type Draw.SkiaToggle

 Base control for toggling between 2 states 



---
#### Method Draw.SkiaToggle.OnToggledChanged

 Base calls ApplyProperties() 



---
#### Method Draw.SkiaToggle.ApplyProperties

 Base call Update() 



---
## Type Draw.ZoomContent

 Wrapper to zoom and pan content by changing the rendering scale so not affecting quality, this is not a transform.TODO add animated movement 



---
#### Property Draw.ZoomContent.ZoomSpeed

 How much of finger movement will afect zoom change 



---
#### Field Draw.ZoomContent.LastValue

 Last ViewportZoom value we are animating from 



---
#### Property Draw.SkiaControl.UseCache

 Never reuse the rendering result. Actually true for ScrollLooped SkiaLayout viewport container to redraw its content several times for creating a looped aspect. 



---
#### Property Draw.SkiaControl.AllowCaching

 Might want to set this to False for certain cases.. 



---
#### Property Draw.SkiaControl.RenderObjectPrevious

 Used by the UseCacheDoubleBuffering process. 



---
#### Property Draw.SkiaControl.RenderObject

 The cached representation of the control. Will be used on redraws without calling Paint etc, until the control is requested to be updated. 



---
#### Property Draw.SkiaControl.CanUseCacheDoubleBuffering

 Indended to prohibit background rendering, useful for streaming controls like camera, gif etc. SkiaBackdrop has it set to True as well. 



---
#### Property Draw.SkiaControl.UsesCacheDoubleBuffering

 Read-only computed flag for internal use. 



---
#### Property Draw.SkiaControl.RenderObjectPreparing

 Used by the UseCacheDoubleBuffering process. This is the new cache beign created in background. It will be copied to RenderObject when ready. 



---
#### Property Draw.SkiaControl.NeedUpdateFrontCache

 Used by ImageDoubleBuffering cache 



---
#### Property Draw.SkiaControl.RenderObjectNeedsUpdate

 Should delete RenderObject when starting new frame rendering 



---
#### Method Draw.SkiaControl.GetCacheRecordingArea(SkiaSharp.SKRect)

 Used for the Operations cache type to record inside the changed area, if your control is not inside the DrawingRect due to transforms/translations. This is NOT changing the rendering object 



---
#### Method Draw.SkiaControl.GetCacheArea(SkiaSharp.SKRect)

 Normally cache is recorded inside DrawingRect, but you might want to exapnd this to include shadows around, for example. 



---
#### Method Draw.SkiaControl.DrawUsingRenderObject(DrawnUi.Maui.Draw.SkiaDrawingContext,System.Single,System.Single,SkiaSharp.SKRect,System.Single)

 Returns true if had drawn. 

|Name | Description |
|-----|------|
|context: ||
|widthRequest: ||
|heightRequest: ||
|destination: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaControl.CreateRenderingObjectAndPaint(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,SkiaSharp.SKRect,System.Action{DrawnUi.Maui.Draw.SkiaDrawingContext})

 This is NOT calling FinalizeDraw()! parameter 'area' Usually is equal to DrawingRect 

|Name | Description |
|-----|------|
|context: ||
|recordArea: ||
|action: ||


---
#### Method Draw.SkiaControl.InvalidateInternal

 Soft invalidation, without requiring update. So next time we try to draw this one it will recalc everything. 



---
#### Method Draw.SkiaControl.Invalidate

 Base calls InvalidateInternal and InvalidateParent 



---
#### Method Draw.SkiaControl.InvalidateByChild(DrawnUi.Maui.Draw.SkiaControl)

 To be able to fast track dirty children 

|Name | Description |
|-----|------|
|child: ||


---
#### Method Draw.SkiaControl.InvalidateViewport

 Indicated that wants to be re-measured without invalidating cache 



---
#### Method Draw.SkiaControl.Reload

 HOTRELOAD IReloadHandler 



---
#### Method Draw.SkiaControl.SetupGradient(SkiaSharp.SKPaint,DrawnUi.Maui.Draw.SkiaGradient,SkiaSharp.SKRect)

 Creates Shader for gradient and sets it to passed SKPaint along with BlendMode 

|Name | Description |
|-----|------|
|paint: ||
|gradient: ||
|destination: ||


---
#### Method Draw.SkiaControl.GetPositionOnCanvasInPoints(System.Boolean)

 Absolute position in points 

**Returns**: 



---
#### Method Draw.SkiaControl.GetFuturePositionOnCanvasInPoints(System.Boolean)

 Absolute position in points 

**Returns**: 



---
#### Method Draw.SkiaControl.GetPositionOnCanvas(System.Boolean)

 Absolute position in pixels afetr drawn. 

**Returns**: 



---
#### Method Draw.SkiaControl.GetFuturePositionOnCanvas(System.Boolean)

 Absolute position in pixels before drawn. 

**Returns**: 



---
#### Method Draw.SkiaControl.GetSelfDrawingPosition

 Find drawing position for control accounting for all caches up the rendering tree. 

**Returns**: 



---
#### Property Draw.SkiaControl.SkipRendering

 Can be set but custom controls while optimizing rendering etc. Will affect CanDraw. 



---
#### Property Draw.SkiaControl.CreateChildren

 To create custom content in code-behind. Will be called from OnLayoutChanged if Views.Count == 0. 



---
#### Method Draw.SkiaControl.CreateChildrenFromCode

 Executed when CreateChildren is set 

**Returns**: 



---
#### Method Draw.SkiaControl.GenerateParentChain

 This actually used by SkiaMauiElement but could be used by other controls. Also might be useful for debugging purposes. 

**Returns**: 



---
#### Method Draw.SkiaControl.SetVisualTransform(DrawnUi.Maui.Infrastructure.VisualTransform)

 //todo base. this is actually used by SkiaMauiElement only 

|Name | Description |
|-----|------|
|transform: ||


---
#### Method Draw.SkiaControl.CommitInvalidations

 Apply all postponed invalidation other logic that was postponed until the first draw for optimization. Use this for special code-behind cases, like tests etc, if you cannot wait until the first Draw(). In this version this affects ItemsSource only. 



---
#### Method Draw.SkiaControl.PostponeInvalidation(System.String,System.Action)

 Used for optimization process, for example, to avoid changing ItemSource several times before the first draw. 

|Name | Description |
|-----|------|
|key: ||
|action: ||


---
#### Method Draw.SkiaControl.GetRenderingScaleFor(System.Single,System.Single)

 Returns rendering scale adapted for another output size, useful for offline rendering 

|Name | Description |
|-----|------|
|width: ||
|height: ||
**Returns**: 



---
#### Method Draw.SkiaControl.AnimateAsync(System.Action{System.Double},System.Action,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)

 Creates a new animator, animates from 0 to 1 over a given time, and calls your callback with the current eased value 

|Name | Description |
|-----|------|
|callback: ||
|length: ||
|easing: ||
|cancel: ||
**Returns**: 



---
#### Method Draw.SkiaControl.FadeToAsync(System.Double,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)

 Fades the view from the current Opacity to end, animator is reused if already running 

|Name | Description |
|-----|------|
|end: ||
|length: ||
|easing: ||
|cancel: ||
**Returns**: 



---
#### Method Draw.SkiaControl.ScaleToAsync(System.Double,System.Double,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)

 Scales the view from the current Scale to x,y, animator is reused if already running 

|Name | Description |
|-----|------|
|x: ||
|y: ||
|length: ||
|easing: ||
|cancel: ||
**Returns**: 



---
#### Method Draw.SkiaControl.TranslateToAsync(System.Double,System.Double,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)

 Translates the view from the current position to x,y, animator is reused if already running 

|Name | Description |
|-----|------|
|x: ||
|y: ||
|length: ||
|easing: ||
|cancel: ||
**Returns**: 



---
#### Method Draw.SkiaControl.RotateToAsync(System.Double,System.UInt32,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)

 Rotates the view from the current rotation to end, animator is reused if already running 

|Name | Description |
|-----|------|
|end: ||
|length: ||
|easing: ||
|cancel: ||
**Returns**: 



---
#### Property Draw.SkiaControl.SizeRequest

 Is set by InvalidateMeasure(); 



---
#### Method Draw.SkiaControl.AdaptWidthConstraintToRequest(System.Single,Microsoft.Maui.Thickness,System.Double)

 Apply margins to SizeRequest 

|Name | Description |
|-----|------|
|widthConstraint: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaControl.AdaptHeightContraintToRequest(System.Single,Microsoft.Maui.Thickness,System.Double)

 Apply margins to SizeRequest 

|Name | Description |
|-----|------|
|heightConstraint: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaControl.AdaptSizeRequestToContent(System.Double,System.Double)

 In UNITS 

|Name | Description |
|-----|------|
|widthRequestPts: ||
|heightRequestPts: ||
**Returns**: 



---
#### Method Draw.SkiaControl.GetTopParentView

 Use Superview from public area 

**Returns**: 



---
#### Method Draw.SkiaControl.GestureIsInside(AppoMobi.Maui.Gestures.TouchActionEventArgs,System.Single,System.Single)

 To detect if current location is inside Destination 

|Name | Description |
|-----|------|
|args: ||
**Returns**: 



---
#### Method Draw.SkiaControl.GestureStartedInside(AppoMobi.Maui.Gestures.TouchActionEventArgs,System.Single,System.Single)

 To detect if a gesture Start point was inside Destination 

|Name | Description |
|-----|------|
|args: ||
**Returns**: 



---
#### Method Draw.SkiaControl.IsPointInside(System.Single,System.Single,System.Single)

 Whether the point is inside Destination 

|Name | Description |
|-----|------|
|x: ||
|y: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaControl.IsPixelInside(System.Single,System.Single)

 Whether the pixel is inside Destination 

|Name | Description |
|-----|------|
|x: ||
|y: ||
**Returns**: 



---
#### Property Draw.SkiaControl.LockChildrenGestures

 What gestures are allowed to be passed to children below. If set to Enabled wit, otherwise can be more specific. 



---
#### Property Draw.SkiaControl.CommandChildTapped

 Child was tapped. Will pass the tapped child as parameter. You might want then read child's BindingContext etc.. This works only if your control implements ISkiaGestureListener. 



---
#### Property Draw.SkiaControl.ClipFrom

 Use clipping area from another control 



---
#### Property Draw.SkiaControl.Parent

 Do not set this directly if you don't know what you are doing, use SetParent() 



---
#### Method Draw.SkiaControl.OnParentVisibilityChanged(System.Boolean)

 todo override for templated skialayout to use ViewsProvider 

|Name | Description |
|-----|------|
|newvalue: ||


---
#### Method Draw.SkiaControl.OnVisibilityChanged(System.Boolean)

 todo override for templated skialayout to use ViewsProvider 

|Name | Description |
|-----|------|
|newvalue: ||


---
#### Method Draw.SkiaControl.OnDisposing

 Base performs some cleanup actions with Superview 



---
#### Property Draw.SkiaControl.ViewportHeightLimit

 Will be used inside GetDrawingRectWithMargins to limit the height of the DrawingRect 



---
#### Property Draw.SkiaControl.ViewportWidthLimit

 Will be used inside GetDrawingRectWithMargins to limit the width of the DrawingRect 



---
#### Property Draw.SkiaControl.Scale

 Please use ScaleX, ScaleY instead of this maui property 



---
#### Property Draw.SkiaControl.LockRatio

 Locks the final size to the min (-1.0 -> 0.0) or max (0.0 -> 1.0) of the provided size. 



---
#### Property Draw.SkiaControl.HeightRequestRatio

 HeightRequest Multiplier, default is 1.0 



---
#### Property Draw.SkiaControl.WidthRequestRatio

 WidthRequest Multiplier, default is 1.0 



---
#### Property Draw.SkiaControl.Margins

 Total calculated margins in points 



---
#### Property Draw.SkiaControl.ExpandCacheRecordingArea

 Normally cache is recorded inside DrawingRect, but you might want to exapnd this to include shadows around, for example. Specify number of points by which you want to expand the recording area. Also you might maybe want to include a bigger area if your control is not inside the DrawingRect due to transforms/translations. Override GetCacheRecordingArea method for a similar action. 



---
#### Property Draw.SkiaControl.IsClippedToBounds

 This cuts shadows etc. You might want to enable it for some cases as it speeds up the rendering, it is False by default 



---
#### Property Draw.SkiaControl.ClipEffects

 This cuts shadows etc 



---
#### Method Draw.SkiaControl.DisposeObject(System.IDisposable)

 Dispose with needed delay. 

|Name | Description |
|-----|------|
|disposable: ||


---
#### Property Draw.SkiaControl.Hero

 Optional scene hero control identifier 



---
#### Method Draw.SkiaControl.DefineAvailableSize(SkiaSharp.SKRect,System.Single,System.Single,System.Single)

 destination in PIXELS, requests in UNITS 

|Name | Description |
|-----|------|
|destination: ||
|widthRequest: ||
|heightRequest: ||
|scale: ||
**Returns**: 



---
#### Property Draw.SkiaControl.IsLayoutDirty

 Set this by parent if needed, normally child can detect this itsself. If true will call Arrange when drawing. 



---
#### Method Draw.SkiaControl.CalculateLayout(SkiaSharp.SKRect,System.Single,System.Single,System.Single)

 destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS. Not using Margins nor Padding Children are responsible to apply Padding to their content and to apply Margin to destination when measuring and drawing 

|Name | Description |
|-----|------|
|destination: |PIXELS|
|widthRequest: |UNITS|
|heightRequest: |UNITS|
|scale: ||


---
#### Property Draw.SkiaControl.DrawingRect

 This is the destination in PIXELS with margins applied, using this to paint background. Since we enabled subpixel drawing (for smooth scroll etc) expect this to have non-rounded values, use CompareRects and similar for comparison. 



---
#### Property Draw.SkiaControl.Bounds

 Overriding VisualElement property, use DrawingRect instead. 



---
#### Method Draw.SkiaControl.HitIsInside(System.Single,System.Single)

 ISkiaGestureListener impl 



---
#### Property Draw.SkiaControl.HitBoxAuto

 This can be absolutely false if we are inside a cached rendering object parent that already moved somewhere. So coords will be of the moment we were first drawn, while if cached parent moved, our coords might differ. todo detect if parent is cached somewhere and offset hotbox by cached parent movement offset... todo think about it baby =) meanwhile just do not set gestures below cached level 



---
#### Method Draw.SkiaControl.TranslateInputCoords(SkiaSharp.SKPoint,System.Boolean)

 Use this to consume gestures in your control only, do not use result for passing gestures below 

|Name | Description |
|-----|------|
|childOffset: ||
**Returns**: 



---
#### Method Draw.SkiaControl.OnLayoutReady

 Layout was changed with dimensions above zero. Rather a helper method, can you more generic OnLayoutChanged(). 



---
#### Method Draw.SkiaControl.Arrange(SkiaSharp.SKRect,System.Single,System.Single,System.Single)

 destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS. DrawUsingRenderObject wil call this among others.. 

|Name | Description |
|-----|------|
|destination: |PIXELS|
|widthRequest: |UNITS|
|heightRequest: |UNITS|
|scale: ||


---
#### Method Draw.SkiaControl.MeasureChild(DrawnUi.Maui.Draw.SkiaControl,System.Double,System.Double,System.Single)

 PIXELS 

|Name | Description |
|-----|------|
|child: ||
|availableWidth: ||
|availableHeight: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaControl.MeasureContent(System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl},SkiaSharp.SKRect,System.Single)

 Measuring as absolute layout for passed children 

|Name | Description |
|-----|------|
|children: ||
|rectForChildrenPixels: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaControl.SetInheritedBindingContext(System.Object)

 This is to be called by layouts to propagate their binding context to children. By overriding this method any child could deny a new context or use any other custom logic. To force new context for child parent would set child's BindingContext directly skipping the use of this method. 

|Name | Description |
|-----|------|
|context: ||


---
#### Method Draw.SkiaControl.ApplyBindingContext

 https://github.com/taublast/DrawnUi.Maui/issues/92#issuecomment-2408805077 



---
#### Method Draw.SkiaControl.OnBindingContextChanged

 First Maui will apply bindings to your controls, then it would call OnBindingContextChanged, so beware on not to break bindings. 



---
#### Method Draw.SkiaControl.Measure(System.Single,System.Single,System.Single)



|Name | Description |
|-----|------|
|widthConstraint: ||
|heightConstraint: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaControl.MeasureAbsoluteBase(SkiaSharp.SKRect,System.Single)

 Base method, not aware of any views provider, not virtual, silly measuring Children. 

|Name | Description |
|-----|------|
|rectForChildrenPixels: ||
|scale: ||
**Returns**: 



---
#### Property Draw.SkiaControl.IsMeasuring

 Flag for internal use, maynly used to avoid conflicts between measuring on ui-thread and in background. If true, measure will return last measured value. 



---
#### Method Draw.SkiaControl.SetMeasured(System.Single,System.Single,System.Boolean,System.Boolean,System.Single)

 Parameters in PIXELS. sets IsLayoutDirty = true; 

|Name | Description |
|-----|------|
|width: ||
|height: ||
|scale: ||
**Returns**: 



---
#### Event Draw.SkiaControl.Measured

 UNITS 



---
#### Property Draw.SkiaControl.NeedDispose

 Developer can use this to mark control as to be disposed by parent custom controls 



---
#### Method Draw.SkiaControl.Dispose

 Avoid setting parent to null before calling this, or set SuperView prop manually for proper cleanup of animations and gestures if any used 



---
#### Property Draw.SkiaControl.IsOverlay

 do not ever erase background 



---
#### Property Draw.SkiaControl.PostAnimators

 Executed after the rendering 



---
#### Method Draw.SkiaControl.ApplyMeasureResult

 Normally get a a Measure by parent then parent calls Draw and we can apply the measure result. But in a case we have measured us ourselves inside PreArrange etc we must call ApplyMeasureResult because this would happen after the Draw and not before. 



---
#### Method Draw.SkiaControl.PreArrange(SkiaSharp.SKRect,System.Single,System.Single,System.Single)

 Returns false if should not render 

**Returns**: 



---
#### Field Draw.SkiaControl.LockDraw

 Lock between replacing and using RenderObject 



---
#### Field Draw.SkiaControl.LockRenderObject

 Creating new cache lock 



---
#### Property Draw.SkiaControl.X

 Absolute position obtained after this control was drawn on the Canvas, this is not relative to parent control. 



---
#### Property Draw.SkiaControl.Y

 Absolute position obtained after this control was drawn on the Canvas, this is not relative to parent control. 



---
#### Method Draw.SkiaControl.FinalizeDrawingWithRenderObject(DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)

 Execute post drawing operations, like post-animators etc 

|Name | Description |
|-----|------|
|context: ||
|scale: ||


---
#### Property Draw.SkiaControl.LastDrawnAt

 Location on the canvas after last drawing completed 



---
#### Method Draw.SkiaControl.DrawViews(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single,System.Boolean)

 Base method will call RenderViewsList. Return number of drawn views. 

|Name | Description |
|-----|------|
|context: ||
|destination: ||
|scale: ||
|debug: ||
**Returns**: 



---
#### Method Draw.SkiaControl.Repaint

 Just make us repaint to apply new transforms etc 



---
#### Property Draw.SkiaControl.CustomizeLayerPaint

 Can customize the SKPaint used for painting the object 



---
#### Method Draw.SkiaControl.ClipSmart(SkiaSharp.SKCanvas,SkiaSharp.SKPath,SkiaSharp.SKClipOperation)

 Use antialiasing from ShouldClipAntialiased 

|Name | Description |
|-----|------|
|canvas: ||
|path: ||
|operation: ||


---
#### Property Draw.SkiaControl.ShouldClipAntialiased

 This is not a static bindable property. Can be set manually or by control, for example SkiaShape sets this to true for non-rectangular shapes, or rounded corners.. 



---
#### Method Draw.SkiaControl.SafePostAction(System.Action)

 If attached to a SuperView and rendering is in progress will run after it. Run now otherwise. 

|Name | Description |
|-----|------|
|action: ||


---
#### Method Draw.SkiaControl.SafeAction(System.Action)

 If attached to a SuperView will run only after draw to avoid memory access conflicts. If not attached will run after 3 secs.. 

|Name | Description |
|-----|------|
|action: ||


---
#### Method Draw.SkiaControl.Paint(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single,System.Object)

 This is the main drawing routine you should override to draw something. Base one paints background color inside DrawingRect that was defined by Arrange inside base.Draw. Pass arguments if you want to use some time-frozen data for painting at any time from any thread.. 

|Name | Description |
|-----|------|
|ctx: ||
|destination: ||
|scale: ||


---
#### Property Draw.SkiaControl.WasDrawn

 Signals if this control was drawn on canvas one time at least, it will be set by Paint method. 



---
#### Method Draw.SkiaControl.CreateClip(System.Object,System.Boolean,SkiaSharp.SKPath)

 Create this control clip for painting content. Pass arguments if you want to use some time-frozen data for painting at any time from any thread.. 

|Name | Description |
|-----|------|
|arguments: ||
**Returns**: 



---
#### Method Draw.SkiaControl.DrawRenderObject(DrawnUi.Maui.Draw.CachedObject,DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)

 Drawing cache, applying clip and transforms as well 

|Name | Description |
|-----|------|
|cache: ||
|ctx: ||
|destination: ||


---
#### Method Draw.SkiaControl.RenderViewsList(System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl},DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single,System.Boolean)

 Use to render Absolute layout. Base method is not supporting templates, override it to implemen your logic. Returns number of drawn children. 

|Name | Description |
|-----|------|
|skiaControls: ||
|context: ||
|destination: ||
|scale: ||
|debug: ||


---
## Type Draw.SkiaControl.SkiaControlWithRect

 Rect is real drawing position 

|Name | Description |
|-----|------|
|Control: ||
|Rect: ||
|Index: ||


---
#### Method Draw.SkiaControl.SkiaControlWithRect.#ctor(DrawnUi.Maui.Draw.SkiaControl,SkiaSharp.SKRect,SkiaSharp.SKRect,System.Int32)

 Rect is real drawing position 

|Name | Description |
|-----|------|
|Control: ||
|Rect: ||
|Index: ||


---
#### Property Draw.SkiaControl.SkiaControlWithRect.Control





---
#### Property Draw.SkiaControl.SkiaControlWithRect.Rect





---
#### Property Draw.SkiaControl.SkiaControlWithRect.Index





---
#### Property Draw.SkiaControl.RenderTree

 Last rendered controls tree. Used by gestures etc.. 



---
#### Property Draw.SkiaControl.NeedUpdate

 For internal use, set by Update method 



---
#### Property Draw.SkiaControl.Superview

 Our canvas 



---
#### Method Draw.SkiaControl.GetOnScreenVisibleArea(System.Single)

 For virtualization 

**Returns**: 



---
#### Property Draw.SkiaControl.WillClipBounds

 Used to check whether to apply IsClippedToBounds property 



---
#### Method Draw.SkiaControl.Update

 Main method to invalidate cache and invoke rendering 



---
#### Event Draw.SkiaControl.Updated

 Triggered by Update method 



---
#### Property Draw.SkiaControl.Destination

 For internal use 



---
#### Property Draw.SkiaControl.IsRenderingWithComposition

 Internal flag indicating that the current frame will use cache composition, old cache will be reused, only dirty children will be redrawn over it 



---
#### Method Draw.SkiaControl.PaintTintBackground(SkiaSharp.SKCanvas,SkiaSharp.SKRect)

 Pixels, if you see no Scale parameter 

|Name | Description |
|-----|------|
|canvas: ||
|destination: ||


---
#### Method Draw.SkiaControl.CalculateMargins

 Summing up Margins and AddMargin.. properties 



---
#### Method Draw.SkiaControl.InvalidateChildrenTree(DrawnUi.Maui.Draw.SkiaControl)

 Will invoke InvalidateInternal on controls and subviews 

|Name | Description |
|-----|------|
|control: ||


---
#### Method Draw.SkiaControl.InvalidateChildren(DrawnUi.Maui.Draw.SkiaControl)

 Will invoke InvalidateInternal on controls and subviews 

|Name | Description |
|-----|------|
|control: ||


---
#### Method Draw.SkiaControl.NeedRepaint(Microsoft.Maui.Controls.BindableObject,System.Object,System.Object)

 Just make us repaint to apply new transforms etc 



---
#### Method Draw.SkiaControl.PlayRippleAnimation(Microsoft.Maui.Graphics.Color,System.Double,System.Double,System.Boolean)

 Expecting input coordinates in POINTs and relative to control coordinates. Use GetOffsetInsideControlInPoints to help. 

|Name | Description |
|-----|------|
|color: ||
|x: ||
|y: ||
|removePrevious: ||


---
#### Method Draw.SkiaControl.SetupShadow(SkiaSharp.SKPaint,DrawnUi.Maui.Draw.SkiaShadow,System.Single)

 Creates and sets an ImageFilter for SKPaint 

|Name | Description |
|-----|------|
|paint: ||
|shadow: ||


---
#### Method Draw.SkiaControl.OnWillDisposeWithChildren

 The OnDisposing might come with a delay to avoid disposing resources at use. This method will be called without delay when Dispose() is invoked. Disposed will set to True and for Views their OnWillDisposeWithChildren will be called. 



---
#### Property Draw.SkiaControl.GestureListeners

 Children we should check for touch hits 



---
#### Property Draw.SkiaControl.ItemTemplate

 Kind of BindableLayout.DrawnTemplate 



---
#### Property Draw.SkiaControl.ItemTemplateType

 ItemTemplate alternative for faster creation 



---
#### Method Draw.SkiaControl.AreClose(System.Single,System.Single)

 Ported from Avalonia: AreClose - Returns whether or not two floats are "close". That is, whether or not they are within epsilon of each other. 

|Name | Description |
|-----|------|
|value1: | The first float to compare. |
|value2: | The second float to compare. |


---
#### Method Draw.SkiaControl.AreClose(System.Double,System.Double)

 Ported from Avalonia: AreClose - Returns whether or not two doubles are "close". That is, whether or not they are within epsilon of each other. 

|Name | Description |
|-----|------|
|value1: | The first double to compare. |
|value2: | The second double to compare. |


---
#### Method Draw.SkiaControl.IsOne(System.Double)

 Avalonia: IsOne - Returns whether or not the double is "close" to 1. Same as AreClose(double, 1), but this is faster. 

|Name | Description |
|-----|------|
|value: | The double to compare to 1. |


---
#### Method Draw.Snapping.SnapPointsToPixel(System.Single,System.Single,System.Double)

 Used by the layout system to round a position translation value applying scale and initial anchor. Pass POINTS only, it wont do its job when receiving pixels! 

|Name | Description |
|-----|------|
|initialPosition: ||
|translation: ||
|scale: ||
**Returns**: 



---
#### Field Draw.SkiaCacheType.None

 True and old school 



---
#### Field Draw.SkiaCacheType.Operations

 Create and reuse SKPicture. Try this first for labels, svg etc. Do not use this when dropping shadows or with other effects, better use Bitmap. 



---
#### Field Draw.SkiaCacheType.OperationsFull

 Create and reuse SKPicture all over the canvas ignoring clipping. Try this first for labels, svg etc. Do not use this when dropping shadows or with other effects, better use Bitmap. 



---
#### Field Draw.SkiaCacheType.Image

 Will use simple SKBitmap cache type, will not use hardware acceleration. Slower but will work for sizes bigger than graphics memory if needed. 



---
#### Field Draw.SkiaCacheType.ImageDoubleBuffered

 Using `Image` cache type with double buffering. Will display a previous cache while rendering the new one in background, thus not slowing scrolling etc. 



---
#### Field Draw.SkiaCacheType.ImageComposite

 Would receive the invalidated area rectangle, then redraw the previous cache but clipped to exclude the dirty area, then would re-create the dirty area and draw it clipped inside the dirty rectangle. This is useful for layouts with many children, like scroll content etc, but useless for non-containers. 



---
#### Field Draw.SkiaCacheType.GPU

 The cached surface will use the same graphic context as your hardware-accelerated canvas. This kind of cache will not apply Opacity as not all platforms support transparency for hardware accelerated layer. Will fallback to simple Image cache type if hardware acceleration is not available. 



---
#### Property Draw.CachedObject.SurfaceIsRecycled

 An existing surface was reused for creating this object 



---
#### Method Draw.SkiaEditor.Submit

 This is Done or Enter key, so maybe just split lines in specific case 



---
#### Method Draw.SkiaEditor.GetCursorPosition(System.Single,System.Single)

 Input in pixels 

|Name | Description |
|-----|------|
|x: ||
|y: ||
**Returns**: 



---
#### Method Draw.SkiaEditor.MoveInternalCursor

 Sets native contol cursor position to CursorPosition and calls UpdateCursorVisibility 



---
#### Method Draw.SkiaEditor.UpdateCursorVisibility

 Positions cursor control where it should be using translation, and sets its visibility. 



---
#### Method Draw.SkiaEditor.MoveCursorTo(System.Double,System.Double)

 Translate cursor from the left top corner, params in pts. 

|Name | Description |
|-----|------|
|x: ||
|y: ||


---
#### Method Draw.SkiaEditor.SetCursorPositionWithDelay(System.Int32,System.Int32)

 We have to sync with a delay after text was changed otherwise the cursor position is not updated yet. Using restarting timer, every time this is called the timer is reset if callback wasn't executed yet. 

|Name | Description |
|-----|------|
|ms: ||
|position: ||


---
#### Property Draw.SkiaEditor.MyTextWatcher.NativeSelectionStart

 This will be read by the parent to check the cursor position at will 



---
#### Method Draw.SkiaMauiElement.MeasureAndArrangeMauiElement(System.Double,System.Double)

 Measure and arrange VisualElement using Maui methods 

|Name | Description |
|-----|------|
|ptsWidth: ||
|ptsHeight: ||
**Returns**: 



---
#### Method Draw.SkiaMauiElement.GetVisualChildren

 For HotReload 

**Returns**: 



---
#### Method Draw.SkiaMauiElement.SetChildren(System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl})

 Prevent usage of subviews as we are using Content property for this control 

|Name | Description |
|-----|------|
|views: ||


---
#### Method Draw.SkiaMauiElement.OnChildAdded(DrawnUi.Maui.Draw.SkiaControl)

 Prevent usage of subviews as we are using Content property for this control 



---
#### Method Draw.SkiaMauiElement.SetContent(Microsoft.Maui.Controls.VisualElement)

 Use Content property for direct access 



---
#### Property Draw.SkiaMauiElement.Element

 Maui Element to be rendered 



---
#### Property Draw.SkiaMauiElement.ElementSize

 PIXELS, for faster checks 



---
#### Property Draw.SkiaMauiElement.AnimateSnapshot

 Set to true if you are hosting the control inside a scroll or similar case where the control position/transforms are animated fast. 



---
#### Method Draw.SkiaMauiElement.SetNativeVisibility(System.Boolean)

 This is mainly ued by show/hide to display Snapshot instead the native view 

|Name | Description |
|-----|------|
|state: ||


---
#### Property Draw.LoadedImageSource.ProtectFromDispose

 As this can be disposed automatically by the consuming control like SkiaImage etc we can manually prohibit this for cases this instance is used elsewhere. 



---
#### Method Draw.SkiaImage.SetImage(DrawnUi.Maui.Draw.LoadedImageSource)

 Do not call this directly, use InstancedBitmap prop 

|Name | Description |
|-----|------|
|loaded: ||


---
#### Method Draw.SkiaImage.GetRenderedSource

 Will containt all the effects and other rendering properties applied, size will correspond to source. 

**Returns**: 



---
#### Property Draw.SkiaImage.Aspect

 Apspect to render image with, default is AspectCover. 



---
#### Property Draw.SkiaImage.ImageBitmap

 this is the source loaded, doesn't reflect any effects or any other rendering properties 



---
#### Property Draw.SkiaImage.LoadSourceOnFirstDraw

 Should the source be loaded on the first draw, useful for the first fast rendering of the screen and loading images after, default is False. Set this to False if you need an off-screen loading and to True to make the screen load faster. While images are loaded in async manner this still has its impact. Useful to set True for for SkiaCarousel cells etc.. 



---
#### Property Draw.SkiaImage.RescalingQuality

 Default value is None. You might want to set this to Medium for static images for better quality. 



---
#### Property Draw.SkiaImage.PreviewBase64

 If setting in code-behind must be set BEFORE you change the Source 



---
#### Method Draw.SkiaImage.SetBitmapInternal(SkiaSharp.SKBitmap,System.Boolean)

 Use only if you know what to do, this internally just sets the new bitmap without any invalidations and not forcing an update. You would want to set InstancedBitmap prop for a usual approach. 

|Name | Description |
|-----|------|
|bitmap: ||


---
#### Property Draw.SkiaImage.LastSource

 Last source that we where loading. Td be reused for reload.. 



---
#### Property Draw.SkiaImage.EraseChangedContent

 Should we erase the existing image when another Source is set but wasn't applied yet (not loaded yet) 



---
#### Field Draw.SkiaImage.ImagePaint

 Reusing this 



---
#### Field Draw.SkiaImage.PaintImageFilter

 Reusing this 



---
#### Field Draw.SkiaImage.PaintColorFilter

 Reusing this 



---
#### Property Draw.SkiaImage.SourceWidth

 From current set Source in points 



---
#### Property Draw.SkiaImage.SourceHeight

 From current set Source in points 



---
#### Property Draw.SkiaImageTiles.TileAspect

 Apspect to render image with, default is AspectFitFill. 



---
#### Property Draw.SkiaImageTiles.Tile

 Cached image that will be used as tile 



---
#### Property Draw.SkiaImageTiles.DrawTiles

 Whether tiles are setup for rendering 



---
#### Method Draw.SkiaImageTiles.OnSourceSuccess

 Source was loaded, we can create tile 



---
#### Method Draw.ContentLayout.GetContentAvailableRect(SkiaSharp.SKRect)

 In PIXELS 

|Name | Description |
|-----|------|
|destination: ||
**Returns**: 



---
#### Property Draw.ContentLayout.Orientation

Gets or sets the scrolling direction of the ScrollView. This is a bindable property.





---
#### Property Draw.ContentLayout.ScrollType

Gets or sets the scrolling direction of the ScrollView. This is a bindable property.





---
#### Property Draw.ContentLayout.Virtualisation

 Default is Enabled, children get the visible viewport area for rendering and can virtualize. 



---
## Type Draw.SkiaLayout.BuildWrapLayout

 Implementation for LayoutType.Wrap 



---
#### Property Draw.SkiaLayout.StackStructure

 Used for StackLayout (Stack, Row) kind of layout 



---
#### Property Draw.SkiaLayout.StackStructureMeasured

 When measuring we set this, and it will be swapped with StackStructure upon drawing so we don't affect the drawing if measuring in background. 



---
## Type Draw.SkiaLayout.SecondPassArrange

 Cell.Area contains the area for layout 

|Name | Description |
|-----|------|
|cell: ||
|child: ||
|scale: ||


---
#### Method Draw.SkiaLayout.SecondPassArrange.#ctor(DrawnUi.Maui.Draw.ControlInStack,DrawnUi.Maui.Draw.SkiaControl,System.Single)

 Cell.Area contains the area for layout 

|Name | Description |
|-----|------|
|cell: ||
|child: ||
|scale: ||


---
#### Method Draw.SkiaLayout.MeasureStack(SkiaSharp.SKRect,System.Single)

 Measuring column/row 

|Name | Description |
|-----|------|
|rectForChildrenPixels: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaLayout.OnTemplatesAvailable

 Will be called by views adapter upot succsessfull execution of InitializeTemplates. When using InitializeTemplatesInBackground this is your callbacl to wait for. 

**Returns**: 



---
#### Property Draw.SkiaLayout.RecyclingTemplate

 In case of ItemsSource+ItemTemplate set will define should we reuse already created views: hidden items views will be reused for currently visible items on screen. If set to true inside a SkiaScrollLooped will cause it to redraw constantly even when idle because of the looped scroll mechanics. 



---
#### Property Draw.SkiaLayout.TemplatedHeader

 Kind of BindableLayout.DrawnTemplate 



---
#### Property Draw.SkiaLayout.TemplatedFooter

 Kind of BindableLayout.DrawnTemplate 



---
#### Property Draw.SkiaLayout.Virtualisation

 Default is Enabled, children get the visible viewport area for rendering and can virtualize. 



---
#### Property Draw.SkiaLayout.VirtualisationInflated

 How much of the hidden content out of visible bounds should be considered visible for rendering, default is 0 pts. Basically how much should be expand in every direction of the visible area prior to checking if content falls into its bounds for rendering controlled with Virtualisation. 



---
#### Property Draw.SkiaLayout.Split

 For Wrap number of columns/rows to split into, If 0 will use auto, if 1+ will have 1+ columns. 



---
#### Property Draw.SkiaLayout.SplitAlign

 Whether should keep same column width among rows 



---
#### Property Draw.SkiaLayout.SplitSpace

 How to distribute free space between children TODO 



---
#### Property Draw.SkiaLayout.DynamicColumns

 If true, will not create additional columns to match SplitMax if there are less real columns, and take additional space for drawing 



---
#### Method Draw.SkiaLayout.Measure(System.Single,System.Single,System.Single)

 If you call this while measurement is in process (IsMeasuring==True) will return last measured value. 

|Name | Description |
|-----|------|
|widthConstraint: ||
|heightConstraint: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaLayout.SetupRenderingWithComposition(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)

 Find intersections between changed children and DrawingRect, add intersecting ones to DirtyChildrenInternal and set IsRenderingWithComposition = true if any. 

|Name | Description |
|-----|------|
|ctx: ||
|destination: ||


---
#### Property Draw.SkiaLayout.IsStack

 Column/Row/Stack 



---
#### Property Draw.SkiaLayout.InitializeTemplatesInBackgroundDelay

 Whether should initialize templates in background instead of blocking UI thread, default is 0. Set your delay in Milliseconds to enable. When this is enabled and RecyclingTemplate is Disabled will also measure the layout in background when templates are available without blocking UI-tread. After that OnTemplatesAvailable will be called on parent layout. 



---
#### Property Draw.SkiaLayout.ItemTemplatePoolSize

 Default is -1, the number od template instances will not be less than data collection count. You can manually set to to a specific number to fill your viewport etc. Beware that if you set this to a number that will not be enough to fill the viewport binding contexts will contasntly be changing triggering screen update. 



---
#### Property Draw.SkiaLayout.Cell.ColumnGridLengthType

 A combination of all the measurement types in the columns this cell spans 



---
#### Property Draw.SkiaLayout.Cell.RowGridLengthType

 A combination of all the measurement types in the rows this cell spans 



---
#### Method Draw.SkiaLayout.DrawChildrenGrid(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single)

 Returns number of drawn children 

|Name | Description |
|-----|------|
|context: ||
|destination: ||
|scale: ||
**Returns**: 



---
#### Property Draw.SkiaLayout.DefaultColumnDefinition

 Will use this to create a missing but required ColumnDefinition 



---
#### Property Draw.SkiaLayout.DefaultRowDefinition

 Will use this to create a missing but required RowDefinition 



---
#### Field Draw.SkiaLayout.SkiaGridStructure._gridWidthConstraint

 Pixels 



---
#### Field Draw.SkiaLayout.SkiaGridStructure._gridHeightConstraint

 Pixels 



---
#### Method Draw.SkiaLayout.SkiaGridStructure.InitializeCells

 We are also going to auto-create column/row definitions here 



---
#### Method Draw.SkiaLayout.MeasureWrap(SkiaSharp.SKRect,System.Single)

 TODO for templated measure only visible?! and just reserve predicted scroll amount for scrolling 

|Name | Description |
|-----|------|
|rectForChildrenPixels: ||
|scale: ||
**Returns**: 



---
#### Method Draw.SkiaLayout.DrawStack(DrawnUi.Maui.Draw.LayoutStructure,DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single)

 Renders stack/wrap layout. Returns number of drawn children. 



---
## Type Draw.ViewsAdapter

 Top level class for working with ItemTemplates. Holds visible views. 



---
#### Field Draw.ViewsAdapter._dicoCellsInUse

 Holds visible prepared views with appropriate context, index is inside ItemsSource 



---
#### Method Draw.ViewsAdapter.AddMoreToPool(System.Int32)

 Keep pool size with `n` templated more oversized, so when we suddenly need more templates they would already be ready, avoiding lag spike, This method is likely to reserve templated views once on layout size changed. 

|Name | Description |
|-----|------|
|oversize: ||


---
#### Method Draw.ViewsAdapter.FillPool(System.Int32,System.Collections.IList)

 Use to manually pre-create views from item templates so when we suddenly need more templates they would already be ready, avoiding lag spike, This will respect pool MaxSize in order not to overpass it. 

|Name | Description |
|-----|------|
|size: ||


---
#### Method Draw.ViewsAdapter.FillPool(System.Int32)

 Use to manually pre-create views from item templates so when we suddenly need more templates they would already be ready, avoiding lag spike, This will respect pool MaxSize in order not to overpass it. 

|Name | Description |
|-----|------|
|size: ||


---
#### Method Draw.ViewsAdapter.InitializeTemplates(System.Func{System.Object},System.Collections.IList,System.Int32,System.Int32)

 Main method to initialize templates, can use InitializeTemplatesInBackground as an option. 

|Name | Description |
|-----|------|
|template: ||
|dataContexts: ||
|poolSize: ||
|reserve: |Pre-create number of views to avoid lag spikes later, useful to do in backgound.|


---
#### Property Draw.ViewsAdapter.TemplatesAvailable

 An important check to consider before consuming templates especially if you initialize templates in background 



---
## Type Draw.ViewsAdapter.TemplatedViewsPool

 Used by ViewsProvider 



---
#### Method Draw.ViewsAdapter.TemplatedViewsPool.CreateFromTemplate

 unsafe 

**Returns**: 



---
#### Method Draw.ViewsAdapter.TemplatedViewsPool.Reserve

 Just create template and save for the future 



---
## Type Draw.ViewsAdapter.ViewsIterator

 To iterate over virtual views 



---
#### Property Draw.SnappingLayout.ContentOffsetBounds

 There are the bounds the scroll offset can go to.. This are NOT the bounds of the whole content. 



---
#### Method Draw.SnappingLayout.ClampOffsetWithRubberBand(System.Single,System.Single)

 Called for manual finger panning 

|Name | Description |
|-----|------|
|x: ||
|y: ||
**Returns**: 



---
#### Property Draw.SnappingLayout.Viewport

 Using this instead of RenderingViewport 



---
#### Method Draw.SnappingLayout.GetContentOffsetBounds

 There are the bounds the scroll offset can go to.. This are NOT the bounds of the whole content. 



---
#### Method Draw.SnappingLayout.SelectNextAnchor(System.Numerics.Vector2,System.Numerics.Vector2)

 Return an anchor depending on direction and strength of of the velocity 

|Name | Description |
|-----|------|
|origin: ||
|velocity: ||
**Returns**: 



---
#### Property Draw.SnappingLayout.SnapDistanceRatio

 0.2 - Part of the distance between snap points the velocity need to cover to trigger going to the next snap point. NOT a bindable property (yet). 



---
#### Method Draw.SnappingLayout.GetAutoVelocity(System.Numerics.Vector2)

 todo calc upon measured size + prop for speed 

|Name | Description |
|-----|------|
|displacement: ||
**Returns**: 



---
#### Method Draw.SnappingLayout.ScrollToOffset(System.Numerics.Vector2,System.Numerics.Vector2,System.Boolean)

 In Units 

lo |Name | Description |
|-----|------|
|offset: ||
|animate: ||


---
#### Property Draw.SnappingLayout.RespondsToGestures

 Can be open/closed by gestures along with code-behind, default is true 



---
#### Property Draw.SnappingLayout.IgnoreWrongDirection

 Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity 



---
#### Property Draw.SnappingLayout.RubberDamping

 If Bounce is enabled this basically controls how less the scroll will bounce when displaced from limit by finger or inertia. Default is 0.8. 



---
#### Property Draw.SnappingLayout.AutoVelocityMultiplyPts

 If velocity is near 0 define how much we multiply the auto-velocity used to animate snappoing point. For example when in carousel you cancel the swipe and release finger.. 



---
#### Property Draw.SnappingLayout.RubberEffect

 If Bounce is enabled this basically controls how far from the limit can the scroll be elastically offset by finger or inertia. Default is 0.15. 



---
#### Method Draw.StackLayoutStructure.Build(SkiaSharp.SKRect,System.Single)

 Will measure children and build appropriate stack structure for the layout 



---
#### Property Draw.RefreshIndicator.Orientation

Gets or sets the scrolling direction of the ScrollView. This is a bindable property.





---
#### Method Draw.RefreshIndicator.SetDragRatio(System.Single)

 0 - 1 

|Name | Description |
|-----|------|
|ratio: ||


---
#### Property Draw.RefreshIndicator.IsRunning

 ReadOnly 



---
#### Field Draw.SkiaScroll.ThesholdSwipeOnUp

 Min velocity in points/sec to flee/swipe when finger is up 



---
#### Field Draw.SkiaScroll.ScrollVelocityThreshold

 To filter micro-gestures while manually panning 



---
#### Field Draw.SkiaScroll.SystemAnimationTimeSecs

 Time for the snapping animations as well as the scroll to top etc animations.. 



---
## Type Draw.SkiaScroll.ScrollingInteractionState

 TODO impement this 



---
#### Property Draw.SkiaScroll.HeaderSticky

 Should the header stay in place when content is scrolling 



---
#### Property Draw.SkiaScroll.RefreshDistanceLimit

 Applyed to RefreshView 



---
#### Property Draw.SkiaScroll.RefreshShowDistance

 Applyed to RefreshView 



---
#### Property Draw.SkiaScroll.OverscrollDistance

 Units 



---
#### Property Draw.SkiaScroll.ContentOffsetBounds

 There are the bounds the scroll offset can go to.. This is NOT the bounds for the whole content. 



---
#### Method Draw.SkiaScroll.ClampOffsetWithRubberBand(System.Single,System.Single)

 Used to clamp while panning while finger is down 

|Name | Description |
|-----|------|
|x: ||
|y: ||
**Returns**: 



---
#### Property Draw.SkiaScroll.RespondsToGestures

 If disabled will not scroll using gestures. Scrolling will still be possible by code. 



---
#### Property Draw.SkiaScroll.CanScrollUsingHeader

 If disabled will not scroll using gestures. Scrolling will still be possible by code. 



---
#### Field Draw.SkiaScroll.InterpolationFactor

 panning interpolation to avoid trembling finlgers 



---
#### Field Draw.SkiaScroll._animatorFlingX

 Fling with deceleration 



---
#### Field Draw.SkiaScroll._animatorFlingY

 Fling with deceleration 



---
#### Field Draw.SkiaScroll._scrollerX

 Direct scroller for ordered scroll, snap etc 



---
#### Field Draw.SkiaScroll._scrollerY

 Direct scroller for ordered scroll, snap etc 



---
#### Field Draw.SkiaScroll._scrollMinX

 Units 



---
#### Field Draw.SkiaScroll._scrollMinY

 Units 



---
#### Field Draw.SkiaScroll.snapMinimumVelocity

 POINTS per sec 



---
#### Field Draw.SkiaScroll.BouncesProperty

 ToDo adapt this to same logic as ScrollLooped has ! 

|Name | Description |
|-----|------|
|force: ||


---
#### Property Draw.SkiaScroll.Bounces

 Should the scroll bounce at edges. Set to false if you want this scroll to let the parent SkiaDrawer respond to scroll when the child scroll reached bounds. 



---
#### Property Draw.SkiaScroll.RubberDamping

 If Bounce is enabled this basically controls how less the scroll will bounce when displaced from limit by finger or inertia. Default is 0.55. 



---
#### Property Draw.SkiaScroll.RubberEffect

 If Bounce is enabled this basically controls how far from the limit can the scroll be elastically offset by finger or inertia. Default is 0.55. 



---
#### Property Draw.SkiaScroll.AutoScrollingSpeedMs

 For snap and ordered scrolling 



---
#### Property Draw.SkiaScroll.FrictionScrolled

 Use this to control how fast the scroll will decelerate. Values 0.1 - 0.9 are the best, default is 0.3. Usually you would set higher friction for ScrollView-like scrolls and much lower for CollectionView-like scrolls (0.1 or 0.2). 



---
#### Property Draw.SkiaScroll.IgnoreWrongDirection

 Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity. Default is False. 



---
#### Property Draw.SkiaScroll.ChangeVelocityScrolled

 For when the finger is up and swipe is detected 



---
#### Property Draw.SkiaScroll.MaxVelocity

 Limit user input velocity 



---
#### Property Draw.SkiaScroll.MaxBounceVelocity

 Limit bounce velocity 



---
#### Property Draw.SkiaScroll.ChangeDIstancePanned

 For when the finger is down and panning 



---
#### Method Draw.SkiaScroll.CalculateVisibleIndex(DrawnUi.Maui.Draw.RelativePositionType)

 Calculates CurrentIndex 



---
#### Method Draw.SkiaScroll.CalculateScrollOffsetForIndex(System.Int32,DrawnUi.Maui.Draw.RelativePositionType)

 ToDo this actually work only for Stack and Row 

|Name | Description |
|-----|------|
|index: ||
|option: ||
**Returns**: 



---
#### Property Draw.SkiaScroll.SnapToChildren

 Whether should snap to children after scrolling stopped 



---
#### Property Draw.SkiaScroll.TrackIndexPosition

 The position in viewport you want to track for content layout child index 



---
#### Property Draw.SkiaScroll.WasSwiping

 Had no panning just down+up with velocity more than threshold 



---
#### Property Draw.SkiaScroll.ZoomScaleInternal

 We might have difference between pinch scale and manually set zoom. 



---
#### Method Draw.SkiaScroll.GetContentAvailableRect(SkiaSharp.SKRect)

 In PIXELS 

|Name | Description |
|-----|------|
|destination: ||
**Returns**: 



---
#### Property Draw.SkiaScroll.InternalViewportOffset

 This is where the view port is actually is after being scrolled. We used this value to offset viewport on drawing the last frame 



---
#### Method Draw.SkiaScroll.PositionViewport(SkiaSharp.SKRect,SkiaSharp.SKPoint,System.Single,System.Single)

 Input offset parameters in PIXELS. We render the scroll Content using pixal snapping but the prepared content will be scrolled (offset) using subpixels for a smooth look. Creates a valid ViewportRect inside. 

|Name | Description |
|-----|------|
|destination: ||
|offsetPtsX: ||
|offsetPtsY: ||
|viewportScale: ||
|scale: ||


---
#### Method Draw.SkiaScroll.OnScrolled

 Notify current scroll offset to some dependent views. 



---
#### Property Draw.SkiaScroll.ContentRectWithOffset

 The viewport for content 



---
#### Method Draw.SkiaScroll.SetContent(DrawnUi.Maui.Draw.SkiaControl)

 Use Content property for direct access 

|Name | Description |
|-----|------|
|view: ||


---
#### Property Draw.SkiaScroll.ScrollingSpeedMs

 Used by range scroller (ScrollToX, ScrollToY) 



---
#### Property Draw.SkiaScroll.VelocityImageLoaderLock

 Range at which the image loader will stop or resume loading images while scrolling 



---
#### Property Draw.SkiaScroll.Orientation

Gets or sets the scrolling direction of the ScrollView. This is a bindable property.





---
#### Property Draw.SkiaScroll.ScrollType

Gets or sets the scrolling direction of the ScrollView. This is a bindable property.





---
#### Property Draw.SkiaScroll.Virtualisation

 Default is true, children get the visible viewport area for rendering and can virtualize. If set to false children get the full content area for rendering and draw all at once. 



---
#### Method Draw.SkiaScroll.ScrollToX(System.Single,System.Boolean)

 Use Range scroller, offset in Units 

|Name | Description |
|-----|------|
|offset: ||
|animate: ||


---
#### Method Draw.SkiaScroll.ScrollToY(System.Single,System.Boolean)

 Use Range scroller, offset in Units 

|Name | Description |
|-----|------|
|offset: ||
|animate: ||


---
#### Method Draw.SkiaScroll.GetClosestSidePoint(SkiaSharp.SKPoint,SkiaSharp.SKRect,SkiaSharp.SKSize)

 This uses whole viewport size, do not use this for snapping 

|Name | Description |
|-----|------|
|overscrollPoint: ||
|contentRect: ||
|viewportSize: ||
**Returns**: 



---
#### Method Draw.SkiaScroll.GetContentOffsetBounds

 There are the bounds the scroll offset can go to.. This is NOT the bounds for the whole content. 



---
#### Field Draw.SkiaScroll.OrderedScrollTo

 We might order a scroll before the control was drawn, so it's a kind of startup position saved every time one calls ScrollTo 



---
#### Field Draw.SkiaScroll.OrderedScrollToIndex

 We might order a scroll before the control was drawn, so it's a kind of startup position saved every time one calls ScrollToIndex 



---
#### Method Draw.SkiaScroll.ScrollToOffset(System.Numerics.Vector2,System.Single)

 In Units 

|Name | Description |
|-----|------|
|offset: ||
|animate: ||


---
## Type Draw.SkiaScrollLooped

 Cycles content, so the scroll never ands but cycles from the start 



---
#### Property Draw.SkiaScrollLooped.CycleSpace

 Space between cycles, pixels 



---
#### Property Draw.SkiaScrollLooped.IsBanner

 Whether this should look like an infinite scrolling text banner 



---
## Type Draw.SkiaBackdrop

 Warning with CPU-rendering edges will not be blurred: https://issues.skia.org/issues/40036320 



---
#### Field Draw.SkiaBackdrop.ImagePaint

 Reusing this 



---
#### Field Draw.SkiaBackdrop.PaintImageFilter

 Reusing this 



---
#### Field Draw.SkiaBackdrop.PaintColorFilter

 Reusing this 



---
#### Property Draw.SkiaBackdrop.UseContext

 Use either context of global Superview background, default is True. 



---
#### Method Draw.SkiaBackdrop.AttachSource

 Designed to be just one-time set 



---
#### Method Draw.SkiaBackdrop.GetImage

 Returns the snapshot that was used for drawing the backdrop. If we have no effects or the control has not yet been drawn the return value will be null. You are responsible to dispose the returned image! 

**Returns**: 



---
#### Field Draw.SkiaHotspot.DelayCallbackMs

 You might want to pause to show effect before executing command. Default is 0. 



---
#### Field Draw.SkiaHotspotZoom.LastValue

 Last ViewportZoom value we are animating from 



---
#### Property Draw.SkiaHotspotZoom.ZoomSpeed

 How much of finger movement will afect zoom change 



---
## Type Draw.SkiaShape

 Implements ISkiaGestureListener to pass gestures to children 

 Implements ISkiaGestureListener to pass gestures to children 



---
#### Property Draw.SkiaShape.PathData

 For Type = Path, use the path markup syntax 



---
#### Property Draw.SkiaShape.ClipBackgroundColor

 This is for the tricky case when you want to drop shadow but keep background transparent to see through, set to True in that case. 



---
#### Property Draw.SkiaShape.DrawPath

 Parsed PathData 



---
#### Method Draw.SkiaSvg.LoadSource(System.String)

 This is not replacing current animation, use SetAnimation for that. 

|Name | Description |
|-----|------|
|fileName: ||
**Returns**: 



---
#### Property Draw.LineGlyph.Width

 Measured text with advance 



---
#### Property Draw.SkiaLabel.Font

 TODO IText? 



---
#### Method Draw.SkiaLabel.DrawText(SkiaSharp.SKCanvas,System.Single,System.Single,System.String,SkiaSharp.SKPaint,SkiaSharp.SKPaint,SkiaSharp.SKPaint,System.Single)

 If strokePaint==null will not stroke 

|Name | Description |
|-----|------|
|canvas: ||
|x: ||
|y: ||
|text: ||
|textPaint: ||
|strokePaint: ||
|scale: ||


---
#### Method Draw.SkiaLabel.DrawCharacter(SkiaSharp.SKCanvas,System.Int32,System.Int32,System.String,System.Single,System.Single,SkiaSharp.SKPaint,SkiaSharp.SKPaint,SkiaSharp.SKPaint,SkiaSharp.SKRect,System.Single)

 This is called when CharByChar is enabled You can override it to apply custom effects to every letter /// 

|Name | Description |
|-----|------|
|canvas: ||
|lineIndex: ||
|letterIndex: ||
|text: ||
|x: ||
|y: ||
|paint: ||
|paintStroke: ||
|scale: ||


---
#### Property Draw.SkiaLabel.SpaceBetweenParagraphs

 todo move this to some font info data block otherwise we wont be able to have multiple fonts 



---
#### Property Draw.SkiaLabel.LineHeightWithSpacing

 todo move this to some font info data block otherwise we wont be able to have multiple fonts 



---
#### Method Draw.SkiaLabel.OnFontUpdated

 A new TypeFace was set 



---
#### Method Draw.SkiaLabel.MeasureText(SkiaSharp.SKPaint,System.String,SkiaSharp.SKRect@)

 Accounts paint transforms like skew etc 

|Name | Description |
|-----|------|
|paint: ||
|text: ||
|bounds: ||


---
#### Method Draw.SkiaLabel.AddEmptyLine(System.Collections.Generic.List{DrawnUi.Maui.Draw.TextLine},DrawnUi.Maui.Draw.TextSpan,System.Single,System.Single,System.Boolean,System.Boolean)

 Returns new totalHeight 

|Name | Description |
|-----|------|
|result: ||
|span: ||
|totalHeight: ||
|heightBlock: ||
|isNewParagraph: ||
|needsShaping: ||


---
#### Property Draw.SkiaLabel.CharacterSpacing

 This applies ONLY when CharByChar is enabled 



---
#### Property Draw.SkiaLabel.FallbackCharacter

 Character to show when glyph is not found in font 



---
#### Property Draw.SkiaLabel.MonoForDigits

 The character to be taken for its width when want digits to simulate Mono, for example "8", default is null. 



---
#### Property Draw.SkiaLabel.LineHeightUniform

 Should we draw with the maximum line height when lines have different height. 



---
#### Property Draw.SkiaLabel.DropShadowOffsetX

 To make DropShadow act like shadow 



---
#### Method Draw.SkiaLabel.OnSpanTapped(DrawnUi.Maui.Draw.TextSpan)

 Return null if you wish not to consume 

|Name | Description |
|-----|------|
|span: ||
**Returns**: 



---
#### Method Draw.SkiaLabel.EmojiData.IsEmojiModifierSequence(System.String,System.Int32)

 Returns the length of EmojiModifierSequence if found at index ins 

|Name | Description |
|-----|------|
|text: ||
|index: ||
**Returns**: 



---
#### Property Draw.SkiaLabel.DecomposedText.HasMoreVerticalSpace

 pixels 



---
#### Property Draw.SkiaLabel.DecomposedText.HasMoreHorizontalSpace

 pixels 



---
#### Property Draw.TextLine.Bounds

 Set during rendering 



---
#### Property Draw.TextSpan.Glyphs

 Ig can be drawn char by char with char spacing etc we use this 



---
#### Property Draw.TextSpan.Shape

 If text can be drawn only shaped we use this 



---
#### Method Draw.TextSpan.CheckGlyphsCanBeRendered

 Parse glyphs, setup typeface, replace unrenderable glyphs with fallback character 



---
#### Method Draw.TextSpan.SetupPaint(System.Double,SkiaSharp.SKPaint)

 Update the paint with current format properties 



---
#### Property Draw.TextSpan.UnderlineWidth

 In points 



---
#### Property Draw.TextSpan.StrikeoutWidth

 In points 



---
#### Property Draw.TextSpan.HasTapHandler

 Will listen to gestures 



---
#### Property Draw.TextSpan.ForceCaptureInput

 When no tap handler or command are set this forces to listen to taps anyway 



---
#### Property Draw.TextSpan.DrawingOffset

 Rendering offset, set when combining spans. Ofset of the first line. 



---
#### Field Draw.TextSpan.Rects

 Relative to DrawingRect 



---
#### Property Draw.TextSpan.AutoFindFont

 If any glyph cannot be rendered with selected font try find system font that supports it and switch to it for the whole span 



---
#### Property Draw.PendulumAnimator.IsOneDirectional

 Returns absolute value, instead of going -/+ along the axis. Basically if true simulates bouncing. 



---
#### Property Draw.PendulumAnimator.InitialVelocity

 the higher the faster will stop 



---
## Type Draw.RenderingAnimator

 This animator renders on canvas instead of just updating a value 



---
#### Method Draw.RenderingAnimator.OnRendering(DrawnUi.Maui.Draw.IDrawnBase,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)

 return true if has drawn something and rendering needs to be applied 

|Name | Description |
|-----|------|
|control: ||
|context: ||
|scale: ||
**Returns**: 



---
#### Property Draw.RippleAnimator.X

 In pts relative to control X,Y. These are coords inside the control and not inside the canvas. 



---
#### Property Draw.RippleAnimator.Y

 In pts relative to control X,Y. These are coords inside the control and not inside the canvas. 



---
#### Property Draw.SkiaValueAnimator.CycleFInished

 Animator self finished a cycle, might still repeat 



---
#### Property Draw.SkiaValueAnimator.Finished

 Animator self finished running without being stopped manually 



---
#### Property Draw.SkiaValueAnimator.Repeat

 -1 means forever.. 



---
#### Method Draw.SkiaValueAnimator.TransformReportedValue(System.Int64)

 /// Passed over mValue, you can change the reported passed value here 

|Name | Description |
|-----|------|
|deltaT: ||
**Returns**: modified mValue for callback consumer



---
#### Method Draw.SkiaValueAnimator.UpdateValue(System.Int64,System.Int64)

 Update mValue using time distance between rendered frames. Return true if anims is finished. 

|Name | Description |
|-----|------|
|deltaT: ||
**Returns**: 



---
#### Property Draw.SkiaValueAnimator.Progress

 We are using this internally to apply easing. Can be above 1 when finishing. If you need progress 0-1 use ProgressAnimator. 



---
## Type Draw.ActionOnTickAnimator

 Just register this animator to run custom code on every frame creating a kind of game loop if needed. 



---
#### Method Draw.AnimateExtensions.AnimateWith(DrawnUi.Maui.Draw.SkiaControl,System.Func{DrawnUi.Maui.Draw.SkiaControl,System.Threading.Tasks.Task}[])

 Animate several tasks at the same time with WhenAll 

|Name | Description |
|-----|------|
|control: ||
|animations: ||
**Returns**: 



---
#### Property Draw.IAfterEffectDelete.TypeId

 For faster scanning of anims of same type 



---
#### Method Draw.IAfterEffectDelete.Render(DrawnUi.Maui.Draw.IDrawnBase,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)

 Called when drawing parent control frame 

|Name | Description |
|-----|------|
|control: ||
|canvas: ||
|scale: ||


---
#### Method Draw.ICanRenderOnCanvas.Render(DrawnUi.Maui.Draw.IDrawnBase,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)

 Renders effect overlay to canvas, return true if has drawn something and rendering needs to be applied. 

|Name | Description |
|-----|------|
|control: ||
|context: ||
|scale: ||
**Returns**: 



---
#### Method Draw.ISkiaAnimator.TickFrame(System.Int64)



|Name | Description |
|-----|------|
|frameTime: ||
**Returns**: Is Finished



---
#### Method Draw.ISkiaAnimator.Pause

 Used by ui, please use play stop for manual control 



---
#### Method Draw.ISkiaAnimator.Resume

 Used by ui, please use play stop for manual control 



---
#### Property Draw.ISkiaAnimator.IsDeactivated

 Can and will be removed 



---
#### Property Draw.ISkiaAnimator.IsPaused

 Just should not execute on tick 



---
#### Property Draw.ISkiaAnimator.IsHiddenInViewTree

 For internal use by the engine 



---
#### Method Draw.DecelerationTimingVectorParameters.ValueAt(System.Single)

 time is in seconds 

|Name | Description |
|-----|------|
|offsetSecs: ||
|time: ||
**Returns**: 



---
## Type Draw.ViscousFluidInterpolator

 Ported from google android 



---
#### Field Draw.ViscousFluidInterpolator.VISCOUS_FLUID_SCALE

 Controls the viscous fluid effect (how much of it). 

---
#### Method Draw.ChainAdjustBrightnessEffect.CreateLightnessFilter(System.Single)

 -1 -> 0 -> 1 

|Name | Description |
|-----|------|
|value: ||
**Returns**: 



---
#### Method Draw.ChainAdjustBrightnessEffect.CreateBrightnessFilter(System.Single)

 -1 -> 0 -> 1 

|Name | Description |
|-----|------|
|value: ||
**Returns**: 



---
#### Method Draw.ChainAdjustLightnessEffect.CreateLightnessFilter(System.Single)

 -1 -> 0 -> 1 

|Name | Description |
|-----|------|
|value: ||
**Returns**: 



---
#### Property Draw.ChainEffectResult.DrawnControl

 Set this to true if you drawn the control of false if you just rendered something else 



---
#### Method Draw.ICanBeUpdated.Update

 Force redrawing, without invalidating the measured size 

**Returns**: 



---
#### Method Draw.IRenderEffect.Draw(SkiaSharp.SKRect,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Action{DrawnUi.Maui.Draw.SkiaDrawingContext})

 Returns true if has drawn control itsself, otherwise it will be drawn over it 

|Name | Description |
|-----|------|
|destination: ||
|ctx: ||
|drawControl: ||
**Returns**: 



---
#### Method Draw.IStateEffect.UpdateState

 Will be invoked before actually painting but after gestures processing and other internal calculations. By SkiaControl.OnBeforeDrawing method. Beware if you call Update() inside will never stop updating. 



---
#### Property Draw.SkiaEffect.Parent

 For public set use Attach/Detach 



---
#### Property Draw.SkiaShaderEffect.UseContext

 Use either context of global Superview background, default is True. 



---
#### Property Draw.SkiaShaderEffect.AutoCreateInputTexture

 Should create a texture from the current drawing to pass to shader as uniform shader iImage1, default is True. You need this set to False only if your shader is output-only. 



---
#### Method Draw.SkiaShaderEffect.CreateSnapshot(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)

 Create snapshot from the current parent control drawing state to use as input texture for the shader 

|Name | Description |
|-----|------|
|ctx: ||
|destination: ||
**Returns**: 



---
#### Method Draw.SkiaShaderEffect.Render(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)

 EffectPostRenderer 

|Name | Description |
|-----|------|
|ctx: ||
|destination: ||


---
#### Method Draw.SkiaFontManager.GetWeightEnum(System.Int32)

 Gets the closest enum value to the given weight. Like 590 would return Semibold. 

|Name | Description |
|-----|------|
|weight: ||
**Returns**: 



---
#### Method Draw.SkiaFontManager.GetEmbeddedResourceStream(System.String)

 Takes the full name of a resource and loads it in to a stream. 

|Name | Description |
|-----|------|
|resourceName: |Assuming an embedded resource is a file called info.png and is located in a folder called Resources, it will be compiled in to the assembly with this fully qualified name: Full.Assembly.Name.Resources.info.png. That is the string that you should pass to this method.|
**Returns**: 



---
#### Method Draw.SkiaFontManager.GetEmbeddedResourceNames

 Get the list of all emdedded resources in the assembly. 

**Returns**: An array of fully qualified resource names



---
## Type Draw.AddGestures

 For fast and lazy gestures handling to attach to dran controls inside the canvas only 



---
## Type Draw.GesturesMode

 Used by the canvas, do not need this for drawn controls 



---
#### Field Draw.GesturesMode.Disabled

 Default 



---
#### Field Draw.GesturesMode.Enabled

 Gestures attached 



---
#### Field Draw.GesturesMode.Lock

 Lock input for self, useful inside scroll view, panning controls like slider etc 



---
#### Field Draw.GesturesMode.Share

 Tries to let other views consume the touch event if this view doesn't handle it 



---
#### Property Draw.IHasBanner.Banner

 Main image 



---
#### Property Draw.IHasBanner.BannerPreloadOrdered

 Indicates that it's already preloading 



---
#### Field Draw.SkiaImageManager.ReuseBitmaps

 If set to true will not return clones for same sources, but will just return the existing cached SKBitmap reference. Useful if you have a lot on images reusing same sources, but you have to be carefull not to dispose the shared image. SkiaImage is aware of this setting and will keep a cached SKBitmap from being disposed. 



---
#### Field Draw.SkiaImageManager.CacheLongevitySecs

 Caching provider setting 



---
#### Field Draw.SkiaImageManager.NativeFilePrefix

 Convention for local files saved in native platform. Shared resources from Resources/Raw/ do not need this prefix. 



---
#### Method Draw.SkiaImageManager.LoadImageAsync(Microsoft.Maui.Controls.ImageSource,System.Threading.CancellationToken)

 Direct load, without any queue or manager cache, for internal use. Please use LoadImageManagedAsync instead. 

|Name | Description |
|-----|------|
|source: ||
|token: ||
**Returns**: 



---
#### Method Draw.SkiaImageManager.LoadImageManagedAsync(Microsoft.Maui.Controls.ImageSource,System.Threading.CancellationTokenSource,DrawnUi.Maui.Draw.LoadPriority)

 Uses queue and manager cache 

|Name | Description |
|-----|------|
|source: ||
|token: ||
**Returns**: 



---
#### Method Draw.SkiaImageManager.AddToCache(System.String,SkiaSharp.SKBitmap,System.Int32)

 Returns false if key already exists 

|Name | Description |
|-----|------|
|uri: ||
|bitmap: ||
|cacheLongevityMinutes: ||
**Returns**: 



---
## Type Draw.MauiKey

 These are platform-independent. They correspond to JavaScript keys. 



---
#### Field Draw.AutoSizeType.FitFillHorizontal

 This might be faster than FitHorizontal or FillHorizontal for dynamically changing text 



---
#### Field Draw.AutoSizeType.FitFillVertical

 This might be faster than FitVertical or FillVertical for dynamically changing text 



---
#### Field Draw.AutoSizeType.FitHorizontal

 todo FIX NOT WORKING!!! If you have dynamically changing text think about using FitFillHorizontal instead 



---
#### Field Draw.AutoSizeType.FillHorizontal

 If you have dynamically changing text think about using FitFillHorizontal instead 



---
#### Field Draw.AutoSizeType.FitVertical

 If you have dynamically changing text think about using FitFillVertical instead 



---
#### Field Draw.AutoSizeType.FillVertical

 If you have dynamically changing text think about using FitFillVertical instead 



---
#### Field Draw.FontWeight.Thin

 The thin 



---
#### Field Draw.FontWeight.ExtraLight

 Also known as Ultra Light 



---
#### Field Draw.FontWeight.Light

 The light 



---
#### Field Draw.FontWeight.Regular

 Also known as Normal 



---
#### Field Draw.FontWeight.Medium

 The medium 



---
#### Field Draw.FontWeight.SemiBold

 The semi bold 



---
#### Field Draw.FontWeight.Bold

 The bold 



---
#### Field Draw.FontWeight.ExtraBold

 Also known as Heavy or UltraBold 



---
#### Field Draw.FontWeight.Black

 The black 



---
#### Field Draw.HardwareAccelerationMode.Disabled

 Default 



---
#### Field Draw.HardwareAccelerationMode.Enabled

 Gestures attached 



---
#### Field Draw.HardwareAccelerationMode.Prerender

 A non-accelerated view will be created first to avoid blank screen while graphic context is being initialized, then swapped with accelerated view 



---
#### Field Draw.LayoutType.Absolute

 Fastest rendering 



---
#### Field Draw.LayoutType.Column

 Vertical stack 



---
#### Field Draw.LayoutType.Row

 Horizontal stack 



---
#### Field Draw.LayoutType.Wrap

 Think of wrap panel 



---
#### Field Draw.LayoutType.Grid

 Use usual grid properties like Grid.Stack, ColumnSpacing etc 



---
#### Field Draw.LockTouch.Enabled

 Pass nothing below and mark all gestures as consumed by this control 



---
#### Field Draw.LockTouch.PassNone

 Pass nothing below 



---
#### Field Draw.LockTouch.PassTap

 Pass only Tapped below 



---
#### Field Draw.LockTouch.PassTapAndLongPress

 Pass only Tapped and LongPressing below 



---
#### Field Draw.PanningModeType.Enabled

 1 and 2 fingers 



---
#### Field Draw.RecycleTemplateType.None

 One cell per item will be created, while a SkiaControl has little memory consumption for some controls like SkiaLayer it might take more, so you might consider recycling for large number o items 



---
#### Field Draw.RecycleTemplateType.FillViewport

 Create cells instances until viewport is filled, then recycle while scrolling 



---
#### Field Draw.RecycleTemplateType.Single

 Try using one cell per template at all times, binding context will change just before drawing. ToDo investigate case of async data changes like images loading from web. 



---
#### Field Draw.ShapeType.Squricle

 TODO 



---
#### Field Draw.SkiaImageEffect.Tint

 Background color will be used to tint 



---
#### Method Draw.SkiaImageEffects.Tint(Microsoft.Maui.Graphics.Color,SkiaSharp.SKBlendMode)

 If you want to Tint: SKBlendMode.SrcATop + ColorTint with alpha below 1 

|Name | Description |
|-----|------|
|color: ||
|mode: ||
**Returns**: 



---
#### Method Draw.SkiaImageEffects.Grayscale

 This effect turns an image to grayscale. This particular version uses the NTSC/PAL/SECAM standard luminance value weights: 0.2989 for red, 0.587 for green, and 0.114 for blue. 

**Returns**: 



---
#### Method Draw.SkiaImageEffects.Grayscale2

 This effect turns an image to grayscale. 

**Returns**: 



---
#### Method Draw.SkiaImageEffects.Sepia

 The sepia effect can give your photos a warm, brownish tone that mimics the look of an older photo. 

**Returns**: 



---
#### Method Draw.SkiaImageEffects.InvertColors

 This effect inverts the colors in an image. NOT WORKING! 

**Returns**: 



---
#### Method Draw.SkiaImageEffects.Contrast(System.Single)

 This effect adjusts the contrast of an image. amount is the adjustment level. Negative values decrease contrast, positive values increase contrast, and 0 means no change. 

|Name | Description |
|-----|------|
|amount: ||
**Returns**: 



---
#### Method Draw.SkiaImageEffects.Saturation(System.Single)

 This effect adjusts the saturation of an image. amount is the adjustment level. Negative values desaturate the image, positive values increase saturation, and 0 means no change. 

|Name | Description |
|-----|------|
|amount: ||
**Returns**: 



---
#### Method Draw.SkiaImageEffects.Brightness(System.Single)

 This effect increases the brightness of an image. amount is between 0 (no change) and 1 (white). 

|Name | Description |
|-----|------|
|amount: ||
**Returns**: 



---
#### Method Draw.SkiaImageEffects.Lightness(System.Single)

 Adjusts the brightness of an image: 

|Name | Description |
|-----|------|
|amount: ||
**Returns**: 



---
#### Method Draw.SkiaImageEffects.Gamma(System.Single)

 This effect applies gamma correction to an image. gamma must be greater than 0. A . 

|Name | Description |
|-----|------|
|gamma: ||
**Returns**: 

[[T:System.ArgumentOutOfRangeException|T:System.ArgumentOutOfRangeException]]: 



---
#### Field Draw.SpaceDistribution.Auto

 Distribute space evenly between all items but do not affect empty space remaining at the end of the last line 



---
#### Field Draw.SpaceDistribution.Full

 Distribute space evenly between all items but do not affect empty space 



---
#### Field Draw.TransformAspect.Fill

 Enlarges to fill the viewport without maintaining aspect ratio if smaller, but does not scale down if larger 



---
#### Field Draw.TransformAspect.Fit

 Fit without maintaining aspect ratio and without enlarging if smaller 



---
#### Field Draw.TransformAspect.AspectFit

 Fit inside viewport respecting aspect without enlarging if smaller, could result in the image having some blank space around 



---
#### Field Draw.TransformAspect.AspectFill

 Covers viewport respecting aspect without scaling down if bigger, could result in the image being cropped 



---
#### Field Draw.TransformAspect.AspectFitFill

 AspectFit + AspectFill. Enlarges to cover the viewport or reduces size to fit inside the viewport both respecting aspect ratio, ensuring the entire image is always visible, potentially leaving some parts of the viewport uncovered. 



---
#### Field Draw.TransformAspect.FitFill

 Fit + Fill. Enlarges to cover the viewport or reduces size to fit inside the viewport without respecting aspect ratio, ensuring the entire image is always visible, potentially leaving some parts of the viewport uncovered. 



---
#### Field Draw.TransformAspect.Cover

 Enlarges to cover the viewport if smaller and reduces size if larger, all without respecting aspect ratio. Same as AspectFitFill but will crop the image to fill entire viewport. 



---
#### Field Draw.TransformAspect.AspectCover

 Enlarges to cover the viewport or reduces size to fit inside the viewport both respecting aspect ratio. Always covers the entire viewport, potentially cropping the image if it's larger, never leaves empty space inside viewport. 



---
#### Field Draw.TransformAspect.Tile

 TODO very soon! 



---
#### Field Draw.VirtualisationType.Disabled

 Visible parent bounds are not accounted for, children are rendred as usual. 



---
#### Field Draw.VirtualisationType.Enabled

 Children not withing visible parent bounds are not rendered 



---
#### Field Draw.VirtualisationType.Smart

 Only the creation of a cached object is permitted for children not within visible parent bounds 



---
#### Property Draw.UiSettings.DesktopWindow

 For desktop: if set will affect the app window at startup. 



---
#### Property Draw.UiSettings.MobileIsFullscreen

 Avoid safe insets and remove system ui like status bar etc if supported by platform 



---
#### Property Draw.UiSettings.UseDesktopKeyboard

 Listen to desktop keyboard keys with KeyboardManager. Windows and Catalyst available. 



---
#### Property Draw.WindowParameters.IsFixedSize

 For desktop: if you set this to true the app window will not be allowed to be resized manually. 



---
## Type Draw.Sk3dView

 Under construction, custom implementation of removed API 



---
#### Method Draw.Sk3dView.Reset

 Resets the current state and clears all saved states 



---
#### Field Draw.Sk3dView.CameraDistance

 2D magic number camera distance 8 inches 



---
## Type Draw.IAnimatorsManager

 This control is responsible for updating screen for running animators 



---
#### Method Draw.IDefinesViewport.InvalidateByChild(DrawnUi.Maui.Draw.SkiaControl)

 So child can call parent to invalidate scrolling offset etc if child size changes 



---
#### Method Draw.IDrawnBase.GetOnScreenVisibleArea(System.Single)

 Obtain rectangle visible on the screen to avoid offscreen rendering etc 

**Returns**: 



---
#### Method Draw.IDrawnBase.Invalidate

 Invalidates the measured size. May or may not call Update() inside, depends on control 



---
#### Method Draw.IDrawnBase.InvalidateParents

 If need the re-measure all parents because child-auto-size has changed 



---
#### Method Draw.IDrawnBase.ClipSmart(SkiaSharp.SKCanvas,SkiaSharp.SKPath,SkiaSharp.SKClipOperation)

 Clip using internal custom settings of the control 

|Name | Description |
|-----|------|
|canvas: ||
|path: ||
|operation: ||


---
#### Method Draw.IDrawnBase.CreateClip(System.Object,System.Boolean,SkiaSharp.SKPath)

 Creates a new disposable SKPath for clipping content according to the control shape and size. Create this control clip for painting content. Pass arguments if you want to use some time-frozen data for painting at any time from any thread.. If applyPosition is false will create clip without using drawing posiition, like if was drawing at 0,0. 

**Returns**: 



---
#### Property Draw.IDrawnBase.PostAnimators

 Executed after the rendering 



---
#### Property Draw.IDrawnBase.Views

 For code-behind access of children, XAML is using Children property 



---
#### Method Draw.IDrawnBase.AddSubView(DrawnUi.Maui.Draw.SkiaControl)

 Directly adds a view to the control, without any layouting. Use this instead of Views.Add() to avoid memory leaks etc 

|Name | Description |
|-----|------|
|view: ||


---
#### Method Draw.IDrawnBase.RemoveSubView(DrawnUi.Maui.Draw.SkiaControl)

 Directly removes a view from the control, without any layouting. Use this instead of Views.Remove() to avoid memory leaks etc 

|Name | Description |
|-----|------|
|view: ||


---
#### Method Draw.IDrawnBase.InvalidateByChild(DrawnUi.Maui.Draw.SkiaControl)

 This is needed by layout to track which child changed to sometimes avoid recalculating other children 

|Name | Description |
|-----|------|
|skiaControl: ||


---
#### Method Draw.IDrawnBase.UpdateByChild(DrawnUi.Maui.Draw.SkiaControl)

 To track dirty area when Updating parent 

|Name | Description |
|-----|------|
|skiaControl: ||


---
#### Method Draw.IInsideViewport.OnLoaded

 Loaded is called when the view is created, but not yet visible 



---
#### Method Draw.IInsideWheelStack.OnPositionChanged(System.Single,System.Boolean)

 Called by parent stack inside picker wheel when position changes 

|Name | Description |
|-----|------|
|offsetRatio: |0.0-X.X offset from selection axis, beyond 1.0 is offscreen. Normally you would change opacity accordingly to this.|
|isSelected: |Whether cell is currently selected, normally you would change text color accordingly.|


---
#### Method Draw.ILayoutInsideViewport.GetVisibleChildIndexAt(SkiaSharp.SKPoint)

 The point here is the rendering location, always on screen 

|Name | Description |
|-----|------|
|point: ||
**Returns**: 



---
#### Method Draw.ILayoutInsideViewport.GetChildIndexAt(SkiaSharp.SKPoint)

 The point here is the position inside parent, can be offscreen 

|Name | Description |
|-----|------|
|point: ||
**Returns**: 



---
#### Method Draw.IRefreshIndicator.SetDragRatio(System.Single)

 0 - 1 

|Name | Description |
|-----|------|
|ratio: ||


---
#### Property Draw.ISkiaControl.IsGhost

 Takes place in layout, acts like is visible, but just not rendering 



---
#### Method Draw.ISkiaControl.Measure(System.Single,System.Single,System.Single)

 Expecting PIXELS as input sets NeedMeasure to false 

|Name | Description |
|-----|------|
|widthConstraint: ||
|heightConstraint: ||
**Returns**: 



---
#### Property Draw.ISkiaDrawable.OnDraw

 Return true if need force invalidation on next frame 



---
#### Method Draw.ISkiaGestureListener.OnSkiaGestureEvent(DrawnUi.Maui.Draw.SkiaGesturesParameters,DrawnUi.Maui.Draw.GestureEventProcessingInfo)

 Called when a gesture is detected. 

|Name | Description |
|-----|------|
|type: ||
|args: ||
|args.Action.Action: ||
|inside: ||
**Returns**: WHO CONSUMED if gesture consumed and blocked to be passed, NULL if gesture not locked and could be passed below. If you pass this to subview you must set your own offset parameters, do not pass what you received its for this level use.



---
#### Method Draw.ISkiaGestureListener.OnFocusChanged(System.Boolean)

 This will be called only for views registered at Superview.FocusedChild. The view must return true of false to indicate if it accepts focus. 

|Name | Description |
|-----|------|
|focus: ||
**Returns**: 



---
#### Property Draw.ISkiaGridLayout.RowDefinitions

 An IGridRowDefinition collection for the GridLayout instance. 



---
#### Property Draw.ISkiaGridLayout.ColumnDefinitions

 An IGridColumnDefinition collection for the GridLayout instance. 



---
#### Property Draw.ISkiaGridLayout.RowSpacing

 Gets the amount of space left between rows in the GridLayout. 



---
#### Property Draw.ISkiaGridLayout.ColumnSpacing

 Gets the amount of space left between columns in the GridLayout. 



---
#### Method Draw.ISkiaGridLayout.GetRow(Microsoft.Maui.Controls.BindableObject)

 Gets the row of the child element. 

|Name | Description |
|-----|------|
|view: |A view that belongs to the Grid layout.|
**Returns**: An integer that represents the row in which the item will appear.



---
#### Method Draw.ISkiaGridLayout.GetRowSpan(Microsoft.Maui.Controls.BindableObject)

 Gets the row span of the child element. 

|Name | Description |
|-----|------|
|view: |A view that belongs to the Grid layout.|
**Returns**: The row that the child element is in.



---
#### Method Draw.ISkiaGridLayout.GetColumn(Microsoft.Maui.Controls.BindableObject)

 Gets the column of the child element. 

|Name | Description |
|-----|------|
|view: |A view that belongs to the Grid layout.|
**Returns**: The column that the child element is in.



---
#### Method Draw.ISkiaGridLayout.GetColumnSpan(Microsoft.Maui.Controls.BindableObject)

 Gets the row span of the child element. 

|Name | Description |
|-----|------|
|view: |A view that belongs to the Grid layout.|
**Returns**: The row that the child element is in.



---
#### Property Draw.ISkiaLayer.LayerPaintArgs

 Cached layer image 



---
#### Property Draw.ISkiaLayer.HasValidSnapshot

 Snapshot was taken 



---
#### Method Draw.ISkiaSharpView.Update(System.Int64)

 Safe InvalidateSurface() call. If nanos not specified will generate ittself 



---
#### Method Draw.ISkiaSharpView.CreateStandaloneSurface(System.Int32,System.Int32)

 This is needed to get a hardware accelerated surface or a normal one depending on the platform 

|Name | Description |
|-----|------|
|width: ||
|height: ||
**Returns**: 



---
#### Method Draw.IVisibilityAware.OnAppearing

 This can sometimes be omitted, 



---
#### Method Draw.IVisibilityAware.OnAppeared

 This event can sometimes be called without prior OnAppearing 



---
#### Property Draw.ControlInStack.ControlIndex

 Index inside enumerator that was passed for measurement OR index inside ItemsSource 



---
#### Property Draw.ControlInStack.Measured

 Measure result 



---
#### Property Draw.ControlInStack.Area

 Available area for Arrange 



---
#### Property Draw.ControlInStack.Destination

 PIXELS, this is to hold our arranged layout 



---
#### Property Draw.ControlInStack.View

 This will be null for recycled views 



---
#### Property Draw.ControlInStack.Drawn

 Was used for actual drawing 



---
#### Property Draw.ControlInStack.Offset

 For internal use by your custom controls 



---
#### Property Draw.SkiaDrawingContext.IsVirtual

 Recording cache 



---
#### Property Draw.SkiaDrawingContext.IsRecycled

 Reusing surface from previous cache 



---
#### Method Draw.DynamicGrid`1.GetColumnCountForRow(System.Int32)

 Returns the column count for the specified row. This value is cached and updated each time an item is added. 

|Name | Description |
|-----|------|
|row: |Row number to get the column count for.|
**Returns**: Number of columns in the specified row.



---
## Type Draw.BindToParentContextExtension

 Compiled-bindings-friendly implementation for "Source.Parent.BindingContext.Path" 



---
## Type Draw.DrawnFontAttributesConverter

 Forked from Microsoft.Maui.Controls as using original class was breaking XAML HotReload for some unknown reason 



---
#### Method Draw.Super.SetFullScreen(Android.App.Activity)

 ToDo resolve obsolete for android api 30 and later 

|Name | Description |
|-----|------|
|activity: ||


---
#### Method Draw.Super.OpenLink(System.String)

 Opens web link in native browser 

|Name | Description |
|-----|------|
|link: ||


---
#### Method Draw.Super.ListResources(System.String)

 Lists assets inside the Resources/Raw subfolder 

|Name | Description |
|-----|------|
|subfolder: ||
**Returns**: 



---
#### Field Draw.Super.CanUseHardwareAcceleration

 Can optionally disable hardware-acceleration with this flag, for example on iOS you would want to avoid creating many metal views. 



---
#### Method Draw.Super.DisplayException(Microsoft.Maui.Controls.Element,System.Exception)

 Display xaml page creation exception 

|Name | Description |
|-----|------|
|view: ||
|e: ||


---
#### Property Draw.Super.IsRtl

 RTL support UNDER CONSTRUCTION 



---
#### Property Draw.Super.FontSubPixelRendering

 Enables sub-pixel font rendering, might provide better antialiasing on some platforms. Default is True; 



---
#### Field Draw.Super.InsetsChanged

 Subscribe your navigation bar to react 



---
#### Field Draw.Super.CapMicroSecs

 Capping FPS, (1 / FPS * 1000_000) 



---
#### Property Draw.Super.NavBarHeight

 In DP 



---
#### Property Draw.Super.StatusBarHeight

 In DP 



---
#### Property Draw.Super.BottomTabsHeight

 In DP 



---
#### Event Draw.Super.OnNativeAppCreated

 App was launched and UI is ready to be created 



---
#### Method Draw.Super.ResizeWindow(Microsoft.Maui.Controls.Window,System.Int32,System.Int32,System.Boolean)

 For desktop platforms, will resize app window and eventually lock it from being resized. 

|Name | Description |
|-----|------|
|window: ||
|width: ||
|height: ||
|isFixed: ||


---
#### Field Draw.Super.OnMauiAppCreated

 Maui App was launched and UI is ready to be consumed 



---
#### Method Draw.Super.NeedGlobalUpdate

 This will force recalculate canvas visibility in ViewTree and update those visible 



---
#### Method Controls.SkiaCarousel.ApplyPosition(System.Numerics.Vector2)

 Will translate child and raise appearing/disappearing events 

|Name | Description |
|-----|------|
|currentPosition: ||


---
#### Method Controls.SkiaCarousel.OnTemplatesAvailable

 This might be called from background thread if we set InitializeTemplatesInBackgroundDelay true 



---
#### Property Controls.SkiaCarousel.ScrollAmount

 Scroll amount from 0 to 1 of the current (SelectedIndex) slide. Another similar but different property would be ScrollProgress. This is not linear as SelectedIndex changes earlier than 0 or 1 are attained. 



---
#### Property Controls.SkiaCarousel.ScrollProgress

 Scroll progress from 0 to (numberOfSlides-1). This is not dependent of the SelectedIndex, just reflects visible progress. Useful to create custom controls attached to carousel. Calculated as (for horizontal): CurrentPosition.X / SnapPoints.Last().X 



---
#### Method Controls.SkiaCarousel.GetContentOffsetBounds

 There are the bounds the scroll offset can go to.. This are NOT the bounds of the whole content. 



---
#### Method Controls.SkiaCarousel.InitializeChildren

 We expect this to be called after this alyout is invalidated 



---
#### Method Controls.SkiaCarousel.AdaptChildren

 Set children layout options according to our settings. Not used for template case. 



---
#### Property Controls.SkiaCarousel.PreloadNeighboors

 Whether should preload neighboors sides cells even when they are hidden, to preload images etc.. Default is true. Beware set this to False if you have complex layouts otherwise rendering might be slow. 



---
#### Property Controls.SkiaCarousel.DynamicSize

 When specific dimension is adapting to children size, will use max child dimension if False, otherwise will change size when children with different dimension size are selected. Default is false. If true, requires MeasureAllItems to be set to all items. 



---
#### Property Controls.SkiaCarousel.IsLooped

 UNIMPLEMENTED YET 



---
#### Property Controls.SkiaCarousel.IsVertical

 Orientation of the carousel 



---
#### Property Controls.SkiaCarousel.LinearSpeedMs

 How long would a whole auto-sliding take, if `Bounces` is `False`. If set (>0) will be used for automatic scrolls instead of using manual velocity. For bouncing carousel 



---
#### Property Controls.SkiaCarousel.SelectedIndex

 Zero-based index of the currently selected slide 



---
#### Property Controls.SkiaCarousel.IsRightToLeft

 TODO 



---
#### Property Controls.SkiaCarousel.SidesOffset

 Basically size margins of every slide, offset from the side of the carousel. Another similar but different property to use would be Spacing between slides. 



---
## Type Controls.SkiaDrawnCell

 Base ISkiaCell implementation 



---
## Type Controls.SkiaDynamicDrawnCell

 This cell can watch binding context property changing 



---
#### Property Controls.SkiaDrawer.AmplitudeSize

 If set to other than -1 will be used instead of HeaderSize for amplitude calculation, amplitude = drawer size - header. 



---
#### Property Controls.SkiaDrawer.HeaderSize

 Size of the area that will remain on screen when drawer is closed 



---
#### Method Controls.SkiaDrawer.GetOffsetToHide

 In points 

**Returns**: 



---
#### Method Controls.SkiaDrawer.GetClosestSidePoint(SkiaSharp.SKPoint,SkiaSharp.SKRect,SkiaSharp.SKSize)

 This uses whole viewport size, do not use this for snapping 

|Name | Description |
|-----|------|
|overscrollPoint: ||
|contentRect: ||
|viewportSize: ||
**Returns**: 



---
#### Method Controls.SkiaDrawer.ClampOffsetWithRubberBand(System.Single,System.Single)

 Called for manual finger panning 

|Name | Description |
|-----|------|
|x: ||
|y: ||
**Returns**: 



---
#### Property Controls.MauiEditor.MaxLines

 WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor 



---
#### Property Controls.MauiEntry.MaxLines

 WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor 



---
#### Event Controls.MauiEntry.OnCompleted

 Occurs when the user finalizes the text in an entry with the return key. 



---
#### Property Controls.SkiaMauiEditor.MaxLines

 WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor 



---
#### Method Controls.SkiaMauiEditor.OnControlFocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)

 Invoked by Maui control 

|Name | Description |
|-----|------|
|sender: ||
|e: ||


---
#### Method Controls.SkiaMauiEditor.OnControlUnfocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)

 Invoked by Maui control 

|Name | Description |
|-----|------|
|sender: ||
|e: ||


---
#### Method Controls.SkiaMauiEditor.OnFocusChanged(System.Boolean)

 Called by DrawnUi when the focus changes 

|Name | Description |
|-----|------|
|focus: ||


---
#### Property Controls.SkiaMauiEditor.LockFocus

 TODO 



---
## Type Controls.SkiaMauiEntry

 Used to draw maui element over a skia canvas. Positions elelement using drawnUi layout and sometimes just renders element bitmap snapshot instead of displaying the real element, for example, when scrolling/animating. 



---
#### Method Controls.SkiaMauiEntry.OnControlFocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)

 Invoked by Maui control 

|Name | Description |
|-----|------|
|sender: ||
|e: ||


---
#### Method Controls.SkiaMauiEntry.OnControlUnfocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)

 Invoked by Maui control 

|Name | Description |
|-----|------|
|sender: ||
|e: ||


---
#### Method Controls.SkiaMauiEntry.OnFocusChanged(System.Boolean)

 Called by DrawnUi when the focus changes 

|Name | Description |
|-----|------|
|focus: ||


---
#### Property Controls.SkiaMauiEntry.LockFocus

 TODO 



---
#### Property Controls.SkiaMauiEntry.MaxLines

 WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor 



---
## Type Controls.GridLayout

 Helper class for SkiaLayout Type = LayoutType.Grid 



---
## Type Controls.HStack

 Helper class for SkiaLayout Type = LayoutType.Stack, SplitMax = 0 



---
## Type Controls.VStack

 Helper class for SkiaLayout Type = LayoutType.Stack, SplitMax = 1 



---
## Type Controls.SkiaShell

 A Canvas with Navigation capabilities 



---
#### Field Controls.SkiaShell.PopupBackgroundColor

 Default background tint for freezing popups/modals etc 



---
#### Field Controls.SkiaShell.PopupsBackgroundBlur

 Default background blur amount for freezing popups/modals etc 



---
#### Property Controls.SkiaShell.NavigationLayout

 The main control that pushes pages, switches tabs etc 



---
#### Property Controls.SkiaShell.RootLayout

 Use this for covering everything in a modal way, precisely tabs 



---
#### Method Controls.SkiaShell.GetTopmostViewInternal

 Gets the topmost visible view: if no popups and modals are open then return NavigationLayout otherwise return the topmost popup or modal depending which ZIndexModals or ZIndexPopups is higher. If pushed view is inside a shell wrapper will return the wrapper. 

**Returns**: 



---
#### Method Controls.SkiaShell.GetTopmostView

 Gets the topmost visible view: if no popups and modals are open then return NavigationLayout otherwise return the topmost popup or modal depending which ZIndexModals or ZIndexPopups is higher. If view is inside a shell wrapper will return just the view. 

**Returns**: 



---
#### Method Controls.SkiaShell.PushAsync(Microsoft.Maui.Controls.BindableObject,System.Boolean,System.Boolean)

 Uses ViewSwitcher to push a view on the canvas, into the current tab if any. 

|Name | Description |
|-----|------|
|page: ||
|animated: ||
**Returns**: 



---
#### Method Controls.SkiaShell.PushAsync(System.String,System.Boolean,System.Collections.Generic.IDictionary{System.String,System.Object})

 Uses ViewSwitcher to push a view on the canvas, into the current tab if any. We can use a route with arguments to instantiate the view instead of passing an instance. 

|Name | Description |
|-----|------|
|page: ||
|animated: ||
**Returns**: 



---
#### Method Controls.SkiaShell.IHandleGoBack.OnShellGoBack(System.Boolean)

 Return true if comsumed, false will use default system behaivour. 

**Returns**: 



---
#### Method Controls.SkiaShell.GoBackInRoute(System.Boolean)

 This will not affect popups 

|Name | Description |
|-----|------|
|animate: ||
**Returns**: 



---
#### Method Controls.SkiaShell.PopAsync(System.Boolean)

 Returns the page so you could dispose it if needed. Uses ViewSwitcher. 

|Name | Description |
|-----|------|
|animated: ||
**Returns**: 



---
#### Method Controls.SkiaShell.WrapScreenshot(DrawnUi.Maui.Draw.SkiaControl,SkiaSharp.SKImage,Microsoft.Maui.Graphics.Color,System.Single,System.Boolean)

 Override this to create your own image with your own effect of the screenshot to be placed under modal controls. Default is image with Darken Effect. 

|Name | Description |
|-----|------|
|screenshot: ||
**Returns**: 



---
#### Method Controls.SkiaShell.PushModalAsync(Microsoft.Maui.Controls.BindableObject,System.Boolean,System.Boolean,System.Boolean,System.Collections.Generic.IDictionary{System.String,System.Object})

 Creates a SkiaDrawer opening over the RootLayout with the passed content. Override this method to create your own implementation. Default freezing background is True, control with frozenLayerBackgroundParameters parameter. 

|Name | Description |
|-----|------|
|page: ||
|animated: ||
**Returns**: 



---
#### Method Controls.SkiaShell.CanFreezeLayout

 Override this if you have custom navigation layers and custom logic to decide if we can unfreeze background. 

**Returns**: 



---
#### Method Controls.SkiaShell.CanUnfreezeLayout

 Override this if you have custom navigation layers and custom logic to decide if we can unfreeze background. 

**Returns**: 



---
#### Method Controls.SkiaShell.SetFrozenLayerVisibility(DrawnUi.Maui.Draw.SkiaControl,System.Boolean)

 Display or hide the background scrrenshot assotiated with an overlay control 

|Name | Description |
|-----|------|
|control: ||
|parameters: ||
**Returns**: 



---
#### Method Controls.SkiaShell.UnfreezeRootLayout(DrawnUi.Maui.Draw.SkiaControl,System.Boolean)

 pass who frozen the layout 

|Name | Description |
|-----|------|
|control: ||
|animated: ||
**Returns**: 



---
#### Method Controls.SkiaShell.FreezeRootLayout(DrawnUi.Maui.Draw.SkiaControl,System.Boolean,Microsoft.Maui.Graphics.Color,System.Single)

 Freezes layout below the overlay: takes screenshot of RootLayout, places it over, then hides RootLayout to avoid rendering it. Can override 



---
#### Method Controls.SkiaShell.OnLayersChanged(DrawnUi.Maui.Draw.SkiaControl)

 Setup _topmost and send OnAppeared / OnDisappeared to views. Occurs when layers configuration changes, some layer go visible, some not, some are added, some are removed. 



---
#### Property Controls.SkiaShell.FrozenLayers

 TODO make this non-concurrent 



---
#### Method Controls.SkiaShell.ClosePopupAsync(System.Boolean)

 Close topmost popup 

|Name | Description |
|-----|------|
|animated: ||
**Returns**: 



---
#### Method Controls.SkiaShell.OpenPopupAsync(DrawnUi.Maui.Draw.SkiaControl,System.Boolean,System.Boolean,System.Boolean,Microsoft.Maui.Graphics.Color,System.Nullable{SkiaSharp.SKPoint})

 Pass pixelsScaleInFrom you you want popup to animate appearing from a specific point instead of screen center. Set freezeBackground to False to keep animations running below popup, default is True for performance reasons. 

|Name | Description |
|-----|------|
|content: ||
|animated: ||
|closeWhenBackgroundTapped: ||
|scaleInFrom: ||
**Returns**: 



---
#### Method Controls.SkiaShell.SetupRoot(DrawnUi.Maui.Draw.ISkiaControl)

 Main control inside RootLayout 

|Name | Description |
|-----|------|
|shellLayout: ||


---
#### Method Controls.SkiaShell.SetRoot(System.String,System.Boolean,System.Collections.Generic.IDictionary{System.String,System.Object})

 Returns true if was replaced 

|Name | Description |
|-----|------|
|host: ||
|replace: ||
|arguments: ||
**Returns**: 

[[T:System.Exception|T:System.Exception]]: 



---
#### Method Controls.SkiaShell.GoToAsync(Microsoft.Maui.Controls.ShellNavigationState,System.Nullable{System.Boolean},System.Collections.Generic.IDictionary{System.String,System.Object})

 Navigate to a registered route. Arguments will be taken from query string of can be passed as parameter. You can receive them by implementing IQueryAttributable or using attribute [QueryProperty] in the page itsself or in the ViewModel that must be the screen's BindingContext upon registered screen instatiation. Animate can be specified otherwise will use it from Shell.PresentationMode attached property. This property will be also used for pushing as page, modal etc. 

|Name | Description |
|-----|------|
|state: ||
|animate: ||
|arguments: ||
**Returns**: 



---
#### Property Controls.SkiaShell.Buffer

 Can use to pass items as models between viewmodels 



---
#### Method Controls.SkiaShell.NavigationLayer`1.#ctor(DrawnUi.Maui.Controls.SkiaShell,System.Boolean)

 if isModel is true than will try to freeze background before showing. otherwise will be just an overlay like toast etc. 

|Name | Description |
|-----|------|
|shell: ||
|isModal: ||


---
#### Method Controls.SkiaShell.InitializeNative(Microsoft.Maui.IViewHandler)

 Fail to understand why destroy the splash background after app launched, not letting us animate app content to fade in after splash screen. All this code to avoid a blink between splash screen and mainpage showing. 

|Name | Description |
|-----|------|
|handler: ||


---
#### Property Controls.SkiaShellNavigatedArgs.Route

 Is never null. 



---
#### Property Controls.SkiaShellNavigatedArgs.View

 The SkiaControl that went upfront 



---
#### Property Controls.SkiaShellNavigatingArgs.Cancel

 If you set this to True the navigation will be canceled 



---
#### Property Controls.SkiaShellNavigatingArgs.Route

 Is never null 



---
#### Property Controls.SkiaShellNavigatingArgs.View

 The SkiaControl that will navigate 



---
#### Property Controls.SkiaShellNavigatingArgs.Previous

 The SkiaControl that is upfront now 



---
## Type Controls.AnimatedFramesRenderer

 Base class for playing frames. Subclass to play spritesheets, gifs, custom animations etc. 



---
#### Method Controls.AnimatedFramesRenderer.OnAnimatorUpdated(System.Double)

 Override this to react on animator running. 

|Name | Description |
|-----|------|
|value: ||


---
#### Method Controls.AnimatedFramesRenderer.GetFrameAt(System.Single)

 ratio 0-1 

|Name | Description |
|-----|------|
|: ||
**Returns**: 



---
#### Event Controls.AnimatedFramesRenderer.Finished

 All repeats completed 



---
#### Property Controls.AnimatedFramesRenderer.Repeat

 >0 how many times should repeat, if less than 0 will loop forever 



---
#### Property Controls.GifAnimation.Frame

 Current frame bitmap, can change with SeekFrame method 



---
#### Method Controls.GifAnimation.SeekFrame(System.Int32)

 Select frame. If you pass a negative value the last frame will be set. 

|Name | Description |
|-----|------|
|frame: ||


---
#### Method Controls.SkiaGif.#ctor

 For standalone use 



---
#### Method Controls.SkiaGif.#ctor(DrawnUi.Maui.Draw.SkiaImage)

 For building custom controls 

|Name | Description |
|-----|------|
|display: ||


---
#### Method Controls.SkiaGif.OnAnimatorUpdated(System.Double)

 invoked by internal animator 

|Name | Description |
|-----|------|
|value: ||


---
#### Method Controls.SkiaGif.OnAnimatorSeeking(System.Double)

 called by Seek 

|Name | Description |
|-----|------|
|frame: ||


---
#### Field Controls.SkiaLottie.CachedAnimations

 To avoid reloading same files multiple times.. 



---
#### Property Controls.SkiaLottie.DefaultFrameWhenOn

 For the case IsOn = True. What frame should we display at start or when stopped. 0 (START) is default, can specify other number. if value is less than 0 then will seek to the last available frame (END). 



---
#### Method Controls.SkiaLottie.OnJsonLoaded(System.String)

 Called by LoadAnimationFromResources after file was loaded so we can modify json if needed before it it consumed. Return json to be used. This is not called by LoadAnimationFromJson. 



---
#### Method Controls.SkiaLottie.LoadAnimationFromJson(System.String)

 This is not replacing current animation, use SetAnimation for that. 

|Name | Description |
|-----|------|
|json: ||
**Returns**: 



---
## Type Controls.SkiaRadioButton

 Switch-like control, can include any content inside. It's aither you use default content (todo templates?..) or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag `Frame`, SkiaShape with Tag `Thumb`. At the same time you can override ApplyProperties() and apply them to your content yourself. 



---
#### Property Controls.SkiaRadioButton.Text

 Bind to your own content! 



---
#### Method Controls.SkiaTabsSelector.ApplySelectedIndex(System.Int32)

 This is called when processing stack of index changes. For example you might have index chnaged 5 times during the time you were executing ApplySelectedIndex (playing the animations etc), so then you just need the lastest index to be applied. At the same time ApplySelectedIndex will not be called again while its already running, this way you would viually apply only the lastest more actual value instead of maybe freezing ui for too many heavy to render changes. 



---
#### Property Controls.SkiaTabsSelector.TabType

 Specify the type of the tab to be included, other types will just render and not be treated as tabs. Using this so we can included any elements inside this control to create any design. 



---
## Type Controls.SkiaViewSwitcher

 Display and hide views, eventually animating them 



---
#### Method Controls.SkiaViewSwitcher.GetTopView(System.Int32)

 Get tab view or tab top subview if has navigation stack 

|Name | Description |
|-----|------|
|selectedIndex: ||
**Returns**: 



---
#### Method Controls.SkiaViewSwitcher.RevealNavigationView(DrawnUi.Maui.Controls.SkiaViewSwitcher.NavigationStackEntry)

 Set IsVisible, reset transforms and opacity and send OnAppeared event 

|Name | Description |
|-----|------|
|newVisibleView: ||


---
#### Method Controls.SkiaViewSwitcher.PopTabToRoot

 Must be launched on main thread only !!! 



---
#### Field Controls.SkiaViewSwitcher.NavigationStacks

 for navigation inside pseudo tabs 



---
## Type Controls.DrawnUiBasePage

 Actually used to: respond to keyboard resizing on mobile and keyboard key presses on Mac. Other than for that this is not needed at all. 



---
## Type Animate.Animators.VelocitySkiaAnimator

 Basically a modified port of Android FlingAnimation 



---
#### Property Animate.Animators.VelocitySkiaAnimator.mMinOverscrollValue

 Must be over 0 



---
#### Property Animate.Animators.VelocitySkiaAnimator.mMaxOverscrollValue

 Must be over 0 



---
#### Property Animate.Animators.VelocitySkiaAnimator.Friction

 The bigger the sooner animation will slow down, default is 1.0 



---
#### Property Animate.Animators.VelocitySkiaAnimator.RemainingVelocity

 This is set after we are done so we will know at OnStop if we have some energy left 



---
#### Method Animate.Animators.VelocitySkiaAnimator.DragForce.GetInitialVelocity(System.Single,System.Single,System.Single)

 inverse of updateValueAndVelocity 

|Name | Description |
|-----|------|
|initialPosition: ||
|finalPosition: ||
|durationTime: ||
**Returns**: 



---
#### Method Infrastructure.Files.ListAssets(System.String)

 tries to get all resources from assets folder Resources/Raw/{subfolder} 

**Returns**: 



---
#### Method Infrastructure.SkSl.Compile(System.String,System.String)

 Will compile your SKSL shader code into SKRuntimeEffect. The filename parameter is used for debugging purposes only 

|Name | Description |
|-----|------|
|shaderCode: ||
|filename: ||
**Returns**: 



---
#### Field Infrastructure.Enums.UpdateMode.Dynamic

 Will update when needed. 



---
#### Field Infrastructure.Enums.UpdateMode.Constant

 Constantly invalidating the canvas after every frame 



---
#### Field Infrastructure.Enums.UpdateMode.Manual

 Will not update until manually invalidated. 



---
#### Method Infrastructure.Extensions.ColorExtensions.ToColorFromHex(System.String)

 ('#000000', 50) --> #808080 ('#EEEEEE', 25) --> #F2F2F2 ('EEE , 25) --> #F2F2F2 

---
#### Method Infrastructure.Helpers.RubberBandUtils.ClampOnTrack(System.Numerics.Vector2,SkiaSharp.SKRect,System.Single)

 track is the bounds of the possible scrolling offset, for example can be like {0, -1000, 0, 0} 

|Name | Description |
|-----|------|
|point: ||
|track: ||
|coeff: ||
**Returns**: 



---
#### Method Infrastructure.Helpers.RubberBandUtils.RubberBandClamp(System.Single,System.Single,DrawnUi.Maui.Draw.RangeF,System.Single,System.Single)

 onEmpty - how much to simulate scrollable area when its zero 

|Name | Description |
|-----|------|
|coord: ||
|coeff: ||
|dim: ||
|limits: ||
|onEmpty: ||
**Returns**: 



---
#### Property Infrastructure.MeasuringConstraints.TotalMargins

 Include padding 



---
## Type Infrastructure.VisualTransform

 Will enhance this in the future to include more properties 



---
#### Property Infrastructure.VisualTransform.Translation

 Units as from TranslationX and TranslationY 



---
#### Property Infrastructure.VisualTransform.Scale

 Units as from ScaleX and ScaleY 



---
#### Method Infrastructure.VisualTransform.ToNative(SkiaSharp.SKRect,System.Single)

 All input rects are in pixels 

|Name | Description |
|-----|------|
|rect: ||
|clipRect: ||
|scale: ||
**Returns**: 



---
#### Property Infrastructure.VisualTransformNative.Rect

 Pixels only 



---
#### Property Infrastructure.VisualTreeChain.Child

 The final node the tree leads to 



---
#### Property Infrastructure.VisualTreeChain.Nodes

 Parents leading to the final node 



---
#### Property Infrastructure.VisualTreeChain.NodeIndices

 Perf cache for node indices 



---
#### Property Infrastructure.VisualTreeChain.Transform

 Final transform of the chain 



---
#### Method Extensions.InternalExtensions.ContainsInclusive(SkiaSharp.SKRect,SkiaSharp.SKPoint)

 The default Skia method is returning false if point is on the bounds, We correct this by custom function. 

|Name | Description |
|-----|------|
|rect: ||
|point: ||
**Returns**: 



---
#### Method Extensions.InternalExtensions.ContainsInclusive(SkiaSharp.SKRect,System.Single,System.Single)

 The default Skia method is returning false if point is on the bounds, We correct this by custom function. 



---
#### Method Extensions.PointExtensions.Add(Microsoft.Maui.Graphics.Point,Microsoft.Maui.Graphics.Point)

 Adds the coordinates of one Point to another. 

|Name | Description |
|-----|------|
|first: ||
|second: ||
**Returns**: 



---
#### Method Extensions.PointExtensions.Center(Microsoft.Maui.Graphics.Point[])

 Gets the center of some touch points. 

|Name | Description |
|-----|------|
|touches: ||
**Returns**: 



---
#### Method Extensions.PointExtensions.Subtract(Microsoft.Maui.Graphics.Point,Microsoft.Maui.Graphics.Point)

 Subtracts the coordinates of one Point from another. 

|Name | Description |
|-----|------|
|first: ||
|second: ||
**Returns**: 



---
#### Method Views.GlideExtensions.IsActivityAlive(Android.Content.Context,Microsoft.Maui.Controls.ImageSource)

 NOTE: see https://github.com/bumptech/glide/issues/1484#issuecomment-365625087 



---
#### Method Views.GlideExtensions.Clear(Bumptech.Glide.RequestManager,Android.Widget.ImageView)

 Cancels the Request and "clears" the ImageView 



---
#### Method Views.DrawnView.CheckElementVisibility(Microsoft.Maui.Controls.VisualElement)

 To optimize rendering and not update controls that are inside storyboard that is offscreen or hidden Apple - UI thread only !!! If you set 

|Name | Description |
|-----|------|
|element: ||


---
#### Method Views.DrawnView.PostponeExecutionBeforeDraw(System.Action)

 Postpone the action to be executed before the next frame being drawn. Exception-safe. 

|Name | Description |
|-----|------|
|action: ||


---
#### Method Views.DrawnView.PostponeExecutionAfterDraw(System.Action)

 Postpone the action to be executed after the current frame is drawn. Exception-safe. 

|Name | Description |
|-----|------|
|action: ||


---
#### Property Views.DrawnView.GestureListeners

 Children we should check for touch hits 



---
#### Method Views.DrawnView.RegisterAnimator(DrawnUi.Maui.Draw.ISkiaAnimator)

 Called by a control that whats to be constantly animated or doesn't anymore, so we know whether we should refresh canvas non-stop 

|Name | Description |
|-----|------|
|uid: ||
|animating: ||


---
#### Property Views.DrawnView.PostAnimators

 Executed after the rendering 



---
#### Property Views.DrawnView.AnimatingControls

 Tracking controls that what to be animated right now so we constantly refresh canvas until there is none left 



---
#### Method Views.DrawnView.OnCanRenderChanged(System.Boolean)

 Invoked when IsHiddenInViewTree changes 

|Name | Description |
|-----|------|
|state: ||


---
#### Property Views.DrawnView.StopDrawingWhenUpdateIsLocked

 Set this to true if you do not want the canvas to be redrawn as transparent and showing content below the canvas (splash?..) when UpdateLocked is True 



---
#### Property Views.DrawnView.InvalidatedCanvas

 A very important tracking prop to avoid saturating main thread with too many updates 



---
#### Method Views.DrawnView.CreateSkiaView

 Will safely destroy existing if any 



---
#### Method Views.DrawnView.Invalidate

 Makes the control dirty, in need to be remeasured and rendered but this doesn't call Update, it's up yo you 



---
#### Method Views.DrawnView.InvalidateChildren

 We need to invalidate children maui changed our storyboard size 



---
#### Method Views.DrawnView.CalculateLayout(SkiaSharp.SKRect,System.Double,System.Double,System.Double)

 destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS. 

|Name | Description |
|-----|------|
|destination: |PIXELS|
|widthRequest: |UNITS|
|heightRequest: |UNITS|
|scale: ||


---
#### Method Views.DrawnView.Arrange(SkiaSharp.SKRect,System.Double,System.Double,System.Double)

 destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS. 

|Name | Description |
|-----|------|
|destination: |PIXELS|
|widthRequest: |UNITS|
|heightRequest: |UNITS|
|scale: ||


---
#### Property Views.DrawnView.DrawingThreadId

 Can use this to manage double buffering to detect if we are in the drawing thread or in background. 



---
#### Field Views.DrawnView._orderedChildren

 will be reset to null by InvalidateViewsList() 



---
#### Method Views.DrawnView.GetOrderedSubviews

 For non templated simple subviews 

**Returns**: 



---
#### Method Views.DrawnView.InvalidateViewsList

 To make GetOrderedSubviews() regenerate next result instead of using cached 



---
#### Property Views.DrawnView.FrameTime

 Frame started rendering nanoseconds 



---
#### Property Views.DrawnView.CanvasFps

 Actual FPS 



---
#### Property Views.DrawnView.FPS

 Average FPS 



---
#### Property Views.DrawnView.DrawingThreads

 For debugging purposes check if dont have concurrent threads 



---
#### Method Views.DrawnView.KickOffscreenCacheRendering

 Make sure offscreen rendering queue is running 



---
#### Property Views.DrawnView.CanRenderOffScreen

 If this is check you view will be refreshed even offScreen or hidden 



---
#### Property Views.DrawnView.CanDraw

 Indicates that it is allowed to be rendered by engine, internal use 

**Returns**: 



---
#### Property Views.DrawnView.IsHiddenInViewTree

 Indicates that view is either hidden or offscreen. This disables rendering if you don't set CanRenderOffScreen to true 



---
#### Method Views.DrawnView.ReportFocus(DrawnUi.Maui.Draw.ISkiaGestureListener,DrawnUi.Maui.Draw.ISkiaGestureListener)

 Internal call by control, after reporting will affect FocusedChild but will not get FocusedItemChanged as it was its own call 

|Name | Description |
|-----|------|
|listener: ||


---
#### Method Views.DrawnView.InvalidateCanvas





---
## Type Views.Canvas

 Optimized DrawnView having only one child inside Content property. Can autosize to to children size. For all drawn app put this directly inside the ContentPage as root view. If you put this inside some Maui control like Grid whatever expect more GC collections during animations making them somewhat less fluid. 



---
#### Method Views.Canvas.SetContent(DrawnUi.Maui.Draw.SkiaControl)

 Use Content property for direct access 



---
#### Method Views.Canvas.ArrangeOverride(Microsoft.Maui.Graphics.Rect)

 We need this mainly to autosize inside grid cells This is also called when parent visibilty changes 

|Name | Description |
|-----|------|
|bounds: ||
**Returns**: 



---
#### Method Views.Canvas.GetMeasuringRectForChildren(System.Single,System.Single,System.Single)

 All in in UNITS, OUT in PIXELS 

|Name | Description |
|-----|------|
|widthConstraint: ||
|heightConstraint: ||
|scale: ||
**Returns**: 



---
#### Method Views.Canvas.AdaptSizeRequestToContent(System.Double,System.Double)

 In UNITS 

|Name | Description |
|-----|------|
|widthRequest: ||
|heightRequest: ||
**Returns**: 



---
#### Method Views.Canvas.MeasureChild(DrawnUi.Maui.Draw.SkiaControl,System.Double,System.Double,System.Double)

 PIXELS 

|Name | Description |
|-----|------|
|child: ||
|availableWidth: ||
|availableHeight: ||
|scale: ||
**Returns**: 



---
#### Field Views.Canvas.FirstPanThreshold

 To filter micro-gestures on super sensitive screens, start passing panning only when threshold is once overpassed 



---
#### Method Views.Canvas.OnGestureEvent(AppoMobi.Maui.Gestures.TouchActionType,AppoMobi.Maui.Gestures.TouchActionEventArgs,AppoMobi.Maui.Gestures.TouchActionResult)

 IGestureListener implementation 

|Name | Description |
|-----|------|
|type: ||
|args1: ||
|args1: ||
|: ||


---
#### Method Views.Canvas.EnableUpdates

 Enable canvas rendering itsself 



---
#### Method Views.Canvas.DisableUpdates

 Disable invalidating and drawing on the canvas 



---
#### Method Views.SkiaView.CalculateFPS(System.Int64,System.Int32)

 Calculates the frames per second (FPS) and updates the rolling average FPS every 'averageAmount' frames. 

|Name | Description |
|-----|------|
|currentTimestamp: |The current timestamp in nanoseconds.|
|averageAmount: |The number of frames over which to average the FPS. Default is 10.|


---
#### Method Views.SkiaViewAccelerated.CalculateFPS(System.Int64,System.Int32)

 Calculates the frames per second (FPS) and updates the rolling average FPS every 'averageAmount' frames. 

|Name | Description |
|-----|------|
|currentTimestamp: |The current timestamp in nanoseconds.|
|averageAmount: |The number of frames over which to average the FPS. Default is 10.|


---
#### Method Views.SkiaViewAccelerated.OnPaintingSurface(System.Object,SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs)

 We are drawing the frame 

|Name | Description |
|-----|------|
|sender: ||
|paintArgs: ||


---


