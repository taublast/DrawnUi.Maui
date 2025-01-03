<a name='assembly'></a>
# DrawnUi.Maui

## Contents

- [ActionOnTickAnimator](#T-DrawnUi-Maui-Draw-ActionOnTickAnimator 'DrawnUi.Maui.Draw.ActionOnTickAnimator')
- [AddGestures](#T-DrawnUi-Maui-Draw-AddGestures 'DrawnUi.Maui.Draw.AddGestures')
- [AnimateExtensions](#T-DrawnUi-Maui-Draw-AnimateExtensions 'DrawnUi.Maui.Draw.AnimateExtensions')
  - [AnimateWith(control,animations)](#M-DrawnUi-Maui-Draw-AnimateExtensions-AnimateWith-DrawnUi-Maui-Draw-SkiaControl,System-Func{DrawnUi-Maui-Draw-SkiaControl,System-Threading-Tasks-Task}[]- 'DrawnUi.Maui.Draw.AnimateExtensions.AnimateWith(DrawnUi.Maui.Draw.SkiaControl,System.Func{DrawnUi.Maui.Draw.SkiaControl,System.Threading.Tasks.Task}[])')
- [AnimatedFramesRenderer](#T-DrawnUi-Maui-Controls-AnimatedFramesRenderer 'DrawnUi.Maui.Controls.AnimatedFramesRenderer')
  - [Repeat](#P-DrawnUi-Maui-Controls-AnimatedFramesRenderer-Repeat 'DrawnUi.Maui.Controls.AnimatedFramesRenderer.Repeat')
  - [GetFrameAt()](#M-DrawnUi-Maui-Controls-AnimatedFramesRenderer-GetFrameAt-System-Single- 'DrawnUi.Maui.Controls.AnimatedFramesRenderer.GetFrameAt(System.Single)')
  - [OnAnimatorUpdated(value)](#M-DrawnUi-Maui-Controls-AnimatedFramesRenderer-OnAnimatorUpdated-System-Double- 'DrawnUi.Maui.Controls.AnimatedFramesRenderer.OnAnimatorUpdated(System.Double)')
- [AutoSizeType](#T-DrawnUi-Maui-Draw-AutoSizeType 'DrawnUi.Maui.Draw.AutoSizeType')
  - [FillHorizontal](#F-DrawnUi-Maui-Draw-AutoSizeType-FillHorizontal 'DrawnUi.Maui.Draw.AutoSizeType.FillHorizontal')
  - [FillVertical](#F-DrawnUi-Maui-Draw-AutoSizeType-FillVertical 'DrawnUi.Maui.Draw.AutoSizeType.FillVertical')
  - [FitFillHorizontal](#F-DrawnUi-Maui-Draw-AutoSizeType-FitFillHorizontal 'DrawnUi.Maui.Draw.AutoSizeType.FitFillHorizontal')
  - [FitFillVertical](#F-DrawnUi-Maui-Draw-AutoSizeType-FitFillVertical 'DrawnUi.Maui.Draw.AutoSizeType.FitFillVertical')
  - [FitHorizontal](#F-DrawnUi-Maui-Draw-AutoSizeType-FitHorizontal 'DrawnUi.Maui.Draw.AutoSizeType.FitHorizontal')
  - [FitVertical](#F-DrawnUi-Maui-Draw-AutoSizeType-FitVertical 'DrawnUi.Maui.Draw.AutoSizeType.FitVertical')
- [BindToParentContextExtension](#T-DrawnUi-Maui-Draw-BindToParentContextExtension 'DrawnUi.Maui.Draw.BindToParentContextExtension')
- [BuildWrapLayout](#T-DrawnUi-Maui-Draw-SkiaLayout-BuildWrapLayout 'DrawnUi.Maui.Draw.SkiaLayout.BuildWrapLayout')
- [CachedObject](#T-DrawnUi-Maui-Draw-CachedObject 'DrawnUi.Maui.Draw.CachedObject')
  - [SurfaceIsRecycled](#P-DrawnUi-Maui-Draw-CachedObject-SurfaceIsRecycled 'DrawnUi.Maui.Draw.CachedObject.SurfaceIsRecycled')
- [Canvas](#T-DrawnUi-Maui-Views-Canvas 'DrawnUi.Maui.Views.Canvas')
  - [FirstPanThreshold](#F-DrawnUi-Maui-Views-Canvas-FirstPanThreshold 'DrawnUi.Maui.Views.Canvas.FirstPanThreshold')
  - [AdaptSizeRequestToContent(widthRequest,heightRequest)](#M-DrawnUi-Maui-Views-Canvas-AdaptSizeRequestToContent-System-Double,System-Double- 'DrawnUi.Maui.Views.Canvas.AdaptSizeRequestToContent(System.Double,System.Double)')
  - [ArrangeOverride(bounds)](#M-DrawnUi-Maui-Views-Canvas-ArrangeOverride-Microsoft-Maui-Graphics-Rect- 'DrawnUi.Maui.Views.Canvas.ArrangeOverride(Microsoft.Maui.Graphics.Rect)')
  - [DisableUpdates()](#M-DrawnUi-Maui-Views-Canvas-DisableUpdates 'DrawnUi.Maui.Views.Canvas.DisableUpdates')
  - [EnableUpdates()](#M-DrawnUi-Maui-Views-Canvas-EnableUpdates 'DrawnUi.Maui.Views.Canvas.EnableUpdates')
  - [GetMeasuringRectForChildren(widthConstraint,heightConstraint,scale)](#M-DrawnUi-Maui-Views-Canvas-GetMeasuringRectForChildren-System-Single,System-Single,System-Single- 'DrawnUi.Maui.Views.Canvas.GetMeasuringRectForChildren(System.Single,System.Single,System.Single)')
  - [MeasureChild(child,availableWidth,availableHeight,scale)](#M-DrawnUi-Maui-Views-Canvas-MeasureChild-DrawnUi-Maui-Draw-SkiaControl,System-Double,System-Double,System-Double- 'DrawnUi.Maui.Views.Canvas.MeasureChild(DrawnUi.Maui.Draw.SkiaControl,System.Double,System.Double,System.Double)')
  - [OnGestureEvent(type,args1,args1,)](#M-DrawnUi-Maui-Views-Canvas-OnGestureEvent-AppoMobi-Maui-Gestures-TouchActionType,AppoMobi-Maui-Gestures-TouchActionEventArgs,AppoMobi-Maui-Gestures-TouchActionResult- 'DrawnUi.Maui.Views.Canvas.OnGestureEvent(AppoMobi.Maui.Gestures.TouchActionType,AppoMobi.Maui.Gestures.TouchActionEventArgs,AppoMobi.Maui.Gestures.TouchActionResult)')
  - [SetContent()](#M-DrawnUi-Maui-Views-Canvas-SetContent-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Views.Canvas.SetContent(DrawnUi.Maui.Draw.SkiaControl)')
- [Cell](#T-DrawnUi-Maui-Draw-SkiaLayout-Cell 'DrawnUi.Maui.Draw.SkiaLayout.Cell')
  - [ColumnGridLengthType](#P-DrawnUi-Maui-Draw-SkiaLayout-Cell-ColumnGridLengthType 'DrawnUi.Maui.Draw.SkiaLayout.Cell.ColumnGridLengthType')
  - [RowGridLengthType](#P-DrawnUi-Maui-Draw-SkiaLayout-Cell-RowGridLengthType 'DrawnUi.Maui.Draw.SkiaLayout.Cell.RowGridLengthType')
- [ChainAdjustBrightnessEffect](#T-DrawnUi-Maui-Draw-ChainAdjustBrightnessEffect 'DrawnUi.Maui.Draw.ChainAdjustBrightnessEffect')
  - [CreateBrightnessFilter(value)](#M-DrawnUi-Maui-Draw-ChainAdjustBrightnessEffect-CreateBrightnessFilter-System-Single- 'DrawnUi.Maui.Draw.ChainAdjustBrightnessEffect.CreateBrightnessFilter(System.Single)')
  - [CreateLightnessFilter(value)](#M-DrawnUi-Maui-Draw-ChainAdjustBrightnessEffect-CreateLightnessFilter-System-Single- 'DrawnUi.Maui.Draw.ChainAdjustBrightnessEffect.CreateLightnessFilter(System.Single)')
- [ChainAdjustLightnessEffect](#T-DrawnUi-Maui-Draw-ChainAdjustLightnessEffect 'DrawnUi.Maui.Draw.ChainAdjustLightnessEffect')
  - [CreateLightnessFilter(value)](#M-DrawnUi-Maui-Draw-ChainAdjustLightnessEffect-CreateLightnessFilter-System-Single- 'DrawnUi.Maui.Draw.ChainAdjustLightnessEffect.CreateLightnessFilter(System.Single)')
- [ChainEffectResult](#T-DrawnUi-Maui-Draw-ChainEffectResult 'DrawnUi.Maui.Draw.ChainEffectResult')
  - [DrawnControl](#P-DrawnUi-Maui-Draw-ChainEffectResult-DrawnControl 'DrawnUi.Maui.Draw.ChainEffectResult.DrawnControl')
- [ColorExtensions](#T-DrawnUi-Maui-Infrastructure-Extensions-ColorExtensions 'DrawnUi.Maui.Infrastructure.Extensions.ColorExtensions')
  - [ToColorFromHex()](#M-DrawnUi-Maui-Infrastructure-Extensions-ColorExtensions-ToColorFromHex-System-String- 'DrawnUi.Maui.Infrastructure.Extensions.ColorExtensions.ToColorFromHex(System.String)')
- [ContentLayout](#T-DrawnUi-Maui-Draw-ContentLayout 'DrawnUi.Maui.Draw.ContentLayout')
  - [Orientation](#P-DrawnUi-Maui-Draw-ContentLayout-Orientation 'DrawnUi.Maui.Draw.ContentLayout.Orientation')
  - [ScrollType](#P-DrawnUi-Maui-Draw-ContentLayout-ScrollType 'DrawnUi.Maui.Draw.ContentLayout.ScrollType')
  - [Virtualisation](#P-DrawnUi-Maui-Draw-ContentLayout-Virtualisation 'DrawnUi.Maui.Draw.ContentLayout.Virtualisation')
  - [GetContentAvailableRect(destination)](#M-DrawnUi-Maui-Draw-ContentLayout-GetContentAvailableRect-SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.ContentLayout.GetContentAvailableRect(SkiaSharp.SKRect)')
- [ControlInStack](#T-DrawnUi-Maui-Draw-ControlInStack 'DrawnUi.Maui.Draw.ControlInStack')
  - [Area](#P-DrawnUi-Maui-Draw-ControlInStack-Area 'DrawnUi.Maui.Draw.ControlInStack.Area')
  - [ControlIndex](#P-DrawnUi-Maui-Draw-ControlInStack-ControlIndex 'DrawnUi.Maui.Draw.ControlInStack.ControlIndex')
  - [Destination](#P-DrawnUi-Maui-Draw-ControlInStack-Destination 'DrawnUi.Maui.Draw.ControlInStack.Destination')
  - [Drawn](#P-DrawnUi-Maui-Draw-ControlInStack-Drawn 'DrawnUi.Maui.Draw.ControlInStack.Drawn')
  - [Measured](#P-DrawnUi-Maui-Draw-ControlInStack-Measured 'DrawnUi.Maui.Draw.ControlInStack.Measured')
  - [Offset](#P-DrawnUi-Maui-Draw-ControlInStack-Offset 'DrawnUi.Maui.Draw.ControlInStack.Offset')
  - [View](#P-DrawnUi-Maui-Draw-ControlInStack-View 'DrawnUi.Maui.Draw.ControlInStack.View')
- [DecelerationTimingVectorParameters](#T-DrawnUi-Maui-Draw-DecelerationTimingVectorParameters 'DrawnUi.Maui.Draw.DecelerationTimingVectorParameters')
  - [ValueAt(offsetSecs,time)](#M-DrawnUi-Maui-Draw-DecelerationTimingVectorParameters-ValueAt-System-Single- 'DrawnUi.Maui.Draw.DecelerationTimingVectorParameters.ValueAt(System.Single)')
- [DecomposedText](#T-DrawnUi-Maui-Draw-SkiaLabel-DecomposedText 'DrawnUi.Maui.Draw.SkiaLabel.DecomposedText')
  - [HasMoreHorizontalSpace](#P-DrawnUi-Maui-Draw-SkiaLabel-DecomposedText-HasMoreHorizontalSpace 'DrawnUi.Maui.Draw.SkiaLabel.DecomposedText.HasMoreHorizontalSpace')
  - [HasMoreVerticalSpace](#P-DrawnUi-Maui-Draw-SkiaLabel-DecomposedText-HasMoreVerticalSpace 'DrawnUi.Maui.Draw.SkiaLabel.DecomposedText.HasMoreVerticalSpace')
- [DragForce](#T-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-DragForce 'DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator.DragForce')
  - [GetInitialVelocity(initialPosition,finalPosition,durationTime)](#M-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-DragForce-GetInitialVelocity-System-Single,System-Single,System-Single- 'DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator.DragForce.GetInitialVelocity(System.Single,System.Single,System.Single)')
- [DrawnFontAttributesConverter](#T-DrawnUi-Maui-Draw-DrawnFontAttributesConverter 'DrawnUi.Maui.Draw.DrawnFontAttributesConverter')
- [DrawnUiBasePage](#T-DrawnUi-Maui-Controls-DrawnUiBasePage 'DrawnUi.Maui.Controls.DrawnUiBasePage')
- [DrawnView](#T-DrawnUi-Maui-Views-DrawnView 'DrawnUi.Maui.Views.DrawnView')
  - [_orderedChildren](#F-DrawnUi-Maui-Views-DrawnView-_orderedChildren 'DrawnUi.Maui.Views.DrawnView._orderedChildren')
  - [AnimatingControls](#P-DrawnUi-Maui-Views-DrawnView-AnimatingControls 'DrawnUi.Maui.Views.DrawnView.AnimatingControls')
  - [CanDraw](#P-DrawnUi-Maui-Views-DrawnView-CanDraw 'DrawnUi.Maui.Views.DrawnView.CanDraw')
  - [CanRenderOffScreen](#P-DrawnUi-Maui-Views-DrawnView-CanRenderOffScreen 'DrawnUi.Maui.Views.DrawnView.CanRenderOffScreen')
  - [CanvasFps](#P-DrawnUi-Maui-Views-DrawnView-CanvasFps 'DrawnUi.Maui.Views.DrawnView.CanvasFps')
  - [DrawingThreadId](#P-DrawnUi-Maui-Views-DrawnView-DrawingThreadId 'DrawnUi.Maui.Views.DrawnView.DrawingThreadId')
  - [DrawingThreads](#P-DrawnUi-Maui-Views-DrawnView-DrawingThreads 'DrawnUi.Maui.Views.DrawnView.DrawingThreads')
  - [FPS](#P-DrawnUi-Maui-Views-DrawnView-FPS 'DrawnUi.Maui.Views.DrawnView.FPS')
  - [FrameTime](#P-DrawnUi-Maui-Views-DrawnView-FrameTime 'DrawnUi.Maui.Views.DrawnView.FrameTime')
  - [GestureListeners](#P-DrawnUi-Maui-Views-DrawnView-GestureListeners 'DrawnUi.Maui.Views.DrawnView.GestureListeners')
  - [InvalidatedCanvas](#P-DrawnUi-Maui-Views-DrawnView-InvalidatedCanvas 'DrawnUi.Maui.Views.DrawnView.InvalidatedCanvas')
  - [IsHiddenInViewTree](#P-DrawnUi-Maui-Views-DrawnView-IsHiddenInViewTree 'DrawnUi.Maui.Views.DrawnView.IsHiddenInViewTree')
  - [PostAnimators](#P-DrawnUi-Maui-Views-DrawnView-PostAnimators 'DrawnUi.Maui.Views.DrawnView.PostAnimators')
  - [StopDrawingWhenUpdateIsLocked](#P-DrawnUi-Maui-Views-DrawnView-StopDrawingWhenUpdateIsLocked 'DrawnUi.Maui.Views.DrawnView.StopDrawingWhenUpdateIsLocked')
  - [Arrange(destination,widthRequest,heightRequest,scale)](#M-DrawnUi-Maui-Views-DrawnView-Arrange-SkiaSharp-SKRect,System-Double,System-Double,System-Double- 'DrawnUi.Maui.Views.DrawnView.Arrange(SkiaSharp.SKRect,System.Double,System.Double,System.Double)')
  - [CalculateLayout(destination,widthRequest,heightRequest,scale)](#M-DrawnUi-Maui-Views-DrawnView-CalculateLayout-SkiaSharp-SKRect,System-Double,System-Double,System-Double- 'DrawnUi.Maui.Views.DrawnView.CalculateLayout(SkiaSharp.SKRect,System.Double,System.Double,System.Double)')
  - [CheckElementVisibility(element)](#M-DrawnUi-Maui-Views-DrawnView-CheckElementVisibility-Microsoft-Maui-Controls-VisualElement- 'DrawnUi.Maui.Views.DrawnView.CheckElementVisibility(Microsoft.Maui.Controls.VisualElement)')
  - [CreateSkiaView()](#M-DrawnUi-Maui-Views-DrawnView-CreateSkiaView 'DrawnUi.Maui.Views.DrawnView.CreateSkiaView')
  - [GetOrderedSubviews()](#M-DrawnUi-Maui-Views-DrawnView-GetOrderedSubviews 'DrawnUi.Maui.Views.DrawnView.GetOrderedSubviews')
  - [Invalidate()](#M-DrawnUi-Maui-Views-DrawnView-Invalidate 'DrawnUi.Maui.Views.DrawnView.Invalidate')
  - [InvalidateCanvas()](#M-DrawnUi-Maui-Views-DrawnView-InvalidateCanvas 'DrawnUi.Maui.Views.DrawnView.InvalidateCanvas')
  - [InvalidateChildren()](#M-DrawnUi-Maui-Views-DrawnView-InvalidateChildren 'DrawnUi.Maui.Views.DrawnView.InvalidateChildren')
  - [InvalidateViewsList()](#M-DrawnUi-Maui-Views-DrawnView-InvalidateViewsList 'DrawnUi.Maui.Views.DrawnView.InvalidateViewsList')
  - [KickOffscreenCacheRendering()](#M-DrawnUi-Maui-Views-DrawnView-KickOffscreenCacheRendering 'DrawnUi.Maui.Views.DrawnView.KickOffscreenCacheRendering')
  - [OnCanRenderChanged(state)](#M-DrawnUi-Maui-Views-DrawnView-OnCanRenderChanged-System-Boolean- 'DrawnUi.Maui.Views.DrawnView.OnCanRenderChanged(System.Boolean)')
  - [PostponeExecutionAfterDraw(action)](#M-DrawnUi-Maui-Views-DrawnView-PostponeExecutionAfterDraw-System-Action- 'DrawnUi.Maui.Views.DrawnView.PostponeExecutionAfterDraw(System.Action)')
  - [PostponeExecutionBeforeDraw(action)](#M-DrawnUi-Maui-Views-DrawnView-PostponeExecutionBeforeDraw-System-Action- 'DrawnUi.Maui.Views.DrawnView.PostponeExecutionBeforeDraw(System.Action)')
  - [RegisterAnimator(uid,animating)](#M-DrawnUi-Maui-Views-DrawnView-RegisterAnimator-DrawnUi-Maui-Draw-ISkiaAnimator- 'DrawnUi.Maui.Views.DrawnView.RegisterAnimator(DrawnUi.Maui.Draw.ISkiaAnimator)')
  - [ReportFocus(listener)](#M-DrawnUi-Maui-Views-DrawnView-ReportFocus-DrawnUi-Maui-Draw-ISkiaGestureListener,DrawnUi-Maui-Draw-ISkiaGestureListener- 'DrawnUi.Maui.Views.DrawnView.ReportFocus(DrawnUi.Maui.Draw.ISkiaGestureListener,DrawnUi.Maui.Draw.ISkiaGestureListener)')
- [DynamicGrid\`1](#T-DrawnUi-Maui-Draw-DynamicGrid`1 'DrawnUi.Maui.Draw.DynamicGrid`1')
  - [GetColumnCountForRow(row)](#M-DrawnUi-Maui-Draw-DynamicGrid`1-GetColumnCountForRow-System-Int32- 'DrawnUi.Maui.Draw.DynamicGrid`1.GetColumnCountForRow(System.Int32)')
- [EmojiData](#T-DrawnUi-Maui-Draw-SkiaLabel-EmojiData 'DrawnUi.Maui.Draw.SkiaLabel.EmojiData')
  - [IsEmojiModifierSequence(text,index)](#M-DrawnUi-Maui-Draw-SkiaLabel-EmojiData-IsEmojiModifierSequence-System-String,System-Int32- 'DrawnUi.Maui.Draw.SkiaLabel.EmojiData.IsEmojiModifierSequence(System.String,System.Int32)')
- [Files](#T-DrawnUi-Maui-Infrastructure-Files 'DrawnUi.Maui.Infrastructure.Files')
  - [ListAssets()](#M-DrawnUi-Maui-Infrastructure-Files-ListAssets-System-String- 'DrawnUi.Maui.Infrastructure.Files.ListAssets(System.String)')
- [FontWeight](#T-DrawnUi-Maui-Draw-FontWeight 'DrawnUi.Maui.Draw.FontWeight')
  - [Black](#F-DrawnUi-Maui-Draw-FontWeight-Black 'DrawnUi.Maui.Draw.FontWeight.Black')
  - [Bold](#F-DrawnUi-Maui-Draw-FontWeight-Bold 'DrawnUi.Maui.Draw.FontWeight.Bold')
  - [ExtraBold](#F-DrawnUi-Maui-Draw-FontWeight-ExtraBold 'DrawnUi.Maui.Draw.FontWeight.ExtraBold')
  - [ExtraLight](#F-DrawnUi-Maui-Draw-FontWeight-ExtraLight 'DrawnUi.Maui.Draw.FontWeight.ExtraLight')
  - [Light](#F-DrawnUi-Maui-Draw-FontWeight-Light 'DrawnUi.Maui.Draw.FontWeight.Light')
  - [Medium](#F-DrawnUi-Maui-Draw-FontWeight-Medium 'DrawnUi.Maui.Draw.FontWeight.Medium')
  - [Regular](#F-DrawnUi-Maui-Draw-FontWeight-Regular 'DrawnUi.Maui.Draw.FontWeight.Regular')
  - [SemiBold](#F-DrawnUi-Maui-Draw-FontWeight-SemiBold 'DrawnUi.Maui.Draw.FontWeight.SemiBold')
  - [Thin](#F-DrawnUi-Maui-Draw-FontWeight-Thin 'DrawnUi.Maui.Draw.FontWeight.Thin')
- [GesturesMode](#T-DrawnUi-Maui-Draw-GesturesMode 'DrawnUi.Maui.Draw.GesturesMode')
  - [Disabled](#F-DrawnUi-Maui-Draw-GesturesMode-Disabled 'DrawnUi.Maui.Draw.GesturesMode.Disabled')
  - [Enabled](#F-DrawnUi-Maui-Draw-GesturesMode-Enabled 'DrawnUi.Maui.Draw.GesturesMode.Enabled')
  - [Lock](#F-DrawnUi-Maui-Draw-GesturesMode-Lock 'DrawnUi.Maui.Draw.GesturesMode.Lock')
  - [Share](#F-DrawnUi-Maui-Draw-GesturesMode-Share 'DrawnUi.Maui.Draw.GesturesMode.Share')
- [GifAnimation](#T-DrawnUi-Maui-Controls-GifAnimation 'DrawnUi.Maui.Controls.GifAnimation')
  - [Frame](#P-DrawnUi-Maui-Controls-GifAnimation-Frame 'DrawnUi.Maui.Controls.GifAnimation.Frame')
  - [SeekFrame(frame)](#M-DrawnUi-Maui-Controls-GifAnimation-SeekFrame-System-Int32- 'DrawnUi.Maui.Controls.GifAnimation.SeekFrame(System.Int32)')
- [GlideExtensions](#T-DrawnUi-Maui-Views-GlideExtensions 'DrawnUi.Maui.Views.GlideExtensions')
  - [Clear()](#M-DrawnUi-Maui-Views-GlideExtensions-Clear-Bumptech-Glide-RequestManager,Android-Widget-ImageView- 'DrawnUi.Maui.Views.GlideExtensions.Clear(Bumptech.Glide.RequestManager,Android.Widget.ImageView)')
  - [IsActivityAlive()](#M-DrawnUi-Maui-Views-GlideExtensions-IsActivityAlive-Android-Content-Context,Microsoft-Maui-Controls-ImageSource- 'DrawnUi.Maui.Views.GlideExtensions.IsActivityAlive(Android.Content.Context,Microsoft.Maui.Controls.ImageSource)')
- [GridLayout](#T-DrawnUi-Maui-Controls-GridLayout 'DrawnUi.Maui.Controls.GridLayout')
- [HStack](#T-DrawnUi-Maui-Controls-HStack 'DrawnUi.Maui.Controls.HStack')
- [HardwareAccelerationMode](#T-DrawnUi-Maui-Draw-HardwareAccelerationMode 'DrawnUi.Maui.Draw.HardwareAccelerationMode')
  - [Disabled](#F-DrawnUi-Maui-Draw-HardwareAccelerationMode-Disabled 'DrawnUi.Maui.Draw.HardwareAccelerationMode.Disabled')
  - [Enabled](#F-DrawnUi-Maui-Draw-HardwareAccelerationMode-Enabled 'DrawnUi.Maui.Draw.HardwareAccelerationMode.Enabled')
  - [Prerender](#F-DrawnUi-Maui-Draw-HardwareAccelerationMode-Prerender 'DrawnUi.Maui.Draw.HardwareAccelerationMode.Prerender')
- [IAfterEffectDelete](#T-DrawnUi-Maui-Draw-IAfterEffectDelete 'DrawnUi.Maui.Draw.IAfterEffectDelete')
  - [TypeId](#P-DrawnUi-Maui-Draw-IAfterEffectDelete-TypeId 'DrawnUi.Maui.Draw.IAfterEffectDelete.TypeId')
  - [Render(control,canvas,scale)](#M-DrawnUi-Maui-Draw-IAfterEffectDelete-Render-DrawnUi-Maui-Draw-IDrawnBase,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double- 'DrawnUi.Maui.Draw.IAfterEffectDelete.Render(DrawnUi.Maui.Draw.IDrawnBase,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)')
- [IAnimatorsManager](#T-DrawnUi-Maui-Draw-IAnimatorsManager 'DrawnUi.Maui.Draw.IAnimatorsManager')
- [ICanBeUpdated](#T-DrawnUi-Maui-Draw-ICanBeUpdated 'DrawnUi.Maui.Draw.ICanBeUpdated')
  - [Update()](#M-DrawnUi-Maui-Draw-ICanBeUpdated-Update 'DrawnUi.Maui.Draw.ICanBeUpdated.Update')
- [ICanRenderOnCanvas](#T-DrawnUi-Maui-Draw-ICanRenderOnCanvas 'DrawnUi.Maui.Draw.ICanRenderOnCanvas')
  - [Render(control,context,scale)](#M-DrawnUi-Maui-Draw-ICanRenderOnCanvas-Render-DrawnUi-Maui-Draw-IDrawnBase,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double- 'DrawnUi.Maui.Draw.ICanRenderOnCanvas.Render(DrawnUi.Maui.Draw.IDrawnBase,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)')
- [IDefinesViewport](#T-DrawnUi-Maui-Draw-IDefinesViewport 'DrawnUi.Maui.Draw.IDefinesViewport')
  - [InvalidateByChild()](#M-DrawnUi-Maui-Draw-IDefinesViewport-InvalidateByChild-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.IDefinesViewport.InvalidateByChild(DrawnUi.Maui.Draw.SkiaControl)')
- [IDrawnBase](#T-DrawnUi-Maui-Draw-IDrawnBase 'DrawnUi.Maui.Draw.IDrawnBase')
  - [PostAnimators](#P-DrawnUi-Maui-Draw-IDrawnBase-PostAnimators 'DrawnUi.Maui.Draw.IDrawnBase.PostAnimators')
  - [Views](#P-DrawnUi-Maui-Draw-IDrawnBase-Views 'DrawnUi.Maui.Draw.IDrawnBase.Views')
  - [AddSubView(view)](#M-DrawnUi-Maui-Draw-IDrawnBase-AddSubView-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.IDrawnBase.AddSubView(DrawnUi.Maui.Draw.SkiaControl)')
  - [ClipSmart(canvas,path,operation)](#M-DrawnUi-Maui-Draw-IDrawnBase-ClipSmart-SkiaSharp-SKCanvas,SkiaSharp-SKPath,SkiaSharp-SKClipOperation- 'DrawnUi.Maui.Draw.IDrawnBase.ClipSmart(SkiaSharp.SKCanvas,SkiaSharp.SKPath,SkiaSharp.SKClipOperation)')
  - [CreateClip()](#M-DrawnUi-Maui-Draw-IDrawnBase-CreateClip-System-Object,System-Boolean,SkiaSharp-SKPath- 'DrawnUi.Maui.Draw.IDrawnBase.CreateClip(System.Object,System.Boolean,SkiaSharp.SKPath)')
  - [GetOnScreenVisibleArea()](#M-DrawnUi-Maui-Draw-IDrawnBase-GetOnScreenVisibleArea-System-Single- 'DrawnUi.Maui.Draw.IDrawnBase.GetOnScreenVisibleArea(System.Single)')
  - [Invalidate()](#M-DrawnUi-Maui-Draw-IDrawnBase-Invalidate 'DrawnUi.Maui.Draw.IDrawnBase.Invalidate')
  - [InvalidateByChild(skiaControl)](#M-DrawnUi-Maui-Draw-IDrawnBase-InvalidateByChild-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.IDrawnBase.InvalidateByChild(DrawnUi.Maui.Draw.SkiaControl)')
  - [InvalidateParents()](#M-DrawnUi-Maui-Draw-IDrawnBase-InvalidateParents 'DrawnUi.Maui.Draw.IDrawnBase.InvalidateParents')
  - [RemoveSubView(view)](#M-DrawnUi-Maui-Draw-IDrawnBase-RemoveSubView-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.IDrawnBase.RemoveSubView(DrawnUi.Maui.Draw.SkiaControl)')
  - [UpdateByChild(skiaControl)](#M-DrawnUi-Maui-Draw-IDrawnBase-UpdateByChild-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.IDrawnBase.UpdateByChild(DrawnUi.Maui.Draw.SkiaControl)')
- [IHandleGoBack](#T-DrawnUi-Maui-Controls-SkiaShell-IHandleGoBack 'DrawnUi.Maui.Controls.SkiaShell.IHandleGoBack')
  - [OnShellGoBack()](#M-DrawnUi-Maui-Controls-SkiaShell-IHandleGoBack-OnShellGoBack-System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.IHandleGoBack.OnShellGoBack(System.Boolean)')
- [IHasBanner](#T-DrawnUi-Maui-Draw-IHasBanner 'DrawnUi.Maui.Draw.IHasBanner')
  - [Banner](#P-DrawnUi-Maui-Draw-IHasBanner-Banner 'DrawnUi.Maui.Draw.IHasBanner.Banner')
  - [BannerPreloadOrdered](#P-DrawnUi-Maui-Draw-IHasBanner-BannerPreloadOrdered 'DrawnUi.Maui.Draw.IHasBanner.BannerPreloadOrdered')
- [IInsideViewport](#T-DrawnUi-Maui-Draw-IInsideViewport 'DrawnUi.Maui.Draw.IInsideViewport')
  - [OnLoaded()](#M-DrawnUi-Maui-Draw-IInsideViewport-OnLoaded 'DrawnUi.Maui.Draw.IInsideViewport.OnLoaded')
- [IInsideWheelStack](#T-DrawnUi-Maui-Draw-IInsideWheelStack 'DrawnUi.Maui.Draw.IInsideWheelStack')
  - [OnPositionChanged(offsetRatio,isSelected)](#M-DrawnUi-Maui-Draw-IInsideWheelStack-OnPositionChanged-System-Single,System-Boolean- 'DrawnUi.Maui.Draw.IInsideWheelStack.OnPositionChanged(System.Single,System.Boolean)')
- [ILayoutInsideViewport](#T-DrawnUi-Maui-Draw-ILayoutInsideViewport 'DrawnUi.Maui.Draw.ILayoutInsideViewport')
  - [GetChildIndexAt(point)](#M-DrawnUi-Maui-Draw-ILayoutInsideViewport-GetChildIndexAt-SkiaSharp-SKPoint- 'DrawnUi.Maui.Draw.ILayoutInsideViewport.GetChildIndexAt(SkiaSharp.SKPoint)')
  - [GetVisibleChildIndexAt(point)](#M-DrawnUi-Maui-Draw-ILayoutInsideViewport-GetVisibleChildIndexAt-SkiaSharp-SKPoint- 'DrawnUi.Maui.Draw.ILayoutInsideViewport.GetVisibleChildIndexAt(SkiaSharp.SKPoint)')
- [IRefreshIndicator](#T-DrawnUi-Maui-Draw-IRefreshIndicator 'DrawnUi.Maui.Draw.IRefreshIndicator')
  - [SetDragRatio(ratio)](#M-DrawnUi-Maui-Draw-IRefreshIndicator-SetDragRatio-System-Single- 'DrawnUi.Maui.Draw.IRefreshIndicator.SetDragRatio(System.Single)')
- [IRenderEffect](#T-DrawnUi-Maui-Draw-IRenderEffect 'DrawnUi.Maui.Draw.IRenderEffect')
  - [Draw(destination,ctx,drawControl)](#M-DrawnUi-Maui-Draw-IRenderEffect-Draw-SkiaSharp-SKRect,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Action{DrawnUi-Maui-Draw-SkiaDrawingContext}- 'DrawnUi.Maui.Draw.IRenderEffect.Draw(SkiaSharp.SKRect,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Action{DrawnUi.Maui.Draw.SkiaDrawingContext})')
- [ISkiaAnimator](#T-DrawnUi-Maui-Draw-ISkiaAnimator 'DrawnUi.Maui.Draw.ISkiaAnimator')
  - [IsDeactivated](#P-DrawnUi-Maui-Draw-ISkiaAnimator-IsDeactivated 'DrawnUi.Maui.Draw.ISkiaAnimator.IsDeactivated')
  - [IsHiddenInViewTree](#P-DrawnUi-Maui-Draw-ISkiaAnimator-IsHiddenInViewTree 'DrawnUi.Maui.Draw.ISkiaAnimator.IsHiddenInViewTree')
  - [IsPaused](#P-DrawnUi-Maui-Draw-ISkiaAnimator-IsPaused 'DrawnUi.Maui.Draw.ISkiaAnimator.IsPaused')
  - [Pause()](#M-DrawnUi-Maui-Draw-ISkiaAnimator-Pause 'DrawnUi.Maui.Draw.ISkiaAnimator.Pause')
  - [Resume()](#M-DrawnUi-Maui-Draw-ISkiaAnimator-Resume 'DrawnUi.Maui.Draw.ISkiaAnimator.Resume')
  - [TickFrame(frameTime)](#M-DrawnUi-Maui-Draw-ISkiaAnimator-TickFrame-System-Int64- 'DrawnUi.Maui.Draw.ISkiaAnimator.TickFrame(System.Int64)')
- [ISkiaControl](#T-DrawnUi-Maui-Draw-ISkiaControl 'DrawnUi.Maui.Draw.ISkiaControl')
  - [IsGhost](#P-DrawnUi-Maui-Draw-ISkiaControl-IsGhost 'DrawnUi.Maui.Draw.ISkiaControl.IsGhost')
  - [Measure(widthConstraint,heightConstraint)](#M-DrawnUi-Maui-Draw-ISkiaControl-Measure-System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.ISkiaControl.Measure(System.Single,System.Single,System.Single)')
- [ISkiaDrawable](#T-DrawnUi-Maui-Draw-ISkiaDrawable 'DrawnUi.Maui.Draw.ISkiaDrawable')
  - [OnDraw](#P-DrawnUi-Maui-Draw-ISkiaDrawable-OnDraw 'DrawnUi.Maui.Draw.ISkiaDrawable.OnDraw')
- [ISkiaGestureListener](#T-DrawnUi-Maui-Draw-ISkiaGestureListener 'DrawnUi.Maui.Draw.ISkiaGestureListener')
  - [OnFocusChanged(focus)](#M-DrawnUi-Maui-Draw-ISkiaGestureListener-OnFocusChanged-System-Boolean- 'DrawnUi.Maui.Draw.ISkiaGestureListener.OnFocusChanged(System.Boolean)')
  - [OnSkiaGestureEvent(type,args,args.Action.Action,inside)](#M-DrawnUi-Maui-Draw-ISkiaGestureListener-OnSkiaGestureEvent-DrawnUi-Maui-Draw-SkiaGesturesParameters,DrawnUi-Maui-Draw-GestureEventProcessingInfo- 'DrawnUi.Maui.Draw.ISkiaGestureListener.OnSkiaGestureEvent(DrawnUi.Maui.Draw.SkiaGesturesParameters,DrawnUi.Maui.Draw.GestureEventProcessingInfo)')
- [ISkiaGridLayout](#T-DrawnUi-Maui-Draw-ISkiaGridLayout 'DrawnUi.Maui.Draw.ISkiaGridLayout')
  - [ColumnDefinitions](#P-DrawnUi-Maui-Draw-ISkiaGridLayout-ColumnDefinitions 'DrawnUi.Maui.Draw.ISkiaGridLayout.ColumnDefinitions')
  - [ColumnSpacing](#P-DrawnUi-Maui-Draw-ISkiaGridLayout-ColumnSpacing 'DrawnUi.Maui.Draw.ISkiaGridLayout.ColumnSpacing')
  - [RowDefinitions](#P-DrawnUi-Maui-Draw-ISkiaGridLayout-RowDefinitions 'DrawnUi.Maui.Draw.ISkiaGridLayout.RowDefinitions')
  - [RowSpacing](#P-DrawnUi-Maui-Draw-ISkiaGridLayout-RowSpacing 'DrawnUi.Maui.Draw.ISkiaGridLayout.RowSpacing')
  - [GetColumn(view)](#M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetColumn-Microsoft-Maui-Controls-BindableObject- 'DrawnUi.Maui.Draw.ISkiaGridLayout.GetColumn(Microsoft.Maui.Controls.BindableObject)')
  - [GetColumnSpan(view)](#M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetColumnSpan-Microsoft-Maui-Controls-BindableObject- 'DrawnUi.Maui.Draw.ISkiaGridLayout.GetColumnSpan(Microsoft.Maui.Controls.BindableObject)')
  - [GetRow(view)](#M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetRow-Microsoft-Maui-Controls-BindableObject- 'DrawnUi.Maui.Draw.ISkiaGridLayout.GetRow(Microsoft.Maui.Controls.BindableObject)')
  - [GetRowSpan(view)](#M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetRowSpan-Microsoft-Maui-Controls-BindableObject- 'DrawnUi.Maui.Draw.ISkiaGridLayout.GetRowSpan(Microsoft.Maui.Controls.BindableObject)')
- [ISkiaLayer](#T-DrawnUi-Maui-Draw-ISkiaLayer 'DrawnUi.Maui.Draw.ISkiaLayer')
  - [HasValidSnapshot](#P-DrawnUi-Maui-Draw-ISkiaLayer-HasValidSnapshot 'DrawnUi.Maui.Draw.ISkiaLayer.HasValidSnapshot')
  - [LayerPaintArgs](#P-DrawnUi-Maui-Draw-ISkiaLayer-LayerPaintArgs 'DrawnUi.Maui.Draw.ISkiaLayer.LayerPaintArgs')
- [ISkiaSharpView](#T-DrawnUi-Maui-Draw-ISkiaSharpView 'DrawnUi.Maui.Draw.ISkiaSharpView')
  - [CreateStandaloneSurface(width,height)](#M-DrawnUi-Maui-Draw-ISkiaSharpView-CreateStandaloneSurface-System-Int32,System-Int32- 'DrawnUi.Maui.Draw.ISkiaSharpView.CreateStandaloneSurface(System.Int32,System.Int32)')
  - [Update()](#M-DrawnUi-Maui-Draw-ISkiaSharpView-Update-System-Int64- 'DrawnUi.Maui.Draw.ISkiaSharpView.Update(System.Int64)')
- [IStateEffect](#T-DrawnUi-Maui-Draw-IStateEffect 'DrawnUi.Maui.Draw.IStateEffect')
  - [UpdateState()](#M-DrawnUi-Maui-Draw-IStateEffect-UpdateState 'DrawnUi.Maui.Draw.IStateEffect.UpdateState')
- [IVisibilityAware](#T-DrawnUi-Maui-Draw-IVisibilityAware 'DrawnUi.Maui.Draw.IVisibilityAware')
  - [OnAppeared()](#M-DrawnUi-Maui-Draw-IVisibilityAware-OnAppeared 'DrawnUi.Maui.Draw.IVisibilityAware.OnAppeared')
  - [OnAppearing()](#M-DrawnUi-Maui-Draw-IVisibilityAware-OnAppearing 'DrawnUi.Maui.Draw.IVisibilityAware.OnAppearing')
- [InternalExtensions](#T-DrawnUi-Maui-Extensions-InternalExtensions 'DrawnUi.Maui.Extensions.InternalExtensions')
  - [ContainsInclusive(rect,point)](#M-DrawnUi-Maui-Extensions-InternalExtensions-ContainsInclusive-SkiaSharp-SKRect,SkiaSharp-SKPoint- 'DrawnUi.Maui.Extensions.InternalExtensions.ContainsInclusive(SkiaSharp.SKRect,SkiaSharp.SKPoint)')
  - [ContainsInclusive()](#M-DrawnUi-Maui-Extensions-InternalExtensions-ContainsInclusive-SkiaSharp-SKRect,System-Single,System-Single- 'DrawnUi.Maui.Extensions.InternalExtensions.ContainsInclusive(SkiaSharp.SKRect,System.Single,System.Single)')
- [LayoutType](#T-DrawnUi-Maui-Draw-LayoutType 'DrawnUi.Maui.Draw.LayoutType')
  - [Absolute](#F-DrawnUi-Maui-Draw-LayoutType-Absolute 'DrawnUi.Maui.Draw.LayoutType.Absolute')
  - [Column](#F-DrawnUi-Maui-Draw-LayoutType-Column 'DrawnUi.Maui.Draw.LayoutType.Column')
  - [Grid](#F-DrawnUi-Maui-Draw-LayoutType-Grid 'DrawnUi.Maui.Draw.LayoutType.Grid')
  - [Row](#F-DrawnUi-Maui-Draw-LayoutType-Row 'DrawnUi.Maui.Draw.LayoutType.Row')
  - [Wrap](#F-DrawnUi-Maui-Draw-LayoutType-Wrap 'DrawnUi.Maui.Draw.LayoutType.Wrap')
- [LineGlyph](#T-DrawnUi-Maui-Draw-LineGlyph 'DrawnUi.Maui.Draw.LineGlyph')
  - [Width](#P-DrawnUi-Maui-Draw-LineGlyph-Width 'DrawnUi.Maui.Draw.LineGlyph.Width')
- [LoadedImageSource](#T-DrawnUi-Maui-Draw-LoadedImageSource 'DrawnUi.Maui.Draw.LoadedImageSource')
  - [ProtectFromDispose](#P-DrawnUi-Maui-Draw-LoadedImageSource-ProtectFromDispose 'DrawnUi.Maui.Draw.LoadedImageSource.ProtectFromDispose')
- [LockTouch](#T-DrawnUi-Maui-Draw-LockTouch 'DrawnUi.Maui.Draw.LockTouch')
  - [Enabled](#F-DrawnUi-Maui-Draw-LockTouch-Enabled 'DrawnUi.Maui.Draw.LockTouch.Enabled')
  - [PassNone](#F-DrawnUi-Maui-Draw-LockTouch-PassNone 'DrawnUi.Maui.Draw.LockTouch.PassNone')
  - [PassTap](#F-DrawnUi-Maui-Draw-LockTouch-PassTap 'DrawnUi.Maui.Draw.LockTouch.PassTap')
  - [PassTapAndLongPress](#F-DrawnUi-Maui-Draw-LockTouch-PassTapAndLongPress 'DrawnUi.Maui.Draw.LockTouch.PassTapAndLongPress')
- [MauiEditor](#T-DrawnUi-Maui-Controls-MauiEditor 'DrawnUi.Maui.Controls.MauiEditor')
  - [MaxLines](#P-DrawnUi-Maui-Controls-MauiEditor-MaxLines 'DrawnUi.Maui.Controls.MauiEditor.MaxLines')
- [MauiEntry](#T-DrawnUi-Maui-Controls-MauiEntry 'DrawnUi.Maui.Controls.MauiEntry')
  - [MaxLines](#P-DrawnUi-Maui-Controls-MauiEntry-MaxLines 'DrawnUi.Maui.Controls.MauiEntry.MaxLines')
- [MauiKey](#T-DrawnUi-Maui-Draw-MauiKey 'DrawnUi.Maui.Draw.MauiKey')
- [MeasuringConstraints](#T-DrawnUi-Maui-Infrastructure-MeasuringConstraints 'DrawnUi.Maui.Infrastructure.MeasuringConstraints')
  - [TotalMargins](#P-DrawnUi-Maui-Infrastructure-MeasuringConstraints-TotalMargins 'DrawnUi.Maui.Infrastructure.MeasuringConstraints.TotalMargins')
- [MyTextWatcher](#T-DrawnUi-Maui-Draw-SkiaEditor-MyTextWatcher 'DrawnUi.Maui.Draw.SkiaEditor.MyTextWatcher')
  - [NativeSelectionStart](#P-DrawnUi-Maui-Draw-SkiaEditor-MyTextWatcher-NativeSelectionStart 'DrawnUi.Maui.Draw.SkiaEditor.MyTextWatcher.NativeSelectionStart')
- [NavigationLayer\`1](#T-DrawnUi-Maui-Controls-SkiaShell-NavigationLayer`1 'DrawnUi.Maui.Controls.SkiaShell.NavigationLayer`1')
  - [#ctor(shell,isModal)](#M-DrawnUi-Maui-Controls-SkiaShell-NavigationLayer`1-#ctor-DrawnUi-Maui-Controls-SkiaShell,System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.NavigationLayer`1.#ctor(DrawnUi.Maui.Controls.SkiaShell,System.Boolean)')
- [PanningModeType](#T-DrawnUi-Maui-Draw-PanningModeType 'DrawnUi.Maui.Draw.PanningModeType')
  - [Enabled](#F-DrawnUi-Maui-Draw-PanningModeType-Enabled 'DrawnUi.Maui.Draw.PanningModeType.Enabled')
- [PendulumAnimator](#T-DrawnUi-Maui-Draw-PendulumAnimator 'DrawnUi.Maui.Draw.PendulumAnimator')
  - [InitialVelocity](#P-DrawnUi-Maui-Draw-PendulumAnimator-InitialVelocity 'DrawnUi.Maui.Draw.PendulumAnimator.InitialVelocity')
  - [IsOneDirectional](#P-DrawnUi-Maui-Draw-PendulumAnimator-IsOneDirectional 'DrawnUi.Maui.Draw.PendulumAnimator.IsOneDirectional')
- [PointExtensions](#T-DrawnUi-Maui-Extensions-PointExtensions 'DrawnUi.Maui.Extensions.PointExtensions')
  - [Add(first,second)](#M-DrawnUi-Maui-Extensions-PointExtensions-Add-Microsoft-Maui-Graphics-Point,Microsoft-Maui-Graphics-Point- 'DrawnUi.Maui.Extensions.PointExtensions.Add(Microsoft.Maui.Graphics.Point,Microsoft.Maui.Graphics.Point)')
  - [Center(touches)](#M-DrawnUi-Maui-Extensions-PointExtensions-Center-Microsoft-Maui-Graphics-Point[]- 'DrawnUi.Maui.Extensions.PointExtensions.Center(Microsoft.Maui.Graphics.Point[])')
  - [Subtract(first,second)](#M-DrawnUi-Maui-Extensions-PointExtensions-Subtract-Microsoft-Maui-Graphics-Point,Microsoft-Maui-Graphics-Point- 'DrawnUi.Maui.Extensions.PointExtensions.Subtract(Microsoft.Maui.Graphics.Point,Microsoft.Maui.Graphics.Point)')
- [RecycleTemplateType](#T-DrawnUi-Maui-Draw-RecycleTemplateType 'DrawnUi.Maui.Draw.RecycleTemplateType')
  - [FillViewport](#F-DrawnUi-Maui-Draw-RecycleTemplateType-FillViewport 'DrawnUi.Maui.Draw.RecycleTemplateType.FillViewport')
  - [None](#F-DrawnUi-Maui-Draw-RecycleTemplateType-None 'DrawnUi.Maui.Draw.RecycleTemplateType.None')
  - [Single](#F-DrawnUi-Maui-Draw-RecycleTemplateType-Single 'DrawnUi.Maui.Draw.RecycleTemplateType.Single')
- [RefreshIndicator](#T-DrawnUi-Maui-Draw-RefreshIndicator 'DrawnUi.Maui.Draw.RefreshIndicator')
  - [IsRunning](#P-DrawnUi-Maui-Draw-RefreshIndicator-IsRunning 'DrawnUi.Maui.Draw.RefreshIndicator.IsRunning')
  - [Orientation](#P-DrawnUi-Maui-Draw-RefreshIndicator-Orientation 'DrawnUi.Maui.Draw.RefreshIndicator.Orientation')
  - [SetDragRatio(ratio)](#M-DrawnUi-Maui-Draw-RefreshIndicator-SetDragRatio-System-Single- 'DrawnUi.Maui.Draw.RefreshIndicator.SetDragRatio(System.Single)')
- [RenderingAnimator](#T-DrawnUi-Maui-Draw-RenderingAnimator 'DrawnUi.Maui.Draw.RenderingAnimator')
  - [OnRendering(control,context,scale)](#M-DrawnUi-Maui-Draw-RenderingAnimator-OnRendering-DrawnUi-Maui-Draw-IDrawnBase,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double- 'DrawnUi.Maui.Draw.RenderingAnimator.OnRendering(DrawnUi.Maui.Draw.IDrawnBase,DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)')
- [RippleAnimator](#T-DrawnUi-Maui-Draw-RippleAnimator 'DrawnUi.Maui.Draw.RippleAnimator')
  - [X](#P-DrawnUi-Maui-Draw-RippleAnimator-X 'DrawnUi.Maui.Draw.RippleAnimator.X')
  - [Y](#P-DrawnUi-Maui-Draw-RippleAnimator-Y 'DrawnUi.Maui.Draw.RippleAnimator.Y')
- [RubberBandUtils](#T-DrawnUi-Maui-Infrastructure-Helpers-RubberBandUtils 'DrawnUi.Maui.Infrastructure.Helpers.RubberBandUtils')
  - [ClampOnTrack(point,track,coeff)](#M-DrawnUi-Maui-Infrastructure-Helpers-RubberBandUtils-ClampOnTrack-System-Numerics-Vector2,SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Infrastructure.Helpers.RubberBandUtils.ClampOnTrack(System.Numerics.Vector2,SkiaSharp.SKRect,System.Single)')
  - [RubberBandClamp(coord,coeff,dim,limits,onEmpty)](#M-DrawnUi-Maui-Infrastructure-Helpers-RubberBandUtils-RubberBandClamp-System-Single,System-Single,DrawnUi-Maui-Draw-RangeF,System-Single,System-Single- 'DrawnUi.Maui.Infrastructure.Helpers.RubberBandUtils.RubberBandClamp(System.Single,System.Single,DrawnUi.Maui.Draw.RangeF,System.Single,System.Single)')
- [ScrollingInteractionState](#T-DrawnUi-Maui-Draw-SkiaScroll-ScrollingInteractionState 'DrawnUi.Maui.Draw.SkiaScroll.ScrollingInteractionState')
- [SecondPassArrange](#T-DrawnUi-Maui-Draw-SkiaLayout-SecondPassArrange 'DrawnUi.Maui.Draw.SkiaLayout.SecondPassArrange')
  - [#ctor(cell,child,scale)](#M-DrawnUi-Maui-Draw-SkiaLayout-SecondPassArrange-#ctor-DrawnUi-Maui-Draw-ControlInStack,DrawnUi-Maui-Draw-SkiaControl,System-Single- 'DrawnUi.Maui.Draw.SkiaLayout.SecondPassArrange.#ctor(DrawnUi.Maui.Draw.ControlInStack,DrawnUi.Maui.Draw.SkiaControl,System.Single)')
- [ShapeType](#T-DrawnUi-Maui-Draw-ShapeType 'DrawnUi.Maui.Draw.ShapeType')
  - [Squricle](#F-DrawnUi-Maui-Draw-ShapeType-Squricle 'DrawnUi.Maui.Draw.ShapeType.Squricle')
- [Sk3dView](#T-DrawnUi-Maui-Draw-Sk3dView 'DrawnUi.Maui.Draw.Sk3dView')
  - [CameraDistance](#F-DrawnUi-Maui-Draw-Sk3dView-CameraDistance 'DrawnUi.Maui.Draw.Sk3dView.CameraDistance')
  - [Reset()](#M-DrawnUi-Maui-Draw-Sk3dView-Reset 'DrawnUi.Maui.Draw.Sk3dView.Reset')
- [SkSl](#T-DrawnUi-Maui-Infrastructure-SkSl 'DrawnUi.Maui.Infrastructure.SkSl')
  - [Compile(shaderCode,filename)](#M-DrawnUi-Maui-Infrastructure-SkSl-Compile-System-String,System-String- 'DrawnUi.Maui.Infrastructure.SkSl.Compile(System.String,System.String)')
- [SkiaBackdrop](#T-DrawnUi-Maui-Draw-SkiaBackdrop 'DrawnUi.Maui.Draw.SkiaBackdrop')
  - [ImagePaint](#F-DrawnUi-Maui-Draw-SkiaBackdrop-ImagePaint 'DrawnUi.Maui.Draw.SkiaBackdrop.ImagePaint')
  - [PaintColorFilter](#F-DrawnUi-Maui-Draw-SkiaBackdrop-PaintColorFilter 'DrawnUi.Maui.Draw.SkiaBackdrop.PaintColorFilter')
  - [PaintImageFilter](#F-DrawnUi-Maui-Draw-SkiaBackdrop-PaintImageFilter 'DrawnUi.Maui.Draw.SkiaBackdrop.PaintImageFilter')
  - [UseContext](#P-DrawnUi-Maui-Draw-SkiaBackdrop-UseContext 'DrawnUi.Maui.Draw.SkiaBackdrop.UseContext')
  - [AttachSource()](#M-DrawnUi-Maui-Draw-SkiaBackdrop-AttachSource 'DrawnUi.Maui.Draw.SkiaBackdrop.AttachSource')
  - [GetImage()](#M-DrawnUi-Maui-Draw-SkiaBackdrop-GetImage 'DrawnUi.Maui.Draw.SkiaBackdrop.GetImage')
- [SkiaButton](#T-DrawnUi-Maui-Draw-SkiaButton 'DrawnUi.Maui.Draw.SkiaButton')
  - [DelayCallbackMs](#F-DrawnUi-Maui-Draw-SkiaButton-DelayCallbackMs 'DrawnUi.Maui.Draw.SkiaButton.DelayCallbackMs')
  - [Text](#P-DrawnUi-Maui-Draw-SkiaButton-Text 'DrawnUi.Maui.Draw.SkiaButton.Text')
  - [CreateClip()](#M-DrawnUi-Maui-Draw-SkiaButton-CreateClip-System-Object,System-Boolean,SkiaSharp-SKPath- 'DrawnUi.Maui.Draw.SkiaButton.CreateClip(System.Object,System.Boolean,SkiaSharp.SKPath)')
- [SkiaCacheType](#T-DrawnUi-Maui-Draw-SkiaCacheType 'DrawnUi.Maui.Draw.SkiaCacheType')
  - [GPU](#F-DrawnUi-Maui-Draw-SkiaCacheType-GPU 'DrawnUi.Maui.Draw.SkiaCacheType.GPU')
  - [Image](#F-DrawnUi-Maui-Draw-SkiaCacheType-Image 'DrawnUi.Maui.Draw.SkiaCacheType.Image')
  - [ImageComposite](#F-DrawnUi-Maui-Draw-SkiaCacheType-ImageComposite 'DrawnUi.Maui.Draw.SkiaCacheType.ImageComposite')
  - [ImageDoubleBuffered](#F-DrawnUi-Maui-Draw-SkiaCacheType-ImageDoubleBuffered 'DrawnUi.Maui.Draw.SkiaCacheType.ImageDoubleBuffered')
  - [None](#F-DrawnUi-Maui-Draw-SkiaCacheType-None 'DrawnUi.Maui.Draw.SkiaCacheType.None')
  - [Operations](#F-DrawnUi-Maui-Draw-SkiaCacheType-Operations 'DrawnUi.Maui.Draw.SkiaCacheType.Operations')
  - [OperationsFull](#F-DrawnUi-Maui-Draw-SkiaCacheType-OperationsFull 'DrawnUi.Maui.Draw.SkiaCacheType.OperationsFull')
- [SkiaCarousel](#T-DrawnUi-Maui-Controls-SkiaCarousel 'DrawnUi.Maui.Controls.SkiaCarousel')
  - [DynamicSize](#P-DrawnUi-Maui-Controls-SkiaCarousel-DynamicSize 'DrawnUi.Maui.Controls.SkiaCarousel.DynamicSize')
  - [IsLooped](#P-DrawnUi-Maui-Controls-SkiaCarousel-IsLooped 'DrawnUi.Maui.Controls.SkiaCarousel.IsLooped')
  - [IsRightToLeft](#P-DrawnUi-Maui-Controls-SkiaCarousel-IsRightToLeft 'DrawnUi.Maui.Controls.SkiaCarousel.IsRightToLeft')
  - [IsVertical](#P-DrawnUi-Maui-Controls-SkiaCarousel-IsVertical 'DrawnUi.Maui.Controls.SkiaCarousel.IsVertical')
  - [LinearSpeedMs](#P-DrawnUi-Maui-Controls-SkiaCarousel-LinearSpeedMs 'DrawnUi.Maui.Controls.SkiaCarousel.LinearSpeedMs')
  - [PreloadNeighboors](#P-DrawnUi-Maui-Controls-SkiaCarousel-PreloadNeighboors 'DrawnUi.Maui.Controls.SkiaCarousel.PreloadNeighboors')
  - [ScrollAmount](#P-DrawnUi-Maui-Controls-SkiaCarousel-ScrollAmount 'DrawnUi.Maui.Controls.SkiaCarousel.ScrollAmount')
  - [ScrollProgress](#P-DrawnUi-Maui-Controls-SkiaCarousel-ScrollProgress 'DrawnUi.Maui.Controls.SkiaCarousel.ScrollProgress')
  - [SelectedIndex](#P-DrawnUi-Maui-Controls-SkiaCarousel-SelectedIndex 'DrawnUi.Maui.Controls.SkiaCarousel.SelectedIndex')
  - [SidesOffset](#P-DrawnUi-Maui-Controls-SkiaCarousel-SidesOffset 'DrawnUi.Maui.Controls.SkiaCarousel.SidesOffset')
  - [AdaptChildren()](#M-DrawnUi-Maui-Controls-SkiaCarousel-AdaptChildren 'DrawnUi.Maui.Controls.SkiaCarousel.AdaptChildren')
  - [ApplyPosition(currentPosition)](#M-DrawnUi-Maui-Controls-SkiaCarousel-ApplyPosition-System-Numerics-Vector2- 'DrawnUi.Maui.Controls.SkiaCarousel.ApplyPosition(System.Numerics.Vector2)')
  - [GetContentOffsetBounds()](#M-DrawnUi-Maui-Controls-SkiaCarousel-GetContentOffsetBounds 'DrawnUi.Maui.Controls.SkiaCarousel.GetContentOffsetBounds')
  - [InitializeChildren()](#M-DrawnUi-Maui-Controls-SkiaCarousel-InitializeChildren 'DrawnUi.Maui.Controls.SkiaCarousel.InitializeChildren')
  - [OnTemplatesAvailable()](#M-DrawnUi-Maui-Controls-SkiaCarousel-OnTemplatesAvailable 'DrawnUi.Maui.Controls.SkiaCarousel.OnTemplatesAvailable')
- [SkiaCheckbox](#T-DrawnUi-Maui-Draw-SkiaCheckbox 'DrawnUi.Maui.Draw.SkiaCheckbox')
- [SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl')
  - [LockDraw](#F-DrawnUi-Maui-Draw-SkiaControl-LockDraw 'DrawnUi.Maui.Draw.SkiaControl.LockDraw')
  - [LockRenderObject](#F-DrawnUi-Maui-Draw-SkiaControl-LockRenderObject 'DrawnUi.Maui.Draw.SkiaControl.LockRenderObject')
  - [AllowCaching](#P-DrawnUi-Maui-Draw-SkiaControl-AllowCaching 'DrawnUi.Maui.Draw.SkiaControl.AllowCaching')
  - [Bounds](#P-DrawnUi-Maui-Draw-SkiaControl-Bounds 'DrawnUi.Maui.Draw.SkiaControl.Bounds')
  - [CanUseCacheDoubleBuffering](#P-DrawnUi-Maui-Draw-SkiaControl-CanUseCacheDoubleBuffering 'DrawnUi.Maui.Draw.SkiaControl.CanUseCacheDoubleBuffering')
  - [ClipEffects](#P-DrawnUi-Maui-Draw-SkiaControl-ClipEffects 'DrawnUi.Maui.Draw.SkiaControl.ClipEffects')
  - [ClipFrom](#P-DrawnUi-Maui-Draw-SkiaControl-ClipFrom 'DrawnUi.Maui.Draw.SkiaControl.ClipFrom')
  - [CommandChildTapped](#P-DrawnUi-Maui-Draw-SkiaControl-CommandChildTapped 'DrawnUi.Maui.Draw.SkiaControl.CommandChildTapped')
  - [CreateChildren](#P-DrawnUi-Maui-Draw-SkiaControl-CreateChildren 'DrawnUi.Maui.Draw.SkiaControl.CreateChildren')
  - [CustomizeLayerPaint](#P-DrawnUi-Maui-Draw-SkiaControl-CustomizeLayerPaint 'DrawnUi.Maui.Draw.SkiaControl.CustomizeLayerPaint')
  - [Destination](#P-DrawnUi-Maui-Draw-SkiaControl-Destination 'DrawnUi.Maui.Draw.SkiaControl.Destination')
  - [DrawingRect](#P-DrawnUi-Maui-Draw-SkiaControl-DrawingRect 'DrawnUi.Maui.Draw.SkiaControl.DrawingRect')
  - [ExpandCacheRecordingArea](#P-DrawnUi-Maui-Draw-SkiaControl-ExpandCacheRecordingArea 'DrawnUi.Maui.Draw.SkiaControl.ExpandCacheRecordingArea')
  - [GestureListeners](#P-DrawnUi-Maui-Draw-SkiaControl-GestureListeners 'DrawnUi.Maui.Draw.SkiaControl.GestureListeners')
  - [HeightRequestRatio](#P-DrawnUi-Maui-Draw-SkiaControl-HeightRequestRatio 'DrawnUi.Maui.Draw.SkiaControl.HeightRequestRatio')
  - [Hero](#P-DrawnUi-Maui-Draw-SkiaControl-Hero 'DrawnUi.Maui.Draw.SkiaControl.Hero')
  - [HitBoxAuto](#P-DrawnUi-Maui-Draw-SkiaControl-HitBoxAuto 'DrawnUi.Maui.Draw.SkiaControl.HitBoxAuto')
  - [IsClippedToBounds](#P-DrawnUi-Maui-Draw-SkiaControl-IsClippedToBounds 'DrawnUi.Maui.Draw.SkiaControl.IsClippedToBounds')
  - [IsLayoutDirty](#P-DrawnUi-Maui-Draw-SkiaControl-IsLayoutDirty 'DrawnUi.Maui.Draw.SkiaControl.IsLayoutDirty')
  - [IsMeasuring](#P-DrawnUi-Maui-Draw-SkiaControl-IsMeasuring 'DrawnUi.Maui.Draw.SkiaControl.IsMeasuring')
  - [IsOverlay](#P-DrawnUi-Maui-Draw-SkiaControl-IsOverlay 'DrawnUi.Maui.Draw.SkiaControl.IsOverlay')
  - [IsRenderingWithComposition](#P-DrawnUi-Maui-Draw-SkiaControl-IsRenderingWithComposition 'DrawnUi.Maui.Draw.SkiaControl.IsRenderingWithComposition')
  - [ItemTemplate](#P-DrawnUi-Maui-Draw-SkiaControl-ItemTemplate 'DrawnUi.Maui.Draw.SkiaControl.ItemTemplate')
  - [ItemTemplateType](#P-DrawnUi-Maui-Draw-SkiaControl-ItemTemplateType 'DrawnUi.Maui.Draw.SkiaControl.ItemTemplateType')
  - [LastDrawnAt](#P-DrawnUi-Maui-Draw-SkiaControl-LastDrawnAt 'DrawnUi.Maui.Draw.SkiaControl.LastDrawnAt')
  - [LockChildrenGestures](#P-DrawnUi-Maui-Draw-SkiaControl-LockChildrenGestures 'DrawnUi.Maui.Draw.SkiaControl.LockChildrenGestures')
  - [LockRatio](#P-DrawnUi-Maui-Draw-SkiaControl-LockRatio 'DrawnUi.Maui.Draw.SkiaControl.LockRatio')
  - [Margins](#P-DrawnUi-Maui-Draw-SkiaControl-Margins 'DrawnUi.Maui.Draw.SkiaControl.Margins')
  - [NeedDispose](#P-DrawnUi-Maui-Draw-SkiaControl-NeedDispose 'DrawnUi.Maui.Draw.SkiaControl.NeedDispose')
  - [NeedUpdate](#P-DrawnUi-Maui-Draw-SkiaControl-NeedUpdate 'DrawnUi.Maui.Draw.SkiaControl.NeedUpdate')
  - [NeedUpdateFrontCache](#P-DrawnUi-Maui-Draw-SkiaControl-NeedUpdateFrontCache 'DrawnUi.Maui.Draw.SkiaControl.NeedUpdateFrontCache')
  - [Parent](#P-DrawnUi-Maui-Draw-SkiaControl-Parent 'DrawnUi.Maui.Draw.SkiaControl.Parent')
  - [PostAnimators](#P-DrawnUi-Maui-Draw-SkiaControl-PostAnimators 'DrawnUi.Maui.Draw.SkiaControl.PostAnimators')
  - [RenderObject](#P-DrawnUi-Maui-Draw-SkiaControl-RenderObject 'DrawnUi.Maui.Draw.SkiaControl.RenderObject')
  - [RenderObjectNeedsUpdate](#P-DrawnUi-Maui-Draw-SkiaControl-RenderObjectNeedsUpdate 'DrawnUi.Maui.Draw.SkiaControl.RenderObjectNeedsUpdate')
  - [RenderObjectPreparing](#P-DrawnUi-Maui-Draw-SkiaControl-RenderObjectPreparing 'DrawnUi.Maui.Draw.SkiaControl.RenderObjectPreparing')
  - [RenderObjectPrevious](#P-DrawnUi-Maui-Draw-SkiaControl-RenderObjectPrevious 'DrawnUi.Maui.Draw.SkiaControl.RenderObjectPrevious')
  - [RenderTree](#P-DrawnUi-Maui-Draw-SkiaControl-RenderTree 'DrawnUi.Maui.Draw.SkiaControl.RenderTree')
  - [Scale](#P-DrawnUi-Maui-Draw-SkiaControl-Scale 'DrawnUi.Maui.Draw.SkiaControl.Scale')
  - [ShouldClipAntialiased](#P-DrawnUi-Maui-Draw-SkiaControl-ShouldClipAntialiased 'DrawnUi.Maui.Draw.SkiaControl.ShouldClipAntialiased')
  - [SizeRequest](#P-DrawnUi-Maui-Draw-SkiaControl-SizeRequest 'DrawnUi.Maui.Draw.SkiaControl.SizeRequest')
  - [SkipRendering](#P-DrawnUi-Maui-Draw-SkiaControl-SkipRendering 'DrawnUi.Maui.Draw.SkiaControl.SkipRendering')
  - [Superview](#P-DrawnUi-Maui-Draw-SkiaControl-Superview 'DrawnUi.Maui.Draw.SkiaControl.Superview')
  - [UseCache](#P-DrawnUi-Maui-Draw-SkiaControl-UseCache 'DrawnUi.Maui.Draw.SkiaControl.UseCache')
  - [UsesCacheDoubleBuffering](#P-DrawnUi-Maui-Draw-SkiaControl-UsesCacheDoubleBuffering 'DrawnUi.Maui.Draw.SkiaControl.UsesCacheDoubleBuffering')
  - [ViewportHeightLimit](#P-DrawnUi-Maui-Draw-SkiaControl-ViewportHeightLimit 'DrawnUi.Maui.Draw.SkiaControl.ViewportHeightLimit')
  - [ViewportWidthLimit](#P-DrawnUi-Maui-Draw-SkiaControl-ViewportWidthLimit 'DrawnUi.Maui.Draw.SkiaControl.ViewportWidthLimit')
  - [WasDrawn](#P-DrawnUi-Maui-Draw-SkiaControl-WasDrawn 'DrawnUi.Maui.Draw.SkiaControl.WasDrawn')
  - [WidthRequestRatio](#P-DrawnUi-Maui-Draw-SkiaControl-WidthRequestRatio 'DrawnUi.Maui.Draw.SkiaControl.WidthRequestRatio')
  - [WillClipBounds](#P-DrawnUi-Maui-Draw-SkiaControl-WillClipBounds 'DrawnUi.Maui.Draw.SkiaControl.WillClipBounds')
  - [X](#P-DrawnUi-Maui-Draw-SkiaControl-X 'DrawnUi.Maui.Draw.SkiaControl.X')
  - [Y](#P-DrawnUi-Maui-Draw-SkiaControl-Y 'DrawnUi.Maui.Draw.SkiaControl.Y')
  - [AdaptHeightContraintToRequest(heightConstraint,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-AdaptHeightContraintToRequest-System-Single,Microsoft-Maui-Thickness,System-Double- 'DrawnUi.Maui.Draw.SkiaControl.AdaptHeightContraintToRequest(System.Single,Microsoft.Maui.Thickness,System.Double)')
  - [AdaptSizeRequestToContent(widthRequestPts,heightRequestPts)](#M-DrawnUi-Maui-Draw-SkiaControl-AdaptSizeRequestToContent-System-Double,System-Double- 'DrawnUi.Maui.Draw.SkiaControl.AdaptSizeRequestToContent(System.Double,System.Double)')
  - [AdaptWidthConstraintToRequest(widthConstraint,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-AdaptWidthConstraintToRequest-System-Single,Microsoft-Maui-Thickness,System-Double- 'DrawnUi.Maui.Draw.SkiaControl.AdaptWidthConstraintToRequest(System.Single,Microsoft.Maui.Thickness,System.Double)')
  - [AnimateAsync(callback,length,easing,cancel)](#M-DrawnUi-Maui-Draw-SkiaControl-AnimateAsync-System-Action{System-Double},System-Action,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource- 'DrawnUi.Maui.Draw.SkiaControl.AnimateAsync(System.Action{System.Double},System.Action,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)')
  - [ApplyBindingContext()](#M-DrawnUi-Maui-Draw-SkiaControl-ApplyBindingContext 'DrawnUi.Maui.Draw.SkiaControl.ApplyBindingContext')
  - [ApplyMeasureResult()](#M-DrawnUi-Maui-Draw-SkiaControl-ApplyMeasureResult 'DrawnUi.Maui.Draw.SkiaControl.ApplyMeasureResult')
  - [AreClose(value1,value2)](#M-DrawnUi-Maui-Draw-SkiaControl-AreClose-System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.AreClose(System.Single,System.Single)')
  - [AreClose(value1,value2)](#M-DrawnUi-Maui-Draw-SkiaControl-AreClose-System-Double,System-Double- 'DrawnUi.Maui.Draw.SkiaControl.AreClose(System.Double,System.Double)')
  - [Arrange(destination,widthRequest,heightRequest,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-Arrange-SkiaSharp-SKRect,System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.Arrange(SkiaSharp.SKRect,System.Single,System.Single,System.Single)')
  - [CalculateLayout(destination,widthRequest,heightRequest,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-CalculateLayout-SkiaSharp-SKRect,System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.CalculateLayout(SkiaSharp.SKRect,System.Single,System.Single,System.Single)')
  - [CalculateMargins()](#M-DrawnUi-Maui-Draw-SkiaControl-CalculateMargins 'DrawnUi.Maui.Draw.SkiaControl.CalculateMargins')
  - [ClipSmart(canvas,path,operation)](#M-DrawnUi-Maui-Draw-SkiaControl-ClipSmart-SkiaSharp-SKCanvas,SkiaSharp-SKPath,SkiaSharp-SKClipOperation- 'DrawnUi.Maui.Draw.SkiaControl.ClipSmart(SkiaSharp.SKCanvas,SkiaSharp.SKPath,SkiaSharp.SKClipOperation)')
  - [CommitInvalidations()](#M-DrawnUi-Maui-Draw-SkiaControl-CommitInvalidations 'DrawnUi.Maui.Draw.SkiaControl.CommitInvalidations')
  - [CreateChildrenFromCode()](#M-DrawnUi-Maui-Draw-SkiaControl-CreateChildrenFromCode 'DrawnUi.Maui.Draw.SkiaControl.CreateChildrenFromCode')
  - [CreateClip(arguments)](#M-DrawnUi-Maui-Draw-SkiaControl-CreateClip-System-Object,System-Boolean,SkiaSharp-SKPath- 'DrawnUi.Maui.Draw.SkiaControl.CreateClip(System.Object,System.Boolean,SkiaSharp.SKPath)')
  - [CreateRenderingObjectAndPaint(context,recordArea,action)](#M-DrawnUi-Maui-Draw-SkiaControl-CreateRenderingObjectAndPaint-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,SkiaSharp-SKRect,System-Action{DrawnUi-Maui-Draw-SkiaDrawingContext}- 'DrawnUi.Maui.Draw.SkiaControl.CreateRenderingObjectAndPaint(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,SkiaSharp.SKRect,System.Action{DrawnUi.Maui.Draw.SkiaDrawingContext})')
  - [DefineAvailableSize(destination,widthRequest,heightRequest,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-DefineAvailableSize-SkiaSharp-SKRect,System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.DefineAvailableSize(SkiaSharp.SKRect,System.Single,System.Single,System.Single)')
  - [Dispose()](#M-DrawnUi-Maui-Draw-SkiaControl-Dispose 'DrawnUi.Maui.Draw.SkiaControl.Dispose')
  - [DisposeObject(disposable)](#M-DrawnUi-Maui-Draw-SkiaControl-DisposeObject-System-IDisposable- 'DrawnUi.Maui.Draw.SkiaControl.DisposeObject(System.IDisposable)')
  - [DrawRenderObject(cache,ctx,destination)](#M-DrawnUi-Maui-Draw-SkiaControl-DrawRenderObject-DrawnUi-Maui-Draw-CachedObject,DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaControl.DrawRenderObject(DrawnUi.Maui.Draw.CachedObject,DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)')
  - [DrawUsingRenderObject(context,widthRequest,heightRequest,destination,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-DrawUsingRenderObject-DrawnUi-Maui-Draw-SkiaDrawingContext,System-Single,System-Single,SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.DrawUsingRenderObject(DrawnUi.Maui.Draw.SkiaDrawingContext,System.Single,System.Single,SkiaSharp.SKRect,System.Single)')
  - [DrawViews(context,destination,scale,debug)](#M-DrawnUi-Maui-Draw-SkiaControl-DrawViews-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single,System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.DrawViews(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single,System.Boolean)')
  - [FadeToAsync(end,length,easing,cancel)](#M-DrawnUi-Maui-Draw-SkiaControl-FadeToAsync-System-Double,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource- 'DrawnUi.Maui.Draw.SkiaControl.FadeToAsync(System.Double,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)')
  - [FinalizeDrawingWithRenderObject(context,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-FinalizeDrawingWithRenderObject-DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double- 'DrawnUi.Maui.Draw.SkiaControl.FinalizeDrawingWithRenderObject(DrawnUi.Maui.Draw.SkiaDrawingContext,System.Double)')
  - [GenerateParentChain()](#M-DrawnUi-Maui-Draw-SkiaControl-GenerateParentChain 'DrawnUi.Maui.Draw.SkiaControl.GenerateParentChain')
  - [GestureIsInside(args)](#M-DrawnUi-Maui-Draw-SkiaControl-GestureIsInside-AppoMobi-Maui-Gestures-TouchActionEventArgs,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.GestureIsInside(AppoMobi.Maui.Gestures.TouchActionEventArgs,System.Single,System.Single)')
  - [GestureStartedInside(args)](#M-DrawnUi-Maui-Draw-SkiaControl-GestureStartedInside-AppoMobi-Maui-Gestures-TouchActionEventArgs,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.GestureStartedInside(AppoMobi.Maui.Gestures.TouchActionEventArgs,System.Single,System.Single)')
  - [GetCacheArea()](#M-DrawnUi-Maui-Draw-SkiaControl-GetCacheArea-SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaControl.GetCacheArea(SkiaSharp.SKRect)')
  - [GetCacheRecordingArea()](#M-DrawnUi-Maui-Draw-SkiaControl-GetCacheRecordingArea-SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaControl.GetCacheRecordingArea(SkiaSharp.SKRect)')
  - [GetFuturePositionOnCanvas()](#M-DrawnUi-Maui-Draw-SkiaControl-GetFuturePositionOnCanvas-System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.GetFuturePositionOnCanvas(System.Boolean)')
  - [GetFuturePositionOnCanvasInPoints()](#M-DrawnUi-Maui-Draw-SkiaControl-GetFuturePositionOnCanvasInPoints-System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.GetFuturePositionOnCanvasInPoints(System.Boolean)')
  - [GetOnScreenVisibleArea()](#M-DrawnUi-Maui-Draw-SkiaControl-GetOnScreenVisibleArea-System-Single- 'DrawnUi.Maui.Draw.SkiaControl.GetOnScreenVisibleArea(System.Single)')
  - [GetPositionOnCanvas()](#M-DrawnUi-Maui-Draw-SkiaControl-GetPositionOnCanvas-System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.GetPositionOnCanvas(System.Boolean)')
  - [GetPositionOnCanvasInPoints()](#M-DrawnUi-Maui-Draw-SkiaControl-GetPositionOnCanvasInPoints-System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.GetPositionOnCanvasInPoints(System.Boolean)')
  - [GetRenderingScaleFor(width,height)](#M-DrawnUi-Maui-Draw-SkiaControl-GetRenderingScaleFor-System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.GetRenderingScaleFor(System.Single,System.Single)')
  - [GetSelfDrawingPosition()](#M-DrawnUi-Maui-Draw-SkiaControl-GetSelfDrawingPosition 'DrawnUi.Maui.Draw.SkiaControl.GetSelfDrawingPosition')
  - [GetTopParentView()](#M-DrawnUi-Maui-Draw-SkiaControl-GetTopParentView 'DrawnUi.Maui.Draw.SkiaControl.GetTopParentView')
  - [HitIsInside()](#M-DrawnUi-Maui-Draw-SkiaControl-HitIsInside-System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.HitIsInside(System.Single,System.Single)')
  - [Invalidate()](#M-DrawnUi-Maui-Draw-SkiaControl-Invalidate 'DrawnUi.Maui.Draw.SkiaControl.Invalidate')
  - [InvalidateByChild(child)](#M-DrawnUi-Maui-Draw-SkiaControl-InvalidateByChild-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.SkiaControl.InvalidateByChild(DrawnUi.Maui.Draw.SkiaControl)')
  - [InvalidateChildren(control)](#M-DrawnUi-Maui-Draw-SkiaControl-InvalidateChildren-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.SkiaControl.InvalidateChildren(DrawnUi.Maui.Draw.SkiaControl)')
  - [InvalidateChildrenTree(control)](#M-DrawnUi-Maui-Draw-SkiaControl-InvalidateChildrenTree-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.SkiaControl.InvalidateChildrenTree(DrawnUi.Maui.Draw.SkiaControl)')
  - [InvalidateInternal()](#M-DrawnUi-Maui-Draw-SkiaControl-InvalidateInternal 'DrawnUi.Maui.Draw.SkiaControl.InvalidateInternal')
  - [InvalidateViewport()](#M-DrawnUi-Maui-Draw-SkiaControl-InvalidateViewport 'DrawnUi.Maui.Draw.SkiaControl.InvalidateViewport')
  - [IsOne(value)](#M-DrawnUi-Maui-Draw-SkiaControl-IsOne-System-Double- 'DrawnUi.Maui.Draw.SkiaControl.IsOne(System.Double)')
  - [IsPixelInside(x,y)](#M-DrawnUi-Maui-Draw-SkiaControl-IsPixelInside-System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.IsPixelInside(System.Single,System.Single)')
  - [IsPointInside(x,y,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-IsPointInside-System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.IsPointInside(System.Single,System.Single,System.Single)')
  - [Measure(widthConstraint,heightConstraint,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-Measure-System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.Measure(System.Single,System.Single,System.Single)')
  - [MeasureAbsoluteBase(rectForChildrenPixels,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-MeasureAbsoluteBase-SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.MeasureAbsoluteBase(SkiaSharp.SKRect,System.Single)')
  - [MeasureChild(child,availableWidth,availableHeight,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-MeasureChild-DrawnUi-Maui-Draw-SkiaControl,System-Double,System-Double,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.MeasureChild(DrawnUi.Maui.Draw.SkiaControl,System.Double,System.Double,System.Single)')
  - [MeasureContent(children,rectForChildrenPixels,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-MeasureContent-System-Collections-Generic-IEnumerable{DrawnUi-Maui-Draw-SkiaControl},SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.MeasureContent(System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl},SkiaSharp.SKRect,System.Single)')
  - [NeedRepaint()](#M-DrawnUi-Maui-Draw-SkiaControl-NeedRepaint-Microsoft-Maui-Controls-BindableObject,System-Object,System-Object- 'DrawnUi.Maui.Draw.SkiaControl.NeedRepaint(Microsoft.Maui.Controls.BindableObject,System.Object,System.Object)')
  - [OnBindingContextChanged()](#M-DrawnUi-Maui-Draw-SkiaControl-OnBindingContextChanged 'DrawnUi.Maui.Draw.SkiaControl.OnBindingContextChanged')
  - [OnDisposing()](#M-DrawnUi-Maui-Draw-SkiaControl-OnDisposing 'DrawnUi.Maui.Draw.SkiaControl.OnDisposing')
  - [OnLayoutReady()](#M-DrawnUi-Maui-Draw-SkiaControl-OnLayoutReady 'DrawnUi.Maui.Draw.SkiaControl.OnLayoutReady')
  - [OnParentVisibilityChanged(newvalue)](#M-DrawnUi-Maui-Draw-SkiaControl-OnParentVisibilityChanged-System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.OnParentVisibilityChanged(System.Boolean)')
  - [OnVisibilityChanged(newvalue)](#M-DrawnUi-Maui-Draw-SkiaControl-OnVisibilityChanged-System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.OnVisibilityChanged(System.Boolean)')
  - [OnWillDisposeWithChildren()](#M-DrawnUi-Maui-Draw-SkiaControl-OnWillDisposeWithChildren 'DrawnUi.Maui.Draw.SkiaControl.OnWillDisposeWithChildren')
  - [Paint(ctx,destination,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-Paint-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single,System-Object- 'DrawnUi.Maui.Draw.SkiaControl.Paint(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single,System.Object)')
  - [PaintTintBackground(canvas,destination)](#M-DrawnUi-Maui-Draw-SkiaControl-PaintTintBackground-SkiaSharp-SKCanvas,SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaControl.PaintTintBackground(SkiaSharp.SKCanvas,SkiaSharp.SKRect)')
  - [PlayRippleAnimation(color,x,y,removePrevious)](#M-DrawnUi-Maui-Draw-SkiaControl-PlayRippleAnimation-Microsoft-Maui-Graphics-Color,System-Double,System-Double,System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.PlayRippleAnimation(Microsoft.Maui.Graphics.Color,System.Double,System.Double,System.Boolean)')
  - [PostponeInvalidation(key,action)](#M-DrawnUi-Maui-Draw-SkiaControl-PostponeInvalidation-System-String,System-Action- 'DrawnUi.Maui.Draw.SkiaControl.PostponeInvalidation(System.String,System.Action)')
  - [PreArrange()](#M-DrawnUi-Maui-Draw-SkiaControl-PreArrange-SkiaSharp-SKRect,System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.PreArrange(SkiaSharp.SKRect,System.Single,System.Single,System.Single)')
  - [Reload()](#M-DrawnUi-Maui-Draw-SkiaControl-Reload 'DrawnUi.Maui.Draw.SkiaControl.Reload')
  - [RenderViewsList(skiaControls,context,destination,scale,debug)](#M-DrawnUi-Maui-Draw-SkiaControl-RenderViewsList-System-Collections-Generic-IEnumerable{DrawnUi-Maui-Draw-SkiaControl},DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single,System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.RenderViewsList(System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl},DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single,System.Boolean)')
  - [Repaint()](#M-DrawnUi-Maui-Draw-SkiaControl-Repaint 'DrawnUi.Maui.Draw.SkiaControl.Repaint')
  - [RotateToAsync(end,length,easing,cancel)](#M-DrawnUi-Maui-Draw-SkiaControl-RotateToAsync-System-Double,System-UInt32,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource- 'DrawnUi.Maui.Draw.SkiaControl.RotateToAsync(System.Double,System.UInt32,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)')
  - [SafeAction(action)](#M-DrawnUi-Maui-Draw-SkiaControl-SafeAction-System-Action- 'DrawnUi.Maui.Draw.SkiaControl.SafeAction(System.Action)')
  - [SafePostAction(action)](#M-DrawnUi-Maui-Draw-SkiaControl-SafePostAction-System-Action- 'DrawnUi.Maui.Draw.SkiaControl.SafePostAction(System.Action)')
  - [ScaleToAsync(x,y,length,easing,cancel)](#M-DrawnUi-Maui-Draw-SkiaControl-ScaleToAsync-System-Double,System-Double,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource- 'DrawnUi.Maui.Draw.SkiaControl.ScaleToAsync(System.Double,System.Double,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)')
  - [SetInheritedBindingContext(context)](#M-DrawnUi-Maui-Draw-SkiaControl-SetInheritedBindingContext-System-Object- 'DrawnUi.Maui.Draw.SkiaControl.SetInheritedBindingContext(System.Object)')
  - [SetMeasured(width,height,scale)](#M-DrawnUi-Maui-Draw-SkiaControl-SetMeasured-System-Single,System-Single,System-Boolean,System-Boolean,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.SetMeasured(System.Single,System.Single,System.Boolean,System.Boolean,System.Single)')
  - [SetVisualTransform(transform)](#M-DrawnUi-Maui-Draw-SkiaControl-SetVisualTransform-DrawnUi-Maui-Infrastructure-VisualTransform- 'DrawnUi.Maui.Draw.SkiaControl.SetVisualTransform(DrawnUi.Maui.Infrastructure.VisualTransform)')
  - [SetupGradient(paint,gradient,destination)](#M-DrawnUi-Maui-Draw-SkiaControl-SetupGradient-SkiaSharp-SKPaint,DrawnUi-Maui-Draw-SkiaGradient,SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaControl.SetupGradient(SkiaSharp.SKPaint,DrawnUi.Maui.Draw.SkiaGradient,SkiaSharp.SKRect)')
  - [SetupShadow(paint,shadow)](#M-DrawnUi-Maui-Draw-SkiaControl-SetupShadow-SkiaSharp-SKPaint,DrawnUi-Maui-Draw-SkiaShadow,System-Single- 'DrawnUi.Maui.Draw.SkiaControl.SetupShadow(SkiaSharp.SKPaint,DrawnUi.Maui.Draw.SkiaShadow,System.Single)')
  - [TranslateInputCoords(childOffset)](#M-DrawnUi-Maui-Draw-SkiaControl-TranslateInputCoords-SkiaSharp-SKPoint,System-Boolean- 'DrawnUi.Maui.Draw.SkiaControl.TranslateInputCoords(SkiaSharp.SKPoint,System.Boolean)')
  - [TranslateToAsync(x,y,length,easing,cancel)](#M-DrawnUi-Maui-Draw-SkiaControl-TranslateToAsync-System-Double,System-Double,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource- 'DrawnUi.Maui.Draw.SkiaControl.TranslateToAsync(System.Double,System.Double,System.Single,Microsoft.Maui.Easing,System.Threading.CancellationTokenSource)')
  - [Update()](#M-DrawnUi-Maui-Draw-SkiaControl-Update 'DrawnUi.Maui.Draw.SkiaControl.Update')
- [SkiaControlWithRect](#T-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect 'DrawnUi.Maui.Draw.SkiaControl.SkiaControlWithRect')
  - [#ctor(Control,Rect,Index)](#M-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-#ctor-DrawnUi-Maui-Draw-SkiaControl,SkiaSharp-SKRect,SkiaSharp-SKRect,System-Int32- 'DrawnUi.Maui.Draw.SkiaControl.SkiaControlWithRect.#ctor(DrawnUi.Maui.Draw.SkiaControl,SkiaSharp.SKRect,SkiaSharp.SKRect,System.Int32)')
  - [Control](#P-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-Control 'DrawnUi.Maui.Draw.SkiaControl.SkiaControlWithRect.Control')
  - [Index](#P-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-Index 'DrawnUi.Maui.Draw.SkiaControl.SkiaControlWithRect.Index')
  - [Rect](#P-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-Rect 'DrawnUi.Maui.Draw.SkiaControl.SkiaControlWithRect.Rect')
- [SkiaDrawer](#T-DrawnUi-Maui-Controls-SkiaDrawer 'DrawnUi.Maui.Controls.SkiaDrawer')
  - [AmplitudeSize](#P-DrawnUi-Maui-Controls-SkiaDrawer-AmplitudeSize 'DrawnUi.Maui.Controls.SkiaDrawer.AmplitudeSize')
  - [HeaderSize](#P-DrawnUi-Maui-Controls-SkiaDrawer-HeaderSize 'DrawnUi.Maui.Controls.SkiaDrawer.HeaderSize')
  - [ClampOffsetWithRubberBand(x,y)](#M-DrawnUi-Maui-Controls-SkiaDrawer-ClampOffsetWithRubberBand-System-Single,System-Single- 'DrawnUi.Maui.Controls.SkiaDrawer.ClampOffsetWithRubberBand(System.Single,System.Single)')
  - [GetClosestSidePoint(overscrollPoint,contentRect,viewportSize)](#M-DrawnUi-Maui-Controls-SkiaDrawer-GetClosestSidePoint-SkiaSharp-SKPoint,SkiaSharp-SKRect,SkiaSharp-SKSize- 'DrawnUi.Maui.Controls.SkiaDrawer.GetClosestSidePoint(SkiaSharp.SKPoint,SkiaSharp.SKRect,SkiaSharp.SKSize)')
  - [GetOffsetToHide()](#M-DrawnUi-Maui-Controls-SkiaDrawer-GetOffsetToHide 'DrawnUi.Maui.Controls.SkiaDrawer.GetOffsetToHide')
- [SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext')
  - [IsRecycled](#P-DrawnUi-Maui-Draw-SkiaDrawingContext-IsRecycled 'DrawnUi.Maui.Draw.SkiaDrawingContext.IsRecycled')
  - [IsVirtual](#P-DrawnUi-Maui-Draw-SkiaDrawingContext-IsVirtual 'DrawnUi.Maui.Draw.SkiaDrawingContext.IsVirtual')
- [SkiaDrawnCell](#T-DrawnUi-Maui-Controls-SkiaDrawnCell 'DrawnUi.Maui.Controls.SkiaDrawnCell')
- [SkiaDynamicDrawnCell](#T-DrawnUi-Maui-Controls-SkiaDynamicDrawnCell 'DrawnUi.Maui.Controls.SkiaDynamicDrawnCell')
- [SkiaEditor](#T-DrawnUi-Maui-Draw-SkiaEditor 'DrawnUi.Maui.Draw.SkiaEditor')
  - [GetCursorPosition(x,y)](#M-DrawnUi-Maui-Draw-SkiaEditor-GetCursorPosition-System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaEditor.GetCursorPosition(System.Single,System.Single)')
  - [MoveCursorTo(x,y)](#M-DrawnUi-Maui-Draw-SkiaEditor-MoveCursorTo-System-Double,System-Double- 'DrawnUi.Maui.Draw.SkiaEditor.MoveCursorTo(System.Double,System.Double)')
  - [MoveInternalCursor()](#M-DrawnUi-Maui-Draw-SkiaEditor-MoveInternalCursor 'DrawnUi.Maui.Draw.SkiaEditor.MoveInternalCursor')
  - [SetCursorPositionWithDelay(ms,position)](#M-DrawnUi-Maui-Draw-SkiaEditor-SetCursorPositionWithDelay-System-Int32,System-Int32- 'DrawnUi.Maui.Draw.SkiaEditor.SetCursorPositionWithDelay(System.Int32,System.Int32)')
  - [Submit()](#M-DrawnUi-Maui-Draw-SkiaEditor-Submit 'DrawnUi.Maui.Draw.SkiaEditor.Submit')
  - [UpdateCursorVisibility()](#M-DrawnUi-Maui-Draw-SkiaEditor-UpdateCursorVisibility 'DrawnUi.Maui.Draw.SkiaEditor.UpdateCursorVisibility')
- [SkiaEffect](#T-DrawnUi-Maui-Draw-SkiaEffect 'DrawnUi.Maui.Draw.SkiaEffect')
  - [Parent](#P-DrawnUi-Maui-Draw-SkiaEffect-Parent 'DrawnUi.Maui.Draw.SkiaEffect.Parent')
- [SkiaFontManager](#T-DrawnUi-Maui-Draw-SkiaFontManager 'DrawnUi.Maui.Draw.SkiaFontManager')
  - [GetEmbeddedResourceNames()](#M-DrawnUi-Maui-Draw-SkiaFontManager-GetEmbeddedResourceNames 'DrawnUi.Maui.Draw.SkiaFontManager.GetEmbeddedResourceNames')
  - [GetEmbeddedResourceStream(resourceName)](#M-DrawnUi-Maui-Draw-SkiaFontManager-GetEmbeddedResourceStream-System-String- 'DrawnUi.Maui.Draw.SkiaFontManager.GetEmbeddedResourceStream(System.String)')
  - [GetWeightEnum(weight)](#M-DrawnUi-Maui-Draw-SkiaFontManager-GetWeightEnum-System-Int32- 'DrawnUi.Maui.Draw.SkiaFontManager.GetWeightEnum(System.Int32)')
- [SkiaGif](#T-DrawnUi-Maui-Controls-SkiaGif 'DrawnUi.Maui.Controls.SkiaGif')
  - [#ctor()](#M-DrawnUi-Maui-Controls-SkiaGif-#ctor 'DrawnUi.Maui.Controls.SkiaGif.#ctor')
  - [#ctor(display)](#M-DrawnUi-Maui-Controls-SkiaGif-#ctor-DrawnUi-Maui-Draw-SkiaImage- 'DrawnUi.Maui.Controls.SkiaGif.#ctor(DrawnUi.Maui.Draw.SkiaImage)')
  - [OnAnimatorSeeking(frame)](#M-DrawnUi-Maui-Controls-SkiaGif-OnAnimatorSeeking-System-Double- 'DrawnUi.Maui.Controls.SkiaGif.OnAnimatorSeeking(System.Double)')
  - [OnAnimatorUpdated(value)](#M-DrawnUi-Maui-Controls-SkiaGif-OnAnimatorUpdated-System-Double- 'DrawnUi.Maui.Controls.SkiaGif.OnAnimatorUpdated(System.Double)')
- [SkiaGridStructure](#T-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure 'DrawnUi.Maui.Draw.SkiaLayout.SkiaGridStructure')
  - [_gridHeightConstraint](#F-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure-_gridHeightConstraint 'DrawnUi.Maui.Draw.SkiaLayout.SkiaGridStructure._gridHeightConstraint')
  - [_gridWidthConstraint](#F-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure-_gridWidthConstraint 'DrawnUi.Maui.Draw.SkiaLayout.SkiaGridStructure._gridWidthConstraint')
  - [InitializeCells()](#M-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure-InitializeCells 'DrawnUi.Maui.Draw.SkiaLayout.SkiaGridStructure.InitializeCells')
- [SkiaHotspot](#T-DrawnUi-Maui-Draw-SkiaHotspot 'DrawnUi.Maui.Draw.SkiaHotspot')
  - [DelayCallbackMs](#F-DrawnUi-Maui-Draw-SkiaHotspot-DelayCallbackMs 'DrawnUi.Maui.Draw.SkiaHotspot.DelayCallbackMs')
- [SkiaHotspotZoom](#T-DrawnUi-Maui-Draw-SkiaHotspotZoom 'DrawnUi.Maui.Draw.SkiaHotspotZoom')
  - [LastValue](#F-DrawnUi-Maui-Draw-SkiaHotspotZoom-LastValue 'DrawnUi.Maui.Draw.SkiaHotspotZoom.LastValue')
  - [ZoomSpeed](#P-DrawnUi-Maui-Draw-SkiaHotspotZoom-ZoomSpeed 'DrawnUi.Maui.Draw.SkiaHotspotZoom.ZoomSpeed')
- [SkiaHoverMask](#T-DrawnUi-Maui-Draw-SkiaHoverMask 'DrawnUi.Maui.Draw.SkiaHoverMask')
- [SkiaImage](#T-DrawnUi-Maui-Draw-SkiaImage 'DrawnUi.Maui.Draw.SkiaImage')
  - [ImagePaint](#F-DrawnUi-Maui-Draw-SkiaImage-ImagePaint 'DrawnUi.Maui.Draw.SkiaImage.ImagePaint')
  - [PaintColorFilter](#F-DrawnUi-Maui-Draw-SkiaImage-PaintColorFilter 'DrawnUi.Maui.Draw.SkiaImage.PaintColorFilter')
  - [PaintImageFilter](#F-DrawnUi-Maui-Draw-SkiaImage-PaintImageFilter 'DrawnUi.Maui.Draw.SkiaImage.PaintImageFilter')
  - [Aspect](#P-DrawnUi-Maui-Draw-SkiaImage-Aspect 'DrawnUi.Maui.Draw.SkiaImage.Aspect')
  - [EraseChangedContent](#P-DrawnUi-Maui-Draw-SkiaImage-EraseChangedContent 'DrawnUi.Maui.Draw.SkiaImage.EraseChangedContent')
  - [ImageBitmap](#P-DrawnUi-Maui-Draw-SkiaImage-ImageBitmap 'DrawnUi.Maui.Draw.SkiaImage.ImageBitmap')
  - [LastSource](#P-DrawnUi-Maui-Draw-SkiaImage-LastSource 'DrawnUi.Maui.Draw.SkiaImage.LastSource')
  - [LoadSourceOnFirstDraw](#P-DrawnUi-Maui-Draw-SkiaImage-LoadSourceOnFirstDraw 'DrawnUi.Maui.Draw.SkiaImage.LoadSourceOnFirstDraw')
  - [PreviewBase64](#P-DrawnUi-Maui-Draw-SkiaImage-PreviewBase64 'DrawnUi.Maui.Draw.SkiaImage.PreviewBase64')
  - [RescalingQuality](#P-DrawnUi-Maui-Draw-SkiaImage-RescalingQuality 'DrawnUi.Maui.Draw.SkiaImage.RescalingQuality')
  - [SourceHeight](#P-DrawnUi-Maui-Draw-SkiaImage-SourceHeight 'DrawnUi.Maui.Draw.SkiaImage.SourceHeight')
  - [SourceWidth](#P-DrawnUi-Maui-Draw-SkiaImage-SourceWidth 'DrawnUi.Maui.Draw.SkiaImage.SourceWidth')
  - [GetRenderedSource()](#M-DrawnUi-Maui-Draw-SkiaImage-GetRenderedSource 'DrawnUi.Maui.Draw.SkiaImage.GetRenderedSource')
  - [SetBitmapInternal(bitmap)](#M-DrawnUi-Maui-Draw-SkiaImage-SetBitmapInternal-SkiaSharp-SKBitmap,System-Boolean- 'DrawnUi.Maui.Draw.SkiaImage.SetBitmapInternal(SkiaSharp.SKBitmap,System.Boolean)')
  - [SetImage(loaded)](#M-DrawnUi-Maui-Draw-SkiaImage-SetImage-DrawnUi-Maui-Draw-LoadedImageSource- 'DrawnUi.Maui.Draw.SkiaImage.SetImage(DrawnUi.Maui.Draw.LoadedImageSource)')
- [SkiaImageEffect](#T-DrawnUi-Maui-Draw-SkiaImageEffect 'DrawnUi.Maui.Draw.SkiaImageEffect')
  - [Tint](#F-DrawnUi-Maui-Draw-SkiaImageEffect-Tint 'DrawnUi.Maui.Draw.SkiaImageEffect.Tint')
- [SkiaImageEffects](#T-DrawnUi-Maui-Draw-SkiaImageEffects 'DrawnUi.Maui.Draw.SkiaImageEffects')
  - [Brightness(amount)](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Brightness-System-Single- 'DrawnUi.Maui.Draw.SkiaImageEffects.Brightness(System.Single)')
  - [Contrast(amount)](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Contrast-System-Single- 'DrawnUi.Maui.Draw.SkiaImageEffects.Contrast(System.Single)')
  - [Gamma(gamma)](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Gamma-System-Single- 'DrawnUi.Maui.Draw.SkiaImageEffects.Gamma(System.Single)')
  - [Grayscale()](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Grayscale 'DrawnUi.Maui.Draw.SkiaImageEffects.Grayscale')
  - [Grayscale2()](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Grayscale2 'DrawnUi.Maui.Draw.SkiaImageEffects.Grayscale2')
  - [InvertColors()](#M-DrawnUi-Maui-Draw-SkiaImageEffects-InvertColors 'DrawnUi.Maui.Draw.SkiaImageEffects.InvertColors')
  - [Lightness(amount)](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Lightness-System-Single- 'DrawnUi.Maui.Draw.SkiaImageEffects.Lightness(System.Single)')
  - [Saturation(amount)](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Saturation-System-Single- 'DrawnUi.Maui.Draw.SkiaImageEffects.Saturation(System.Single)')
  - [Sepia()](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Sepia 'DrawnUi.Maui.Draw.SkiaImageEffects.Sepia')
  - [Tint(color,mode)](#M-DrawnUi-Maui-Draw-SkiaImageEffects-Tint-Microsoft-Maui-Graphics-Color,SkiaSharp-SKBlendMode- 'DrawnUi.Maui.Draw.SkiaImageEffects.Tint(Microsoft.Maui.Graphics.Color,SkiaSharp.SKBlendMode)')
- [SkiaImageManager](#T-DrawnUi-Maui-Draw-SkiaImageManager 'DrawnUi.Maui.Draw.SkiaImageManager')
  - [CacheLongevitySecs](#F-DrawnUi-Maui-Draw-SkiaImageManager-CacheLongevitySecs 'DrawnUi.Maui.Draw.SkiaImageManager.CacheLongevitySecs')
  - [NativeFilePrefix](#F-DrawnUi-Maui-Draw-SkiaImageManager-NativeFilePrefix 'DrawnUi.Maui.Draw.SkiaImageManager.NativeFilePrefix')
  - [ReuseBitmaps](#F-DrawnUi-Maui-Draw-SkiaImageManager-ReuseBitmaps 'DrawnUi.Maui.Draw.SkiaImageManager.ReuseBitmaps')
  - [AddToCache(uri,bitmap,cacheLongevityMinutes)](#M-DrawnUi-Maui-Draw-SkiaImageManager-AddToCache-System-String,SkiaSharp-SKBitmap,System-Int32- 'DrawnUi.Maui.Draw.SkiaImageManager.AddToCache(System.String,SkiaSharp.SKBitmap,System.Int32)')
  - [LoadImageAsync(source,token)](#M-DrawnUi-Maui-Draw-SkiaImageManager-LoadImageAsync-Microsoft-Maui-Controls-ImageSource,System-Threading-CancellationToken- 'DrawnUi.Maui.Draw.SkiaImageManager.LoadImageAsync(Microsoft.Maui.Controls.ImageSource,System.Threading.CancellationToken)')
  - [LoadImageManagedAsync(source,token)](#M-DrawnUi-Maui-Draw-SkiaImageManager-LoadImageManagedAsync-Microsoft-Maui-Controls-ImageSource,System-Threading-CancellationTokenSource,DrawnUi-Maui-Draw-LoadPriority- 'DrawnUi.Maui.Draw.SkiaImageManager.LoadImageManagedAsync(Microsoft.Maui.Controls.ImageSource,System.Threading.CancellationTokenSource,DrawnUi.Maui.Draw.LoadPriority)')
- [SkiaImageTiles](#T-DrawnUi-Maui-Draw-SkiaImageTiles 'DrawnUi.Maui.Draw.SkiaImageTiles')
  - [DrawTiles](#P-DrawnUi-Maui-Draw-SkiaImageTiles-DrawTiles 'DrawnUi.Maui.Draw.SkiaImageTiles.DrawTiles')
  - [Tile](#P-DrawnUi-Maui-Draw-SkiaImageTiles-Tile 'DrawnUi.Maui.Draw.SkiaImageTiles.Tile')
  - [TileAspect](#P-DrawnUi-Maui-Draw-SkiaImageTiles-TileAspect 'DrawnUi.Maui.Draw.SkiaImageTiles.TileAspect')
  - [OnSourceSuccess()](#M-DrawnUi-Maui-Draw-SkiaImageTiles-OnSourceSuccess 'DrawnUi.Maui.Draw.SkiaImageTiles.OnSourceSuccess')
- [SkiaLabel](#T-DrawnUi-Maui-Draw-SkiaLabel 'DrawnUi.Maui.Draw.SkiaLabel')
  - [CharacterSpacing](#P-DrawnUi-Maui-Draw-SkiaLabel-CharacterSpacing 'DrawnUi.Maui.Draw.SkiaLabel.CharacterSpacing')
  - [DropShadowOffsetX](#P-DrawnUi-Maui-Draw-SkiaLabel-DropShadowOffsetX 'DrawnUi.Maui.Draw.SkiaLabel.DropShadowOffsetX')
  - [FallbackCharacter](#P-DrawnUi-Maui-Draw-SkiaLabel-FallbackCharacter 'DrawnUi.Maui.Draw.SkiaLabel.FallbackCharacter')
  - [Font](#P-DrawnUi-Maui-Draw-SkiaLabel-Font 'DrawnUi.Maui.Draw.SkiaLabel.Font')
  - [LineHeightUniform](#P-DrawnUi-Maui-Draw-SkiaLabel-LineHeightUniform 'DrawnUi.Maui.Draw.SkiaLabel.LineHeightUniform')
  - [LineHeightWithSpacing](#P-DrawnUi-Maui-Draw-SkiaLabel-LineHeightWithSpacing 'DrawnUi.Maui.Draw.SkiaLabel.LineHeightWithSpacing')
  - [MonoForDigits](#P-DrawnUi-Maui-Draw-SkiaLabel-MonoForDigits 'DrawnUi.Maui.Draw.SkiaLabel.MonoForDigits')
  - [SpaceBetweenParagraphs](#P-DrawnUi-Maui-Draw-SkiaLabel-SpaceBetweenParagraphs 'DrawnUi.Maui.Draw.SkiaLabel.SpaceBetweenParagraphs')
  - [AddEmptyLine(result,span,totalHeight,heightBlock,isNewParagraph,needsShaping)](#M-DrawnUi-Maui-Draw-SkiaLabel-AddEmptyLine-System-Collections-Generic-List{DrawnUi-Maui-Draw-TextLine},DrawnUi-Maui-Draw-TextSpan,System-Single,System-Single,System-Boolean,System-Boolean- 'DrawnUi.Maui.Draw.SkiaLabel.AddEmptyLine(System.Collections.Generic.List{DrawnUi.Maui.Draw.TextLine},DrawnUi.Maui.Draw.TextSpan,System.Single,System.Single,System.Boolean,System.Boolean)')
  - [DrawCharacter(canvas,lineIndex,letterIndex,text,x,y,paint,paintStroke,scale)](#M-DrawnUi-Maui-Draw-SkiaLabel-DrawCharacter-SkiaSharp-SKCanvas,System-Int32,System-Int32,System-String,System-Single,System-Single,SkiaSharp-SKPaint,SkiaSharp-SKPaint,SkiaSharp-SKPaint,SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaLabel.DrawCharacter(SkiaSharp.SKCanvas,System.Int32,System.Int32,System.String,System.Single,System.Single,SkiaSharp.SKPaint,SkiaSharp.SKPaint,SkiaSharp.SKPaint,SkiaSharp.SKRect,System.Single)')
  - [DrawText(canvas,x,y,text,textPaint,strokePaint,scale)](#M-DrawnUi-Maui-Draw-SkiaLabel-DrawText-SkiaSharp-SKCanvas,System-Single,System-Single,System-String,SkiaSharp-SKPaint,SkiaSharp-SKPaint,SkiaSharp-SKPaint,System-Single- 'DrawnUi.Maui.Draw.SkiaLabel.DrawText(SkiaSharp.SKCanvas,System.Single,System.Single,System.String,SkiaSharp.SKPaint,SkiaSharp.SKPaint,SkiaSharp.SKPaint,System.Single)')
  - [MeasureText(paint,text,bounds)](#M-DrawnUi-Maui-Draw-SkiaLabel-MeasureText-SkiaSharp-SKPaint,System-String,SkiaSharp-SKRect@- 'DrawnUi.Maui.Draw.SkiaLabel.MeasureText(SkiaSharp.SKPaint,System.String,SkiaSharp.SKRect@)')
  - [OnFontUpdated()](#M-DrawnUi-Maui-Draw-SkiaLabel-OnFontUpdated 'DrawnUi.Maui.Draw.SkiaLabel.OnFontUpdated')
  - [OnSpanTapped(span)](#M-DrawnUi-Maui-Draw-SkiaLabel-OnSpanTapped-DrawnUi-Maui-Draw-TextSpan- 'DrawnUi.Maui.Draw.SkiaLabel.OnSpanTapped(DrawnUi.Maui.Draw.TextSpan)')
- [SkiaLayout](#T-DrawnUi-Maui-Draw-SkiaLayout 'DrawnUi.Maui.Draw.SkiaLayout')
  - [DefaultColumnDefinition](#P-DrawnUi-Maui-Draw-SkiaLayout-DefaultColumnDefinition 'DrawnUi.Maui.Draw.SkiaLayout.DefaultColumnDefinition')
  - [DefaultRowDefinition](#P-DrawnUi-Maui-Draw-SkiaLayout-DefaultRowDefinition 'DrawnUi.Maui.Draw.SkiaLayout.DefaultRowDefinition')
  - [DynamicColumns](#P-DrawnUi-Maui-Draw-SkiaLayout-DynamicColumns 'DrawnUi.Maui.Draw.SkiaLayout.DynamicColumns')
  - [InitializeTemplatesInBackgroundDelay](#P-DrawnUi-Maui-Draw-SkiaLayout-InitializeTemplatesInBackgroundDelay 'DrawnUi.Maui.Draw.SkiaLayout.InitializeTemplatesInBackgroundDelay')
  - [IsStack](#P-DrawnUi-Maui-Draw-SkiaLayout-IsStack 'DrawnUi.Maui.Draw.SkiaLayout.IsStack')
  - [ItemTemplatePoolSize](#P-DrawnUi-Maui-Draw-SkiaLayout-ItemTemplatePoolSize 'DrawnUi.Maui.Draw.SkiaLayout.ItemTemplatePoolSize')
  - [RecyclingTemplate](#P-DrawnUi-Maui-Draw-SkiaLayout-RecyclingTemplate 'DrawnUi.Maui.Draw.SkiaLayout.RecyclingTemplate')
  - [Split](#P-DrawnUi-Maui-Draw-SkiaLayout-Split 'DrawnUi.Maui.Draw.SkiaLayout.Split')
  - [SplitAlign](#P-DrawnUi-Maui-Draw-SkiaLayout-SplitAlign 'DrawnUi.Maui.Draw.SkiaLayout.SplitAlign')
  - [SplitSpace](#P-DrawnUi-Maui-Draw-SkiaLayout-SplitSpace 'DrawnUi.Maui.Draw.SkiaLayout.SplitSpace')
  - [StackStructure](#P-DrawnUi-Maui-Draw-SkiaLayout-StackStructure 'DrawnUi.Maui.Draw.SkiaLayout.StackStructure')
  - [StackStructureMeasured](#P-DrawnUi-Maui-Draw-SkiaLayout-StackStructureMeasured 'DrawnUi.Maui.Draw.SkiaLayout.StackStructureMeasured')
  - [TemplatedFooter](#P-DrawnUi-Maui-Draw-SkiaLayout-TemplatedFooter 'DrawnUi.Maui.Draw.SkiaLayout.TemplatedFooter')
  - [TemplatedHeader](#P-DrawnUi-Maui-Draw-SkiaLayout-TemplatedHeader 'DrawnUi.Maui.Draw.SkiaLayout.TemplatedHeader')
  - [Virtualisation](#P-DrawnUi-Maui-Draw-SkiaLayout-Virtualisation 'DrawnUi.Maui.Draw.SkiaLayout.Virtualisation')
  - [VirtualisationInflated](#P-DrawnUi-Maui-Draw-SkiaLayout-VirtualisationInflated 'DrawnUi.Maui.Draw.SkiaLayout.VirtualisationInflated')
  - [DrawChildrenGrid(context,destination,scale)](#M-DrawnUi-Maui-Draw-SkiaLayout-DrawChildrenGrid-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaLayout.DrawChildrenGrid(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single)')
  - [DrawStack()](#M-DrawnUi-Maui-Draw-SkiaLayout-DrawStack-DrawnUi-Maui-Draw-LayoutStructure,DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaLayout.DrawStack(DrawnUi.Maui.Draw.LayoutStructure,DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect,System.Single)')
  - [Measure(widthConstraint,heightConstraint,scale)](#M-DrawnUi-Maui-Draw-SkiaLayout-Measure-System-Single,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaLayout.Measure(System.Single,System.Single,System.Single)')
  - [MeasureStack(rectForChildrenPixels,scale)](#M-DrawnUi-Maui-Draw-SkiaLayout-MeasureStack-SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaLayout.MeasureStack(SkiaSharp.SKRect,System.Single)')
  - [MeasureWrap(rectForChildrenPixels,scale)](#M-DrawnUi-Maui-Draw-SkiaLayout-MeasureWrap-SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.SkiaLayout.MeasureWrap(SkiaSharp.SKRect,System.Single)')
  - [OnTemplatesAvailable()](#M-DrawnUi-Maui-Draw-SkiaLayout-OnTemplatesAvailable 'DrawnUi.Maui.Draw.SkiaLayout.OnTemplatesAvailable')
  - [SetupRenderingWithComposition(ctx,destination)](#M-DrawnUi-Maui-Draw-SkiaLayout-SetupRenderingWithComposition-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaLayout.SetupRenderingWithComposition(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)')
- [SkiaLottie](#T-DrawnUi-Maui-Controls-SkiaLottie 'DrawnUi.Maui.Controls.SkiaLottie')
  - [CachedAnimations](#F-DrawnUi-Maui-Controls-SkiaLottie-CachedAnimations 'DrawnUi.Maui.Controls.SkiaLottie.CachedAnimations')
  - [DefaultFrameWhenOn](#P-DrawnUi-Maui-Controls-SkiaLottie-DefaultFrameWhenOn 'DrawnUi.Maui.Controls.SkiaLottie.DefaultFrameWhenOn')
  - [LoadAnimationFromJson(json)](#M-DrawnUi-Maui-Controls-SkiaLottie-LoadAnimationFromJson-System-String- 'DrawnUi.Maui.Controls.SkiaLottie.LoadAnimationFromJson(System.String)')
  - [OnJsonLoaded()](#M-DrawnUi-Maui-Controls-SkiaLottie-OnJsonLoaded-System-String- 'DrawnUi.Maui.Controls.SkiaLottie.OnJsonLoaded(System.String)')
- [SkiaMarkdownLabel](#T-DrawnUi-Maui-Draw-SkiaMarkdownLabel 'DrawnUi.Maui.Draw.SkiaMarkdownLabel')
  - [OnSpanTapped(span)](#M-DrawnUi-Maui-Draw-SkiaMarkdownLabel-OnSpanTapped-DrawnUi-Maui-Draw-TextSpan- 'DrawnUi.Maui.Draw.SkiaMarkdownLabel.OnSpanTapped(DrawnUi.Maui.Draw.TextSpan)')
  - [ProcessSpanData(spanData,originalTypeFace)](#M-DrawnUi-Maui-Draw-SkiaMarkdownLabel-ProcessSpanData-System-Collections-Generic-List{System-ValueTuple{System-String,SkiaSharp-SKTypeface,System-Int32,System-Boolean}}@,SkiaSharp-SKTypeface- 'DrawnUi.Maui.Draw.SkiaMarkdownLabel.ProcessSpanData(System.Collections.Generic.List{System.ValueTuple{System.String,SkiaSharp.SKTypeface,System.Int32,System.Boolean}}@,SkiaSharp.SKTypeface)')
- [SkiaMauiEditor](#T-DrawnUi-Maui-Controls-SkiaMauiEditor 'DrawnUi.Maui.Controls.SkiaMauiEditor')
  - [LockFocus](#P-DrawnUi-Maui-Controls-SkiaMauiEditor-LockFocus 'DrawnUi.Maui.Controls.SkiaMauiEditor.LockFocus')
  - [MaxLines](#P-DrawnUi-Maui-Controls-SkiaMauiEditor-MaxLines 'DrawnUi.Maui.Controls.SkiaMauiEditor.MaxLines')
  - [OnControlFocused(sender,e)](#M-DrawnUi-Maui-Controls-SkiaMauiEditor-OnControlFocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs- 'DrawnUi.Maui.Controls.SkiaMauiEditor.OnControlFocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)')
  - [OnControlUnfocused(sender,e)](#M-DrawnUi-Maui-Controls-SkiaMauiEditor-OnControlUnfocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs- 'DrawnUi.Maui.Controls.SkiaMauiEditor.OnControlUnfocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)')
  - [OnFocusChanged(focus)](#M-DrawnUi-Maui-Controls-SkiaMauiEditor-OnFocusChanged-System-Boolean- 'DrawnUi.Maui.Controls.SkiaMauiEditor.OnFocusChanged(System.Boolean)')
- [SkiaMauiElement](#T-DrawnUi-Maui-Draw-SkiaMauiElement 'DrawnUi.Maui.Draw.SkiaMauiElement')
  - [AnimateSnapshot](#P-DrawnUi-Maui-Draw-SkiaMauiElement-AnimateSnapshot 'DrawnUi.Maui.Draw.SkiaMauiElement.AnimateSnapshot')
  - [Element](#P-DrawnUi-Maui-Draw-SkiaMauiElement-Element 'DrawnUi.Maui.Draw.SkiaMauiElement.Element')
  - [ElementSize](#P-DrawnUi-Maui-Draw-SkiaMauiElement-ElementSize 'DrawnUi.Maui.Draw.SkiaMauiElement.ElementSize')
  - [GetVisualChildren()](#M-DrawnUi-Maui-Draw-SkiaMauiElement-GetVisualChildren 'DrawnUi.Maui.Draw.SkiaMauiElement.GetVisualChildren')
  - [MeasureAndArrangeMauiElement(ptsWidth,ptsHeight)](#M-DrawnUi-Maui-Draw-SkiaMauiElement-MeasureAndArrangeMauiElement-System-Double,System-Double- 'DrawnUi.Maui.Draw.SkiaMauiElement.MeasureAndArrangeMauiElement(System.Double,System.Double)')
  - [OnChildAdded()](#M-DrawnUi-Maui-Draw-SkiaMauiElement-OnChildAdded-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.SkiaMauiElement.OnChildAdded(DrawnUi.Maui.Draw.SkiaControl)')
  - [SetChildren(views)](#M-DrawnUi-Maui-Draw-SkiaMauiElement-SetChildren-System-Collections-Generic-IEnumerable{DrawnUi-Maui-Draw-SkiaControl}- 'DrawnUi.Maui.Draw.SkiaMauiElement.SetChildren(System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl})')
  - [SetContent()](#M-DrawnUi-Maui-Draw-SkiaMauiElement-SetContent-Microsoft-Maui-Controls-VisualElement- 'DrawnUi.Maui.Draw.SkiaMauiElement.SetContent(Microsoft.Maui.Controls.VisualElement)')
  - [SetNativeVisibility(state)](#M-DrawnUi-Maui-Draw-SkiaMauiElement-SetNativeVisibility-System-Boolean- 'DrawnUi.Maui.Draw.SkiaMauiElement.SetNativeVisibility(System.Boolean)')
- [SkiaMauiEntry](#T-DrawnUi-Maui-Controls-SkiaMauiEntry 'DrawnUi.Maui.Controls.SkiaMauiEntry')
  - [LockFocus](#P-DrawnUi-Maui-Controls-SkiaMauiEntry-LockFocus 'DrawnUi.Maui.Controls.SkiaMauiEntry.LockFocus')
  - [MaxLines](#P-DrawnUi-Maui-Controls-SkiaMauiEntry-MaxLines 'DrawnUi.Maui.Controls.SkiaMauiEntry.MaxLines')
  - [OnControlFocused(sender,e)](#M-DrawnUi-Maui-Controls-SkiaMauiEntry-OnControlFocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs- 'DrawnUi.Maui.Controls.SkiaMauiEntry.OnControlFocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)')
  - [OnControlUnfocused(sender,e)](#M-DrawnUi-Maui-Controls-SkiaMauiEntry-OnControlUnfocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs- 'DrawnUi.Maui.Controls.SkiaMauiEntry.OnControlUnfocused(System.Object,Microsoft.Maui.Controls.FocusEventArgs)')
  - [OnFocusChanged(focus)](#M-DrawnUi-Maui-Controls-SkiaMauiEntry-OnFocusChanged-System-Boolean- 'DrawnUi.Maui.Controls.SkiaMauiEntry.OnFocusChanged(System.Boolean)')
- [SkiaRadioButton](#T-DrawnUi-Maui-Controls-SkiaRadioButton 'DrawnUi.Maui.Controls.SkiaRadioButton')
  - [Text](#P-DrawnUi-Maui-Controls-SkiaRadioButton-Text 'DrawnUi.Maui.Controls.SkiaRadioButton.Text')
- [SkiaScroll](#T-DrawnUi-Maui-Draw-SkiaScroll 'DrawnUi.Maui.Draw.SkiaScroll')
  - [BouncesProperty](#F-DrawnUi-Maui-Draw-SkiaScroll-BouncesProperty 'DrawnUi.Maui.Draw.SkiaScroll.BouncesProperty')
  - [InterpolationFactor](#F-DrawnUi-Maui-Draw-SkiaScroll-InterpolationFactor 'DrawnUi.Maui.Draw.SkiaScroll.InterpolationFactor')
  - [OrderedScrollTo](#F-DrawnUi-Maui-Draw-SkiaScroll-OrderedScrollTo 'DrawnUi.Maui.Draw.SkiaScroll.OrderedScrollTo')
  - [OrderedScrollToIndex](#F-DrawnUi-Maui-Draw-SkiaScroll-OrderedScrollToIndex 'DrawnUi.Maui.Draw.SkiaScroll.OrderedScrollToIndex')
  - [ScrollVelocityThreshold](#F-DrawnUi-Maui-Draw-SkiaScroll-ScrollVelocityThreshold 'DrawnUi.Maui.Draw.SkiaScroll.ScrollVelocityThreshold')
  - [SystemAnimationTimeSecs](#F-DrawnUi-Maui-Draw-SkiaScroll-SystemAnimationTimeSecs 'DrawnUi.Maui.Draw.SkiaScroll.SystemAnimationTimeSecs')
  - [ThesholdSwipeOnUp](#F-DrawnUi-Maui-Draw-SkiaScroll-ThesholdSwipeOnUp 'DrawnUi.Maui.Draw.SkiaScroll.ThesholdSwipeOnUp')
  - [_animatorFlingX](#F-DrawnUi-Maui-Draw-SkiaScroll-_animatorFlingX 'DrawnUi.Maui.Draw.SkiaScroll._animatorFlingX')
  - [_animatorFlingY](#F-DrawnUi-Maui-Draw-SkiaScroll-_animatorFlingY 'DrawnUi.Maui.Draw.SkiaScroll._animatorFlingY')
  - [_scrollMinX](#F-DrawnUi-Maui-Draw-SkiaScroll-_scrollMinX 'DrawnUi.Maui.Draw.SkiaScroll._scrollMinX')
  - [_scrollMinY](#F-DrawnUi-Maui-Draw-SkiaScroll-_scrollMinY 'DrawnUi.Maui.Draw.SkiaScroll._scrollMinY')
  - [_scrollerX](#F-DrawnUi-Maui-Draw-SkiaScroll-_scrollerX 'DrawnUi.Maui.Draw.SkiaScroll._scrollerX')
  - [_scrollerY](#F-DrawnUi-Maui-Draw-SkiaScroll-_scrollerY 'DrawnUi.Maui.Draw.SkiaScroll._scrollerY')
  - [snapMinimumVelocity](#F-DrawnUi-Maui-Draw-SkiaScroll-snapMinimumVelocity 'DrawnUi.Maui.Draw.SkiaScroll.snapMinimumVelocity')
  - [AutoScrollingSpeedMs](#P-DrawnUi-Maui-Draw-SkiaScroll-AutoScrollingSpeedMs 'DrawnUi.Maui.Draw.SkiaScroll.AutoScrollingSpeedMs')
  - [Bounces](#P-DrawnUi-Maui-Draw-SkiaScroll-Bounces 'DrawnUi.Maui.Draw.SkiaScroll.Bounces')
  - [CanScrollUsingHeader](#P-DrawnUi-Maui-Draw-SkiaScroll-CanScrollUsingHeader 'DrawnUi.Maui.Draw.SkiaScroll.CanScrollUsingHeader')
  - [ChangeDistancePanned](#P-DrawnUi-Maui-Draw-SkiaScroll-ChangeDistancePanned 'DrawnUi.Maui.Draw.SkiaScroll.ChangeDistancePanned')
  - [ChangeVelocityScrolled](#P-DrawnUi-Maui-Draw-SkiaScroll-ChangeVelocityScrolled 'DrawnUi.Maui.Draw.SkiaScroll.ChangeVelocityScrolled')
  - [ContentOffsetBounds](#P-DrawnUi-Maui-Draw-SkiaScroll-ContentOffsetBounds 'DrawnUi.Maui.Draw.SkiaScroll.ContentOffsetBounds')
  - [ContentRectWithOffset](#P-DrawnUi-Maui-Draw-SkiaScroll-ContentRectWithOffset 'DrawnUi.Maui.Draw.SkiaScroll.ContentRectWithOffset')
  - [FrictionScrolled](#P-DrawnUi-Maui-Draw-SkiaScroll-FrictionScrolled 'DrawnUi.Maui.Draw.SkiaScroll.FrictionScrolled')
  - [HeaderSticky](#P-DrawnUi-Maui-Draw-SkiaScroll-HeaderSticky 'DrawnUi.Maui.Draw.SkiaScroll.HeaderSticky')
  - [IgnoreWrongDirection](#P-DrawnUi-Maui-Draw-SkiaScroll-IgnoreWrongDirection 'DrawnUi.Maui.Draw.SkiaScroll.IgnoreWrongDirection')
  - [InternalViewportOffset](#P-DrawnUi-Maui-Draw-SkiaScroll-InternalViewportOffset 'DrawnUi.Maui.Draw.SkiaScroll.InternalViewportOffset')
  - [MaxBounceVelocity](#P-DrawnUi-Maui-Draw-SkiaScroll-MaxBounceVelocity 'DrawnUi.Maui.Draw.SkiaScroll.MaxBounceVelocity')
  - [MaxVelocity](#P-DrawnUi-Maui-Draw-SkiaScroll-MaxVelocity 'DrawnUi.Maui.Draw.SkiaScroll.MaxVelocity')
  - [Orientation](#P-DrawnUi-Maui-Draw-SkiaScroll-Orientation 'DrawnUi.Maui.Draw.SkiaScroll.Orientation')
  - [OverscrollDistance](#P-DrawnUi-Maui-Draw-SkiaScroll-OverscrollDistance 'DrawnUi.Maui.Draw.SkiaScroll.OverscrollDistance')
  - [RefreshDistanceLimit](#P-DrawnUi-Maui-Draw-SkiaScroll-RefreshDistanceLimit 'DrawnUi.Maui.Draw.SkiaScroll.RefreshDistanceLimit')
  - [RefreshShowDistance](#P-DrawnUi-Maui-Draw-SkiaScroll-RefreshShowDistance 'DrawnUi.Maui.Draw.SkiaScroll.RefreshShowDistance')
  - [RespondsToGestures](#P-DrawnUi-Maui-Draw-SkiaScroll-RespondsToGestures 'DrawnUi.Maui.Draw.SkiaScroll.RespondsToGestures')
  - [RubberDamping](#P-DrawnUi-Maui-Draw-SkiaScroll-RubberDamping 'DrawnUi.Maui.Draw.SkiaScroll.RubberDamping')
  - [RubberEffect](#P-DrawnUi-Maui-Draw-SkiaScroll-RubberEffect 'DrawnUi.Maui.Draw.SkiaScroll.RubberEffect')
  - [ScrollType](#P-DrawnUi-Maui-Draw-SkiaScroll-ScrollType 'DrawnUi.Maui.Draw.SkiaScroll.ScrollType')
  - [ScrollingSpeedMs](#P-DrawnUi-Maui-Draw-SkiaScroll-ScrollingSpeedMs 'DrawnUi.Maui.Draw.SkiaScroll.ScrollingSpeedMs')
  - [SnapToChildren](#P-DrawnUi-Maui-Draw-SkiaScroll-SnapToChildren 'DrawnUi.Maui.Draw.SkiaScroll.SnapToChildren')
  - [TrackIndexPosition](#P-DrawnUi-Maui-Draw-SkiaScroll-TrackIndexPosition 'DrawnUi.Maui.Draw.SkiaScroll.TrackIndexPosition')
  - [VelocityImageLoaderLock](#P-DrawnUi-Maui-Draw-SkiaScroll-VelocityImageLoaderLock 'DrawnUi.Maui.Draw.SkiaScroll.VelocityImageLoaderLock')
  - [Virtualisation](#P-DrawnUi-Maui-Draw-SkiaScroll-Virtualisation 'DrawnUi.Maui.Draw.SkiaScroll.Virtualisation')
  - [WasSwiping](#P-DrawnUi-Maui-Draw-SkiaScroll-WasSwiping 'DrawnUi.Maui.Draw.SkiaScroll.WasSwiping')
  - [ZoomScaleInternal](#P-DrawnUi-Maui-Draw-SkiaScroll-ZoomScaleInternal 'DrawnUi.Maui.Draw.SkiaScroll.ZoomScaleInternal')
  - [CalculateScrollOffsetForIndex(index,option)](#M-DrawnUi-Maui-Draw-SkiaScroll-CalculateScrollOffsetForIndex-System-Int32,DrawnUi-Maui-Draw-RelativePositionType- 'DrawnUi.Maui.Draw.SkiaScroll.CalculateScrollOffsetForIndex(System.Int32,DrawnUi.Maui.Draw.RelativePositionType)')
  - [CalculateVisibleIndex()](#M-DrawnUi-Maui-Draw-SkiaScroll-CalculateVisibleIndex-DrawnUi-Maui-Draw-RelativePositionType- 'DrawnUi.Maui.Draw.SkiaScroll.CalculateVisibleIndex(DrawnUi.Maui.Draw.RelativePositionType)')
  - [ClampOffsetWithRubberBand(x,y)](#M-DrawnUi-Maui-Draw-SkiaScroll-ClampOffsetWithRubberBand-System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaScroll.ClampOffsetWithRubberBand(System.Single,System.Single)')
  - [GetClosestSidePoint(overscrollPoint,contentRect,viewportSize)](#M-DrawnUi-Maui-Draw-SkiaScroll-GetClosestSidePoint-SkiaSharp-SKPoint,SkiaSharp-SKRect,SkiaSharp-SKSize- 'DrawnUi.Maui.Draw.SkiaScroll.GetClosestSidePoint(SkiaSharp.SKPoint,SkiaSharp.SKRect,SkiaSharp.SKSize)')
  - [GetContentAvailableRect(destination)](#M-DrawnUi-Maui-Draw-SkiaScroll-GetContentAvailableRect-SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaScroll.GetContentAvailableRect(SkiaSharp.SKRect)')
  - [GetContentOffsetBounds()](#M-DrawnUi-Maui-Draw-SkiaScroll-GetContentOffsetBounds 'DrawnUi.Maui.Draw.SkiaScroll.GetContentOffsetBounds')
  - [OnScrolled()](#M-DrawnUi-Maui-Draw-SkiaScroll-OnScrolled 'DrawnUi.Maui.Draw.SkiaScroll.OnScrolled')
  - [PositionViewport(destination,offsetPtsX,offsetPtsY,viewportScale,scale)](#M-DrawnUi-Maui-Draw-SkiaScroll-PositionViewport-SkiaSharp-SKRect,SkiaSharp-SKPoint,System-Single,System-Single- 'DrawnUi.Maui.Draw.SkiaScroll.PositionViewport(SkiaSharp.SKRect,SkiaSharp.SKPoint,System.Single,System.Single)')
  - [ScrollToOffset(offset,animate)](#M-DrawnUi-Maui-Draw-SkiaScroll-ScrollToOffset-System-Numerics-Vector2,System-Single- 'DrawnUi.Maui.Draw.SkiaScroll.ScrollToOffset(System.Numerics.Vector2,System.Single)')
  - [ScrollToX(offset,animate)](#M-DrawnUi-Maui-Draw-SkiaScroll-ScrollToX-System-Single,System-Boolean- 'DrawnUi.Maui.Draw.SkiaScroll.ScrollToX(System.Single,System.Boolean)')
  - [ScrollToY(offset,animate)](#M-DrawnUi-Maui-Draw-SkiaScroll-ScrollToY-System-Single,System-Boolean- 'DrawnUi.Maui.Draw.SkiaScroll.ScrollToY(System.Single,System.Boolean)')
  - [SetContent(view)](#M-DrawnUi-Maui-Draw-SkiaScroll-SetContent-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Draw.SkiaScroll.SetContent(DrawnUi.Maui.Draw.SkiaControl)')
- [SkiaScrollLooped](#T-DrawnUi-Maui-Draw-SkiaScrollLooped 'DrawnUi.Maui.Draw.SkiaScrollLooped')
  - [CycleSpace](#P-DrawnUi-Maui-Draw-SkiaScrollLooped-CycleSpace 'DrawnUi.Maui.Draw.SkiaScrollLooped.CycleSpace')
  - [IsBanner](#P-DrawnUi-Maui-Draw-SkiaScrollLooped-IsBanner 'DrawnUi.Maui.Draw.SkiaScrollLooped.IsBanner')
- [SkiaShaderEffect](#T-DrawnUi-Maui-Draw-SkiaShaderEffect 'DrawnUi.Maui.Draw.SkiaShaderEffect')
  - [AutoCreateInputTexture](#P-DrawnUi-Maui-Draw-SkiaShaderEffect-AutoCreateInputTexture 'DrawnUi.Maui.Draw.SkiaShaderEffect.AutoCreateInputTexture')
  - [UseContext](#P-DrawnUi-Maui-Draw-SkiaShaderEffect-UseContext 'DrawnUi.Maui.Draw.SkiaShaderEffect.UseContext')
  - [CreateSnapshot(ctx,destination)](#M-DrawnUi-Maui-Draw-SkiaShaderEffect-CreateSnapshot-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaShaderEffect.CreateSnapshot(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)')
  - [Render(ctx,destination)](#M-DrawnUi-Maui-Draw-SkiaShaderEffect-Render-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect- 'DrawnUi.Maui.Draw.SkiaShaderEffect.Render(DrawnUi.Maui.Draw.SkiaDrawingContext,SkiaSharp.SKRect)')
- [SkiaShape](#T-DrawnUi-Maui-Draw-SkiaShape 'DrawnUi.Maui.Draw.SkiaShape')
  - [ClipBackgroundColor](#P-DrawnUi-Maui-Draw-SkiaShape-ClipBackgroundColor 'DrawnUi.Maui.Draw.SkiaShape.ClipBackgroundColor')
  - [DrawPath](#P-DrawnUi-Maui-Draw-SkiaShape-DrawPath 'DrawnUi.Maui.Draw.SkiaShape.DrawPath')
  - [PathData](#P-DrawnUi-Maui-Draw-SkiaShape-PathData 'DrawnUi.Maui.Draw.SkiaShape.PathData')
- [SkiaShell](#T-DrawnUi-Maui-Controls-SkiaShell 'DrawnUi.Maui.Controls.SkiaShell')
  - [PopupBackgroundColor](#F-DrawnUi-Maui-Controls-SkiaShell-PopupBackgroundColor 'DrawnUi.Maui.Controls.SkiaShell.PopupBackgroundColor')
  - [PopupsBackgroundBlur](#F-DrawnUi-Maui-Controls-SkiaShell-PopupsBackgroundBlur 'DrawnUi.Maui.Controls.SkiaShell.PopupsBackgroundBlur')
  - [Buffer](#P-DrawnUi-Maui-Controls-SkiaShell-Buffer 'DrawnUi.Maui.Controls.SkiaShell.Buffer')
  - [FrozenLayers](#P-DrawnUi-Maui-Controls-SkiaShell-FrozenLayers 'DrawnUi.Maui.Controls.SkiaShell.FrozenLayers')
  - [NavigationLayout](#P-DrawnUi-Maui-Controls-SkiaShell-NavigationLayout 'DrawnUi.Maui.Controls.SkiaShell.NavigationLayout')
  - [RootLayout](#P-DrawnUi-Maui-Controls-SkiaShell-RootLayout 'DrawnUi.Maui.Controls.SkiaShell.RootLayout')
  - [CanFreezeLayout()](#M-DrawnUi-Maui-Controls-SkiaShell-CanFreezeLayout 'DrawnUi.Maui.Controls.SkiaShell.CanFreezeLayout')
  - [CanUnfreezeLayout()](#M-DrawnUi-Maui-Controls-SkiaShell-CanUnfreezeLayout 'DrawnUi.Maui.Controls.SkiaShell.CanUnfreezeLayout')
  - [ClosePopupAsync(animated)](#M-DrawnUi-Maui-Controls-SkiaShell-ClosePopupAsync-System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.ClosePopupAsync(System.Boolean)')
  - [FreezeRootLayout()](#M-DrawnUi-Maui-Controls-SkiaShell-FreezeRootLayout-DrawnUi-Maui-Draw-SkiaControl,System-Boolean,Microsoft-Maui-Graphics-Color,System-Single- 'DrawnUi.Maui.Controls.SkiaShell.FreezeRootLayout(DrawnUi.Maui.Draw.SkiaControl,System.Boolean,Microsoft.Maui.Graphics.Color,System.Single)')
  - [GetTopmostView()](#M-DrawnUi-Maui-Controls-SkiaShell-GetTopmostView 'DrawnUi.Maui.Controls.SkiaShell.GetTopmostView')
  - [GetTopmostViewInternal()](#M-DrawnUi-Maui-Controls-SkiaShell-GetTopmostViewInternal 'DrawnUi.Maui.Controls.SkiaShell.GetTopmostViewInternal')
  - [GoBackInRoute(animate)](#M-DrawnUi-Maui-Controls-SkiaShell-GoBackInRoute-System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.GoBackInRoute(System.Boolean)')
  - [GoToAsync(state,animate,arguments)](#M-DrawnUi-Maui-Controls-SkiaShell-GoToAsync-Microsoft-Maui-Controls-ShellNavigationState,System-Nullable{System-Boolean},System-Collections-Generic-IDictionary{System-String,System-Object}- 'DrawnUi.Maui.Controls.SkiaShell.GoToAsync(Microsoft.Maui.Controls.ShellNavigationState,System.Nullable{System.Boolean},System.Collections.Generic.IDictionary{System.String,System.Object})')
  - [InitializeNative(handler)](#M-DrawnUi-Maui-Controls-SkiaShell-InitializeNative-Microsoft-Maui-IViewHandler- 'DrawnUi.Maui.Controls.SkiaShell.InitializeNative(Microsoft.Maui.IViewHandler)')
  - [OnLayersChanged()](#M-DrawnUi-Maui-Controls-SkiaShell-OnLayersChanged-DrawnUi-Maui-Draw-SkiaControl- 'DrawnUi.Maui.Controls.SkiaShell.OnLayersChanged(DrawnUi.Maui.Draw.SkiaControl)')
  - [OpenPopupAsync(content,animated,closeWhenBackgroundTapped,scaleInFrom)](#M-DrawnUi-Maui-Controls-SkiaShell-OpenPopupAsync-DrawnUi-Maui-Draw-SkiaControl,System-Boolean,System-Boolean,System-Boolean,Microsoft-Maui-Graphics-Color,System-Nullable{SkiaSharp-SKPoint}- 'DrawnUi.Maui.Controls.SkiaShell.OpenPopupAsync(DrawnUi.Maui.Draw.SkiaControl,System.Boolean,System.Boolean,System.Boolean,Microsoft.Maui.Graphics.Color,System.Nullable{SkiaSharp.SKPoint})')
  - [PopAsync(animated)](#M-DrawnUi-Maui-Controls-SkiaShell-PopAsync-System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.PopAsync(System.Boolean)')
  - [PushAsync(page,animated)](#M-DrawnUi-Maui-Controls-SkiaShell-PushAsync-Microsoft-Maui-Controls-BindableObject,System-Boolean,System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.PushAsync(Microsoft.Maui.Controls.BindableObject,System.Boolean,System.Boolean)')
  - [PushAsync(page,animated)](#M-DrawnUi-Maui-Controls-SkiaShell-PushAsync-System-String,System-Boolean,System-Collections-Generic-IDictionary{System-String,System-Object}- 'DrawnUi.Maui.Controls.SkiaShell.PushAsync(System.String,System.Boolean,System.Collections.Generic.IDictionary{System.String,System.Object})')
  - [PushModalAsync(page,animated)](#M-DrawnUi-Maui-Controls-SkiaShell-PushModalAsync-Microsoft-Maui-Controls-BindableObject,System-Boolean,System-Boolean,System-Boolean,System-Collections-Generic-IDictionary{System-String,System-Object}- 'DrawnUi.Maui.Controls.SkiaShell.PushModalAsync(Microsoft.Maui.Controls.BindableObject,System.Boolean,System.Boolean,System.Boolean,System.Collections.Generic.IDictionary{System.String,System.Object})')
  - [SetFrozenLayerVisibility(control,parameters)](#M-DrawnUi-Maui-Controls-SkiaShell-SetFrozenLayerVisibility-DrawnUi-Maui-Draw-SkiaControl,System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.SetFrozenLayerVisibility(DrawnUi.Maui.Draw.SkiaControl,System.Boolean)')
  - [SetRoot(host,replace,arguments)](#M-DrawnUi-Maui-Controls-SkiaShell-SetRoot-System-String,System-Boolean,System-Collections-Generic-IDictionary{System-String,System-Object}- 'DrawnUi.Maui.Controls.SkiaShell.SetRoot(System.String,System.Boolean,System.Collections.Generic.IDictionary{System.String,System.Object})')
  - [SetupRoot(shellLayout)](#M-DrawnUi-Maui-Controls-SkiaShell-SetupRoot-DrawnUi-Maui-Draw-ISkiaControl- 'DrawnUi.Maui.Controls.SkiaShell.SetupRoot(DrawnUi.Maui.Draw.ISkiaControl)')
  - [UnfreezeRootLayout(control,animated)](#M-DrawnUi-Maui-Controls-SkiaShell-UnfreezeRootLayout-DrawnUi-Maui-Draw-SkiaControl,System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.UnfreezeRootLayout(DrawnUi.Maui.Draw.SkiaControl,System.Boolean)')
  - [WrapScreenshot(screenshot)](#M-DrawnUi-Maui-Controls-SkiaShell-WrapScreenshot-DrawnUi-Maui-Draw-SkiaControl,SkiaSharp-SKImage,Microsoft-Maui-Graphics-Color,System-Single,System-Boolean- 'DrawnUi.Maui.Controls.SkiaShell.WrapScreenshot(DrawnUi.Maui.Draw.SkiaControl,SkiaSharp.SKImage,Microsoft.Maui.Graphics.Color,System.Single,System.Boolean)')
- [SkiaShellNavigatedArgs](#T-DrawnUi-Maui-Controls-SkiaShellNavigatedArgs 'DrawnUi.Maui.Controls.SkiaShellNavigatedArgs')
  - [Route](#P-DrawnUi-Maui-Controls-SkiaShellNavigatedArgs-Route 'DrawnUi.Maui.Controls.SkiaShellNavigatedArgs.Route')
  - [View](#P-DrawnUi-Maui-Controls-SkiaShellNavigatedArgs-View 'DrawnUi.Maui.Controls.SkiaShellNavigatedArgs.View')
- [SkiaShellNavigatingArgs](#T-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs 'DrawnUi.Maui.Controls.SkiaShellNavigatingArgs')
  - [Cancel](#P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-Cancel 'DrawnUi.Maui.Controls.SkiaShellNavigatingArgs.Cancel')
  - [Previous](#P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-Previous 'DrawnUi.Maui.Controls.SkiaShellNavigatingArgs.Previous')
  - [Route](#P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-Route 'DrawnUi.Maui.Controls.SkiaShellNavigatingArgs.Route')
  - [View](#P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-View 'DrawnUi.Maui.Controls.SkiaShellNavigatingArgs.View')
- [SkiaSlider](#T-DrawnUi-Maui-Draw-SkiaSlider 'DrawnUi.Maui.Draw.SkiaSlider')
  - [moreHotspotSize](#F-DrawnUi-Maui-Draw-SkiaSlider-moreHotspotSize 'DrawnUi.Maui.Draw.SkiaSlider.moreHotspotSize')
  - [touchArea](#F-DrawnUi-Maui-Draw-SkiaSlider-touchArea 'DrawnUi.Maui.Draw.SkiaSlider.touchArea')
  - [End](#P-DrawnUi-Maui-Draw-SkiaSlider-End 'DrawnUi.Maui.Draw.SkiaSlider.End')
  - [IgnoreWrongDirection](#P-DrawnUi-Maui-Draw-SkiaSlider-IgnoreWrongDirection 'DrawnUi.Maui.Draw.SkiaSlider.IgnoreWrongDirection')
  - [Orientation](#P-DrawnUi-Maui-Draw-SkiaSlider-Orientation 'DrawnUi.Maui.Draw.SkiaSlider.Orientation')
  - [RespondsToGestures](#P-DrawnUi-Maui-Draw-SkiaSlider-RespondsToGestures 'DrawnUi.Maui.Draw.SkiaSlider.RespondsToGestures')
  - [Start](#P-DrawnUi-Maui-Draw-SkiaSlider-Start 'DrawnUi.Maui.Draw.SkiaSlider.Start')
- [SkiaSvg](#T-DrawnUi-Maui-Draw-SkiaSvg 'DrawnUi.Maui.Draw.SkiaSvg')
  - [LoadSource(fileName)](#M-DrawnUi-Maui-Draw-SkiaSvg-LoadSource-System-String- 'DrawnUi.Maui.Draw.SkiaSvg.LoadSource(System.String)')
- [SkiaSwitch](#T-DrawnUi-Maui-Draw-SkiaSwitch 'DrawnUi.Maui.Draw.SkiaSwitch')
- [SkiaTabsSelector](#T-DrawnUi-Maui-Controls-SkiaTabsSelector 'DrawnUi.Maui.Controls.SkiaTabsSelector')
  - [TabType](#P-DrawnUi-Maui-Controls-SkiaTabsSelector-TabType 'DrawnUi.Maui.Controls.SkiaTabsSelector.TabType')
  - [ApplySelectedIndex()](#M-DrawnUi-Maui-Controls-SkiaTabsSelector-ApplySelectedIndex-System-Int32- 'DrawnUi.Maui.Controls.SkiaTabsSelector.ApplySelectedIndex(System.Int32)')
- [SkiaToggle](#T-DrawnUi-Maui-Draw-SkiaToggle 'DrawnUi.Maui.Draw.SkiaToggle')
  - [ApplyProperties()](#M-DrawnUi-Maui-Draw-SkiaToggle-ApplyProperties 'DrawnUi.Maui.Draw.SkiaToggle.ApplyProperties')
  - [OnToggledChanged()](#M-DrawnUi-Maui-Draw-SkiaToggle-OnToggledChanged 'DrawnUi.Maui.Draw.SkiaToggle.OnToggledChanged')
- [SkiaValueAnimator](#T-DrawnUi-Maui-Draw-SkiaValueAnimator 'DrawnUi.Maui.Draw.SkiaValueAnimator')
  - [CycleFInished](#P-DrawnUi-Maui-Draw-SkiaValueAnimator-CycleFInished 'DrawnUi.Maui.Draw.SkiaValueAnimator.CycleFInished')
  - [Finished](#P-DrawnUi-Maui-Draw-SkiaValueAnimator-Finished 'DrawnUi.Maui.Draw.SkiaValueAnimator.Finished')
  - [Progress](#P-DrawnUi-Maui-Draw-SkiaValueAnimator-Progress 'DrawnUi.Maui.Draw.SkiaValueAnimator.Progress')
  - [Repeat](#P-DrawnUi-Maui-Draw-SkiaValueAnimator-Repeat 'DrawnUi.Maui.Draw.SkiaValueAnimator.Repeat')
  - [TransformReportedValue(deltaT)](#M-DrawnUi-Maui-Draw-SkiaValueAnimator-TransformReportedValue-System-Int64- 'DrawnUi.Maui.Draw.SkiaValueAnimator.TransformReportedValue(System.Int64)')
  - [UpdateValue(deltaT)](#M-DrawnUi-Maui-Draw-SkiaValueAnimator-UpdateValue-System-Int64,System-Int64- 'DrawnUi.Maui.Draw.SkiaValueAnimator.UpdateValue(System.Int64,System.Int64)')
- [SkiaView](#T-DrawnUi-Maui-Views-SkiaView 'DrawnUi.Maui.Views.SkiaView')
  - [CalculateFPS(currentTimestamp,averageAmount)](#M-DrawnUi-Maui-Views-SkiaView-CalculateFPS-System-Int64,System-Int32- 'DrawnUi.Maui.Views.SkiaView.CalculateFPS(System.Int64,System.Int32)')
- [SkiaViewAccelerated](#T-DrawnUi-Maui-Views-SkiaViewAccelerated 'DrawnUi.Maui.Views.SkiaViewAccelerated')
  - [CalculateFPS(currentTimestamp,averageAmount)](#M-DrawnUi-Maui-Views-SkiaViewAccelerated-CalculateFPS-System-Int64,System-Int32- 'DrawnUi.Maui.Views.SkiaViewAccelerated.CalculateFPS(System.Int64,System.Int32)')
  - [OnPaintingSurface(sender,paintArgs)](#M-DrawnUi-Maui-Views-SkiaViewAccelerated-OnPaintingSurface-System-Object,SkiaSharp-Views-Maui-SKPaintGLSurfaceEventArgs- 'DrawnUi.Maui.Views.SkiaViewAccelerated.OnPaintingSurface(System.Object,SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs)')
- [SkiaViewSwitcher](#T-DrawnUi-Maui-Controls-SkiaViewSwitcher 'DrawnUi.Maui.Controls.SkiaViewSwitcher')
  - [NavigationStacks](#F-DrawnUi-Maui-Controls-SkiaViewSwitcher-NavigationStacks 'DrawnUi.Maui.Controls.SkiaViewSwitcher.NavigationStacks')
  - [GetTopView(selectedIndex)](#M-DrawnUi-Maui-Controls-SkiaViewSwitcher-GetTopView-System-Int32- 'DrawnUi.Maui.Controls.SkiaViewSwitcher.GetTopView(System.Int32)')
  - [PopTabToRoot()](#M-DrawnUi-Maui-Controls-SkiaViewSwitcher-PopTabToRoot 'DrawnUi.Maui.Controls.SkiaViewSwitcher.PopTabToRoot')
  - [RevealNavigationView(newVisibleView)](#M-DrawnUi-Maui-Controls-SkiaViewSwitcher-RevealNavigationView-DrawnUi-Maui-Controls-SkiaViewSwitcher-NavigationStackEntry- 'DrawnUi.Maui.Controls.SkiaViewSwitcher.RevealNavigationView(DrawnUi.Maui.Controls.SkiaViewSwitcher.NavigationStackEntry)')
- [Snapping](#T-DrawnUi-Maui-Draw-Snapping 'DrawnUi.Maui.Draw.Snapping')
  - [SnapPointsToPixel(initialPosition,translation,scale)](#M-DrawnUi-Maui-Draw-Snapping-SnapPointsToPixel-System-Single,System-Single,System-Double- 'DrawnUi.Maui.Draw.Snapping.SnapPointsToPixel(System.Single,System.Single,System.Double)')
- [SnappingLayout](#T-DrawnUi-Maui-Draw-SnappingLayout 'DrawnUi.Maui.Draw.SnappingLayout')
  - [AutoVelocityMultiplyPts](#P-DrawnUi-Maui-Draw-SnappingLayout-AutoVelocityMultiplyPts 'DrawnUi.Maui.Draw.SnappingLayout.AutoVelocityMultiplyPts')
  - [ContentOffsetBounds](#P-DrawnUi-Maui-Draw-SnappingLayout-ContentOffsetBounds 'DrawnUi.Maui.Draw.SnappingLayout.ContentOffsetBounds')
  - [IgnoreWrongDirection](#P-DrawnUi-Maui-Draw-SnappingLayout-IgnoreWrongDirection 'DrawnUi.Maui.Draw.SnappingLayout.IgnoreWrongDirection')
  - [RespondsToGestures](#P-DrawnUi-Maui-Draw-SnappingLayout-RespondsToGestures 'DrawnUi.Maui.Draw.SnappingLayout.RespondsToGestures')
  - [RubberDamping](#P-DrawnUi-Maui-Draw-SnappingLayout-RubberDamping 'DrawnUi.Maui.Draw.SnappingLayout.RubberDamping')
  - [RubberEffect](#P-DrawnUi-Maui-Draw-SnappingLayout-RubberEffect 'DrawnUi.Maui.Draw.SnappingLayout.RubberEffect')
  - [SnapDistanceRatio](#P-DrawnUi-Maui-Draw-SnappingLayout-SnapDistanceRatio 'DrawnUi.Maui.Draw.SnappingLayout.SnapDistanceRatio')
  - [Viewport](#P-DrawnUi-Maui-Draw-SnappingLayout-Viewport 'DrawnUi.Maui.Draw.SnappingLayout.Viewport')
  - [ClampOffsetWithRubberBand(x,y)](#M-DrawnUi-Maui-Draw-SnappingLayout-ClampOffsetWithRubberBand-System-Single,System-Single- 'DrawnUi.Maui.Draw.SnappingLayout.ClampOffsetWithRubberBand(System.Single,System.Single)')
  - [GetAutoVelocity(displacement)](#M-DrawnUi-Maui-Draw-SnappingLayout-GetAutoVelocity-System-Numerics-Vector2- 'DrawnUi.Maui.Draw.SnappingLayout.GetAutoVelocity(System.Numerics.Vector2)')
  - [GetContentOffsetBounds()](#M-DrawnUi-Maui-Draw-SnappingLayout-GetContentOffsetBounds 'DrawnUi.Maui.Draw.SnappingLayout.GetContentOffsetBounds')
  - [ScrollToOffset(offset,animate)](#M-DrawnUi-Maui-Draw-SnappingLayout-ScrollToOffset-System-Numerics-Vector2,System-Numerics-Vector2,System-Boolean- 'DrawnUi.Maui.Draw.SnappingLayout.ScrollToOffset(System.Numerics.Vector2,System.Numerics.Vector2,System.Boolean)')
  - [SelectNextAnchor(origin,velocity)](#M-DrawnUi-Maui-Draw-SnappingLayout-SelectNextAnchor-System-Numerics-Vector2,System-Numerics-Vector2- 'DrawnUi.Maui.Draw.SnappingLayout.SelectNextAnchor(System.Numerics.Vector2,System.Numerics.Vector2)')
- [SpaceDistribution](#T-DrawnUi-Maui-Draw-SpaceDistribution 'DrawnUi.Maui.Draw.SpaceDistribution')
  - [Auto](#F-DrawnUi-Maui-Draw-SpaceDistribution-Auto 'DrawnUi.Maui.Draw.SpaceDistribution.Auto')
  - [Full](#F-DrawnUi-Maui-Draw-SpaceDistribution-Full 'DrawnUi.Maui.Draw.SpaceDistribution.Full')
- [StackLayoutStructure](#T-DrawnUi-Maui-Draw-StackLayoutStructure 'DrawnUi.Maui.Draw.StackLayoutStructure')
  - [Build()](#M-DrawnUi-Maui-Draw-StackLayoutStructure-Build-SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Draw.StackLayoutStructure.Build(SkiaSharp.SKRect,System.Single)')
- [Super](#T-DrawnUi-Maui-Draw-Super 'DrawnUi.Maui.Draw.Super')
  - [CanUseHardwareAcceleration](#F-DrawnUi-Maui-Draw-Super-CanUseHardwareAcceleration 'DrawnUi.Maui.Draw.Super.CanUseHardwareAcceleration')
  - [CapMicroSecs](#F-DrawnUi-Maui-Draw-Super-CapMicroSecs 'DrawnUi.Maui.Draw.Super.CapMicroSecs')
  - [InsetsChanged](#F-DrawnUi-Maui-Draw-Super-InsetsChanged 'DrawnUi.Maui.Draw.Super.InsetsChanged')
  - [OnMauiAppCreated](#F-DrawnUi-Maui-Draw-Super-OnMauiAppCreated 'DrawnUi.Maui.Draw.Super.OnMauiAppCreated')
  - [BottomTabsHeight](#P-DrawnUi-Maui-Draw-Super-BottomTabsHeight 'DrawnUi.Maui.Draw.Super.BottomTabsHeight')
  - [FontSubPixelRendering](#P-DrawnUi-Maui-Draw-Super-FontSubPixelRendering 'DrawnUi.Maui.Draw.Super.FontSubPixelRendering')
  - [IsRtl](#P-DrawnUi-Maui-Draw-Super-IsRtl 'DrawnUi.Maui.Draw.Super.IsRtl')
  - [NavBarHeight](#P-DrawnUi-Maui-Draw-Super-NavBarHeight 'DrawnUi.Maui.Draw.Super.NavBarHeight')
  - [StatusBarHeight](#P-DrawnUi-Maui-Draw-Super-StatusBarHeight 'DrawnUi.Maui.Draw.Super.StatusBarHeight')
  - [DisplayException(view,e)](#M-DrawnUi-Maui-Draw-Super-DisplayException-Microsoft-Maui-Controls-Element,System-Exception- 'DrawnUi.Maui.Draw.Super.DisplayException(Microsoft.Maui.Controls.Element,System.Exception)')
  - [ListResources(subfolder)](#M-DrawnUi-Maui-Draw-Super-ListResources-System-String- 'DrawnUi.Maui.Draw.Super.ListResources(System.String)')
  - [NeedGlobalUpdate()](#M-DrawnUi-Maui-Draw-Super-NeedGlobalUpdate 'DrawnUi.Maui.Draw.Super.NeedGlobalUpdate')
  - [OpenLink(link)](#M-DrawnUi-Maui-Draw-Super-OpenLink-System-String- 'DrawnUi.Maui.Draw.Super.OpenLink(System.String)')
  - [ResizeWindow(window,width,height,isFixed)](#M-DrawnUi-Maui-Draw-Super-ResizeWindow-Microsoft-Maui-Controls-Window,System-Int32,System-Int32,System-Boolean- 'DrawnUi.Maui.Draw.Super.ResizeWindow(Microsoft.Maui.Controls.Window,System.Int32,System.Int32,System.Boolean)')
  - [SetFullScreen(activity)](#M-DrawnUi-Maui-Draw-Super-SetFullScreen-Android-App-Activity- 'DrawnUi.Maui.Draw.Super.SetFullScreen(Android.App.Activity)')
- [TemplatedViewsPool](#T-DrawnUi-Maui-Draw-ViewsAdapter-TemplatedViewsPool 'DrawnUi.Maui.Draw.ViewsAdapter.TemplatedViewsPool')
  - [CreateFromTemplate()](#M-DrawnUi-Maui-Draw-ViewsAdapter-TemplatedViewsPool-CreateFromTemplate 'DrawnUi.Maui.Draw.ViewsAdapter.TemplatedViewsPool.CreateFromTemplate')
  - [Reserve()](#M-DrawnUi-Maui-Draw-ViewsAdapter-TemplatedViewsPool-Reserve 'DrawnUi.Maui.Draw.ViewsAdapter.TemplatedViewsPool.Reserve')
- [TextLine](#T-DrawnUi-Maui-Draw-TextLine 'DrawnUi.Maui.Draw.TextLine')
  - [Bounds](#P-DrawnUi-Maui-Draw-TextLine-Bounds 'DrawnUi.Maui.Draw.TextLine.Bounds')
- [TextSpan](#T-DrawnUi-Maui-Draw-TextSpan 'DrawnUi.Maui.Draw.TextSpan')
  - [Rects](#F-DrawnUi-Maui-Draw-TextSpan-Rects 'DrawnUi.Maui.Draw.TextSpan.Rects')
  - [AutoFindFont](#P-DrawnUi-Maui-Draw-TextSpan-AutoFindFont 'DrawnUi.Maui.Draw.TextSpan.AutoFindFont')
  - [DrawingOffset](#P-DrawnUi-Maui-Draw-TextSpan-DrawingOffset 'DrawnUi.Maui.Draw.TextSpan.DrawingOffset')
  - [ForceCaptureInput](#P-DrawnUi-Maui-Draw-TextSpan-ForceCaptureInput 'DrawnUi.Maui.Draw.TextSpan.ForceCaptureInput')
  - [Glyphs](#P-DrawnUi-Maui-Draw-TextSpan-Glyphs 'DrawnUi.Maui.Draw.TextSpan.Glyphs')
  - [HasTapHandler](#P-DrawnUi-Maui-Draw-TextSpan-HasTapHandler 'DrawnUi.Maui.Draw.TextSpan.HasTapHandler')
  - [Shape](#P-DrawnUi-Maui-Draw-TextSpan-Shape 'DrawnUi.Maui.Draw.TextSpan.Shape')
  - [StrikeoutWidth](#P-DrawnUi-Maui-Draw-TextSpan-StrikeoutWidth 'DrawnUi.Maui.Draw.TextSpan.StrikeoutWidth')
  - [UnderlineWidth](#P-DrawnUi-Maui-Draw-TextSpan-UnderlineWidth 'DrawnUi.Maui.Draw.TextSpan.UnderlineWidth')
  - [CheckGlyphsCanBeRendered()](#M-DrawnUi-Maui-Draw-TextSpan-CheckGlyphsCanBeRendered 'DrawnUi.Maui.Draw.TextSpan.CheckGlyphsCanBeRendered')
  - [SetupPaint()](#M-DrawnUi-Maui-Draw-TextSpan-SetupPaint-System-Double,SkiaSharp-SKPaint- 'DrawnUi.Maui.Draw.TextSpan.SetupPaint(System.Double,SkiaSharp.SKPaint)')
- [TransformAspect](#T-DrawnUi-Maui-Draw-TransformAspect 'DrawnUi.Maui.Draw.TransformAspect')
  - [AspectCover](#F-DrawnUi-Maui-Draw-TransformAspect-AspectCover 'DrawnUi.Maui.Draw.TransformAspect.AspectCover')
  - [AspectFill](#F-DrawnUi-Maui-Draw-TransformAspect-AspectFill 'DrawnUi.Maui.Draw.TransformAspect.AspectFill')
  - [AspectFit](#F-DrawnUi-Maui-Draw-TransformAspect-AspectFit 'DrawnUi.Maui.Draw.TransformAspect.AspectFit')
  - [AspectFitFill](#F-DrawnUi-Maui-Draw-TransformAspect-AspectFitFill 'DrawnUi.Maui.Draw.TransformAspect.AspectFitFill')
  - [Cover](#F-DrawnUi-Maui-Draw-TransformAspect-Cover 'DrawnUi.Maui.Draw.TransformAspect.Cover')
  - [Fill](#F-DrawnUi-Maui-Draw-TransformAspect-Fill 'DrawnUi.Maui.Draw.TransformAspect.Fill')
  - [Fit](#F-DrawnUi-Maui-Draw-TransformAspect-Fit 'DrawnUi.Maui.Draw.TransformAspect.Fit')
  - [FitFill](#F-DrawnUi-Maui-Draw-TransformAspect-FitFill 'DrawnUi.Maui.Draw.TransformAspect.FitFill')
  - [Tile](#F-DrawnUi-Maui-Draw-TransformAspect-Tile 'DrawnUi.Maui.Draw.TransformAspect.Tile')
- [UiSettings](#T-DrawnUi-Maui-Draw-UiSettings 'DrawnUi.Maui.Draw.UiSettings')
  - [DesktopWindow](#P-DrawnUi-Maui-Draw-UiSettings-DesktopWindow 'DrawnUi.Maui.Draw.UiSettings.DesktopWindow')
  - [MobileIsFullscreen](#P-DrawnUi-Maui-Draw-UiSettings-MobileIsFullscreen 'DrawnUi.Maui.Draw.UiSettings.MobileIsFullscreen')
  - [UseDesktopKeyboard](#P-DrawnUi-Maui-Draw-UiSettings-UseDesktopKeyboard 'DrawnUi.Maui.Draw.UiSettings.UseDesktopKeyboard')
- [UpdateMode](#T-DrawnUi-Maui-Infrastructure-Enums-UpdateMode 'DrawnUi.Maui.Infrastructure.Enums.UpdateMode')
  - [Constant](#F-DrawnUi-Maui-Infrastructure-Enums-UpdateMode-Constant 'DrawnUi.Maui.Infrastructure.Enums.UpdateMode.Constant')
  - [Dynamic](#F-DrawnUi-Maui-Infrastructure-Enums-UpdateMode-Dynamic 'DrawnUi.Maui.Infrastructure.Enums.UpdateMode.Dynamic')
  - [Manual](#F-DrawnUi-Maui-Infrastructure-Enums-UpdateMode-Manual 'DrawnUi.Maui.Infrastructure.Enums.UpdateMode.Manual')
- [VStack](#T-DrawnUi-Maui-Controls-VStack 'DrawnUi.Maui.Controls.VStack')
- [VelocitySkiaAnimator](#T-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator 'DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator')
  - [Friction](#P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-Friction 'DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator.Friction')
  - [RemainingVelocity](#P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-RemainingVelocity 'DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator.RemainingVelocity')
  - [mMaxOverscrollValue](#P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-mMaxOverscrollValue 'DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator.mMaxOverscrollValue')
  - [mMinOverscrollValue](#P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-mMinOverscrollValue 'DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator.mMinOverscrollValue')
- [ViewsAdapter](#T-DrawnUi-Maui-Draw-ViewsAdapter 'DrawnUi.Maui.Draw.ViewsAdapter')
  - [_dicoCellsInUse](#F-DrawnUi-Maui-Draw-ViewsAdapter-_dicoCellsInUse 'DrawnUi.Maui.Draw.ViewsAdapter._dicoCellsInUse')
  - [TemplatesAvailable](#P-DrawnUi-Maui-Draw-ViewsAdapter-TemplatesAvailable 'DrawnUi.Maui.Draw.ViewsAdapter.TemplatesAvailable')
  - [AddMoreToPool(oversize)](#M-DrawnUi-Maui-Draw-ViewsAdapter-AddMoreToPool-System-Int32- 'DrawnUi.Maui.Draw.ViewsAdapter.AddMoreToPool(System.Int32)')
  - [FillPool(size)](#M-DrawnUi-Maui-Draw-ViewsAdapter-FillPool-System-Int32,System-Collections-IList- 'DrawnUi.Maui.Draw.ViewsAdapter.FillPool(System.Int32,System.Collections.IList)')
  - [FillPool(size)](#M-DrawnUi-Maui-Draw-ViewsAdapter-FillPool-System-Int32- 'DrawnUi.Maui.Draw.ViewsAdapter.FillPool(System.Int32)')
  - [InitializeTemplates(template,dataContexts,poolSize,reserve)](#M-DrawnUi-Maui-Draw-ViewsAdapter-InitializeTemplates-System-Func{System-Object},System-Collections-IList,System-Int32,System-Int32- 'DrawnUi.Maui.Draw.ViewsAdapter.InitializeTemplates(System.Func{System.Object},System.Collections.IList,System.Int32,System.Int32)')
- [ViewsIterator](#T-DrawnUi-Maui-Draw-ViewsAdapter-ViewsIterator 'DrawnUi.Maui.Draw.ViewsAdapter.ViewsIterator')
- [VirtualisationType](#T-DrawnUi-Maui-Draw-VirtualisationType 'DrawnUi.Maui.Draw.VirtualisationType')
  - [Disabled](#F-DrawnUi-Maui-Draw-VirtualisationType-Disabled 'DrawnUi.Maui.Draw.VirtualisationType.Disabled')
  - [Enabled](#F-DrawnUi-Maui-Draw-VirtualisationType-Enabled 'DrawnUi.Maui.Draw.VirtualisationType.Enabled')
  - [Smart](#F-DrawnUi-Maui-Draw-VirtualisationType-Smart 'DrawnUi.Maui.Draw.VirtualisationType.Smart')
- [ViscousFluidInterpolator](#T-DrawnUi-Maui-Draw-ViscousFluidInterpolator 'DrawnUi.Maui.Draw.ViscousFluidInterpolator')
  - [VISCOUS_FLUID_SCALE](#F-DrawnUi-Maui-Draw-ViscousFluidInterpolator-VISCOUS_FLUID_SCALE 'DrawnUi.Maui.Draw.ViscousFluidInterpolator.VISCOUS_FLUID_SCALE')
- [VisualTransform](#T-DrawnUi-Maui-Infrastructure-VisualTransform 'DrawnUi.Maui.Infrastructure.VisualTransform')
  - [Scale](#P-DrawnUi-Maui-Infrastructure-VisualTransform-Scale 'DrawnUi.Maui.Infrastructure.VisualTransform.Scale')
  - [Translation](#P-DrawnUi-Maui-Infrastructure-VisualTransform-Translation 'DrawnUi.Maui.Infrastructure.VisualTransform.Translation')
  - [ToNative(rect,clipRect,scale)](#M-DrawnUi-Maui-Infrastructure-VisualTransform-ToNative-SkiaSharp-SKRect,System-Single- 'DrawnUi.Maui.Infrastructure.VisualTransform.ToNative(SkiaSharp.SKRect,System.Single)')
- [VisualTransformNative](#T-DrawnUi-Maui-Infrastructure-VisualTransformNative 'DrawnUi.Maui.Infrastructure.VisualTransformNative')
  - [Rect](#P-DrawnUi-Maui-Infrastructure-VisualTransformNative-Rect 'DrawnUi.Maui.Infrastructure.VisualTransformNative.Rect')
- [VisualTreeChain](#T-DrawnUi-Maui-Infrastructure-VisualTreeChain 'DrawnUi.Maui.Infrastructure.VisualTreeChain')
  - [Child](#P-DrawnUi-Maui-Infrastructure-VisualTreeChain-Child 'DrawnUi.Maui.Infrastructure.VisualTreeChain.Child')
  - [NodeIndices](#P-DrawnUi-Maui-Infrastructure-VisualTreeChain-NodeIndices 'DrawnUi.Maui.Infrastructure.VisualTreeChain.NodeIndices')
  - [Nodes](#P-DrawnUi-Maui-Infrastructure-VisualTreeChain-Nodes 'DrawnUi.Maui.Infrastructure.VisualTreeChain.Nodes')
  - [Transform](#P-DrawnUi-Maui-Infrastructure-VisualTreeChain-Transform 'DrawnUi.Maui.Infrastructure.VisualTreeChain.Transform')
- [WindowParameters](#T-DrawnUi-Maui-Draw-WindowParameters 'DrawnUi.Maui.Draw.WindowParameters')
  - [IsFixedSize](#P-DrawnUi-Maui-Draw-WindowParameters-IsFixedSize 'DrawnUi.Maui.Draw.WindowParameters.IsFixedSize')
- [ZoomContent](#T-DrawnUi-Maui-Draw-ZoomContent 'DrawnUi.Maui.Draw.ZoomContent')
  - [LastValue](#F-DrawnUi-Maui-Draw-ZoomContent-LastValue 'DrawnUi.Maui.Draw.ZoomContent.LastValue')
  - [ZoomSpeed](#P-DrawnUi-Maui-Draw-ZoomContent-ZoomSpeed 'DrawnUi.Maui.Draw.ZoomContent.ZoomSpeed')

<a name='T-DrawnUi-Maui-Draw-ActionOnTickAnimator'></a>
## ActionOnTickAnimator `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Just register this animator to run custom code on every frame creating a kind of game loop if needed.

<a name='T-DrawnUi-Maui-Draw-AddGestures'></a>
## AddGestures `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

For fast and lazy gestures handling to attach to dran controls inside the canvas only

<a name='T-DrawnUi-Maui-Draw-AnimateExtensions'></a>
## AnimateExtensions `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-AnimateExtensions-AnimateWith-DrawnUi-Maui-Draw-SkiaControl,System-Func{DrawnUi-Maui-Draw-SkiaControl,System-Threading-Tasks-Task}[]-'></a>
### AnimateWith(control,animations) `method`

##### Summary

Animate several tasks at the same time with WhenAll

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| animations | [System.Func{DrawnUi.Maui.Draw.SkiaControl,System.Threading.Tasks.Task}[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{DrawnUi.Maui.Draw.SkiaControl,System.Threading.Tasks.Task}[]') |  |

<a name='T-DrawnUi-Maui-Controls-AnimatedFramesRenderer'></a>
## AnimatedFramesRenderer `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Base class for playing frames. Subclass to play spritesheets, gifs, custom animations etc.

<a name='P-DrawnUi-Maui-Controls-AnimatedFramesRenderer-Repeat'></a>
### Repeat `property`

##### Summary

>0 how many times should repeat, if less than 0 will loop forever

<a name='M-DrawnUi-Maui-Controls-AnimatedFramesRenderer-GetFrameAt-System-Single-'></a>
### GetFrameAt() `method`

##### Summary

ratio 0-1

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
|  | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Controls-AnimatedFramesRenderer-OnAnimatorUpdated-System-Double-'></a>
### OnAnimatorUpdated(value) `method`

##### Summary

Override this to react on animator running.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='T-DrawnUi-Maui-Draw-AutoSizeType'></a>
## AutoSizeType `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-AutoSizeType-FillHorizontal'></a>
### FillHorizontal `constants`

##### Summary

If you have dynamically changing text think about using FitFillHorizontal instead

<a name='F-DrawnUi-Maui-Draw-AutoSizeType-FillVertical'></a>
### FillVertical `constants`

##### Summary

If you have dynamically changing text think about using FitFillVertical instead

<a name='F-DrawnUi-Maui-Draw-AutoSizeType-FitFillHorizontal'></a>
### FitFillHorizontal `constants`

##### Summary

This might be faster than FitHorizontal or FillHorizontal for dynamically changing text

<a name='F-DrawnUi-Maui-Draw-AutoSizeType-FitFillVertical'></a>
### FitFillVertical `constants`

##### Summary

This might be faster than FitVertical or FillVertical for dynamically changing text

<a name='F-DrawnUi-Maui-Draw-AutoSizeType-FitHorizontal'></a>
### FitHorizontal `constants`

##### Summary

todo FIX NOT WORKING!!! If you have dynamically changing text think about using FitFillHorizontal instead

<a name='F-DrawnUi-Maui-Draw-AutoSizeType-FitVertical'></a>
### FitVertical `constants`

##### Summary

If you have dynamically changing text think about using FitFillVertical instead

<a name='T-DrawnUi-Maui-Draw-BindToParentContextExtension'></a>
## BindToParentContextExtension `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Compiled-bindings-friendly implementation for "Source.Parent.BindingContext.Path"

<a name='T-DrawnUi-Maui-Draw-SkiaLayout-BuildWrapLayout'></a>
## BuildWrapLayout `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaLayout

##### Summary

Implementation for LayoutType.Wrap

<a name='T-DrawnUi-Maui-Draw-CachedObject'></a>
## CachedObject `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-CachedObject-SurfaceIsRecycled'></a>
### SurfaceIsRecycled `property`

##### Summary

An existing surface was reused for creating this object

<a name='T-DrawnUi-Maui-Views-Canvas'></a>
## Canvas `type`

##### Namespace

DrawnUi.Maui.Views

##### Summary

Optimized DrawnView having only one child inside Content property. Can autosize to to children size.
For all drawn app put this directly inside the ContentPage as root view.
If you put this inside some Maui control like Grid whatever expect more GC collections during animations making them somewhat less fluid.

<a name='F-DrawnUi-Maui-Views-Canvas-FirstPanThreshold'></a>
### FirstPanThreshold `constants`

##### Summary

To filter micro-gestures on super sensitive screens, start passing panning only when threshold is once overpassed

<a name='M-DrawnUi-Maui-Views-Canvas-AdaptSizeRequestToContent-System-Double,System-Double-'></a>
### AdaptSizeRequestToContent(widthRequest,heightRequest) `method`

##### Summary

In UNITS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| widthRequest | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| heightRequest | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Views-Canvas-ArrangeOverride-Microsoft-Maui-Graphics-Rect-'></a>
### ArrangeOverride(bounds) `method`

##### Summary

We need this mainly to autosize inside grid cells
This is also called when parent visibilty changes

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| bounds | [Microsoft.Maui.Graphics.Rect](#T-Microsoft-Maui-Graphics-Rect 'Microsoft.Maui.Graphics.Rect') |  |

<a name='M-DrawnUi-Maui-Views-Canvas-DisableUpdates'></a>
### DisableUpdates() `method`

##### Summary

Disable invalidating and drawing on the canvas

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-Canvas-EnableUpdates'></a>
### EnableUpdates() `method`

##### Summary

Enable canvas rendering itsself

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-Canvas-GetMeasuringRectForChildren-System-Single,System-Single,System-Single-'></a>
### GetMeasuringRectForChildren(widthConstraint,heightConstraint,scale) `method`

##### Summary

All in in UNITS, OUT in PIXELS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| widthConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| heightConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Views-Canvas-MeasureChild-DrawnUi-Maui-Draw-SkiaControl,System-Double,System-Double,System-Double-'></a>
### MeasureChild(child,availableWidth,availableHeight,scale) `method`

##### Summary

PIXELS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| child | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| availableWidth | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| availableHeight | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Views-Canvas-OnGestureEvent-AppoMobi-Maui-Gestures-TouchActionType,AppoMobi-Maui-Gestures-TouchActionEventArgs,AppoMobi-Maui-Gestures-TouchActionResult-'></a>
### OnGestureEvent(type,args1,args1,) `method`

##### Summary

IGestureListener implementation

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| type | [AppoMobi.Maui.Gestures.TouchActionType](#T-AppoMobi-Maui-Gestures-TouchActionType 'AppoMobi.Maui.Gestures.TouchActionType') |  |
| args1 | [AppoMobi.Maui.Gestures.TouchActionEventArgs](#T-AppoMobi-Maui-Gestures-TouchActionEventArgs 'AppoMobi.Maui.Gestures.TouchActionEventArgs') |  |
| args1 | [AppoMobi.Maui.Gestures.TouchActionResult](#T-AppoMobi-Maui-Gestures-TouchActionResult 'AppoMobi.Maui.Gestures.TouchActionResult') |  |

<a name='M-DrawnUi-Maui-Views-Canvas-SetContent-DrawnUi-Maui-Draw-SkiaControl-'></a>
### SetContent() `method`

##### Summary

Use Content property for direct access

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaLayout-Cell'></a>
## Cell `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaLayout

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-Cell-ColumnGridLengthType'></a>
### ColumnGridLengthType `property`

##### Summary

A combination of all the measurement types in the columns this cell spans

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-Cell-RowGridLengthType'></a>
### RowGridLengthType `property`

##### Summary

A combination of all the measurement types in the rows this cell spans

<a name='T-DrawnUi-Maui-Draw-ChainAdjustBrightnessEffect'></a>
## ChainAdjustBrightnessEffect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-ChainAdjustBrightnessEffect-CreateBrightnessFilter-System-Single-'></a>
### CreateBrightnessFilter(value) `method`

##### Summary

-1 -> 0 -> 1

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-ChainAdjustBrightnessEffect-CreateLightnessFilter-System-Single-'></a>
### CreateLightnessFilter(value) `method`

##### Summary

-1 -> 0 -> 1

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-ChainAdjustLightnessEffect'></a>
## ChainAdjustLightnessEffect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-ChainAdjustLightnessEffect-CreateLightnessFilter-System-Single-'></a>
### CreateLightnessFilter(value) `method`

##### Summary

-1 -> 0 -> 1

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-ChainEffectResult'></a>
## ChainEffectResult `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ChainEffectResult-DrawnControl'></a>
### DrawnControl `property`

##### Summary

Set this to true if you drawn the control of false if you just rendered something else

<a name='T-DrawnUi-Maui-Infrastructure-Extensions-ColorExtensions'></a>
## ColorExtensions `type`

##### Namespace

DrawnUi.Maui.Infrastructure.Extensions

<a name='M-DrawnUi-Maui-Infrastructure-Extensions-ColorExtensions-ToColorFromHex-System-String-'></a>
### ToColorFromHex() `method`

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-ContentLayout'></a>
## ContentLayout `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ContentLayout-Orientation'></a>
### Orientation `property`

##### Summary



<a name='P-DrawnUi-Maui-Draw-ContentLayout-ScrollType'></a>
### ScrollType `property`

##### Summary



<a name='P-DrawnUi-Maui-Draw-ContentLayout-Virtualisation'></a>
### Virtualisation `property`

##### Summary

Default is Enabled, children get the visible viewport area for rendering and can virtualize.

<a name='M-DrawnUi-Maui-Draw-ContentLayout-GetContentAvailableRect-SkiaSharp-SKRect-'></a>
### GetContentAvailableRect(destination) `method`

##### Summary

In PIXELS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='T-DrawnUi-Maui-Draw-ControlInStack'></a>
## ControlInStack `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ControlInStack-Area'></a>
### Area `property`

##### Summary

Available area for Arrange

<a name='P-DrawnUi-Maui-Draw-ControlInStack-ControlIndex'></a>
### ControlIndex `property`

##### Summary

Index inside enumerator that was passed for measurement OR index inside ItemsSource

<a name='P-DrawnUi-Maui-Draw-ControlInStack-Destination'></a>
### Destination `property`

##### Summary

PIXELS, this is to hold our arranged layout

<a name='P-DrawnUi-Maui-Draw-ControlInStack-Drawn'></a>
### Drawn `property`

##### Summary

Was used for actual drawing

<a name='P-DrawnUi-Maui-Draw-ControlInStack-Measured'></a>
### Measured `property`

##### Summary

Measure result

<a name='P-DrawnUi-Maui-Draw-ControlInStack-Offset'></a>
### Offset `property`

##### Summary

For internal use by your custom controls

<a name='P-DrawnUi-Maui-Draw-ControlInStack-View'></a>
### View `property`

##### Summary

This will be null for recycled views

<a name='T-DrawnUi-Maui-Draw-DecelerationTimingVectorParameters'></a>
## DecelerationTimingVectorParameters `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-DecelerationTimingVectorParameters-ValueAt-System-Single-'></a>
### ValueAt(offsetSecs,time) `method`

##### Summary

time is in seconds

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| offsetSecs | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaLabel-DecomposedText'></a>
## DecomposedText `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaLabel

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-DecomposedText-HasMoreHorizontalSpace'></a>
### HasMoreHorizontalSpace `property`

##### Summary

pixels

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-DecomposedText-HasMoreVerticalSpace'></a>
### HasMoreVerticalSpace `property`

##### Summary

pixels

<a name='T-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-DragForce'></a>
## DragForce `type`

##### Namespace

DrawnUi.Maui.Animate.Animators.VelocitySkiaAnimator

<a name='M-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-DragForce-GetInitialVelocity-System-Single,System-Single,System-Single-'></a>
### GetInitialVelocity(initialPosition,finalPosition,durationTime) `method`

##### Summary

inverse of updateValueAndVelocity

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| initialPosition | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| finalPosition | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| durationTime | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-DrawnFontAttributesConverter'></a>
## DrawnFontAttributesConverter `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Forked from Microsoft.Maui.Controls as using original class was breaking XAML HotReload for some unknown reason

<a name='T-DrawnUi-Maui-Controls-DrawnUiBasePage'></a>
## DrawnUiBasePage `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Actually used to: respond to keyboard resizing on mobile and keyboard key presses on Mac. Other than for that this
    is not needed at all.

<a name='T-DrawnUi-Maui-Views-DrawnView'></a>
## DrawnView `type`

##### Namespace

DrawnUi.Maui.Views

<a name='F-DrawnUi-Maui-Views-DrawnView-_orderedChildren'></a>
### _orderedChildren `constants`

##### Summary

will be reset to null by InvalidateViewsList()

<a name='P-DrawnUi-Maui-Views-DrawnView-AnimatingControls'></a>
### AnimatingControls `property`

##### Summary

Tracking controls that what to be animated right now so we constantly refresh
canvas until there is none left

<a name='P-DrawnUi-Maui-Views-DrawnView-CanDraw'></a>
### CanDraw `property`

##### Summary

Indicates that it is allowed to be rendered by engine, internal use

##### Returns



<a name='P-DrawnUi-Maui-Views-DrawnView-CanRenderOffScreen'></a>
### CanRenderOffScreen `property`

##### Summary

If this is check you view will be refreshed even offScreen or hidden

<a name='P-DrawnUi-Maui-Views-DrawnView-CanvasFps'></a>
### CanvasFps `property`

##### Summary

Actual FPS

<a name='P-DrawnUi-Maui-Views-DrawnView-DrawingThreadId'></a>
### DrawingThreadId `property`

##### Summary

Can use this to manage double buffering to detect if we are in the drawing thread or in background.

<a name='P-DrawnUi-Maui-Views-DrawnView-DrawingThreads'></a>
### DrawingThreads `property`

##### Summary

For debugging purposes check if dont have concurrent threads

<a name='P-DrawnUi-Maui-Views-DrawnView-FPS'></a>
### FPS `property`

##### Summary

Average FPS

<a name='P-DrawnUi-Maui-Views-DrawnView-FrameTime'></a>
### FrameTime `property`

##### Summary

Frame started rendering nanoseconds

<a name='P-DrawnUi-Maui-Views-DrawnView-GestureListeners'></a>
### GestureListeners `property`

##### Summary

Children we should check for touch hits

<a name='P-DrawnUi-Maui-Views-DrawnView-InvalidatedCanvas'></a>
### InvalidatedCanvas `property`

##### Summary

A very important tracking prop to avoid saturating main thread with too many updates

<a name='P-DrawnUi-Maui-Views-DrawnView-IsHiddenInViewTree'></a>
### IsHiddenInViewTree `property`

##### Summary

Indicates that view is either hidden or offscreen.
This disables rendering if you don't set CanRenderOffScreen to true

<a name='P-DrawnUi-Maui-Views-DrawnView-PostAnimators'></a>
### PostAnimators `property`

##### Summary

Executed after the rendering

<a name='P-DrawnUi-Maui-Views-DrawnView-StopDrawingWhenUpdateIsLocked'></a>
### StopDrawingWhenUpdateIsLocked `property`

##### Summary

Set this to true if you do not want the canvas to be redrawn as transparent and showing content below the canvas (splash?..) when UpdateLocked is True

<a name='M-DrawnUi-Maui-Views-DrawnView-Arrange-SkiaSharp-SKRect,System-Double,System-Double,System-Double-'></a>
### Arrange(destination,widthRequest,heightRequest,scale) `method`

##### Summary

destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') | PIXELS |
| widthRequest | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') | UNITS |
| heightRequest | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') | UNITS |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Views-DrawnView-CalculateLayout-SkiaSharp-SKRect,System-Double,System-Double,System-Double-'></a>
### CalculateLayout(destination,widthRequest,heightRequest,scale) `method`

##### Summary

destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') | PIXELS |
| widthRequest | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') | UNITS |
| heightRequest | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') | UNITS |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Views-DrawnView-CheckElementVisibility-Microsoft-Maui-Controls-VisualElement-'></a>
### CheckElementVisibility(element) `method`

##### Summary

To optimize rendering and not update controls that are inside storyboard that is offscreen or hidden
Apple - UI thread only !!!
If you set

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| element | [Microsoft.Maui.Controls.VisualElement](#T-Microsoft-Maui-Controls-VisualElement 'Microsoft.Maui.Controls.VisualElement') |  |

<a name='M-DrawnUi-Maui-Views-DrawnView-CreateSkiaView'></a>
### CreateSkiaView() `method`

##### Summary

Will safely destroy existing if any

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-DrawnView-GetOrderedSubviews'></a>
### GetOrderedSubviews() `method`

##### Summary

For non templated simple subviews

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-DrawnView-Invalidate'></a>
### Invalidate() `method`

##### Summary

Makes the control dirty, in need to be remeasured and rendered but this doesn't call Update, it's up yo you

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-DrawnView-InvalidateCanvas'></a>
### InvalidateCanvas() `method`

##### Summary



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-DrawnView-InvalidateChildren'></a>
### InvalidateChildren() `method`

##### Summary

We need to invalidate children maui changed our storyboard size

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-DrawnView-InvalidateViewsList'></a>
### InvalidateViewsList() `method`

##### Summary

To make GetOrderedSubviews() regenerate next result instead of using cached

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-DrawnView-KickOffscreenCacheRendering'></a>
### KickOffscreenCacheRendering() `method`

##### Summary

Make sure offscreen rendering queue is running

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-DrawnView-OnCanRenderChanged-System-Boolean-'></a>
### OnCanRenderChanged(state) `method`

##### Summary

Invoked when IsHiddenInViewTree changes

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| state | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Views-DrawnView-PostponeExecutionAfterDraw-System-Action-'></a>
### PostponeExecutionAfterDraw(action) `method`

##### Summary

Postpone the action to be executed after the current frame is drawn. Exception-safe.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') |  |

<a name='M-DrawnUi-Maui-Views-DrawnView-PostponeExecutionBeforeDraw-System-Action-'></a>
### PostponeExecutionBeforeDraw(action) `method`

##### Summary

Postpone the action to be executed before the next frame being drawn. Exception-safe.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') |  |

<a name='M-DrawnUi-Maui-Views-DrawnView-RegisterAnimator-DrawnUi-Maui-Draw-ISkiaAnimator-'></a>
### RegisterAnimator(uid,animating) `method`

##### Summary

Called by a control that whats to be constantly animated or doesn't anymore,
so we know whether we should refresh canvas non-stop

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| uid | [DrawnUi.Maui.Draw.ISkiaAnimator](#T-DrawnUi-Maui-Draw-ISkiaAnimator 'DrawnUi.Maui.Draw.ISkiaAnimator') |  |

<a name='M-DrawnUi-Maui-Views-DrawnView-ReportFocus-DrawnUi-Maui-Draw-ISkiaGestureListener,DrawnUi-Maui-Draw-ISkiaGestureListener-'></a>
### ReportFocus(listener) `method`

##### Summary

Internal call by control, after reporting will affect FocusedChild but will not get FocusedItemChanged as it was its own call

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| listener | [DrawnUi.Maui.Draw.ISkiaGestureListener](#T-DrawnUi-Maui-Draw-ISkiaGestureListener 'DrawnUi.Maui.Draw.ISkiaGestureListener') |  |

<a name='T-DrawnUi-Maui-Draw-DynamicGrid`1'></a>
## DynamicGrid\`1 `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-DynamicGrid`1-GetColumnCountForRow-System-Int32-'></a>
### GetColumnCountForRow(row) `method`

##### Summary

Returns the column count for the specified row.
This value is cached and updated each time an item is added.

##### Returns

Number of columns in the specified row.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| row | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Row number to get the column count for. |

<a name='T-DrawnUi-Maui-Draw-SkiaLabel-EmojiData'></a>
## EmojiData `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaLabel

<a name='M-DrawnUi-Maui-Draw-SkiaLabel-EmojiData-IsEmojiModifierSequence-System-String,System-Int32-'></a>
### IsEmojiModifierSequence(text,index) `method`

##### Summary

Returns the length of EmojiModifierSequence if found at index ins

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| text | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| index | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='T-DrawnUi-Maui-Infrastructure-Files'></a>
## Files `type`

##### Namespace

DrawnUi.Maui.Infrastructure

<a name='M-DrawnUi-Maui-Infrastructure-Files-ListAssets-System-String-'></a>
### ListAssets() `method`

##### Summary

tries to get all resources from assets folder Resources/Raw/{subfolder}

##### Returns



##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-FontWeight'></a>
## FontWeight `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-FontWeight-Black'></a>
### Black `constants`

##### Summary

The black

<a name='F-DrawnUi-Maui-Draw-FontWeight-Bold'></a>
### Bold `constants`

##### Summary

The bold

<a name='F-DrawnUi-Maui-Draw-FontWeight-ExtraBold'></a>
### ExtraBold `constants`

##### Summary

Also known as Heavy or UltraBold

<a name='F-DrawnUi-Maui-Draw-FontWeight-ExtraLight'></a>
### ExtraLight `constants`

##### Summary

Also known as Ultra Light

<a name='F-DrawnUi-Maui-Draw-FontWeight-Light'></a>
### Light `constants`

##### Summary

The light

<a name='F-DrawnUi-Maui-Draw-FontWeight-Medium'></a>
### Medium `constants`

##### Summary

The medium

<a name='F-DrawnUi-Maui-Draw-FontWeight-Regular'></a>
### Regular `constants`

##### Summary

Also known as Normal

<a name='F-DrawnUi-Maui-Draw-FontWeight-SemiBold'></a>
### SemiBold `constants`

##### Summary

The semi bold

<a name='F-DrawnUi-Maui-Draw-FontWeight-Thin'></a>
### Thin `constants`

##### Summary

The thin

<a name='T-DrawnUi-Maui-Draw-GesturesMode'></a>
## GesturesMode `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Used by the canvas, do not need this for drawn controls

<a name='F-DrawnUi-Maui-Draw-GesturesMode-Disabled'></a>
### Disabled `constants`

##### Summary

Default

<a name='F-DrawnUi-Maui-Draw-GesturesMode-Enabled'></a>
### Enabled `constants`

##### Summary

Gestures attached

<a name='F-DrawnUi-Maui-Draw-GesturesMode-Lock'></a>
### Lock `constants`

##### Summary

Lock input for self, useful inside scroll view, panning controls like slider etc

<a name='F-DrawnUi-Maui-Draw-GesturesMode-Share'></a>
### Share `constants`

##### Summary

Tries to let other views consume the touch event if this view doesn't handle it

<a name='T-DrawnUi-Maui-Controls-GifAnimation'></a>
## GifAnimation `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-GifAnimation-Frame'></a>
### Frame `property`

##### Summary

Current frame bitmap, can change with SeekFrame method

<a name='M-DrawnUi-Maui-Controls-GifAnimation-SeekFrame-System-Int32-'></a>
### SeekFrame(frame) `method`

##### Summary

Select frame. If you pass a negative value the last frame will be set.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| frame | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='T-DrawnUi-Maui-Views-GlideExtensions'></a>
## GlideExtensions `type`

##### Namespace

DrawnUi.Maui.Views

<a name='M-DrawnUi-Maui-Views-GlideExtensions-Clear-Bumptech-Glide-RequestManager,Android-Widget-ImageView-'></a>
### Clear() `method`

##### Summary

Cancels the Request and "clears" the ImageView

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Views-GlideExtensions-IsActivityAlive-Android-Content-Context,Microsoft-Maui-Controls-ImageSource-'></a>
### IsActivityAlive() `method`

##### Summary

NOTE: see https://github.com/bumptech/glide/issues/1484#issuecomment-365625087

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Controls-GridLayout'></a>
## GridLayout `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Helper class for SkiaLayout Type = LayoutType.Grid

<a name='T-DrawnUi-Maui-Controls-HStack'></a>
## HStack `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Helper class for SkiaLayout Type = LayoutType.Stack,  SplitMax = 0

<a name='T-DrawnUi-Maui-Draw-HardwareAccelerationMode'></a>
## HardwareAccelerationMode `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-HardwareAccelerationMode-Disabled'></a>
### Disabled `constants`

##### Summary

Default

<a name='F-DrawnUi-Maui-Draw-HardwareAccelerationMode-Enabled'></a>
### Enabled `constants`

##### Summary

Gestures attached

<a name='F-DrawnUi-Maui-Draw-HardwareAccelerationMode-Prerender'></a>
### Prerender `constants`

##### Summary

A non-accelerated view will be created first to avoid blank screen while graphic context is being initialized, then swapped with accelerated view

<a name='T-DrawnUi-Maui-Draw-IAfterEffectDelete'></a>
## IAfterEffectDelete `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-IAfterEffectDelete-TypeId'></a>
### TypeId `property`

##### Summary

For faster scanning of anims of same type

<a name='M-DrawnUi-Maui-Draw-IAfterEffectDelete-Render-DrawnUi-Maui-Draw-IDrawnBase,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double-'></a>
### Render(control,canvas,scale) `method`

##### Summary

Called when drawing parent control frame

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.IDrawnBase](#T-DrawnUi-Maui-Draw-IDrawnBase 'DrawnUi.Maui.Draw.IDrawnBase') |  |
| canvas | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='T-DrawnUi-Maui-Draw-IAnimatorsManager'></a>
## IAnimatorsManager `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

This control is responsible for updating screen for running animators

<a name='T-DrawnUi-Maui-Draw-ICanBeUpdated'></a>
## ICanBeUpdated `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-ICanBeUpdated-Update'></a>
### Update() `method`

##### Summary

Force redrawing, without invalidating the measured size

##### Returns



##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-ICanRenderOnCanvas'></a>
## ICanRenderOnCanvas `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-ICanRenderOnCanvas-Render-DrawnUi-Maui-Draw-IDrawnBase,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double-'></a>
### Render(control,context,scale) `method`

##### Summary

Renders effect overlay to canvas, return true if has drawn something and rendering needs to be applied.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.IDrawnBase](#T-DrawnUi-Maui-Draw-IDrawnBase 'DrawnUi.Maui.Draw.IDrawnBase') |  |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='T-DrawnUi-Maui-Draw-IDefinesViewport'></a>
## IDefinesViewport `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-IDefinesViewport-InvalidateByChild-DrawnUi-Maui-Draw-SkiaControl-'></a>
### InvalidateByChild() `method`

##### Summary

So child can call parent to invalidate scrolling offset etc if child size changes

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-IDrawnBase'></a>
## IDrawnBase `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-IDrawnBase-PostAnimators'></a>
### PostAnimators `property`

##### Summary

Executed after the rendering

<a name='P-DrawnUi-Maui-Draw-IDrawnBase-Views'></a>
### Views `property`

##### Summary

For code-behind access of children, XAML is using Children property

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-AddSubView-DrawnUi-Maui-Draw-SkiaControl-'></a>
### AddSubView(view) `method`

##### Summary

Directly adds a view to the control, without any layouting. Use this instead of Views.Add() to avoid memory leaks etc

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-ClipSmart-SkiaSharp-SKCanvas,SkiaSharp-SKPath,SkiaSharp-SKClipOperation-'></a>
### ClipSmart(canvas,path,operation) `method`

##### Summary

Clip using internal custom settings of the control

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| canvas | [SkiaSharp.SKCanvas](#T-SkiaSharp-SKCanvas 'SkiaSharp.SKCanvas') |  |
| path | [SkiaSharp.SKPath](#T-SkiaSharp-SKPath 'SkiaSharp.SKPath') |  |
| operation | [SkiaSharp.SKClipOperation](#T-SkiaSharp-SKClipOperation 'SkiaSharp.SKClipOperation') |  |

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-CreateClip-System-Object,System-Boolean,SkiaSharp-SKPath-'></a>
### CreateClip() `method`

##### Summary

Creates a new disposable SKPath for clipping content according to the control shape and size.
Create this control clip for painting content.
Pass arguments if you want to use some time-frozen data for painting at any time from any thread..
If applyPosition is false will create clip without using drawing posiition, like if was drawing at 0,0.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-GetOnScreenVisibleArea-System-Single-'></a>
### GetOnScreenVisibleArea() `method`

##### Summary

Obtain rectangle visible on the screen to avoid offscreen rendering etc

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-Invalidate'></a>
### Invalidate() `method`

##### Summary

Invalidates the measured size. May or may not call Update() inside, depends on control

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-InvalidateByChild-DrawnUi-Maui-Draw-SkiaControl-'></a>
### InvalidateByChild(skiaControl) `method`

##### Summary

This is needed by layout to track which child changed to sometimes avoid recalculating other children

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| skiaControl | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-InvalidateParents'></a>
### InvalidateParents() `method`

##### Summary

If need the re-measure all parents because child-auto-size has changed

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-RemoveSubView-DrawnUi-Maui-Draw-SkiaControl-'></a>
### RemoveSubView(view) `method`

##### Summary

Directly removes a view from the control, without any layouting.
Use this instead of Views.Remove() to avoid memory leaks etc

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='M-DrawnUi-Maui-Draw-IDrawnBase-UpdateByChild-DrawnUi-Maui-Draw-SkiaControl-'></a>
### UpdateByChild(skiaControl) `method`

##### Summary

To track dirty area when Updating parent

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| skiaControl | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaShell-IHandleGoBack'></a>
## IHandleGoBack `type`

##### Namespace

DrawnUi.Maui.Controls.SkiaShell

<a name='M-DrawnUi-Maui-Controls-SkiaShell-IHandleGoBack-OnShellGoBack-System-Boolean-'></a>
### OnShellGoBack() `method`

##### Summary

Return true if comsumed, false will use default system behaivour.

##### Returns



##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-IHasBanner'></a>
## IHasBanner `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-IHasBanner-Banner'></a>
### Banner `property`

##### Summary

Main image

<a name='P-DrawnUi-Maui-Draw-IHasBanner-BannerPreloadOrdered'></a>
### BannerPreloadOrdered `property`

##### Summary

Indicates that it's already preloading

<a name='T-DrawnUi-Maui-Draw-IInsideViewport'></a>
## IInsideViewport `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-IInsideViewport-OnLoaded'></a>
### OnLoaded() `method`

##### Summary

Loaded is called when the view is created, but not yet visible

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-IInsideWheelStack'></a>
## IInsideWheelStack `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-IInsideWheelStack-OnPositionChanged-System-Single,System-Boolean-'></a>
### OnPositionChanged(offsetRatio,isSelected) `method`

##### Summary

Called by parent stack inside picker wheel when position changes

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| offsetRatio | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | 0.0-X.X offset from selection axis, beyond 1.0 is offscreen. Normally you would change opacity accordingly to this. |
| isSelected | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Whether cell is currently selected, normally you would change text color accordingly. |

<a name='T-DrawnUi-Maui-Draw-ILayoutInsideViewport'></a>
## ILayoutInsideViewport `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-ILayoutInsideViewport-GetChildIndexAt-SkiaSharp-SKPoint-'></a>
### GetChildIndexAt(point) `method`

##### Summary

The point here is the position inside parent, can be offscreen

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| point | [SkiaSharp.SKPoint](#T-SkiaSharp-SKPoint 'SkiaSharp.SKPoint') |  |

<a name='M-DrawnUi-Maui-Draw-ILayoutInsideViewport-GetVisibleChildIndexAt-SkiaSharp-SKPoint-'></a>
### GetVisibleChildIndexAt(point) `method`

##### Summary

The point here is the rendering location, always on screen

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| point | [SkiaSharp.SKPoint](#T-SkiaSharp-SKPoint 'SkiaSharp.SKPoint') |  |

<a name='T-DrawnUi-Maui-Draw-IRefreshIndicator'></a>
## IRefreshIndicator `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-IRefreshIndicator-SetDragRatio-System-Single-'></a>
### SetDragRatio(ratio) `method`

##### Summary

0 - 1

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ratio | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-IRenderEffect'></a>
## IRenderEffect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-IRenderEffect-Draw-SkiaSharp-SKRect,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Action{DrawnUi-Maui-Draw-SkiaDrawingContext}-'></a>
### Draw(destination,ctx,drawControl) `method`

##### Summary

Returns true if has drawn control itsself, otherwise it will be drawn over it

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| ctx | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| drawControl | [System.Action{DrawnUi.Maui.Draw.SkiaDrawingContext}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{DrawnUi.Maui.Draw.SkiaDrawingContext}') |  |

<a name='T-DrawnUi-Maui-Draw-ISkiaAnimator'></a>
## ISkiaAnimator `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ISkiaAnimator-IsDeactivated'></a>
### IsDeactivated `property`

##### Summary

Can and will be removed

<a name='P-DrawnUi-Maui-Draw-ISkiaAnimator-IsHiddenInViewTree'></a>
### IsHiddenInViewTree `property`

##### Summary

For internal use by the engine

<a name='P-DrawnUi-Maui-Draw-ISkiaAnimator-IsPaused'></a>
### IsPaused `property`

##### Summary

Just should not execute on tick

<a name='M-DrawnUi-Maui-Draw-ISkiaAnimator-Pause'></a>
### Pause() `method`

##### Summary

Used by ui, please use play stop for manual control

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-ISkiaAnimator-Resume'></a>
### Resume() `method`

##### Summary

Used by ui, please use play stop for manual control

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-ISkiaAnimator-TickFrame-System-Int64-'></a>
### TickFrame(frameTime) `method`

##### Summary



##### Returns

Is Finished

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| frameTime | [System.Int64](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int64 'System.Int64') |  |

<a name='T-DrawnUi-Maui-Draw-ISkiaControl'></a>
## ISkiaControl `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ISkiaControl-IsGhost'></a>
### IsGhost `property`

##### Summary

Takes place in layout, acts like is visible, but just not rendering

<a name='M-DrawnUi-Maui-Draw-ISkiaControl-Measure-System-Single,System-Single,System-Single-'></a>
### Measure(widthConstraint,heightConstraint) `method`

##### Summary

Expecting PIXELS as input
sets NeedMeasure to false

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| widthConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| heightConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-ISkiaDrawable'></a>
## ISkiaDrawable `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ISkiaDrawable-OnDraw'></a>
### OnDraw `property`

##### Summary

Return true if need force invalidation on next frame

<a name='T-DrawnUi-Maui-Draw-ISkiaGestureListener'></a>
## ISkiaGestureListener `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-ISkiaGestureListener-OnFocusChanged-System-Boolean-'></a>
### OnFocusChanged(focus) `method`

##### Summary

This will be called only for views registered at Superview.FocusedChild.
The view must return true of false to indicate if it accepts focus.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| focus | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-ISkiaGestureListener-OnSkiaGestureEvent-DrawnUi-Maui-Draw-SkiaGesturesParameters,DrawnUi-Maui-Draw-GestureEventProcessingInfo-'></a>
### OnSkiaGestureEvent(type,args,args.Action.Action,inside) `method`

##### Summary

Called when a gesture is detected.

##### Returns

WHO CONSUMED if gesture consumed and blocked to be passed, NULL if gesture not locked and could be passed below.
If you pass this to subview you must set your own offset parameters, do not pass what you received its for this level use.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| type | [DrawnUi.Maui.Draw.SkiaGesturesParameters](#T-DrawnUi-Maui-Draw-SkiaGesturesParameters 'DrawnUi.Maui.Draw.SkiaGesturesParameters') |  |
| args | [DrawnUi.Maui.Draw.GestureEventProcessingInfo](#T-DrawnUi-Maui-Draw-GestureEventProcessingInfo 'DrawnUi.Maui.Draw.GestureEventProcessingInfo') |  |

<a name='T-DrawnUi-Maui-Draw-ISkiaGridLayout'></a>
## ISkiaGridLayout `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ISkiaGridLayout-ColumnDefinitions'></a>
### ColumnDefinitions `property`

##### Summary

An IGridColumnDefinition collection for the GridLayout instance.

<a name='P-DrawnUi-Maui-Draw-ISkiaGridLayout-ColumnSpacing'></a>
### ColumnSpacing `property`

##### Summary

Gets the amount of space left between columns in the GridLayout.

<a name='P-DrawnUi-Maui-Draw-ISkiaGridLayout-RowDefinitions'></a>
### RowDefinitions `property`

##### Summary

An IGridRowDefinition collection for the GridLayout instance.

<a name='P-DrawnUi-Maui-Draw-ISkiaGridLayout-RowSpacing'></a>
### RowSpacing `property`

##### Summary

Gets the amount of space left between rows in the GridLayout.

<a name='M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetColumn-Microsoft-Maui-Controls-BindableObject-'></a>
### GetColumn(view) `method`

##### Summary

Gets the column of the child element.

##### Returns

The column that the child element is in.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [Microsoft.Maui.Controls.BindableObject](#T-Microsoft-Maui-Controls-BindableObject 'Microsoft.Maui.Controls.BindableObject') | A view that belongs to the Grid layout. |

<a name='M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetColumnSpan-Microsoft-Maui-Controls-BindableObject-'></a>
### GetColumnSpan(view) `method`

##### Summary

Gets the row span of the child element.

##### Returns

The row that the child element is in.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [Microsoft.Maui.Controls.BindableObject](#T-Microsoft-Maui-Controls-BindableObject 'Microsoft.Maui.Controls.BindableObject') | A view that belongs to the Grid layout. |

<a name='M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetRow-Microsoft-Maui-Controls-BindableObject-'></a>
### GetRow(view) `method`

##### Summary

Gets the row of the child element.

##### Returns

An integer that represents the row in which the item will appear.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [Microsoft.Maui.Controls.BindableObject](#T-Microsoft-Maui-Controls-BindableObject 'Microsoft.Maui.Controls.BindableObject') | A view that belongs to the Grid layout. |

<a name='M-DrawnUi-Maui-Draw-ISkiaGridLayout-GetRowSpan-Microsoft-Maui-Controls-BindableObject-'></a>
### GetRowSpan(view) `method`

##### Summary

Gets the row span of the child element.

##### Returns

The row that the child element is in.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [Microsoft.Maui.Controls.BindableObject](#T-Microsoft-Maui-Controls-BindableObject 'Microsoft.Maui.Controls.BindableObject') | A view that belongs to the Grid layout. |

<a name='T-DrawnUi-Maui-Draw-ISkiaLayer'></a>
## ISkiaLayer `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-ISkiaLayer-HasValidSnapshot'></a>
### HasValidSnapshot `property`

##### Summary

Snapshot was taken

<a name='P-DrawnUi-Maui-Draw-ISkiaLayer-LayerPaintArgs'></a>
### LayerPaintArgs `property`

##### Summary

Cached layer image

<a name='T-DrawnUi-Maui-Draw-ISkiaSharpView'></a>
## ISkiaSharpView `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-ISkiaSharpView-CreateStandaloneSurface-System-Int32,System-Int32-'></a>
### CreateStandaloneSurface(width,height) `method`

##### Summary

This is needed to get a hardware accelerated surface or a normal one depending on the platform

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| width | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| height | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='M-DrawnUi-Maui-Draw-ISkiaSharpView-Update-System-Int64-'></a>
### Update() `method`

##### Summary

Safe InvalidateSurface() call. If nanos not specified will generate ittself

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-IStateEffect'></a>
## IStateEffect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-IStateEffect-UpdateState'></a>
### UpdateState() `method`

##### Summary

Will be invoked before actually painting but after gestures processing and other internal calculations. By SkiaControl.OnBeforeDrawing method. Beware if you call Update() inside will never stop updating.

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-IVisibilityAware'></a>
## IVisibilityAware `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-IVisibilityAware-OnAppeared'></a>
### OnAppeared() `method`

##### Summary

This event can sometimes be called without prior OnAppearing

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-IVisibilityAware-OnAppearing'></a>
### OnAppearing() `method`

##### Summary

This can sometimes be omitted,

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Extensions-InternalExtensions'></a>
## InternalExtensions `type`

##### Namespace

DrawnUi.Maui.Extensions

<a name='M-DrawnUi-Maui-Extensions-InternalExtensions-ContainsInclusive-SkiaSharp-SKRect,SkiaSharp-SKPoint-'></a>
### ContainsInclusive(rect,point) `method`

##### Summary

The default Skia method is returning false if point is on the bounds, We correct this by custom function.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| rect | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| point | [SkiaSharp.SKPoint](#T-SkiaSharp-SKPoint 'SkiaSharp.SKPoint') |  |

<a name='M-DrawnUi-Maui-Extensions-InternalExtensions-ContainsInclusive-SkiaSharp-SKRect,System-Single,System-Single-'></a>
### ContainsInclusive() `method`

##### Summary

The default Skia method is returning false if point is on the bounds, We correct this by custom function.

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-LayoutType'></a>
## LayoutType `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-LayoutType-Absolute'></a>
### Absolute `constants`

##### Summary

Fastest rendering

<a name='F-DrawnUi-Maui-Draw-LayoutType-Column'></a>
### Column `constants`

##### Summary

Vertical stack

<a name='F-DrawnUi-Maui-Draw-LayoutType-Grid'></a>
### Grid `constants`

##### Summary

Use usual grid properties like Grid.Stack, ColumnSpacing etc

<a name='F-DrawnUi-Maui-Draw-LayoutType-Row'></a>
### Row `constants`

##### Summary

Horizontal stack

<a name='F-DrawnUi-Maui-Draw-LayoutType-Wrap'></a>
### Wrap `constants`

##### Summary

Think of wrap panel

<a name='T-DrawnUi-Maui-Draw-LineGlyph'></a>
## LineGlyph `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-LineGlyph-Width'></a>
### Width `property`

##### Summary

Measured text with advance

<a name='T-DrawnUi-Maui-Draw-LoadedImageSource'></a>
## LoadedImageSource `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-LoadedImageSource-ProtectFromDispose'></a>
### ProtectFromDispose `property`

##### Summary

As this can be disposed automatically by the consuming control like SkiaImage etc we can manually prohibit this for cases this instance is used elsewhere.

<a name='T-DrawnUi-Maui-Draw-LockTouch'></a>
## LockTouch `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-LockTouch-Enabled'></a>
### Enabled `constants`

##### Summary

Pass nothing below and mark all gestures as consumed by this control

<a name='F-DrawnUi-Maui-Draw-LockTouch-PassNone'></a>
### PassNone `constants`

##### Summary

Pass nothing below

<a name='F-DrawnUi-Maui-Draw-LockTouch-PassTap'></a>
### PassTap `constants`

##### Summary

Pass only Tapped below

<a name='F-DrawnUi-Maui-Draw-LockTouch-PassTapAndLongPress'></a>
### PassTapAndLongPress `constants`

##### Summary

Pass only Tapped and LongPressing below

<a name='T-DrawnUi-Maui-Controls-MauiEditor'></a>
## MauiEditor `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-MauiEditor-MaxLines'></a>
### MaxLines `property`

##### Summary

WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor

<a name='T-DrawnUi-Maui-Controls-MauiEntry'></a>
## MauiEntry `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-MauiEntry-MaxLines'></a>
### MaxLines `property`

##### Summary

WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor

<a name='T-DrawnUi-Maui-Draw-MauiKey'></a>
## MauiKey `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

These are platform-independent. They correspond to JavaScript keys.

<a name='T-DrawnUi-Maui-Infrastructure-MeasuringConstraints'></a>
## MeasuringConstraints `type`

##### Namespace

DrawnUi.Maui.Infrastructure

<a name='P-DrawnUi-Maui-Infrastructure-MeasuringConstraints-TotalMargins'></a>
### TotalMargins `property`

##### Summary

Include padding

<a name='T-DrawnUi-Maui-Draw-SkiaEditor-MyTextWatcher'></a>
## MyTextWatcher `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaEditor

<a name='P-DrawnUi-Maui-Draw-SkiaEditor-MyTextWatcher-NativeSelectionStart'></a>
### NativeSelectionStart `property`

##### Summary

This will be read by the parent to check the cursor position at will

<a name='T-DrawnUi-Maui-Controls-SkiaShell-NavigationLayer`1'></a>
## NavigationLayer\`1 `type`

##### Namespace

DrawnUi.Maui.Controls.SkiaShell

<a name='M-DrawnUi-Maui-Controls-SkiaShell-NavigationLayer`1-#ctor-DrawnUi-Maui-Controls-SkiaShell,System-Boolean-'></a>
### #ctor(shell,isModal) `constructor`

##### Summary

if isModel is true than will try to freeze background before showing. otherwise will be just an overlay like toast etc.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| shell | [DrawnUi.Maui.Controls.SkiaShell](#T-DrawnUi-Maui-Controls-SkiaShell 'DrawnUi.Maui.Controls.SkiaShell') |  |
| isModal | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='T-DrawnUi-Maui-Draw-PanningModeType'></a>
## PanningModeType `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-PanningModeType-Enabled'></a>
### Enabled `constants`

##### Summary

1 and 2 fingers

<a name='T-DrawnUi-Maui-Draw-PendulumAnimator'></a>
## PendulumAnimator `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-PendulumAnimator-InitialVelocity'></a>
### InitialVelocity `property`

##### Summary

the higher the faster will stop

<a name='P-DrawnUi-Maui-Draw-PendulumAnimator-IsOneDirectional'></a>
### IsOneDirectional `property`

##### Summary

Returns absolute value, instead of going -/+ along the axis. Basically if true simulates bouncing.

<a name='T-DrawnUi-Maui-Extensions-PointExtensions'></a>
## PointExtensions `type`

##### Namespace

DrawnUi.Maui.Extensions

<a name='M-DrawnUi-Maui-Extensions-PointExtensions-Add-Microsoft-Maui-Graphics-Point,Microsoft-Maui-Graphics-Point-'></a>
### Add(first,second) `method`

##### Summary

Adds the coordinates of one Point to another.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| first | [Microsoft.Maui.Graphics.Point](#T-Microsoft-Maui-Graphics-Point 'Microsoft.Maui.Graphics.Point') |  |
| second | [Microsoft.Maui.Graphics.Point](#T-Microsoft-Maui-Graphics-Point 'Microsoft.Maui.Graphics.Point') |  |

<a name='M-DrawnUi-Maui-Extensions-PointExtensions-Center-Microsoft-Maui-Graphics-Point[]-'></a>
### Center(touches) `method`

##### Summary

Gets the center of some touch points.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| touches | [Microsoft.Maui.Graphics.Point[]](#T-Microsoft-Maui-Graphics-Point[] 'Microsoft.Maui.Graphics.Point[]') |  |

<a name='M-DrawnUi-Maui-Extensions-PointExtensions-Subtract-Microsoft-Maui-Graphics-Point,Microsoft-Maui-Graphics-Point-'></a>
### Subtract(first,second) `method`

##### Summary

Subtracts the coordinates of one Point from another.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| first | [Microsoft.Maui.Graphics.Point](#T-Microsoft-Maui-Graphics-Point 'Microsoft.Maui.Graphics.Point') |  |
| second | [Microsoft.Maui.Graphics.Point](#T-Microsoft-Maui-Graphics-Point 'Microsoft.Maui.Graphics.Point') |  |

<a name='T-DrawnUi-Maui-Draw-RecycleTemplateType'></a>
## RecycleTemplateType `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-RecycleTemplateType-FillViewport'></a>
### FillViewport `constants`

##### Summary

Create cells instances until viewport is filled, then recycle while scrolling

<a name='F-DrawnUi-Maui-Draw-RecycleTemplateType-None'></a>
### None `constants`

##### Summary

One cell per item will be created, while a SkiaControl has little memory consumption for some controls
like SkiaLayer it might take more, so you might consider recycling for large number o items

<a name='F-DrawnUi-Maui-Draw-RecycleTemplateType-Single'></a>
### Single `constants`

##### Summary

Try using one cell per template at all times, binding context will change just before drawing.
ToDo investigate case of async data changes like images loading from web.

<a name='T-DrawnUi-Maui-Draw-RefreshIndicator'></a>
## RefreshIndicator `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-RefreshIndicator-IsRunning'></a>
### IsRunning `property`

##### Summary

ReadOnly

<a name='P-DrawnUi-Maui-Draw-RefreshIndicator-Orientation'></a>
### Orientation `property`

##### Summary



<a name='M-DrawnUi-Maui-Draw-RefreshIndicator-SetDragRatio-System-Single-'></a>
### SetDragRatio(ratio) `method`

##### Summary

0 - 1

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ratio | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-RenderingAnimator'></a>
## RenderingAnimator `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

This animator renders on canvas instead of just updating a value

<a name='M-DrawnUi-Maui-Draw-RenderingAnimator-OnRendering-DrawnUi-Maui-Draw-IDrawnBase,DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double-'></a>
### OnRendering(control,context,scale) `method`

##### Summary

return true if has drawn something and rendering needs to be applied

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.IDrawnBase](#T-DrawnUi-Maui-Draw-IDrawnBase 'DrawnUi.Maui.Draw.IDrawnBase') |  |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='T-DrawnUi-Maui-Draw-RippleAnimator'></a>
## RippleAnimator `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-RippleAnimator-X'></a>
### X `property`

##### Summary

In pts relative to control X,Y. These are coords inside the control and not inside the canvas.

<a name='P-DrawnUi-Maui-Draw-RippleAnimator-Y'></a>
### Y `property`

##### Summary

In pts relative to control X,Y. These are coords inside the control and not inside the canvas.

<a name='T-DrawnUi-Maui-Infrastructure-Helpers-RubberBandUtils'></a>
## RubberBandUtils `type`

##### Namespace

DrawnUi.Maui.Infrastructure.Helpers

<a name='M-DrawnUi-Maui-Infrastructure-Helpers-RubberBandUtils-ClampOnTrack-System-Numerics-Vector2,SkiaSharp-SKRect,System-Single-'></a>
### ClampOnTrack(point,track,coeff) `method`

##### Summary

track is the bounds of the possible scrolling offset, for example can be like {0, -1000, 0, 0}

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| point | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |
| track | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| coeff | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Infrastructure-Helpers-RubberBandUtils-RubberBandClamp-System-Single,System-Single,DrawnUi-Maui-Draw-RangeF,System-Single,System-Single-'></a>
### RubberBandClamp(coord,coeff,dim,limits,onEmpty) `method`

##### Summary

onEmpty - how much to simulate scrollable area when its zero

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| coord | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| coeff | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| dim | [DrawnUi.Maui.Draw.RangeF](#T-DrawnUi-Maui-Draw-RangeF 'DrawnUi.Maui.Draw.RangeF') |  |
| limits | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| onEmpty | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaScroll-ScrollingInteractionState'></a>
## ScrollingInteractionState `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaScroll

##### Summary

TODO impement this

<a name='T-DrawnUi-Maui-Draw-SkiaLayout-SecondPassArrange'></a>
## SecondPassArrange `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaLayout

##### Summary

Cell.Area contains the area for layout

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| cell | [T:DrawnUi.Maui.Draw.SkiaLayout.SecondPassArrange](#T-T-DrawnUi-Maui-Draw-SkiaLayout-SecondPassArrange 'T:DrawnUi.Maui.Draw.SkiaLayout.SecondPassArrange') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-SecondPassArrange-#ctor-DrawnUi-Maui-Draw-ControlInStack,DrawnUi-Maui-Draw-SkiaControl,System-Single-'></a>
### #ctor(cell,child,scale) `constructor`

##### Summary

Cell.Area contains the area for layout

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| cell | [DrawnUi.Maui.Draw.ControlInStack](#T-DrawnUi-Maui-Draw-ControlInStack 'DrawnUi.Maui.Draw.ControlInStack') |  |
| child | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Draw-ShapeType'></a>
## ShapeType `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-ShapeType-Squricle'></a>
### Squricle `constants`

##### Summary

TODO

<a name='T-DrawnUi-Maui-Draw-Sk3dView'></a>
## Sk3dView `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Under construction, custom implementation of removed API

<a name='F-DrawnUi-Maui-Draw-Sk3dView-CameraDistance'></a>
### CameraDistance `constants`

##### Summary

2D magic number camera distance 8 inches

<a name='M-DrawnUi-Maui-Draw-Sk3dView-Reset'></a>
### Reset() `method`

##### Summary

Resets the current state and clears all saved states

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Infrastructure-SkSl'></a>
## SkSl `type`

##### Namespace

DrawnUi.Maui.Infrastructure

<a name='M-DrawnUi-Maui-Infrastructure-SkSl-Compile-System-String,System-String-'></a>
### Compile(shaderCode,filename) `method`

##### Summary

Will compile your SKSL shader code into SKRuntimeEffect.
The filename parameter is used for debugging purposes only

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| shaderCode | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| filename | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaBackdrop'></a>
## SkiaBackdrop `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Warning with CPU-rendering edges will not be blurred: https://issues.skia.org/issues/40036320

<a name='F-DrawnUi-Maui-Draw-SkiaBackdrop-ImagePaint'></a>
### ImagePaint `constants`

##### Summary

Reusing this

<a name='F-DrawnUi-Maui-Draw-SkiaBackdrop-PaintColorFilter'></a>
### PaintColorFilter `constants`

##### Summary

Reusing this

<a name='F-DrawnUi-Maui-Draw-SkiaBackdrop-PaintImageFilter'></a>
### PaintImageFilter `constants`

##### Summary

Reusing this

<a name='P-DrawnUi-Maui-Draw-SkiaBackdrop-UseContext'></a>
### UseContext `property`

##### Summary

Use either context of global Superview background, default is True.

<a name='M-DrawnUi-Maui-Draw-SkiaBackdrop-AttachSource'></a>
### AttachSource() `method`

##### Summary

Designed to be just one-time set

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaBackdrop-GetImage'></a>
### GetImage() `method`

##### Summary

Returns the snapshot that was used for drawing the backdrop.
If we have no effects or the control has not yet been drawn the return value will be null.
You are responsible to dispose the returned image!

##### Returns



##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaButton'></a>
## SkiaButton `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Button-like control, can include any content inside. It's either you use default content (todo templates?..)
or can include any content inside, and properties will by applied by convention to a SkiaLabel with Tag \`MainLabel\`, SkiaShape with Tag \`MainFrame\`. At the same time you can override ApplyProperties() and apply them to your content yourself.

<a name='F-DrawnUi-Maui-Draw-SkiaButton-DelayCallbackMs'></a>
### DelayCallbackMs `constants`

##### Summary

You might want to pause to show effect before executing command. Default is 0.

<a name='P-DrawnUi-Maui-Draw-SkiaButton-Text'></a>
### Text `property`

##### Summary

Bind to your own content!

<a name='M-DrawnUi-Maui-Draw-SkiaButton-CreateClip-System-Object,System-Boolean,SkiaSharp-SKPath-'></a>
### CreateClip() `method`

##### Summary

Clip effects with rounded rect of the frame inside

##### Returns



##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaCacheType'></a>
## SkiaCacheType `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaCacheType-GPU'></a>
### GPU `constants`

##### Summary

The cached surface will use the same graphic context as your hardware-accelerated canvas.
This kind of cache will not apply Opacity as not all platforms support transparency for hardware accelerated layer.
Will fallback to simple Image cache type if hardware acceleration is not available.

<a name='F-DrawnUi-Maui-Draw-SkiaCacheType-Image'></a>
### Image `constants`

##### Summary

Will use simple SKBitmap cache type, will not use hardware acceleration.
Slower but will work for sizes bigger than graphics memory if needed.

<a name='F-DrawnUi-Maui-Draw-SkiaCacheType-ImageComposite'></a>
### ImageComposite `constants`

##### Summary

Would receive the invalidated area rectangle, then redraw the previous cache but clipped to exclude the dirty area, then would re-create the dirty area and draw it clipped inside the dirty rectangle. This is useful for layouts with many children, like scroll content etc, but useless for non-containers.

<a name='F-DrawnUi-Maui-Draw-SkiaCacheType-ImageDoubleBuffered'></a>
### ImageDoubleBuffered `constants`

##### Summary

Using \`Image\` cache type with double buffering. Will display a previous cache while rendering the new one in background, thus not slowing scrolling etc.

<a name='F-DrawnUi-Maui-Draw-SkiaCacheType-None'></a>
### None `constants`

##### Summary

True and old school

<a name='F-DrawnUi-Maui-Draw-SkiaCacheType-Operations'></a>
### Operations `constants`

##### Summary

Create and reuse SKPicture. Try this first for labels, svg etc. 
Do not use this when dropping shadows or with other effects, better use Bitmap.

<a name='F-DrawnUi-Maui-Draw-SkiaCacheType-OperationsFull'></a>
### OperationsFull `constants`

##### Summary

Create and reuse SKPicture all over the canvas ignoring clipping.
Try this first for labels, svg etc. 
Do not use this when dropping shadows or with other effects, better use Bitmap.

<a name='T-DrawnUi-Maui-Controls-SkiaCarousel'></a>
## SkiaCarousel `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-DynamicSize'></a>
### DynamicSize `property`

##### Summary

When specific dimension is adapting to children size, will use max child dimension if False,
otherwise will change size when children with different dimension size are selected. Default is false.
If true, requires MeasureAllItems to be set to all items.

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-IsLooped'></a>
### IsLooped `property`

##### Summary

UNIMPLEMENTED YET

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-IsRightToLeft'></a>
### IsRightToLeft `property`

##### Summary

TODO

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-IsVertical'></a>
### IsVertical `property`

##### Summary

Orientation of the carousel

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-LinearSpeedMs'></a>
### LinearSpeedMs `property`

##### Summary

How long would a whole auto-sliding take, if \`Bounces\` is \`False\`.
If set (>0) will be used for automatic scrolls instead of using manual velocity.
For bouncing carousel

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-PreloadNeighboors'></a>
### PreloadNeighboors `property`

##### Summary

Whether should preload neighboors sides cells even when they are hidden, to preload images etc.. Default is true. Beware set this to False if you have complex layouts otherwise rendering might be slow.

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-ScrollAmount'></a>
### ScrollAmount `property`

##### Summary

Scroll amount from 0 to 1 of the current (SelectedIndex) slide. Another similar but different property would be ScrollProgress. This is not linear as SelectedIndex changes earlier than 0 or 1 are attained.

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-ScrollProgress'></a>
### ScrollProgress `property`

##### Summary

Scroll progress from 0 to (numberOfSlides-1).
This is not dependent of the SelectedIndex, just reflects visible progress. Useful to create custom controls attached to carousel.
Calculated as (for horizontal): CurrentPosition.X / SnapPoints.Last().X

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-SelectedIndex'></a>
### SelectedIndex `property`

##### Summary

Zero-based index of the currently selected slide

<a name='P-DrawnUi-Maui-Controls-SkiaCarousel-SidesOffset'></a>
### SidesOffset `property`

##### Summary

Basically size margins of every slide, offset from the side of the carousel. Another similar but different property to use would be Spacing between slides.

<a name='M-DrawnUi-Maui-Controls-SkiaCarousel-AdaptChildren'></a>
### AdaptChildren() `method`

##### Summary

Set children layout options according to our settings. Not used for template case.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaCarousel-ApplyPosition-System-Numerics-Vector2-'></a>
### ApplyPosition(currentPosition) `method`

##### Summary

Will translate child and raise appearing/disappearing events

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| currentPosition | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaCarousel-GetContentOffsetBounds'></a>
### GetContentOffsetBounds() `method`

##### Summary

There are the bounds the scroll offset can go to.. This are NOT the bounds of the whole content.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaCarousel-InitializeChildren'></a>
### InitializeChildren() `method`

##### Summary

We expect this to be called after this alyout is invalidated

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaCarousel-OnTemplatesAvailable'></a>
### OnTemplatesAvailable() `method`

##### Summary

This might be called from background thread if we set InitializeTemplatesInBackgroundDelay true

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaCheckbox'></a>
## SkiaCheckbox `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Switch-like control, can include any content inside. It's aither you use default content (todo templates?..)
or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag \`Frame\`, SkiaShape with Tag \`Thumb\`. At the same time you can override ApplyProperties() and apply them to your content yourself.

<a name='T-DrawnUi-Maui-Draw-SkiaControl'></a>
## SkiaControl `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaControl-LockDraw'></a>
### LockDraw `constants`

##### Summary

Lock between replacing and using RenderObject

<a name='F-DrawnUi-Maui-Draw-SkiaControl-LockRenderObject'></a>
### LockRenderObject `constants`

##### Summary

Creating new cache lock

<a name='P-DrawnUi-Maui-Draw-SkiaControl-AllowCaching'></a>
### AllowCaching `property`

##### Summary

Might want to set this to False for certain cases..

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Bounds'></a>
### Bounds `property`

##### Summary

Overriding VisualElement property, use DrawingRect instead.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-CanUseCacheDoubleBuffering'></a>
### CanUseCacheDoubleBuffering `property`

##### Summary

Indended to prohibit background rendering, useful for streaming controls like camera, gif etc. SkiaBackdrop has it set to True as well.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ClipEffects'></a>
### ClipEffects `property`

##### Summary

This cuts shadows etc

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ClipFrom'></a>
### ClipFrom `property`

##### Summary

Use clipping area from another control

<a name='P-DrawnUi-Maui-Draw-SkiaControl-CommandChildTapped'></a>
### CommandChildTapped `property`

##### Summary

Child was tapped. Will pass the tapped child as parameter. You might want then read child's BindingContext etc.. This works only if your control implements ISkiaGestureListener.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-CreateChildren'></a>
### CreateChildren `property`

##### Summary

To create custom content in code-behind. Will be called from OnLayoutChanged if Views.Count == 0.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-CustomizeLayerPaint'></a>
### CustomizeLayerPaint `property`

##### Summary

Can customize the SKPaint used for painting the object

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Destination'></a>
### Destination `property`

##### Summary

For internal use

<a name='P-DrawnUi-Maui-Draw-SkiaControl-DrawingRect'></a>
### DrawingRect `property`

##### Summary

This is the destination in PIXELS with margins applied, using this to paint background. Since we enabled subpixel drawing (for smooth scroll etc) expect this to have non-rounded values, use CompareRects and similar for comparison.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ExpandCacheRecordingArea'></a>
### ExpandCacheRecordingArea `property`

##### Summary

Normally cache is recorded inside DrawingRect, but you might want to exapnd this to include shadows around, for example.
Specify number of points by which you want to expand the recording area.
Also you might maybe want to include a bigger area if your control is not inside the DrawingRect due to transforms/translations.
Override GetCacheRecordingArea method for a similar action.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-GestureListeners'></a>
### GestureListeners `property`

##### Summary

Children we should check for touch hits

<a name='P-DrawnUi-Maui-Draw-SkiaControl-HeightRequestRatio'></a>
### HeightRequestRatio `property`

##### Summary

HeightRequest Multiplier, default is 1.0

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Hero'></a>
### Hero `property`

##### Summary

Optional scene hero control identifier

<a name='P-DrawnUi-Maui-Draw-SkiaControl-HitBoxAuto'></a>
### HitBoxAuto `property`

##### Summary

This can be absolutely false if we are inside a cached 
rendering object parent that already moved somewhere.
So coords will be of the moment we were first drawn,
while if cached parent moved, our coords might differ.
todo detect if parent is cached somewhere and offset hotbox by cached parent movement offset...
todo think about it baby =) meanwhile just do not set gestures below cached level

<a name='P-DrawnUi-Maui-Draw-SkiaControl-IsClippedToBounds'></a>
### IsClippedToBounds `property`

##### Summary

This cuts shadows etc. You might want to enable it for some cases as it speeds up the rendering, it is False by default

<a name='P-DrawnUi-Maui-Draw-SkiaControl-IsLayoutDirty'></a>
### IsLayoutDirty `property`

##### Summary

Set this by parent if needed, normally child can detect this itsself. If true will call Arrange when drawing.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-IsMeasuring'></a>
### IsMeasuring `property`

##### Summary

Flag for internal use, maynly used to avoid conflicts between measuring on ui-thread and in background. If true, measure will return last measured value.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-IsOverlay'></a>
### IsOverlay `property`

##### Summary

do not ever erase background

<a name='P-DrawnUi-Maui-Draw-SkiaControl-IsRenderingWithComposition'></a>
### IsRenderingWithComposition `property`

##### Summary

Internal flag indicating that the current frame will use cache composition, old cache will be reused, only dirty children will be redrawn over it

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ItemTemplate'></a>
### ItemTemplate `property`

##### Summary

Kind of BindableLayout.DrawnTemplate

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ItemTemplateType'></a>
### ItemTemplateType `property`

##### Summary

ItemTemplate alternative for faster creation

<a name='P-DrawnUi-Maui-Draw-SkiaControl-LastDrawnAt'></a>
### LastDrawnAt `property`

##### Summary

Location on the canvas after last drawing completed

<a name='P-DrawnUi-Maui-Draw-SkiaControl-LockChildrenGestures'></a>
### LockChildrenGestures `property`

##### Summary

What gestures are allowed to be passed to children below.
If set to Enabled wit, otherwise can be more specific.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-LockRatio'></a>
### LockRatio `property`

##### Summary

Locks the final size to the min (-1.0 -> 0.0) or max (0.0 -> 1.0) of the provided size.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Margins'></a>
### Margins `property`

##### Summary

Total calculated margins in points

<a name='P-DrawnUi-Maui-Draw-SkiaControl-NeedDispose'></a>
### NeedDispose `property`

##### Summary

Developer can use this to mark control as to be disposed by parent custom controls

<a name='P-DrawnUi-Maui-Draw-SkiaControl-NeedUpdate'></a>
### NeedUpdate `property`

##### Summary

For internal use, set by Update method

<a name='P-DrawnUi-Maui-Draw-SkiaControl-NeedUpdateFrontCache'></a>
### NeedUpdateFrontCache `property`

##### Summary

Used by ImageDoubleBuffering cache

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Parent'></a>
### Parent `property`

##### Summary

Do not set this directly if you don't know what you are doing, use SetParent()

<a name='P-DrawnUi-Maui-Draw-SkiaControl-PostAnimators'></a>
### PostAnimators `property`

##### Summary

Executed after the rendering

<a name='P-DrawnUi-Maui-Draw-SkiaControl-RenderObject'></a>
### RenderObject `property`

##### Summary

The cached representation of the control. Will be used on redraws without calling Paint etc, until the control is requested to be updated.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-RenderObjectNeedsUpdate'></a>
### RenderObjectNeedsUpdate `property`

##### Summary

Should delete RenderObject when starting new frame rendering

<a name='P-DrawnUi-Maui-Draw-SkiaControl-RenderObjectPreparing'></a>
### RenderObjectPreparing `property`

##### Summary

Used by the UseCacheDoubleBuffering process. This is the new cache beign created in background. It will be copied to RenderObject when ready.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-RenderObjectPrevious'></a>
### RenderObjectPrevious `property`

##### Summary

Used by the UseCacheDoubleBuffering process.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-RenderTree'></a>
### RenderTree `property`

##### Summary

Last rendered controls tree. Used by gestures etc..

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Scale'></a>
### Scale `property`

##### Summary

Please use ScaleX, ScaleY instead of this maui property

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ShouldClipAntialiased'></a>
### ShouldClipAntialiased `property`

##### Summary

This is not a static bindable property. Can be set manually or by control, for example SkiaShape sets this to true for non-rectangular shapes, or rounded corners..

<a name='P-DrawnUi-Maui-Draw-SkiaControl-SizeRequest'></a>
### SizeRequest `property`

##### Summary

Is set by InvalidateMeasure();

<a name='P-DrawnUi-Maui-Draw-SkiaControl-SkipRendering'></a>
### SkipRendering `property`

##### Summary

Can be set but custom controls while optimizing rendering etc. Will affect CanDraw.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Superview'></a>
### Superview `property`

##### Summary

Our canvas

<a name='P-DrawnUi-Maui-Draw-SkiaControl-UseCache'></a>
### UseCache `property`

##### Summary

Never reuse the rendering result. Actually true for ScrollLooped SkiaLayout viewport container to redraw its content several times for creating a looped aspect.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-UsesCacheDoubleBuffering'></a>
### UsesCacheDoubleBuffering `property`

##### Summary

Read-only computed flag for internal use.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ViewportHeightLimit'></a>
### ViewportHeightLimit `property`

##### Summary

Will be used inside GetDrawingRectWithMargins to limit the height of the DrawingRect

<a name='P-DrawnUi-Maui-Draw-SkiaControl-ViewportWidthLimit'></a>
### ViewportWidthLimit `property`

##### Summary

Will be used inside GetDrawingRectWithMargins to limit the width of the DrawingRect

<a name='P-DrawnUi-Maui-Draw-SkiaControl-WasDrawn'></a>
### WasDrawn `property`

##### Summary

Signals if this control was drawn on canvas one time at least, it will be set by Paint method.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-WidthRequestRatio'></a>
### WidthRequestRatio `property`

##### Summary

WidthRequest Multiplier, default is 1.0

<a name='P-DrawnUi-Maui-Draw-SkiaControl-WillClipBounds'></a>
### WillClipBounds `property`

##### Summary

Used to check whether to apply IsClippedToBounds property

<a name='P-DrawnUi-Maui-Draw-SkiaControl-X'></a>
### X `property`

##### Summary

Absolute position obtained after this control was drawn on the Canvas, this is not relative to parent control.

<a name='P-DrawnUi-Maui-Draw-SkiaControl-Y'></a>
### Y `property`

##### Summary

Absolute position obtained after this control was drawn on the Canvas, this is not relative to parent control.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-AdaptHeightContraintToRequest-System-Single,Microsoft-Maui-Thickness,System-Double-'></a>
### AdaptHeightContraintToRequest(heightConstraint,scale) `method`

##### Summary

Apply margins to SizeRequest

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| heightConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [Microsoft.Maui.Thickness](#T-Microsoft-Maui-Thickness 'Microsoft.Maui.Thickness') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-AdaptSizeRequestToContent-System-Double,System-Double-'></a>
### AdaptSizeRequestToContent(widthRequestPts,heightRequestPts) `method`

##### Summary

In UNITS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| widthRequestPts | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| heightRequestPts | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-AdaptWidthConstraintToRequest-System-Single,Microsoft-Maui-Thickness,System-Double-'></a>
### AdaptWidthConstraintToRequest(widthConstraint,scale) `method`

##### Summary

Apply margins to SizeRequest

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| widthConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [Microsoft.Maui.Thickness](#T-Microsoft-Maui-Thickness 'Microsoft.Maui.Thickness') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-AnimateAsync-System-Action{System-Double},System-Action,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource-'></a>
### AnimateAsync(callback,length,easing,cancel) `method`

##### Summary

Creates a new animator, animates from 0 to 1 over a given time, and calls your callback with the current eased value

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| callback | [System.Action{System.Double}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{System.Double}') |  |
| length | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') |  |
| easing | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| cancel | [Microsoft.Maui.Easing](#T-Microsoft-Maui-Easing 'Microsoft.Maui.Easing') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-ApplyBindingContext'></a>
### ApplyBindingContext() `method`

##### Summary

https://github.com/taublast/DrawnUi.Maui/issues/92#issuecomment-2408805077

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-ApplyMeasureResult'></a>
### ApplyMeasureResult() `method`

##### Summary

Normally get a a Measure by parent then parent calls Draw and we can apply the measure result.
But in a case we have measured us ourselves inside PreArrange etc we must call ApplyMeasureResult because this would happen after the Draw and not before.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-AreClose-System-Single,System-Single-'></a>
### AreClose(value1,value2) `method`

##### Summary

Ported from Avalonia: AreClose - Returns whether or not two floats are "close".  That is, whether or 
not they are within epsilon of each other.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value1 | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | The first float to compare. |
| value2 | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | The second float to compare. |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-AreClose-System-Double,System-Double-'></a>
### AreClose(value1,value2) `method`

##### Summary

Ported from Avalonia: AreClose - Returns whether or not two doubles are "close".  That is, whether or 
not they are within epsilon of each other.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value1 | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') | The first double to compare. |
| value2 | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') | The second double to compare. |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Arrange-SkiaSharp-SKRect,System-Single,System-Single,System-Single-'></a>
### Arrange(destination,widthRequest,heightRequest,scale) `method`

##### Summary

destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS.
DrawUsingRenderObject wil call this among others..

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') | PIXELS |
| widthRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | UNITS |
| heightRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | UNITS |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-CalculateLayout-SkiaSharp-SKRect,System-Single,System-Single,System-Single-'></a>
### CalculateLayout(destination,widthRequest,heightRequest,scale) `method`

##### Summary

destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS.
Not using Margins nor Padding
Children are responsible to apply Padding to their content and to apply Margin to destination when measuring and drawing

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') | PIXELS |
| widthRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | UNITS |
| heightRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | UNITS |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-CalculateMargins'></a>
### CalculateMargins() `method`

##### Summary

Summing up Margins and AddMargin.. properties

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-ClipSmart-SkiaSharp-SKCanvas,SkiaSharp-SKPath,SkiaSharp-SKClipOperation-'></a>
### ClipSmart(canvas,path,operation) `method`

##### Summary

Use antialiasing from ShouldClipAntialiased

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| canvas | [SkiaSharp.SKCanvas](#T-SkiaSharp-SKCanvas 'SkiaSharp.SKCanvas') |  |
| path | [SkiaSharp.SKPath](#T-SkiaSharp-SKPath 'SkiaSharp.SKPath') |  |
| operation | [SkiaSharp.SKClipOperation](#T-SkiaSharp-SKClipOperation 'SkiaSharp.SKClipOperation') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-CommitInvalidations'></a>
### CommitInvalidations() `method`

##### Summary

Apply all postponed invalidation other logic that was postponed until the first draw for optimization. Use this for special code-behind cases, like tests etc, if you cannot wait until the first Draw(). In this version this affects ItemsSource only.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-CreateChildrenFromCode'></a>
### CreateChildrenFromCode() `method`

##### Summary

Executed when CreateChildren is set

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-CreateClip-System-Object,System-Boolean,SkiaSharp-SKPath-'></a>
### CreateClip(arguments) `method`

##### Summary

Create this control clip for painting content.
Pass arguments if you want to use some time-frozen data for painting at any time from any thread..

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| arguments | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-CreateRenderingObjectAndPaint-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,SkiaSharp-SKRect,System-Action{DrawnUi-Maui-Draw-SkiaDrawingContext}-'></a>
### CreateRenderingObjectAndPaint(context,recordArea,action) `method`

##### Summary

This is NOT calling FinalizeDraw()!
parameter 'area' Usually is equal to DrawingRect

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| recordArea | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| action | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-DefineAvailableSize-SkiaSharp-SKRect,System-Single,System-Single,System-Single-'></a>
### DefineAvailableSize(destination,widthRequest,heightRequest,scale) `method`

##### Summary

destination in PIXELS, requests in UNITS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| widthRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| heightRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Dispose'></a>
### Dispose() `method`

##### Summary

Avoid setting parent to null before calling this, or set SuperView prop manually for proper cleanup of animations and gestures if any used

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-DisposeObject-System-IDisposable-'></a>
### DisposeObject(disposable) `method`

##### Summary

Dispose with needed delay.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| disposable | [System.IDisposable](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.IDisposable 'System.IDisposable') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-DrawRenderObject-DrawnUi-Maui-Draw-CachedObject,DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect-'></a>
### DrawRenderObject(cache,ctx,destination) `method`

##### Summary

Drawing cache, applying clip and transforms as well

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| cache | [DrawnUi.Maui.Draw.CachedObject](#T-DrawnUi-Maui-Draw-CachedObject 'DrawnUi.Maui.Draw.CachedObject') |  |
| ctx | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-DrawUsingRenderObject-DrawnUi-Maui-Draw-SkiaDrawingContext,System-Single,System-Single,SkiaSharp-SKRect,System-Single-'></a>
### DrawUsingRenderObject(context,widthRequest,heightRequest,destination,scale) `method`

##### Summary

Returns true if had drawn.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| widthRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| heightRequest | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-DrawViews-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single,System-Boolean-'></a>
### DrawViews(context,destination,scale,debug) `method`

##### Summary

Base method will call RenderViewsList.
Return number of drawn views.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| debug | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-FadeToAsync-System-Double,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource-'></a>
### FadeToAsync(end,length,easing,cancel) `method`

##### Summary

Fades the view from the current Opacity to end, animator is reused if already running

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| end | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| length | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| easing | [Microsoft.Maui.Easing](#T-Microsoft-Maui-Easing 'Microsoft.Maui.Easing') |  |
| cancel | [System.Threading.CancellationTokenSource](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Threading.CancellationTokenSource 'System.Threading.CancellationTokenSource') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-FinalizeDrawingWithRenderObject-DrawnUi-Maui-Draw-SkiaDrawingContext,System-Double-'></a>
### FinalizeDrawingWithRenderObject(context,scale) `method`

##### Summary

Execute post drawing operations, like post-animators etc

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GenerateParentChain'></a>
### GenerateParentChain() `method`

##### Summary

This actually used by SkiaMauiElement but could be used by other controls. Also might be useful for debugging purposes.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GestureIsInside-AppoMobi-Maui-Gestures-TouchActionEventArgs,System-Single,System-Single-'></a>
### GestureIsInside(args) `method`

##### Summary

To detect if current location is inside Destination

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| args | [AppoMobi.Maui.Gestures.TouchActionEventArgs](#T-AppoMobi-Maui-Gestures-TouchActionEventArgs 'AppoMobi.Maui.Gestures.TouchActionEventArgs') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GestureStartedInside-AppoMobi-Maui-Gestures-TouchActionEventArgs,System-Single,System-Single-'></a>
### GestureStartedInside(args) `method`

##### Summary

To detect if a gesture Start point was inside Destination

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| args | [AppoMobi.Maui.Gestures.TouchActionEventArgs](#T-AppoMobi-Maui-Gestures-TouchActionEventArgs 'AppoMobi.Maui.Gestures.TouchActionEventArgs') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetCacheArea-SkiaSharp-SKRect-'></a>
### GetCacheArea() `method`

##### Summary

Normally cache is recorded inside DrawingRect, but you might want to exapnd this to include shadows around, for example.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetCacheRecordingArea-SkiaSharp-SKRect-'></a>
### GetCacheRecordingArea() `method`

##### Summary

Used for the Operations cache type to record inside the changed area, if your control is not inside the DrawingRect due to transforms/translations. This is NOT changing the rendering object

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetFuturePositionOnCanvas-System-Boolean-'></a>
### GetFuturePositionOnCanvas() `method`

##### Summary

Absolute position in pixels before drawn.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetFuturePositionOnCanvasInPoints-System-Boolean-'></a>
### GetFuturePositionOnCanvasInPoints() `method`

##### Summary

Absolute position in points

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetOnScreenVisibleArea-System-Single-'></a>
### GetOnScreenVisibleArea() `method`

##### Summary

For virtualization

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetPositionOnCanvas-System-Boolean-'></a>
### GetPositionOnCanvas() `method`

##### Summary

Absolute position in pixels afetr drawn.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetPositionOnCanvasInPoints-System-Boolean-'></a>
### GetPositionOnCanvasInPoints() `method`

##### Summary

Absolute position in points

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetRenderingScaleFor-System-Single,System-Single-'></a>
### GetRenderingScaleFor(width,height) `method`

##### Summary

Returns rendering scale adapted for another output size, useful for offline rendering

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| width | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| height | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetSelfDrawingPosition'></a>
### GetSelfDrawingPosition() `method`

##### Summary

Find drawing position for control accounting for all caches up the rendering tree.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-GetTopParentView'></a>
### GetTopParentView() `method`

##### Summary

Use Superview from public area

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-HitIsInside-System-Single,System-Single-'></a>
### HitIsInside() `method`

##### Summary

ISkiaGestureListener impl

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Invalidate'></a>
### Invalidate() `method`

##### Summary

Base calls InvalidateInternal and InvalidateParent

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-InvalidateByChild-DrawnUi-Maui-Draw-SkiaControl-'></a>
### InvalidateByChild(child) `method`

##### Summary

To be able to fast track dirty children

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| child | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-InvalidateChildren-DrawnUi-Maui-Draw-SkiaControl-'></a>
### InvalidateChildren(control) `method`

##### Summary

Will invoke InvalidateInternal on controls and subviews

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-InvalidateChildrenTree-DrawnUi-Maui-Draw-SkiaControl-'></a>
### InvalidateChildrenTree(control) `method`

##### Summary

Will invoke InvalidateInternal on controls and subviews

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-InvalidateInternal'></a>
### InvalidateInternal() `method`

##### Summary

Soft invalidation, without requiring update. So next time we try to draw this one it will recalc everything.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-InvalidateViewport'></a>
### InvalidateViewport() `method`

##### Summary

Indicated that wants to be re-measured without invalidating cache

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-IsOne-System-Double-'></a>
### IsOne(value) `method`

##### Summary

Avalonia: IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
but this is faster.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') | The double to compare to 1. |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-IsPixelInside-System-Single,System-Single-'></a>
### IsPixelInside(x,y) `method`

##### Summary

Whether the pixel is inside Destination

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-IsPointInside-System-Single,System-Single,System-Single-'></a>
### IsPointInside(x,y,scale) `method`

##### Summary

Whether the point is inside Destination

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Measure-System-Single,System-Single,System-Single-'></a>
### Measure(widthConstraint,heightConstraint,scale) `method`

##### Summary



##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| widthConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| heightConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-MeasureAbsoluteBase-SkiaSharp-SKRect,System-Single-'></a>
### MeasureAbsoluteBase(rectForChildrenPixels,scale) `method`

##### Summary

Base method, not aware of any views provider, not virtual, silly measuring Children.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| rectForChildrenPixels | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-MeasureChild-DrawnUi-Maui-Draw-SkiaControl,System-Double,System-Double,System-Single-'></a>
### MeasureChild(child,availableWidth,availableHeight,scale) `method`

##### Summary

PIXELS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| child | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| availableWidth | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| availableHeight | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-MeasureContent-System-Collections-Generic-IEnumerable{DrawnUi-Maui-Draw-SkiaControl},SkiaSharp-SKRect,System-Single-'></a>
### MeasureContent(children,rectForChildrenPixels,scale) `method`

##### Summary

Measuring as absolute layout for passed children

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| children | [System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl}') |  |
| rectForChildrenPixels | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-NeedRepaint-Microsoft-Maui-Controls-BindableObject,System-Object,System-Object-'></a>
### NeedRepaint() `method`

##### Summary

Just make us repaint to apply new transforms etc

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-OnBindingContextChanged'></a>
### OnBindingContextChanged() `method`

##### Summary

First Maui will apply bindings to your controls, then it would call OnBindingContextChanged, so beware on not to break bindings.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-OnDisposing'></a>
### OnDisposing() `method`

##### Summary

Base performs some cleanup actions with Superview

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-OnLayoutReady'></a>
### OnLayoutReady() `method`

##### Summary

Layout was changed with dimensions above zero. Rather a helper method, can you more generic OnLayoutChanged().

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-OnParentVisibilityChanged-System-Boolean-'></a>
### OnParentVisibilityChanged(newvalue) `method`

##### Summary

todo override for templated skialayout to use ViewsProvider

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| newvalue | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-OnVisibilityChanged-System-Boolean-'></a>
### OnVisibilityChanged(newvalue) `method`

##### Summary

todo override for templated skialayout to use ViewsProvider

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| newvalue | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-OnWillDisposeWithChildren'></a>
### OnWillDisposeWithChildren() `method`

##### Summary

The OnDisposing might come with a delay to avoid disposing resources at use.
This method will be called without delay when Dispose() is invoked. Disposed will set to True and for Views their OnWillDisposeWithChildren will be called.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Paint-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single,System-Object-'></a>
### Paint(ctx,destination,scale) `method`

##### Summary

This is the main drawing routine you should override to draw something.
Base one paints background color inside DrawingRect that was defined by Arrange inside base.Draw.
Pass arguments if you want to use some time-frozen data for painting at any time from any thread..

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ctx | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-PaintTintBackground-SkiaSharp-SKCanvas,SkiaSharp-SKRect-'></a>
### PaintTintBackground(canvas,destination) `method`

##### Summary

Pixels, if you see no Scale parameter

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| canvas | [SkiaSharp.SKCanvas](#T-SkiaSharp-SKCanvas 'SkiaSharp.SKCanvas') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-PlayRippleAnimation-Microsoft-Maui-Graphics-Color,System-Double,System-Double,System-Boolean-'></a>
### PlayRippleAnimation(color,x,y,removePrevious) `method`

##### Summary

Expecting input coordinates in POINTs and relative to control coordinates. Use GetOffsetInsideControlInPoints to help.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| color | [Microsoft.Maui.Graphics.Color](#T-Microsoft-Maui-Graphics-Color 'Microsoft.Maui.Graphics.Color') |  |
| x | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| y | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| removePrevious | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-PostponeInvalidation-System-String,System-Action-'></a>
### PostponeInvalidation(key,action) `method`

##### Summary

Used for optimization process, for example, to avoid changing ItemSource several times before the first draw.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| key | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| action | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-PreArrange-SkiaSharp-SKRect,System-Single,System-Single,System-Single-'></a>
### PreArrange() `method`

##### Summary

Returns false if should not render

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Reload'></a>
### Reload() `method`

##### Summary

HOTRELOAD IReloadHandler

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-RenderViewsList-System-Collections-Generic-IEnumerable{DrawnUi-Maui-Draw-SkiaControl},DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single,System-Boolean-'></a>
### RenderViewsList(skiaControls,context,destination,scale,debug) `method`

##### Summary

Use to render Absolute layout. Base method is not supporting templates, override it to implemen your logic.
Returns number of drawn children.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| skiaControls | [System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl}') |  |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| debug | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Repaint'></a>
### Repaint() `method`

##### Summary

Just make us repaint to apply new transforms etc

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaControl-RotateToAsync-System-Double,System-UInt32,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource-'></a>
### RotateToAsync(end,length,easing,cancel) `method`

##### Summary

Rotates the view from the current rotation to end, animator is reused if already running

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| end | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| length | [System.UInt32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.UInt32 'System.UInt32') |  |
| easing | [Microsoft.Maui.Easing](#T-Microsoft-Maui-Easing 'Microsoft.Maui.Easing') |  |
| cancel | [System.Threading.CancellationTokenSource](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Threading.CancellationTokenSource 'System.Threading.CancellationTokenSource') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SafeAction-System-Action-'></a>
### SafeAction(action) `method`

##### Summary

If attached to a SuperView will run only after draw to avoid memory access conflicts. If not attached will run after 3 secs..

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SafePostAction-System-Action-'></a>
### SafePostAction(action) `method`

##### Summary

If attached to a SuperView and rendering is in progress will run after it. Run now otherwise.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-ScaleToAsync-System-Double,System-Double,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource-'></a>
### ScaleToAsync(x,y,length,easing,cancel) `method`

##### Summary

Scales the view from the current Scale to x,y, animator is reused if already running

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| y | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| length | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| easing | [Microsoft.Maui.Easing](#T-Microsoft-Maui-Easing 'Microsoft.Maui.Easing') |  |
| cancel | [System.Threading.CancellationTokenSource](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Threading.CancellationTokenSource 'System.Threading.CancellationTokenSource') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SetInheritedBindingContext-System-Object-'></a>
### SetInheritedBindingContext(context) `method`

##### Summary

This is to be called by layouts to propagate their binding context to children.
By overriding this method any child could deny a new context or use any other custom logic.
To force new context for child parent would set child's BindingContext directly skipping the use of this method.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SetMeasured-System-Single,System-Single,System-Boolean,System-Boolean,System-Single-'></a>
### SetMeasured(width,height,scale) `method`

##### Summary

Parameters in PIXELS. sets IsLayoutDirty = true;

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| width | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| height | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SetVisualTransform-DrawnUi-Maui-Infrastructure-VisualTransform-'></a>
### SetVisualTransform(transform) `method`

##### Summary

//todo base. this is actually used by SkiaMauiElement only

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| transform | [DrawnUi.Maui.Infrastructure.VisualTransform](#T-DrawnUi-Maui-Infrastructure-VisualTransform 'DrawnUi.Maui.Infrastructure.VisualTransform') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SetupGradient-SkiaSharp-SKPaint,DrawnUi-Maui-Draw-SkiaGradient,SkiaSharp-SKRect-'></a>
### SetupGradient(paint,gradient,destination) `method`

##### Summary

Creates Shader for gradient and sets it to passed SKPaint along with BlendMode

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| paint | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |
| gradient | [DrawnUi.Maui.Draw.SkiaGradient](#T-DrawnUi-Maui-Draw-SkiaGradient 'DrawnUi.Maui.Draw.SkiaGradient') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SetupShadow-SkiaSharp-SKPaint,DrawnUi-Maui-Draw-SkiaShadow,System-Single-'></a>
### SetupShadow(paint,shadow) `method`

##### Summary

Creates and sets an ImageFilter for SKPaint

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| paint | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |
| shadow | [DrawnUi.Maui.Draw.SkiaShadow](#T-DrawnUi-Maui-Draw-SkiaShadow 'DrawnUi.Maui.Draw.SkiaShadow') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-TranslateInputCoords-SkiaSharp-SKPoint,System-Boolean-'></a>
### TranslateInputCoords(childOffset) `method`

##### Summary

Use this to consume gestures in your control only,
do not use result for passing gestures below

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| childOffset | [SkiaSharp.SKPoint](#T-SkiaSharp-SKPoint 'SkiaSharp.SKPoint') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-TranslateToAsync-System-Double,System-Double,System-Single,Microsoft-Maui-Easing,System-Threading-CancellationTokenSource-'></a>
### TranslateToAsync(x,y,length,easing,cancel) `method`

##### Summary

Translates the view from the current position to x,y, animator is reused if already running

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| y | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| length | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| easing | [Microsoft.Maui.Easing](#T-Microsoft-Maui-Easing 'Microsoft.Maui.Easing') |  |
| cancel | [System.Threading.CancellationTokenSource](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Threading.CancellationTokenSource 'System.Threading.CancellationTokenSource') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-Update'></a>
### Update() `method`

##### Summary

Main method to invalidate cache and invoke rendering

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect'></a>
## SkiaControlWithRect `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaControl

##### Summary

Rect is real drawing position

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| Control | [T:DrawnUi.Maui.Draw.SkiaControl.SkiaControlWithRect](#T-T-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect 'T:DrawnUi.Maui.Draw.SkiaControl.SkiaControlWithRect') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-#ctor-DrawnUi-Maui-Draw-SkiaControl,SkiaSharp-SKRect,SkiaSharp-SKRect,System-Int32-'></a>
### #ctor(Control,Rect,Index) `constructor`

##### Summary

Rect is real drawing position

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| Control | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| Rect | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| Index | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='P-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-Control'></a>
### Control `property`

##### Summary



<a name='P-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-Index'></a>
### Index `property`

##### Summary



<a name='P-DrawnUi-Maui-Draw-SkiaControl-SkiaControlWithRect-Rect'></a>
### Rect `property`

##### Summary



<a name='T-DrawnUi-Maui-Controls-SkiaDrawer'></a>
## SkiaDrawer `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-SkiaDrawer-AmplitudeSize'></a>
### AmplitudeSize `property`

##### Summary

If set to other than -1 will be used instead of HeaderSize for amplitude calculation, amplitude = drawer size - header.

<a name='P-DrawnUi-Maui-Controls-SkiaDrawer-HeaderSize'></a>
### HeaderSize `property`

##### Summary

Size of the area that will remain on screen when drawer is closed

<a name='M-DrawnUi-Maui-Controls-SkiaDrawer-ClampOffsetWithRubberBand-System-Single,System-Single-'></a>
### ClampOffsetWithRubberBand(x,y) `method`

##### Summary

Called for manual finger panning

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaDrawer-GetClosestSidePoint-SkiaSharp-SKPoint,SkiaSharp-SKRect,SkiaSharp-SKSize-'></a>
### GetClosestSidePoint(overscrollPoint,contentRect,viewportSize) `method`

##### Summary

This uses whole viewport size, do not use this for snapping

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| overscrollPoint | [SkiaSharp.SKPoint](#T-SkiaSharp-SKPoint 'SkiaSharp.SKPoint') |  |
| contentRect | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| viewportSize | [SkiaSharp.SKSize](#T-SkiaSharp-SKSize 'SkiaSharp.SKSize') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaDrawer-GetOffsetToHide'></a>
### GetOffsetToHide() `method`

##### Summary

In points

##### Returns



##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaDrawingContext'></a>
## SkiaDrawingContext `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaDrawingContext-IsRecycled'></a>
### IsRecycled `property`

##### Summary

Reusing surface from previous cache

<a name='P-DrawnUi-Maui-Draw-SkiaDrawingContext-IsVirtual'></a>
### IsVirtual `property`

##### Summary

Recording cache

<a name='T-DrawnUi-Maui-Controls-SkiaDrawnCell'></a>
## SkiaDrawnCell `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Base ISkiaCell implementation

<a name='T-DrawnUi-Maui-Controls-SkiaDynamicDrawnCell'></a>
## SkiaDynamicDrawnCell `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

This cell can watch binding context property changing

<a name='T-DrawnUi-Maui-Draw-SkiaEditor'></a>
## SkiaEditor `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-SkiaEditor-GetCursorPosition-System-Single,System-Single-'></a>
### GetCursorPosition(x,y) `method`

##### Summary

Input in pixels

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaEditor-MoveCursorTo-System-Double,System-Double-'></a>
### MoveCursorTo(x,y) `method`

##### Summary

Translate cursor from the left top corner, params in pts.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| y | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaEditor-MoveInternalCursor'></a>
### MoveInternalCursor() `method`

##### Summary

Sets native contol cursor position to CursorPosition and calls UpdateCursorVisibility

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaEditor-SetCursorPositionWithDelay-System-Int32,System-Int32-'></a>
### SetCursorPositionWithDelay(ms,position) `method`

##### Summary

We have to sync with a delay after text was changed otherwise the cursor position is not updated yet.
Using restarting timer, every time this is called the timer is reset if callback wasn't executed yet.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ms | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| position | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaEditor-Submit'></a>
### Submit() `method`

##### Summary

This is Done or Enter key, so maybe just split lines in specific case

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaEditor-UpdateCursorVisibility'></a>
### UpdateCursorVisibility() `method`

##### Summary

Positions cursor control where it should be using translation, and sets its visibility.

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaEffect'></a>
## SkiaEffect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaEffect-Parent'></a>
### Parent `property`

##### Summary

For public set use Attach/Detach

<a name='T-DrawnUi-Maui-Draw-SkiaFontManager'></a>
## SkiaFontManager `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-SkiaFontManager-GetEmbeddedResourceNames'></a>
### GetEmbeddedResourceNames() `method`

##### Summary

Get the list of all emdedded resources in the assembly.

##### Returns

An array of fully qualified resource names

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaFontManager-GetEmbeddedResourceStream-System-String-'></a>
### GetEmbeddedResourceStream(resourceName) `method`

##### Summary

Takes the full name of a resource and loads it in to a stream.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| resourceName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Assuming an embedded resource is a file
called info.png and is located in a folder called Resources, it
will be compiled in to the assembly with this fully qualified
name: Full.Assembly.Name.Resources.info.png. That is the string
that you should pass to this method. |

<a name='M-DrawnUi-Maui-Draw-SkiaFontManager-GetWeightEnum-System-Int32-'></a>
### GetWeightEnum(weight) `method`

##### Summary

Gets the closest enum value to the given weight. Like 590 would return Semibold.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| weight | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaGif'></a>
## SkiaGif `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='M-DrawnUi-Maui-Controls-SkiaGif-#ctor'></a>
### #ctor() `constructor`

##### Summary

For standalone use

##### Parameters

This constructor has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaGif-#ctor-DrawnUi-Maui-Draw-SkiaImage-'></a>
### #ctor(display) `constructor`

##### Summary

For building custom controls

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| display | [DrawnUi.Maui.Draw.SkiaImage](#T-DrawnUi-Maui-Draw-SkiaImage 'DrawnUi.Maui.Draw.SkiaImage') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaGif-OnAnimatorSeeking-System-Double-'></a>
### OnAnimatorSeeking(frame) `method`

##### Summary

called by Seek

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| frame | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaGif-OnAnimatorUpdated-System-Double-'></a>
### OnAnimatorUpdated(value) `method`

##### Summary

invoked by internal animator

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure'></a>
## SkiaGridStructure `type`

##### Namespace

DrawnUi.Maui.Draw.SkiaLayout

<a name='F-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure-_gridHeightConstraint'></a>
### _gridHeightConstraint `constants`

##### Summary

Pixels

<a name='F-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure-_gridWidthConstraint'></a>
### _gridWidthConstraint `constants`

##### Summary

Pixels

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-SkiaGridStructure-InitializeCells'></a>
### InitializeCells() `method`

##### Summary

We are also going to auto-create column/row definitions here

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaHotspot'></a>
## SkiaHotspot `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaHotspot-DelayCallbackMs'></a>
### DelayCallbackMs `constants`

##### Summary

You might want to pause to show effect before executing command. Default is 0.

<a name='T-DrawnUi-Maui-Draw-SkiaHotspotZoom'></a>
## SkiaHotspotZoom `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaHotspotZoom-LastValue'></a>
### LastValue `constants`

##### Summary

Last ViewportZoom value we are animating from

<a name='P-DrawnUi-Maui-Draw-SkiaHotspotZoom-ZoomSpeed'></a>
### ZoomSpeed `property`

##### Summary

How much of finger movement will afect zoom change

<a name='T-DrawnUi-Maui-Draw-SkiaHoverMask'></a>
## SkiaHoverMask `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Paints the parent view with the background color with a clipped viewport oth this view size

<a name='T-DrawnUi-Maui-Draw-SkiaImage'></a>
## SkiaImage `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaImage-ImagePaint'></a>
### ImagePaint `constants`

##### Summary

Reusing this

<a name='F-DrawnUi-Maui-Draw-SkiaImage-PaintColorFilter'></a>
### PaintColorFilter `constants`

##### Summary

Reusing this

<a name='F-DrawnUi-Maui-Draw-SkiaImage-PaintImageFilter'></a>
### PaintImageFilter `constants`

##### Summary

Reusing this

<a name='P-DrawnUi-Maui-Draw-SkiaImage-Aspect'></a>
### Aspect `property`

##### Summary

Apspect to render image with, default is AspectCover.

<a name='P-DrawnUi-Maui-Draw-SkiaImage-EraseChangedContent'></a>
### EraseChangedContent `property`

##### Summary

Should we erase the existing image when another Source is set but wasn't applied yet (not loaded yet)

<a name='P-DrawnUi-Maui-Draw-SkiaImage-ImageBitmap'></a>
### ImageBitmap `property`

##### Summary

this is the source loaded, doesn't reflect any effects or any other rendering properties

<a name='P-DrawnUi-Maui-Draw-SkiaImage-LastSource'></a>
### LastSource `property`

##### Summary

Last source that we where loading. Td be reused for reload..

<a name='P-DrawnUi-Maui-Draw-SkiaImage-LoadSourceOnFirstDraw'></a>
### LoadSourceOnFirstDraw `property`

##### Summary

Should the source be loaded on the first draw, useful for the first fast rendering of the screen and loading images after,
default is False.
Set this to False if you need an off-screen loading and to True to make the screen load faster.
While images are loaded in async manner this still has its impact.
Useful to set True for for SkiaCarousel cells etc..

<a name='P-DrawnUi-Maui-Draw-SkiaImage-PreviewBase64'></a>
### PreviewBase64 `property`

##### Summary

If setting in code-behind must be set BEFORE you change the Source

<a name='P-DrawnUi-Maui-Draw-SkiaImage-RescalingQuality'></a>
### RescalingQuality `property`

##### Summary

Default value is None.
You might want to set this to Medium for static images for better quality.

<a name='P-DrawnUi-Maui-Draw-SkiaImage-SourceHeight'></a>
### SourceHeight `property`

##### Summary

From current set Source in points

<a name='P-DrawnUi-Maui-Draw-SkiaImage-SourceWidth'></a>
### SourceWidth `property`

##### Summary

From current set Source in points

<a name='M-DrawnUi-Maui-Draw-SkiaImage-GetRenderedSource'></a>
### GetRenderedSource() `method`

##### Summary

Will containt all the effects and other rendering properties applied, size will correspond to source.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaImage-SetBitmapInternal-SkiaSharp-SKBitmap,System-Boolean-'></a>
### SetBitmapInternal(bitmap) `method`

##### Summary

Use only if you know what to do, this internally just sets the new bitmap without any invalidations and not forcing an update.
You would want to set InstancedBitmap prop for a usual approach.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| bitmap | [SkiaSharp.SKBitmap](#T-SkiaSharp-SKBitmap 'SkiaSharp.SKBitmap') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImage-SetImage-DrawnUi-Maui-Draw-LoadedImageSource-'></a>
### SetImage(loaded) `method`

##### Summary

Do not call this directly, use InstancedBitmap prop

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| loaded | [DrawnUi.Maui.Draw.LoadedImageSource](#T-DrawnUi-Maui-Draw-LoadedImageSource 'DrawnUi.Maui.Draw.LoadedImageSource') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaImageEffect'></a>
## SkiaImageEffect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaImageEffect-Tint'></a>
### Tint `constants`

##### Summary

Background color will be used to tint

<a name='T-DrawnUi-Maui-Draw-SkiaImageEffects'></a>
## SkiaImageEffects `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Brightness-System-Single-'></a>
### Brightness(amount) `method`

##### Summary

This effect increases the brightness of an image. amount is between 0 (no change) and 1 (white).

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| amount | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Contrast-System-Single-'></a>
### Contrast(amount) `method`

##### Summary

This effect adjusts the contrast of an image. amount is the adjustment level. Negative values decrease contrast, positive values increase contrast, and 0 means no change.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| amount | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Gamma-System-Single-'></a>
### Gamma(gamma) `method`

##### Summary

This effect applies gamma correction to an image. gamma must be greater than 0. A .

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| gamma | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

##### Exceptions

| Name | Description |
| ---- | ----------- |
| [System.ArgumentOutOfRangeException](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Grayscale'></a>
### Grayscale() `method`

##### Summary

This effect turns an image to grayscale. This particular version uses the NTSC/PAL/SECAM standard luminance value weights: 0.2989 for red, 0.587 for green, and 0.114 for blue.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Grayscale2'></a>
### Grayscale2() `method`

##### Summary

This effect turns an image to grayscale.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-InvertColors'></a>
### InvertColors() `method`

##### Summary

This effect inverts the colors in an image. NOT WORKING!

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Lightness-System-Single-'></a>
### Lightness(amount) `method`

##### Summary

Adjusts the brightness of an image:

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| amount | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Saturation-System-Single-'></a>
### Saturation(amount) `method`

##### Summary

This effect adjusts the saturation of an image. amount is the adjustment level. Negative values desaturate the image, positive values increase saturation, and 0 means no change.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| amount | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Sepia'></a>
### Sepia() `method`

##### Summary

The sepia effect can give your photos a warm, brownish tone that mimics the look of an older photo.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaImageEffects-Tint-Microsoft-Maui-Graphics-Color,SkiaSharp-SKBlendMode-'></a>
### Tint(color,mode) `method`

##### Summary

If you want to Tint: SKBlendMode.SrcATop + ColorTint with alpha below 1

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| color | [Microsoft.Maui.Graphics.Color](#T-Microsoft-Maui-Graphics-Color 'Microsoft.Maui.Graphics.Color') |  |
| mode | [SkiaSharp.SKBlendMode](#T-SkiaSharp-SKBlendMode 'SkiaSharp.SKBlendMode') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaImageManager'></a>
## SkiaImageManager `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaImageManager-CacheLongevitySecs'></a>
### CacheLongevitySecs `constants`

##### Summary

Caching provider setting

<a name='F-DrawnUi-Maui-Draw-SkiaImageManager-NativeFilePrefix'></a>
### NativeFilePrefix `constants`

##### Summary

Convention for local files saved in native platform. Shared resources from Resources/Raw/ do not need this prefix.

<a name='F-DrawnUi-Maui-Draw-SkiaImageManager-ReuseBitmaps'></a>
### ReuseBitmaps `constants`

##### Summary

If set to true will not return clones for same sources, but will just return the existing cached SKBitmap reference. Useful if you have a lot on images reusing same sources, but you have to be carefull not to dispose the shared image. SkiaImage is aware of this setting and will keep a cached SKBitmap from being disposed.

<a name='M-DrawnUi-Maui-Draw-SkiaImageManager-AddToCache-System-String,SkiaSharp-SKBitmap,System-Int32-'></a>
### AddToCache(uri,bitmap,cacheLongevityMinutes) `method`

##### Summary

Returns false if key already exists

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| uri | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| bitmap | [SkiaSharp.SKBitmap](#T-SkiaSharp-SKBitmap 'SkiaSharp.SKBitmap') |  |
| cacheLongevityMinutes | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImageManager-LoadImageAsync-Microsoft-Maui-Controls-ImageSource,System-Threading-CancellationToken-'></a>
### LoadImageAsync(source,token) `method`

##### Summary

Direct load, without any queue or manager cache, for internal use. Please use LoadImageManagedAsync instead.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| source | [Microsoft.Maui.Controls.ImageSource](#T-Microsoft-Maui-Controls-ImageSource 'Microsoft.Maui.Controls.ImageSource') |  |
| token | [System.Threading.CancellationToken](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Threading.CancellationToken 'System.Threading.CancellationToken') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaImageManager-LoadImageManagedAsync-Microsoft-Maui-Controls-ImageSource,System-Threading-CancellationTokenSource,DrawnUi-Maui-Draw-LoadPriority-'></a>
### LoadImageManagedAsync(source,token) `method`

##### Summary

Uses queue and manager cache

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| source | [Microsoft.Maui.Controls.ImageSource](#T-Microsoft-Maui-Controls-ImageSource 'Microsoft.Maui.Controls.ImageSource') |  |
| token | [System.Threading.CancellationTokenSource](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Threading.CancellationTokenSource 'System.Threading.CancellationTokenSource') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaImageTiles'></a>
## SkiaImageTiles `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaImageTiles-DrawTiles'></a>
### DrawTiles `property`

##### Summary

Whether tiles are setup for rendering

<a name='P-DrawnUi-Maui-Draw-SkiaImageTiles-Tile'></a>
### Tile `property`

##### Summary

Cached image that will be used as tile

<a name='P-DrawnUi-Maui-Draw-SkiaImageTiles-TileAspect'></a>
### TileAspect `property`

##### Summary

Apspect to render image with, default is AspectFitFill.

<a name='M-DrawnUi-Maui-Draw-SkiaImageTiles-OnSourceSuccess'></a>
### OnSourceSuccess() `method`

##### Summary

Source was loaded, we can create tile

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaLabel'></a>
## SkiaLabel `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-CharacterSpacing'></a>
### CharacterSpacing `property`

##### Summary

This applies ONLY when CharByChar is enabled

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-DropShadowOffsetX'></a>
### DropShadowOffsetX `property`

##### Summary

To make DropShadow act like shadow

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-FallbackCharacter'></a>
### FallbackCharacter `property`

##### Summary

Character to show when glyph is not found in font

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-Font'></a>
### Font `property`

##### Summary

TODO IText?

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-LineHeightUniform'></a>
### LineHeightUniform `property`

##### Summary

Should we draw with the maximum line height when lines have different height.

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-LineHeightWithSpacing'></a>
### LineHeightWithSpacing `property`

##### Summary

todo move this to some font info data block
otherwise we wont be able to have multiple fonts

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-MonoForDigits'></a>
### MonoForDigits `property`

##### Summary

The character to be taken for its width when want digits to simulate Mono, for example "8", default is null.

<a name='P-DrawnUi-Maui-Draw-SkiaLabel-SpaceBetweenParagraphs'></a>
### SpaceBetweenParagraphs `property`

##### Summary

todo move this to some font info data block
otherwise we wont be able to have multiple fonts

<a name='M-DrawnUi-Maui-Draw-SkiaLabel-AddEmptyLine-System-Collections-Generic-List{DrawnUi-Maui-Draw-TextLine},DrawnUi-Maui-Draw-TextSpan,System-Single,System-Single,System-Boolean,System-Boolean-'></a>
### AddEmptyLine(result,span,totalHeight,heightBlock,isNewParagraph,needsShaping) `method`

##### Summary

Returns new totalHeight

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| result | [System.Collections.Generic.List{DrawnUi.Maui.Draw.TextLine}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{DrawnUi.Maui.Draw.TextLine}') |  |
| span | [DrawnUi.Maui.Draw.TextSpan](#T-DrawnUi-Maui-Draw-TextSpan 'DrawnUi.Maui.Draw.TextSpan') |  |
| totalHeight | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| heightBlock | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| isNewParagraph | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |
| needsShaping | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLabel-DrawCharacter-SkiaSharp-SKCanvas,System-Int32,System-Int32,System-String,System-Single,System-Single,SkiaSharp-SKPaint,SkiaSharp-SKPaint,SkiaSharp-SKPaint,SkiaSharp-SKRect,System-Single-'></a>
### DrawCharacter(canvas,lineIndex,letterIndex,text,x,y,paint,paintStroke,scale) `method`

##### Summary

This is called when CharByChar is enabled
You can override it to apply custom effects to every letter		///

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| canvas | [SkiaSharp.SKCanvas](#T-SkiaSharp-SKCanvas 'SkiaSharp.SKCanvas') |  |
| lineIndex | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| letterIndex | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| text | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| paint | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |
| paintStroke | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |
| scale | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLabel-DrawText-SkiaSharp-SKCanvas,System-Single,System-Single,System-String,SkiaSharp-SKPaint,SkiaSharp-SKPaint,SkiaSharp-SKPaint,System-Single-'></a>
### DrawText(canvas,x,y,text,textPaint,strokePaint,scale) `method`

##### Summary

If strokePaint==null will not stroke

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| canvas | [SkiaSharp.SKCanvas](#T-SkiaSharp-SKCanvas 'SkiaSharp.SKCanvas') |  |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| text | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| textPaint | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |
| strokePaint | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |
| scale | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLabel-MeasureText-SkiaSharp-SKPaint,System-String,SkiaSharp-SKRect@-'></a>
### MeasureText(paint,text,bounds) `method`

##### Summary

Accounts paint transforms like skew etc

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| paint | [SkiaSharp.SKPaint](#T-SkiaSharp-SKPaint 'SkiaSharp.SKPaint') |  |
| text | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| bounds | [SkiaSharp.SKRect@](#T-SkiaSharp-SKRect@ 'SkiaSharp.SKRect@') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLabel-OnFontUpdated'></a>
### OnFontUpdated() `method`

##### Summary

A new TypeFace was set

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaLabel-OnSpanTapped-DrawnUi-Maui-Draw-TextSpan-'></a>
### OnSpanTapped(span) `method`

##### Summary

Return null if you wish not to consume

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| span | [DrawnUi.Maui.Draw.TextSpan](#T-DrawnUi-Maui-Draw-TextSpan 'DrawnUi.Maui.Draw.TextSpan') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaLayout'></a>
## SkiaLayout `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-DefaultColumnDefinition'></a>
### DefaultColumnDefinition `property`

##### Summary

Will use this to create a missing but required ColumnDefinition

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-DefaultRowDefinition'></a>
### DefaultRowDefinition `property`

##### Summary

Will use this to create a missing but required RowDefinition

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-DynamicColumns'></a>
### DynamicColumns `property`

##### Summary

If true, will not create additional columns to match SplitMax if there are less real columns, and take additional space for drawing

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-InitializeTemplatesInBackgroundDelay'></a>
### InitializeTemplatesInBackgroundDelay `property`

##### Summary

Whether should initialize templates in background instead of blocking UI thread, default is 0.
Set your delay in Milliseconds to enable.
When this is enabled and RecyclingTemplate is Disabled will also measure the layout in background
when templates are available without blocking UI-tread.
After that OnTemplatesAvailable will be called on parent layout.

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-IsStack'></a>
### IsStack `property`

##### Summary

Column/Row/Stack

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-ItemTemplatePoolSize'></a>
### ItemTemplatePoolSize `property`

##### Summary

Default is -1, the number od template instances will not be less than data collection count. You can manually set to to a specific number to fill your viewport etc. Beware that if you set this to a number that will not be enough to fill the viewport binding contexts will contasntly be changing triggering screen update.

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-RecyclingTemplate'></a>
### RecyclingTemplate `property`

##### Summary

In case of ItemsSource+ItemTemplate set will define should we reuse already created views: hidden items views will be reused for currently visible items on screen.
If set to true inside a SkiaScrollLooped will cause it to redraw constantly even when idle because of the looped scroll mechanics.

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-Split'></a>
### Split `property`

##### Summary

For Wrap number of columns/rows to split into, If 0 will use auto, if 1+ will have 1+ columns.

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-SplitAlign'></a>
### SplitAlign `property`

##### Summary

Whether should keep same column width among rows

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-SplitSpace'></a>
### SplitSpace `property`

##### Summary

How to distribute free space between children TODO

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-StackStructure'></a>
### StackStructure `property`

##### Summary

Used for StackLayout (Stack, Row) kind of layout

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-StackStructureMeasured'></a>
### StackStructureMeasured `property`

##### Summary

When measuring we set this, and it will be swapped with StackStructure upon drawing so we don't affect the drawing if measuring in background.

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-TemplatedFooter'></a>
### TemplatedFooter `property`

##### Summary

Kind of BindableLayout.DrawnTemplate

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-TemplatedHeader'></a>
### TemplatedHeader `property`

##### Summary

Kind of BindableLayout.DrawnTemplate

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-Virtualisation'></a>
### Virtualisation `property`

##### Summary

Default is Enabled, children get the visible viewport area for rendering and can virtualize.

<a name='P-DrawnUi-Maui-Draw-SkiaLayout-VirtualisationInflated'></a>
### VirtualisationInflated `property`

##### Summary

How much of the hidden content out of visible bounds should be considered visible for rendering,
default is 0 pts.
Basically how much should be expand in every direction of the visible area prior to checking if content falls
into its bounds for rendering controlled with Virtualisation.

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-DrawChildrenGrid-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single-'></a>
### DrawChildrenGrid(context,destination,scale) `method`

##### Summary

Returns number of drawn children

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-DrawStack-DrawnUi-Maui-Draw-LayoutStructure,DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect,System-Single-'></a>
### DrawStack() `method`

##### Summary

Renders stack/wrap layout.
Returns number of drawn children.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-Measure-System-Single,System-Single,System-Single-'></a>
### Measure(widthConstraint,heightConstraint,scale) `method`

##### Summary

If you call this while measurement is in process (IsMeasuring==True) will return last measured value.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| widthConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| heightConstraint | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-MeasureStack-SkiaSharp-SKRect,System-Single-'></a>
### MeasureStack(rectForChildrenPixels,scale) `method`

##### Summary

Measuring column/row

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| rectForChildrenPixels | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-MeasureWrap-SkiaSharp-SKRect,System-Single-'></a>
### MeasureWrap(rectForChildrenPixels,scale) `method`

##### Summary

TODO for templated measure only visible?! and just reserve predicted scroll amount for scrolling

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| rectForChildrenPixels | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| scale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-OnTemplatesAvailable'></a>
### OnTemplatesAvailable() `method`

##### Summary

Will be called by views adapter upot succsessfull execution of InitializeTemplates.
When using InitializeTemplatesInBackground this is your callbacl to wait for.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaLayout-SetupRenderingWithComposition-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect-'></a>
### SetupRenderingWithComposition(ctx,destination) `method`

##### Summary

Find intersections between changed children and DrawingRect,
add intersecting ones to DirtyChildrenInternal and set IsRenderingWithComposition = true if any.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ctx | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaLottie'></a>
## SkiaLottie `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='F-DrawnUi-Maui-Controls-SkiaLottie-CachedAnimations'></a>
### CachedAnimations `constants`

##### Summary

To avoid reloading same files multiple times..

<a name='P-DrawnUi-Maui-Controls-SkiaLottie-DefaultFrameWhenOn'></a>
### DefaultFrameWhenOn `property`

##### Summary

For the case IsOn = True. What frame should we display at start or when stopped. 0 (START) is default, can specify other number. if value is less than 0 then will seek to the last available frame (END).

<a name='M-DrawnUi-Maui-Controls-SkiaLottie-LoadAnimationFromJson-System-String-'></a>
### LoadAnimationFromJson(json) `method`

##### Summary

This is not replacing current animation, use SetAnimation for that.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| json | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaLottie-OnJsonLoaded-System-String-'></a>
### OnJsonLoaded() `method`

##### Summary

Called by LoadAnimationFromResources after file was loaded so we can modify json if needed before it it consumed.
    Return json to be used.
    This is not called by LoadAnimationFromJson.

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaMarkdownLabel'></a>
## SkiaMarkdownLabel `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Will internally create spans from markdown.
Spans property must not be set directly.

<a name='M-DrawnUi-Maui-Draw-SkiaMarkdownLabel-OnSpanTapped-DrawnUi-Maui-Draw-TextSpan-'></a>
### OnSpanTapped(span) `method`

##### Summary

Url will be inside Tag

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| span | [DrawnUi.Maui.Draw.TextSpan](#T-DrawnUi-Maui-Draw-TextSpan 'DrawnUi.Maui.Draw.TextSpan') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaMarkdownLabel-ProcessSpanData-System-Collections-Generic-List{System-ValueTuple{System-String,SkiaSharp-SKTypeface,System-Int32,System-Boolean}}@,SkiaSharp-SKTypeface-'></a>
### ProcessSpanData(spanData,originalTypeFace) `method`

##### Summary

Do not let spans with non-default typeface end with standart symbols like ' ', move them to span with original typecase

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| spanData | [System.Collections.Generic.List{System.ValueTuple{System.String,SkiaSharp.SKTypeface,System.Int32,System.Boolean}}@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{System.ValueTuple{System.String,SkiaSharp.SKTypeface,System.Int32,System.Boolean}}@') |  |
| originalTypeFace | [SkiaSharp.SKTypeface](#T-SkiaSharp-SKTypeface 'SkiaSharp.SKTypeface') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaMauiEditor'></a>
## SkiaMauiEditor `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-SkiaMauiEditor-LockFocus'></a>
### LockFocus `property`

##### Summary

TODO

<a name='P-DrawnUi-Maui-Controls-SkiaMauiEditor-MaxLines'></a>
### MaxLines `property`

##### Summary

WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor

<a name='M-DrawnUi-Maui-Controls-SkiaMauiEditor-OnControlFocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs-'></a>
### OnControlFocused(sender,e) `method`

##### Summary

Invoked by Maui control

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| sender | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |
| e | [Microsoft.Maui.Controls.FocusEventArgs](#T-Microsoft-Maui-Controls-FocusEventArgs 'Microsoft.Maui.Controls.FocusEventArgs') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaMauiEditor-OnControlUnfocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs-'></a>
### OnControlUnfocused(sender,e) `method`

##### Summary

Invoked by Maui control

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| sender | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |
| e | [Microsoft.Maui.Controls.FocusEventArgs](#T-Microsoft-Maui-Controls-FocusEventArgs 'Microsoft.Maui.Controls.FocusEventArgs') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaMauiEditor-OnFocusChanged-System-Boolean-'></a>
### OnFocusChanged(focus) `method`

##### Summary

Called by DrawnUi when the focus changes

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| focus | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaMauiElement'></a>
## SkiaMauiElement `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaMauiElement-AnimateSnapshot'></a>
### AnimateSnapshot `property`

##### Summary

Set to true if you are hosting the control inside a scroll or similar case
where the control position/transforms are animated fast.

<a name='P-DrawnUi-Maui-Draw-SkiaMauiElement-Element'></a>
### Element `property`

##### Summary

Maui Element to be rendered

<a name='P-DrawnUi-Maui-Draw-SkiaMauiElement-ElementSize'></a>
### ElementSize `property`

##### Summary

PIXELS, for faster checks

<a name='M-DrawnUi-Maui-Draw-SkiaMauiElement-GetVisualChildren'></a>
### GetVisualChildren() `method`

##### Summary

For HotReload

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaMauiElement-MeasureAndArrangeMauiElement-System-Double,System-Double-'></a>
### MeasureAndArrangeMauiElement(ptsWidth,ptsHeight) `method`

##### Summary

Measure and arrange VisualElement using Maui methods

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ptsWidth | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |
| ptsHeight | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaMauiElement-OnChildAdded-DrawnUi-Maui-Draw-SkiaControl-'></a>
### OnChildAdded() `method`

##### Summary

Prevent usage of subviews as we are using Content property for this control

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaMauiElement-SetChildren-System-Collections-Generic-IEnumerable{DrawnUi-Maui-Draw-SkiaControl}-'></a>
### SetChildren(views) `method`

##### Summary

Prevent usage of subviews as we are using Content property for this control

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| views | [System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{DrawnUi.Maui.Draw.SkiaControl}') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaMauiElement-SetContent-Microsoft-Maui-Controls-VisualElement-'></a>
### SetContent() `method`

##### Summary

Use Content property for direct access

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaMauiElement-SetNativeVisibility-System-Boolean-'></a>
### SetNativeVisibility(state) `method`

##### Summary

This is mainly ued by show/hide to display Snapshot instead the native view

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| state | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaMauiEntry'></a>
## SkiaMauiEntry `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Used to draw maui element over a skia canvas. Positions elelement using drawnUi layout and sometimes just renders element bitmap snapshot instead of displaying the real element, for example, when scrolling/animating.

<a name='P-DrawnUi-Maui-Controls-SkiaMauiEntry-LockFocus'></a>
### LockFocus `property`

##### Summary

TODO

<a name='P-DrawnUi-Maui-Controls-SkiaMauiEntry-MaxLines'></a>
### MaxLines `property`

##### Summary

WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor

<a name='M-DrawnUi-Maui-Controls-SkiaMauiEntry-OnControlFocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs-'></a>
### OnControlFocused(sender,e) `method`

##### Summary

Invoked by Maui control

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| sender | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |
| e | [Microsoft.Maui.Controls.FocusEventArgs](#T-Microsoft-Maui-Controls-FocusEventArgs 'Microsoft.Maui.Controls.FocusEventArgs') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaMauiEntry-OnControlUnfocused-System-Object,Microsoft-Maui-Controls-FocusEventArgs-'></a>
### OnControlUnfocused(sender,e) `method`

##### Summary

Invoked by Maui control

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| sender | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |
| e | [Microsoft.Maui.Controls.FocusEventArgs](#T-Microsoft-Maui-Controls-FocusEventArgs 'Microsoft.Maui.Controls.FocusEventArgs') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaMauiEntry-OnFocusChanged-System-Boolean-'></a>
### OnFocusChanged(focus) `method`

##### Summary

Called by DrawnUi when the focus changes

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| focus | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaRadioButton'></a>
## SkiaRadioButton `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Switch-like control, can include any content inside. It's aither you use default content (todo templates?..)
or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag \`Frame\`, SkiaShape with Tag \`Thumb\`. At the same time you can override ApplyProperties() and apply them to your content yourself.

<a name='P-DrawnUi-Maui-Controls-SkiaRadioButton-Text'></a>
### Text `property`

##### Summary

Bind to your own content!

<a name='T-DrawnUi-Maui-Draw-SkiaScroll'></a>
## SkiaScroll `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-BouncesProperty'></a>
### BouncesProperty `constants`

##### Summary

ToDo adapt this to same logic as ScrollLooped has !

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| force | [F:DrawnUi.Maui.Draw.SkiaScroll.BouncesProperty](#T-F-DrawnUi-Maui-Draw-SkiaScroll-BouncesProperty 'F:DrawnUi.Maui.Draw.SkiaScroll.BouncesProperty') |  |

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-InterpolationFactor'></a>
### InterpolationFactor `constants`

##### Summary

panning interpolation to avoid trembling finlgers

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-OrderedScrollTo'></a>
### OrderedScrollTo `constants`

##### Summary

We might order a scroll before the control was drawn, so it's a kind of startup position
saved every time one calls ScrollTo

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-OrderedScrollToIndex'></a>
### OrderedScrollToIndex `constants`

##### Summary

We might order a scroll before the control was drawn, so it's a kind of startup position
saved every time one calls ScrollToIndex

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-ScrollVelocityThreshold'></a>
### ScrollVelocityThreshold `constants`

##### Summary

To filter micro-gestures while manually panning

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-SystemAnimationTimeSecs'></a>
### SystemAnimationTimeSecs `constants`

##### Summary

Time for the snapping animations as well as the scroll to top etc animations..

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-ThesholdSwipeOnUp'></a>
### ThesholdSwipeOnUp `constants`

##### Summary

Min velocity in points/sec to flee/swipe when finger is up

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-_animatorFlingX'></a>
### _animatorFlingX `constants`

##### Summary

Fling with deceleration

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-_animatorFlingY'></a>
### _animatorFlingY `constants`

##### Summary

Fling with deceleration

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-_scrollMinX'></a>
### _scrollMinX `constants`

##### Summary

Units

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-_scrollMinY'></a>
### _scrollMinY `constants`

##### Summary

Units

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-_scrollerX'></a>
### _scrollerX `constants`

##### Summary

Direct scroller for ordered scroll, snap etc

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-_scrollerY'></a>
### _scrollerY `constants`

##### Summary

Direct scroller for ordered scroll, snap etc

<a name='F-DrawnUi-Maui-Draw-SkiaScroll-snapMinimumVelocity'></a>
### snapMinimumVelocity `constants`

##### Summary

POINTS per sec

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-AutoScrollingSpeedMs'></a>
### AutoScrollingSpeedMs `property`

##### Summary

For snap and ordered scrolling

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-Bounces'></a>
### Bounces `property`

##### Summary

Should the scroll bounce at edges. Set to false if you want this scroll to let the parent SkiaDrawer respond to scroll when the child scroll reached bounds.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-CanScrollUsingHeader'></a>
### CanScrollUsingHeader `property`

##### Summary

If disabled will not scroll using gestures. Scrolling will still be possible by code.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-ChangeDistancePanned'></a>
### ChangeDistancePanned `property`

##### Summary

For when the finger is down and panning

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-ChangeVelocityScrolled'></a>
### ChangeVelocityScrolled `property`

##### Summary

For when the finger is up and swipe is detected

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-ContentOffsetBounds'></a>
### ContentOffsetBounds `property`

##### Summary

There are the bounds the scroll offset can go to.. This is NOT the bounds for the whole content.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-ContentRectWithOffset'></a>
### ContentRectWithOffset `property`

##### Summary

The viewport for content

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-FrictionScrolled'></a>
### FrictionScrolled `property`

##### Summary

Use this to control how fast the scroll will decelerate. Values 0.1 - 0.9 are the best, default is 0.3. Usually you would set higher friction for ScrollView-like scrolls and much lower for CollectionView-like scrolls (0.1 or 0.2).

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-HeaderSticky'></a>
### HeaderSticky `property`

##### Summary

Should the header stay in place when content is scrolling

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-IgnoreWrongDirection'></a>
### IgnoreWrongDirection `property`

##### Summary

Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity. Default is False.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-InternalViewportOffset'></a>
### InternalViewportOffset `property`

##### Summary

This is where the view port is actually is after being scrolled. We used this value to offset viewport on drawing the last frame

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-MaxBounceVelocity'></a>
### MaxBounceVelocity `property`

##### Summary

Limit bounce velocity

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-MaxVelocity'></a>
### MaxVelocity `property`

##### Summary

Limit user input velocity

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-Orientation'></a>
### Orientation `property`

##### Summary



<a name='P-DrawnUi-Maui-Draw-SkiaScroll-OverscrollDistance'></a>
### OverscrollDistance `property`

##### Summary

Units

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-RefreshDistanceLimit'></a>
### RefreshDistanceLimit `property`

##### Summary

Applyed to RefreshView

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-RefreshShowDistance'></a>
### RefreshShowDistance `property`

##### Summary

Applyed to RefreshView

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-RespondsToGestures'></a>
### RespondsToGestures `property`

##### Summary

If disabled will not scroll using gestures. Scrolling will still be possible by code.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-RubberDamping'></a>
### RubberDamping `property`

##### Summary

If Bounce is enabled this basically controls how less the scroll will bounce when displaced from limit by finger or inertia. Default is 0.55.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-RubberEffect'></a>
### RubberEffect `property`

##### Summary

If Bounce is enabled this basically controls how far from the limit can the scroll be elastically offset by finger or inertia. Default is 0.55.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-ScrollType'></a>
### ScrollType `property`

##### Summary



<a name='P-DrawnUi-Maui-Draw-SkiaScroll-ScrollingSpeedMs'></a>
### ScrollingSpeedMs `property`

##### Summary

Used by range scroller (ScrollToX, ScrollToY)

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-SnapToChildren'></a>
### SnapToChildren `property`

##### Summary

Whether should snap to children after scrolling stopped

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-TrackIndexPosition'></a>
### TrackIndexPosition `property`

##### Summary

The position in viewport you want to track for content layout child index

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-VelocityImageLoaderLock'></a>
### VelocityImageLoaderLock `property`

##### Summary

Range at which the image loader will stop or resume loading images while scrolling

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-Virtualisation'></a>
### Virtualisation `property`

##### Summary

Default is true, children get the visible viewport area for rendering and can virtualize.
If set to false children get the full content area for rendering and draw all at once.

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-WasSwiping'></a>
### WasSwiping `property`

##### Summary

Had no panning just down+up with velocity more than threshold

<a name='P-DrawnUi-Maui-Draw-SkiaScroll-ZoomScaleInternal'></a>
### ZoomScaleInternal `property`

##### Summary

We might have difference between pinch scale and manually set zoom.

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-CalculateScrollOffsetForIndex-System-Int32,DrawnUi-Maui-Draw-RelativePositionType-'></a>
### CalculateScrollOffsetForIndex(index,option) `method`

##### Summary

ToDo this actually work only for Stack and Row

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| index | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| option | [DrawnUi.Maui.Draw.RelativePositionType](#T-DrawnUi-Maui-Draw-RelativePositionType 'DrawnUi.Maui.Draw.RelativePositionType') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-CalculateVisibleIndex-DrawnUi-Maui-Draw-RelativePositionType-'></a>
### CalculateVisibleIndex() `method`

##### Summary

Calculates CurrentIndex

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-ClampOffsetWithRubberBand-System-Single,System-Single-'></a>
### ClampOffsetWithRubberBand(x,y) `method`

##### Summary

Used to clamp while panning while finger is down

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-GetClosestSidePoint-SkiaSharp-SKPoint,SkiaSharp-SKRect,SkiaSharp-SKSize-'></a>
### GetClosestSidePoint(overscrollPoint,contentRect,viewportSize) `method`

##### Summary

This uses whole viewport size, do not use this for snapping

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| overscrollPoint | [SkiaSharp.SKPoint](#T-SkiaSharp-SKPoint 'SkiaSharp.SKPoint') |  |
| contentRect | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| viewportSize | [SkiaSharp.SKSize](#T-SkiaSharp-SKSize 'SkiaSharp.SKSize') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-GetContentAvailableRect-SkiaSharp-SKRect-'></a>
### GetContentAvailableRect(destination) `method`

##### Summary

In PIXELS

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-GetContentOffsetBounds'></a>
### GetContentOffsetBounds() `method`

##### Summary

There are the bounds the scroll offset can go to.. This is NOT the bounds for the whole content.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-OnScrolled'></a>
### OnScrolled() `method`

##### Summary

Notify current scroll offset to some dependent views.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-PositionViewport-SkiaSharp-SKRect,SkiaSharp-SKPoint,System-Single,System-Single-'></a>
### PositionViewport(destination,offsetPtsX,offsetPtsY,viewportScale,scale) `method`

##### Summary

Input offset parameters in PIXELS. We render the scroll Content using pixal snapping but the prepared content will be scrolled (offset) using subpixels for a smooth look.
Creates a valid ViewportRect inside.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| offsetPtsX | [SkiaSharp.SKPoint](#T-SkiaSharp-SKPoint 'SkiaSharp.SKPoint') |  |
| offsetPtsY | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| viewportScale | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-ScrollToOffset-System-Numerics-Vector2,System-Single-'></a>
### ScrollToOffset(offset,animate) `method`

##### Summary

In Units

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| offset | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |
| animate | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-ScrollToX-System-Single,System-Boolean-'></a>
### ScrollToX(offset,animate) `method`

##### Summary

Use Range scroller, offset in Units

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| offset | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| animate | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-ScrollToY-System-Single,System-Boolean-'></a>
### ScrollToY(offset,animate) `method`

##### Summary

Use Range scroller, offset in Units

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| offset | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| animate | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaScroll-SetContent-DrawnUi-Maui-Draw-SkiaControl-'></a>
### SetContent(view) `method`

##### Summary

Use Content property for direct access

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaScrollLooped'></a>
## SkiaScrollLooped `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Cycles content, so the scroll never ands but cycles from the start

<a name='P-DrawnUi-Maui-Draw-SkiaScrollLooped-CycleSpace'></a>
### CycleSpace `property`

##### Summary

Space between cycles, pixels

<a name='P-DrawnUi-Maui-Draw-SkiaScrollLooped-IsBanner'></a>
### IsBanner `property`

##### Summary

Whether this should look like an infinite scrolling text banner

<a name='T-DrawnUi-Maui-Draw-SkiaShaderEffect'></a>
## SkiaShaderEffect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaShaderEffect-AutoCreateInputTexture'></a>
### AutoCreateInputTexture `property`

##### Summary

Should create a texture from the current drawing to pass to shader as uniform shader iImage1, default is True. You need this set to False only if your shader is output-only.

<a name='P-DrawnUi-Maui-Draw-SkiaShaderEffect-UseContext'></a>
### UseContext `property`

##### Summary

Use either context of global Superview background, default is True.

<a name='M-DrawnUi-Maui-Draw-SkiaShaderEffect-CreateSnapshot-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect-'></a>
### CreateSnapshot(ctx,destination) `method`

##### Summary

Create snapshot from the current parent control drawing state to use as input texture for the shader

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ctx | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaShaderEffect-Render-DrawnUi-Maui-Draw-SkiaDrawingContext,SkiaSharp-SKRect-'></a>
### Render(ctx,destination) `method`

##### Summary

EffectPostRenderer

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ctx | [DrawnUi.Maui.Draw.SkiaDrawingContext](#T-DrawnUi-Maui-Draw-SkiaDrawingContext 'DrawnUi.Maui.Draw.SkiaDrawingContext') |  |
| destination | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaShape'></a>
## SkiaShape `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Implements ISkiaGestureListener to pass gestures to children

<a name='P-DrawnUi-Maui-Draw-SkiaShape-ClipBackgroundColor'></a>
### ClipBackgroundColor `property`

##### Summary

This is for the tricky case when you want to drop shadow but keep background transparent to see through, set to True in that case.

<a name='P-DrawnUi-Maui-Draw-SkiaShape-DrawPath'></a>
### DrawPath `property`

##### Summary

Parsed PathData

<a name='P-DrawnUi-Maui-Draw-SkiaShape-PathData'></a>
### PathData `property`

##### Summary

For Type = Path, use the path markup syntax

<a name='T-DrawnUi-Maui-Controls-SkiaShell'></a>
## SkiaShell `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

A Canvas with Navigation capabilities

<a name='F-DrawnUi-Maui-Controls-SkiaShell-PopupBackgroundColor'></a>
### PopupBackgroundColor `constants`

##### Summary

Default background tint for freezing popups/modals etc

<a name='F-DrawnUi-Maui-Controls-SkiaShell-PopupsBackgroundBlur'></a>
### PopupsBackgroundBlur `constants`

##### Summary

Default background blur amount for freezing popups/modals etc

<a name='P-DrawnUi-Maui-Controls-SkiaShell-Buffer'></a>
### Buffer `property`

##### Summary

Can use to pass items as models between viewmodels

<a name='P-DrawnUi-Maui-Controls-SkiaShell-FrozenLayers'></a>
### FrozenLayers `property`

##### Summary

TODO make this non-concurrent

<a name='P-DrawnUi-Maui-Controls-SkiaShell-NavigationLayout'></a>
### NavigationLayout `property`

##### Summary

The main control that pushes pages, switches tabs etc

<a name='P-DrawnUi-Maui-Controls-SkiaShell-RootLayout'></a>
### RootLayout `property`

##### Summary

Use this for covering everything in a modal way, precisely tabs

<a name='M-DrawnUi-Maui-Controls-SkiaShell-CanFreezeLayout'></a>
### CanFreezeLayout() `method`

##### Summary

Override this if you have custom navigation layers and custom logic to decide if we can unfreeze background.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaShell-CanUnfreezeLayout'></a>
### CanUnfreezeLayout() `method`

##### Summary

Override this if you have custom navigation layers and custom logic to decide if we can unfreeze background.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaShell-ClosePopupAsync-System-Boolean-'></a>
### ClosePopupAsync(animated) `method`

##### Summary

Close topmost popup

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| animated | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-FreezeRootLayout-DrawnUi-Maui-Draw-SkiaControl,System-Boolean,Microsoft-Maui-Graphics-Color,System-Single-'></a>
### FreezeRootLayout() `method`

##### Summary

Freezes layout below the overlay: takes screenshot of RootLayout, places it over, then hides RootLayout to avoid rendering it. Can override

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaShell-GetTopmostView'></a>
### GetTopmostView() `method`

##### Summary

Gets the topmost visible view:
if no popups and modals are open then return NavigationLayout
otherwise return the topmost popup or modal
depending which ZIndexModals or ZIndexPopups is higher.
If view is inside a shell wrapper will return just the view.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaShell-GetTopmostViewInternal'></a>
### GetTopmostViewInternal() `method`

##### Summary

Gets the topmost visible view:
if no popups and modals are open then return NavigationLayout
otherwise return the topmost popup or modal
depending which ZIndexModals or ZIndexPopups is higher.
If pushed view is inside a shell wrapper will return the wrapper.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaShell-GoBackInRoute-System-Boolean-'></a>
### GoBackInRoute(animate) `method`

##### Summary

This will not affect popups

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| animate | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-GoToAsync-Microsoft-Maui-Controls-ShellNavigationState,System-Nullable{System-Boolean},System-Collections-Generic-IDictionary{System-String,System-Object}-'></a>
### GoToAsync(state,animate,arguments) `method`

##### Summary

Navigate to a registered route. Arguments will be taken from query string of can be passed as parameter. You can receive them by implementing IQueryAttributable or using attribute [QueryProperty] in the page itsself or in the ViewModel that must be the screen's BindingContext upon registered screen instatiation.
Animate can be specified otherwise will use it from Shell.PresentationMode attached property. This property will be also used for pushing as page, modal etc.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| state | [Microsoft.Maui.Controls.ShellNavigationState](#T-Microsoft-Maui-Controls-ShellNavigationState 'Microsoft.Maui.Controls.ShellNavigationState') |  |
| animate | [System.Nullable{System.Boolean}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Nullable 'System.Nullable{System.Boolean}') |  |
| arguments | [System.Collections.Generic.IDictionary{System.String,System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IDictionary 'System.Collections.Generic.IDictionary{System.String,System.Object}') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-InitializeNative-Microsoft-Maui-IViewHandler-'></a>
### InitializeNative(handler) `method`

##### Summary

Fail to understand why destroy the splash background after app launched,
not letting us animate app content to fade in after splash screen.
All this code to avoid a blink between splash screen and mainpage showing.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| handler | [Microsoft.Maui.IViewHandler](#T-Microsoft-Maui-IViewHandler 'Microsoft.Maui.IViewHandler') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-OnLayersChanged-DrawnUi-Maui-Draw-SkiaControl-'></a>
### OnLayersChanged() `method`

##### Summary

Setup _topmost and send OnAppeared / OnDisappeared to views.
Occurs when layers configuration changes,
some layer go visible, some not, some are added, some are removed.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaShell-OpenPopupAsync-DrawnUi-Maui-Draw-SkiaControl,System-Boolean,System-Boolean,System-Boolean,Microsoft-Maui-Graphics-Color,System-Nullable{SkiaSharp-SKPoint}-'></a>
### OpenPopupAsync(content,animated,closeWhenBackgroundTapped,scaleInFrom) `method`

##### Summary

Pass pixelsScaleInFrom you you want popup to animate appearing from a specific point instead of screen center.
Set freezeBackground to False to keep animations running below popup, default is True for performance reasons.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| content | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| animated | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |
| closeWhenBackgroundTapped | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |
| scaleInFrom | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-PopAsync-System-Boolean-'></a>
### PopAsync(animated) `method`

##### Summary

Returns the page so you could dispose it if needed. Uses ViewSwitcher.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| animated | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-PushAsync-Microsoft-Maui-Controls-BindableObject,System-Boolean,System-Boolean-'></a>
### PushAsync(page,animated) `method`

##### Summary

Uses ViewSwitcher to push a view on the canvas, into the current tab if any.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| page | [Microsoft.Maui.Controls.BindableObject](#T-Microsoft-Maui-Controls-BindableObject 'Microsoft.Maui.Controls.BindableObject') |  |
| animated | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-PushAsync-System-String,System-Boolean,System-Collections-Generic-IDictionary{System-String,System-Object}-'></a>
### PushAsync(page,animated) `method`

##### Summary

Uses ViewSwitcher to push a view on the canvas, into the current tab if any. We can use a route with arguments to instantiate the view instead of passing an instance.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| page | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| animated | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-PushModalAsync-Microsoft-Maui-Controls-BindableObject,System-Boolean,System-Boolean,System-Boolean,System-Collections-Generic-IDictionary{System-String,System-Object}-'></a>
### PushModalAsync(page,animated) `method`

##### Summary

Creates a SkiaDrawer opening over the RootLayout with the passed content. Override this method to create your own implementation.
Default freezing background is True, control with frozenLayerBackgroundParameters parameter.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| page | [Microsoft.Maui.Controls.BindableObject](#T-Microsoft-Maui-Controls-BindableObject 'Microsoft.Maui.Controls.BindableObject') |  |
| animated | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-SetFrozenLayerVisibility-DrawnUi-Maui-Draw-SkiaControl,System-Boolean-'></a>
### SetFrozenLayerVisibility(control,parameters) `method`

##### Summary

Display or hide the background scrrenshot assotiated with an overlay control

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| parameters | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-SetRoot-System-String,System-Boolean,System-Collections-Generic-IDictionary{System-String,System-Object}-'></a>
### SetRoot(host,replace,arguments) `method`

##### Summary

Returns true if was replaced

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| host | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |
| replace | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |
| arguments | [System.Collections.Generic.IDictionary{System.String,System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IDictionary 'System.Collections.Generic.IDictionary{System.String,System.Object}') |  |

##### Exceptions

| Name | Description |
| ---- | ----------- |
| [System.Exception](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Exception 'System.Exception') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-SetupRoot-DrawnUi-Maui-Draw-ISkiaControl-'></a>
### SetupRoot(shellLayout) `method`

##### Summary

Main control inside RootLayout

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| shellLayout | [DrawnUi.Maui.Draw.ISkiaControl](#T-DrawnUi-Maui-Draw-ISkiaControl 'DrawnUi.Maui.Draw.ISkiaControl') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-UnfreezeRootLayout-DrawnUi-Maui-Draw-SkiaControl,System-Boolean-'></a>
### UnfreezeRootLayout(control,animated) `method`

##### Summary

pass who frozen the layout

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| control | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |
| animated | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaShell-WrapScreenshot-DrawnUi-Maui-Draw-SkiaControl,SkiaSharp-SKImage,Microsoft-Maui-Graphics-Color,System-Single,System-Boolean-'></a>
### WrapScreenshot(screenshot) `method`

##### Summary

Override this to create your own image with your own effect of the screenshot to be placed under modal controls. Default is image with Darken Effect.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| screenshot | [DrawnUi.Maui.Draw.SkiaControl](#T-DrawnUi-Maui-Draw-SkiaControl 'DrawnUi.Maui.Draw.SkiaControl') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaShellNavigatedArgs'></a>
## SkiaShellNavigatedArgs `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-SkiaShellNavigatedArgs-Route'></a>
### Route `property`

##### Summary

Is never null.

<a name='P-DrawnUi-Maui-Controls-SkiaShellNavigatedArgs-View'></a>
### View `property`

##### Summary

The SkiaControl that went upfront

<a name='T-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs'></a>
## SkiaShellNavigatingArgs `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-Cancel'></a>
### Cancel `property`

##### Summary

If you set this to True the navigation will be canceled

<a name='P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-Previous'></a>
### Previous `property`

##### Summary

The SkiaControl that is upfront now

<a name='P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-Route'></a>
### Route `property`

##### Summary

Is never null

<a name='P-DrawnUi-Maui-Controls-SkiaShellNavigatingArgs-View'></a>
### View `property`

##### Summary

The SkiaControl that will navigate

<a name='T-DrawnUi-Maui-Draw-SkiaSlider'></a>
## SkiaSlider `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SkiaSlider-moreHotspotSize'></a>
### moreHotspotSize `constants`

##### Summary

enlarge hotspot by pts

<a name='F-DrawnUi-Maui-Draw-SkiaSlider-touchArea'></a>
### touchArea `constants`

##### Summary

track touched area type

<a name='P-DrawnUi-Maui-Draw-SkiaSlider-End'></a>
### End `property`

##### Summary

For non-ranged this is your main value

<a name='P-DrawnUi-Maui-Draw-SkiaSlider-IgnoreWrongDirection'></a>
### IgnoreWrongDirection `property`

##### Summary

Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity

<a name='P-DrawnUi-Maui-Draw-SkiaSlider-Orientation'></a>
### Orientation `property`

##### Summary



<a name='P-DrawnUi-Maui-Draw-SkiaSlider-RespondsToGestures'></a>
### RespondsToGestures `property`

##### Summary

Can be open/closed by gestures along with code-behind, default is true

<a name='P-DrawnUi-Maui-Draw-SkiaSlider-Start'></a>
### Start `property`

##### Summary

Enabled for ranged

<a name='T-DrawnUi-Maui-Draw-SkiaSvg'></a>
## SkiaSvg `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-SkiaSvg-LoadSource-System-String-'></a>
### LoadSource(fileName) `method`

##### Summary

This is not replacing current animation, use SetAnimation for that.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| fileName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='T-DrawnUi-Maui-Draw-SkiaSwitch'></a>
## SkiaSwitch `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Switch-like control, can include any content inside. It's aither you use default content (todo templates?..)
or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag \`Frame\`, SkiaShape with Tag \`Thumb\`. At the same time you can override ApplyProperties() and apply them to your content yourself.

<a name='T-DrawnUi-Maui-Controls-SkiaTabsSelector'></a>
## SkiaTabsSelector `type`

##### Namespace

DrawnUi.Maui.Controls

<a name='P-DrawnUi-Maui-Controls-SkiaTabsSelector-TabType'></a>
### TabType `property`

##### Summary

Specify the type of the tab to be included, other types will just render and not be treated as tabs. Using this so we can included any elements inside this control to create any design.

<a name='M-DrawnUi-Maui-Controls-SkiaTabsSelector-ApplySelectedIndex-System-Int32-'></a>
### ApplySelectedIndex() `method`

##### Summary

This is called when processing stack of index changes. For example you might have index chnaged 5 times during the time you were executing ApplySelectedIndex (playing the animations etc), so then you just need the lastest index to be applied. At the same time ApplySelectedIndex will not be called again while its already running, this way you would viually apply only the lastest more actual value instead of maybe freezing ui for too many heavy to render changes.

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaToggle'></a>
## SkiaToggle `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Base control for toggling between 2 states

<a name='M-DrawnUi-Maui-Draw-SkiaToggle-ApplyProperties'></a>
### ApplyProperties() `method`

##### Summary

Base call Update()

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SkiaToggle-OnToggledChanged'></a>
### OnToggledChanged() `method`

##### Summary

Base calls ApplyProperties()

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-SkiaValueAnimator'></a>
## SkiaValueAnimator `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SkiaValueAnimator-CycleFInished'></a>
### CycleFInished `property`

##### Summary

Animator self finished a cycle, might still repeat

<a name='P-DrawnUi-Maui-Draw-SkiaValueAnimator-Finished'></a>
### Finished `property`

##### Summary

Animator self finished running without being stopped manually

<a name='P-DrawnUi-Maui-Draw-SkiaValueAnimator-Progress'></a>
### Progress `property`

##### Summary

We are using this internally to apply easing. Can be above 1 when finishing. If you need progress 0-1 use ProgressAnimator.

<a name='P-DrawnUi-Maui-Draw-SkiaValueAnimator-Repeat'></a>
### Repeat `property`

##### Summary

-1 means forever..

<a name='M-DrawnUi-Maui-Draw-SkiaValueAnimator-TransformReportedValue-System-Int64-'></a>
### TransformReportedValue(deltaT) `method`

##### Summary

/// Passed over mValue, you can change the reported passed value here

##### Returns

modified mValue for callback consumer

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| deltaT | [System.Int64](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int64 'System.Int64') |  |

<a name='M-DrawnUi-Maui-Draw-SkiaValueAnimator-UpdateValue-System-Int64,System-Int64-'></a>
### UpdateValue(deltaT) `method`

##### Summary

Update mValue using time distance between rendered frames.
Return true if anims is finished.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| deltaT | [System.Int64](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int64 'System.Int64') |  |

<a name='T-DrawnUi-Maui-Views-SkiaView'></a>
## SkiaView `type`

##### Namespace

DrawnUi.Maui.Views

<a name='M-DrawnUi-Maui-Views-SkiaView-CalculateFPS-System-Int64,System-Int32-'></a>
### CalculateFPS(currentTimestamp,averageAmount) `method`

##### Summary

Calculates the frames per second (FPS) and updates the rolling average FPS every 'averageAmount' frames.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| currentTimestamp | [System.Int64](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int64 'System.Int64') | The current timestamp in nanoseconds. |
| averageAmount | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | The number of frames over which to average the FPS. Default is 10. |

<a name='T-DrawnUi-Maui-Views-SkiaViewAccelerated'></a>
## SkiaViewAccelerated `type`

##### Namespace

DrawnUi.Maui.Views

<a name='M-DrawnUi-Maui-Views-SkiaViewAccelerated-CalculateFPS-System-Int64,System-Int32-'></a>
### CalculateFPS(currentTimestamp,averageAmount) `method`

##### Summary

Calculates the frames per second (FPS) and updates the rolling average FPS every 'averageAmount' frames.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| currentTimestamp | [System.Int64](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int64 'System.Int64') | The current timestamp in nanoseconds. |
| averageAmount | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | The number of frames over which to average the FPS. Default is 10. |

<a name='M-DrawnUi-Maui-Views-SkiaViewAccelerated-OnPaintingSurface-System-Object,SkiaSharp-Views-Maui-SKPaintGLSurfaceEventArgs-'></a>
### OnPaintingSurface(sender,paintArgs) `method`

##### Summary

We are drawing the frame

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| sender | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |
| paintArgs | [SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs](#T-SkiaSharp-Views-Maui-SKPaintGLSurfaceEventArgs 'SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs') |  |

<a name='T-DrawnUi-Maui-Controls-SkiaViewSwitcher'></a>
## SkiaViewSwitcher `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Display and hide views, eventually animating them

<a name='F-DrawnUi-Maui-Controls-SkiaViewSwitcher-NavigationStacks'></a>
### NavigationStacks `constants`

##### Summary

for navigation inside pseudo tabs

<a name='M-DrawnUi-Maui-Controls-SkiaViewSwitcher-GetTopView-System-Int32-'></a>
### GetTopView(selectedIndex) `method`

##### Summary

Get tab view or tab top subview if has navigation stack

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| selectedIndex | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='M-DrawnUi-Maui-Controls-SkiaViewSwitcher-PopTabToRoot'></a>
### PopTabToRoot() `method`

##### Summary

Must be launched on main thread only !!!

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Controls-SkiaViewSwitcher-RevealNavigationView-DrawnUi-Maui-Controls-SkiaViewSwitcher-NavigationStackEntry-'></a>
### RevealNavigationView(newVisibleView) `method`

##### Summary

Set IsVisible, reset transforms and opacity and send OnAppeared event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| newVisibleView | [DrawnUi.Maui.Controls.SkiaViewSwitcher.NavigationStackEntry](#T-DrawnUi-Maui-Controls-SkiaViewSwitcher-NavigationStackEntry 'DrawnUi.Maui.Controls.SkiaViewSwitcher.NavigationStackEntry') |  |

<a name='T-DrawnUi-Maui-Draw-Snapping'></a>
## Snapping `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-Snapping-SnapPointsToPixel-System-Single,System-Single,System-Double-'></a>
### SnapPointsToPixel(initialPosition,translation,scale) `method`

##### Summary

Used by the layout system to round a position translation value applying scale and initial anchor. Pass POINTS only, it wont do its job when receiving pixels!

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| initialPosition | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| translation | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| scale | [System.Double](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Double 'System.Double') |  |

<a name='T-DrawnUi-Maui-Draw-SnappingLayout'></a>
## SnappingLayout `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-AutoVelocityMultiplyPts'></a>
### AutoVelocityMultiplyPts `property`

##### Summary

If velocity is near 0 define how much we multiply the auto-velocity used to animate snappoing point. For example when in carousel you cancel the swipe and release finger..

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-ContentOffsetBounds'></a>
### ContentOffsetBounds `property`

##### Summary

There are the bounds the scroll offset can go to.. This are NOT the bounds of the whole content.

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-IgnoreWrongDirection'></a>
### IgnoreWrongDirection `property`

##### Summary

Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-RespondsToGestures'></a>
### RespondsToGestures `property`

##### Summary

Can be open/closed by gestures along with code-behind, default is true

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-RubberDamping'></a>
### RubberDamping `property`

##### Summary

If Bounce is enabled this basically controls how less the scroll will bounce when displaced from limit by finger or inertia. Default is 0.8.

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-RubberEffect'></a>
### RubberEffect `property`

##### Summary

If Bounce is enabled this basically controls how far from the limit can the scroll be elastically offset by finger or inertia. Default is 0.15.

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-SnapDistanceRatio'></a>
### SnapDistanceRatio `property`

##### Summary

0.2 - Part of the distance between snap points the velocity need to cover to trigger going to the next snap point. NOT a bindable property (yet).

<a name='P-DrawnUi-Maui-Draw-SnappingLayout-Viewport'></a>
### Viewport `property`

##### Summary

Using this instead of RenderingViewport

<a name='M-DrawnUi-Maui-Draw-SnappingLayout-ClampOffsetWithRubberBand-System-Single,System-Single-'></a>
### ClampOffsetWithRubberBand(x,y) `method`

##### Summary

Called for manual finger panning

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| x | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |
| y | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='M-DrawnUi-Maui-Draw-SnappingLayout-GetAutoVelocity-System-Numerics-Vector2-'></a>
### GetAutoVelocity(displacement) `method`

##### Summary

todo calc upon measured size + prop for speed

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| displacement | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |

<a name='M-DrawnUi-Maui-Draw-SnappingLayout-GetContentOffsetBounds'></a>
### GetContentOffsetBounds() `method`

##### Summary

There are the bounds the scroll offset can go to.. This are NOT the bounds of the whole content.

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-SnappingLayout-ScrollToOffset-System-Numerics-Vector2,System-Numerics-Vector2,System-Boolean-'></a>
### ScrollToOffset(offset,animate) `method`

##### Summary

In Units

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| offset | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |
| animate | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |

<a name='M-DrawnUi-Maui-Draw-SnappingLayout-SelectNextAnchor-System-Numerics-Vector2,System-Numerics-Vector2-'></a>
### SelectNextAnchor(origin,velocity) `method`

##### Summary

Return an anchor depending on direction and strength of of the velocity

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| origin | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |
| velocity | [System.Numerics.Vector2](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Numerics.Vector2 'System.Numerics.Vector2') |  |

<a name='T-DrawnUi-Maui-Draw-SpaceDistribution'></a>
## SpaceDistribution `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-SpaceDistribution-Auto'></a>
### Auto `constants`

##### Summary

Distribute space evenly between all items but do not affect empty space remaining at the end of the last line

<a name='F-DrawnUi-Maui-Draw-SpaceDistribution-Full'></a>
### Full `constants`

##### Summary

Distribute space evenly between all items but do not affect empty space

<a name='T-DrawnUi-Maui-Draw-StackLayoutStructure'></a>
## StackLayoutStructure `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='M-DrawnUi-Maui-Draw-StackLayoutStructure-Build-SkiaSharp-SKRect,System-Single-'></a>
### Build() `method`

##### Summary

Will measure children and build appropriate stack structure for the layout

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-Super'></a>
## Super `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-Super-CanUseHardwareAcceleration'></a>
### CanUseHardwareAcceleration `constants`

##### Summary

Can optionally disable hardware-acceleration with this flag, for example on iOS you would want to avoid creating many metal views.

<a name='F-DrawnUi-Maui-Draw-Super-CapMicroSecs'></a>
### CapMicroSecs `constants`

##### Summary

Capping FPS, (1 / FPS * 1000_000)

<a name='F-DrawnUi-Maui-Draw-Super-InsetsChanged'></a>
### InsetsChanged `constants`

##### Summary

Subscribe your navigation bar to react

<a name='F-DrawnUi-Maui-Draw-Super-OnMauiAppCreated'></a>
### OnMauiAppCreated `constants`

##### Summary

Maui App was launched and UI is ready to be consumed

<a name='P-DrawnUi-Maui-Draw-Super-BottomTabsHeight'></a>
### BottomTabsHeight `property`

##### Summary

In DP

<a name='P-DrawnUi-Maui-Draw-Super-FontSubPixelRendering'></a>
### FontSubPixelRendering `property`

##### Summary

Enables sub-pixel font rendering, might provide better antialiasing on some platforms. Default is True;

<a name='P-DrawnUi-Maui-Draw-Super-IsRtl'></a>
### IsRtl `property`

##### Summary

RTL support UNDER CONSTRUCTION

<a name='P-DrawnUi-Maui-Draw-Super-NavBarHeight'></a>
### NavBarHeight `property`

##### Summary

In DP

<a name='P-DrawnUi-Maui-Draw-Super-StatusBarHeight'></a>
### StatusBarHeight `property`

##### Summary

In DP

<a name='M-DrawnUi-Maui-Draw-Super-DisplayException-Microsoft-Maui-Controls-Element,System-Exception-'></a>
### DisplayException(view,e) `method`

##### Summary

Display xaml page creation exception

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [Microsoft.Maui.Controls.Element](#T-Microsoft-Maui-Controls-Element 'Microsoft.Maui.Controls.Element') |  |
| e | [System.Exception](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Exception 'System.Exception') |  |

<a name='M-DrawnUi-Maui-Draw-Super-ListResources-System-String-'></a>
### ListResources(subfolder) `method`

##### Summary

Lists assets inside the Resources/Raw subfolder

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| subfolder | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='M-DrawnUi-Maui-Draw-Super-NeedGlobalUpdate'></a>
### NeedGlobalUpdate() `method`

##### Summary

This will force recalculate canvas visibility in ViewTree and update those visible

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-Super-OpenLink-System-String-'></a>
### OpenLink(link) `method`

##### Summary

Opens web link in native browser

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| link | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='M-DrawnUi-Maui-Draw-Super-ResizeWindow-Microsoft-Maui-Controls-Window,System-Int32,System-Int32,System-Boolean-'></a>
### ResizeWindow(window,width,height,isFixed) `method`

##### Summary

For desktop platforms, will resize app window and eventually lock it from being resized.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| window | [Microsoft.Maui.Controls.Window](#T-Microsoft-Maui-Controls-Window 'Microsoft.Maui.Controls.Window') |  |
| width | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| height | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| isFixed | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') |  |

<a name='M-DrawnUi-Maui-Draw-Super-SetFullScreen-Android-App-Activity-'></a>
### SetFullScreen(activity) `method`

##### Summary

ToDo resolve obsolete for android api 30 and later

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| activity | [Android.App.Activity](#T-Android-App-Activity 'Android.App.Activity') |  |

<a name='T-DrawnUi-Maui-Draw-ViewsAdapter-TemplatedViewsPool'></a>
## TemplatedViewsPool `type`

##### Namespace

DrawnUi.Maui.Draw.ViewsAdapter

##### Summary

Used by ViewsProvider

<a name='M-DrawnUi-Maui-Draw-ViewsAdapter-TemplatedViewsPool-CreateFromTemplate'></a>
### CreateFromTemplate() `method`

##### Summary

unsafe

##### Returns



##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-ViewsAdapter-TemplatedViewsPool-Reserve'></a>
### Reserve() `method`

##### Summary

Just create template and save for the future

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-TextLine'></a>
## TextLine `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-TextLine-Bounds'></a>
### Bounds `property`

##### Summary

Set during rendering

<a name='T-DrawnUi-Maui-Draw-TextSpan'></a>
## TextSpan `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-TextSpan-Rects'></a>
### Rects `constants`

##### Summary

Relative to DrawingRect

<a name='P-DrawnUi-Maui-Draw-TextSpan-AutoFindFont'></a>
### AutoFindFont `property`

##### Summary

If any glyph cannot be rendered with selected font try find system font that supports it and switch to it for the whole span

<a name='P-DrawnUi-Maui-Draw-TextSpan-DrawingOffset'></a>
### DrawingOffset `property`

##### Summary

Rendering offset, set when combining spans. Ofset of the first line.

<a name='P-DrawnUi-Maui-Draw-TextSpan-ForceCaptureInput'></a>
### ForceCaptureInput `property`

##### Summary

When no tap handler or command are set this forces to listen to taps anyway

<a name='P-DrawnUi-Maui-Draw-TextSpan-Glyphs'></a>
### Glyphs `property`

##### Summary

Ig can be drawn char by char with char spacing etc we use this

<a name='P-DrawnUi-Maui-Draw-TextSpan-HasTapHandler'></a>
### HasTapHandler `property`

##### Summary

Will listen to gestures

<a name='P-DrawnUi-Maui-Draw-TextSpan-Shape'></a>
### Shape `property`

##### Summary

If text can be drawn only shaped we use this

<a name='P-DrawnUi-Maui-Draw-TextSpan-StrikeoutWidth'></a>
### StrikeoutWidth `property`

##### Summary

In points

<a name='P-DrawnUi-Maui-Draw-TextSpan-UnderlineWidth'></a>
### UnderlineWidth `property`

##### Summary

In points

<a name='M-DrawnUi-Maui-Draw-TextSpan-CheckGlyphsCanBeRendered'></a>
### CheckGlyphsCanBeRendered() `method`

##### Summary

Parse glyphs, setup typeface, replace unrenderable glyphs with fallback character

##### Parameters

This method has no parameters.

<a name='M-DrawnUi-Maui-Draw-TextSpan-SetupPaint-System-Double,SkiaSharp-SKPaint-'></a>
### SetupPaint() `method`

##### Summary

Update the paint with current format properties

##### Parameters

This method has no parameters.

<a name='T-DrawnUi-Maui-Draw-TransformAspect'></a>
## TransformAspect `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-TransformAspect-AspectCover'></a>
### AspectCover `constants`

##### Summary

Enlarges to cover the viewport or reduces size to fit inside the viewport both respecting aspect ratio. Always covers the entire viewport, potentially cropping the image if it's larger,
never leaves empty space inside viewport.

<a name='F-DrawnUi-Maui-Draw-TransformAspect-AspectFill'></a>
### AspectFill `constants`

##### Summary

Covers viewport respecting aspect without scaling down if bigger, could result in the image being cropped

<a name='F-DrawnUi-Maui-Draw-TransformAspect-AspectFit'></a>
### AspectFit `constants`

##### Summary

Fit inside viewport respecting aspect without enlarging if smaller, could result in the image having some blank space around

<a name='F-DrawnUi-Maui-Draw-TransformAspect-AspectFitFill'></a>
### AspectFitFill `constants`

##### Summary

AspectFit + AspectFill. Enlarges to cover the viewport or reduces size to fit inside the viewport both respecting aspect ratio, ensuring the entire image is always visible, potentially leaving some parts of the viewport uncovered.

<a name='F-DrawnUi-Maui-Draw-TransformAspect-Cover'></a>
### Cover `constants`

##### Summary

Enlarges to cover the viewport if smaller and reduces size if larger, all without respecting aspect ratio. Same as AspectFitFill but will crop the image to fill entire viewport.

<a name='F-DrawnUi-Maui-Draw-TransformAspect-Fill'></a>
### Fill `constants`

##### Summary

Enlarges to fill the viewport without maintaining aspect ratio if smaller, but does not scale down if larger

<a name='F-DrawnUi-Maui-Draw-TransformAspect-Fit'></a>
### Fit `constants`

##### Summary

Fit without maintaining aspect ratio and without enlarging if smaller

<a name='F-DrawnUi-Maui-Draw-TransformAspect-FitFill'></a>
### FitFill `constants`

##### Summary

Fit + Fill. Enlarges to cover the viewport or reduces size to fit inside the viewport without respecting aspect ratio, ensuring the entire image is always visible, potentially leaving some parts of the viewport uncovered.

<a name='F-DrawnUi-Maui-Draw-TransformAspect-Tile'></a>
### Tile `constants`

##### Summary

TODO very soon!

<a name='T-DrawnUi-Maui-Draw-UiSettings'></a>
## UiSettings `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-UiSettings-DesktopWindow'></a>
### DesktopWindow `property`

##### Summary

For desktop: if set will affect the app window at startup.

<a name='P-DrawnUi-Maui-Draw-UiSettings-MobileIsFullscreen'></a>
### MobileIsFullscreen `property`

##### Summary

Avoid safe insets and remove system ui like status bar etc if supported by platform

<a name='P-DrawnUi-Maui-Draw-UiSettings-UseDesktopKeyboard'></a>
### UseDesktopKeyboard `property`

##### Summary

Listen to desktop keyboard keys with KeyboardManager. Windows and Catalyst available.

<a name='T-DrawnUi-Maui-Infrastructure-Enums-UpdateMode'></a>
## UpdateMode `type`

##### Namespace

DrawnUi.Maui.Infrastructure.Enums

<a name='F-DrawnUi-Maui-Infrastructure-Enums-UpdateMode-Constant'></a>
### Constant `constants`

##### Summary

Constantly invalidating the canvas after every frame

<a name='F-DrawnUi-Maui-Infrastructure-Enums-UpdateMode-Dynamic'></a>
### Dynamic `constants`

##### Summary

Will update when needed.

<a name='F-DrawnUi-Maui-Infrastructure-Enums-UpdateMode-Manual'></a>
### Manual `constants`

##### Summary

Will not update until manually invalidated.

<a name='T-DrawnUi-Maui-Controls-VStack'></a>
## VStack `type`

##### Namespace

DrawnUi.Maui.Controls

##### Summary

Helper class for SkiaLayout Type = LayoutType.Stack,  SplitMax = 1

<a name='T-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator'></a>
## VelocitySkiaAnimator `type`

##### Namespace

DrawnUi.Maui.Animate.Animators

##### Summary

Basically a modified port of Android FlingAnimation

<a name='P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-Friction'></a>
### Friction `property`

##### Summary

The bigger the sooner animation will slow down, default is 1.0

<a name='P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-RemainingVelocity'></a>
### RemainingVelocity `property`

##### Summary

This is set after we are done so we will know at OnStop if we have some energy left

<a name='P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-mMaxOverscrollValue'></a>
### mMaxOverscrollValue `property`

##### Summary

Must be over 0

<a name='P-DrawnUi-Maui-Animate-Animators-VelocitySkiaAnimator-mMinOverscrollValue'></a>
### mMinOverscrollValue `property`

##### Summary

Must be over 0

<a name='T-DrawnUi-Maui-Draw-ViewsAdapter'></a>
## ViewsAdapter `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Top level class for working with ItemTemplates. Holds visible views.

<a name='F-DrawnUi-Maui-Draw-ViewsAdapter-_dicoCellsInUse'></a>
### _dicoCellsInUse `constants`

##### Summary

Holds visible prepared views with appropriate context, index is inside ItemsSource

<a name='P-DrawnUi-Maui-Draw-ViewsAdapter-TemplatesAvailable'></a>
### TemplatesAvailable `property`

##### Summary

An important check to consider before consuming templates especially if you initialize templates in background

<a name='M-DrawnUi-Maui-Draw-ViewsAdapter-AddMoreToPool-System-Int32-'></a>
### AddMoreToPool(oversize) `method`

##### Summary

Keep pool size with \`n\` templated more oversized, so when we suddenly need more templates they would already be ready, avoiding lag spike,
This method is likely to reserve templated views once on layout size changed.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| oversize | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='M-DrawnUi-Maui-Draw-ViewsAdapter-FillPool-System-Int32,System-Collections-IList-'></a>
### FillPool(size) `method`

##### Summary

Use to manually pre-create views from item templates so when we suddenly need more templates they would already be ready, avoiding lag spike,
This will respect pool MaxSize in order not to overpass it.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| size | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='M-DrawnUi-Maui-Draw-ViewsAdapter-FillPool-System-Int32-'></a>
### FillPool(size) `method`

##### Summary

Use to manually pre-create views from item templates so when we suddenly need more templates they would already be ready, avoiding lag spike,
This will respect pool MaxSize in order not to overpass it.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| size | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='M-DrawnUi-Maui-Draw-ViewsAdapter-InitializeTemplates-System-Func{System-Object},System-Collections-IList,System-Int32,System-Int32-'></a>
### InitializeTemplates(template,dataContexts,poolSize,reserve) `method`

##### Summary

Main method to initialize templates, can use InitializeTemplatesInBackground as an option.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| template | [System.Func{System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{System.Object}') |  |
| dataContexts | [System.Collections.IList](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.IList 'System.Collections.IList') |  |
| poolSize | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |
| reserve | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Pre-create number of views to avoid lag spikes later, useful to do in backgound. |

<a name='T-DrawnUi-Maui-Draw-ViewsAdapter-ViewsIterator'></a>
## ViewsIterator `type`

##### Namespace

DrawnUi.Maui.Draw.ViewsAdapter

##### Summary

To iterate over virtual views

<a name='T-DrawnUi-Maui-Draw-VirtualisationType'></a>
## VirtualisationType `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='F-DrawnUi-Maui-Draw-VirtualisationType-Disabled'></a>
### Disabled `constants`

##### Summary

Visible parent bounds are not accounted for, children are rendred as usual.

<a name='F-DrawnUi-Maui-Draw-VirtualisationType-Enabled'></a>
### Enabled `constants`

##### Summary

Children not withing visible parent bounds are not rendered

<a name='F-DrawnUi-Maui-Draw-VirtualisationType-Smart'></a>
### Smart `constants`

##### Summary

Only the creation of a cached object is permitted for children not within visible parent bounds

<a name='T-DrawnUi-Maui-Draw-ViscousFluidInterpolator'></a>
## ViscousFluidInterpolator `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Ported from google android

<a name='F-DrawnUi-Maui-Draw-ViscousFluidInterpolator-VISCOUS_FLUID_SCALE'></a>
### VISCOUS_FLUID_SCALE `constants`

<a name='T-DrawnUi-Maui-Infrastructure-VisualTransform'></a>
## VisualTransform `type`

##### Namespace

DrawnUi.Maui.Infrastructure

##### Summary

Will enhance this in the future to include more properties

<a name='P-DrawnUi-Maui-Infrastructure-VisualTransform-Scale'></a>
### Scale `property`

##### Summary

Units as from ScaleX and ScaleY

<a name='P-DrawnUi-Maui-Infrastructure-VisualTransform-Translation'></a>
### Translation `property`

##### Summary

Units as from TranslationX and TranslationY

<a name='M-DrawnUi-Maui-Infrastructure-VisualTransform-ToNative-SkiaSharp-SKRect,System-Single-'></a>
### ToNative(rect,clipRect,scale) `method`

##### Summary

All input rects are in pixels

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| rect | [SkiaSharp.SKRect](#T-SkiaSharp-SKRect 'SkiaSharp.SKRect') |  |
| clipRect | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') |  |

<a name='T-DrawnUi-Maui-Infrastructure-VisualTransformNative'></a>
## VisualTransformNative `type`

##### Namespace

DrawnUi.Maui.Infrastructure

<a name='P-DrawnUi-Maui-Infrastructure-VisualTransformNative-Rect'></a>
### Rect `property`

##### Summary

Pixels only

<a name='T-DrawnUi-Maui-Infrastructure-VisualTreeChain'></a>
## VisualTreeChain `type`

##### Namespace

DrawnUi.Maui.Infrastructure

<a name='P-DrawnUi-Maui-Infrastructure-VisualTreeChain-Child'></a>
### Child `property`

##### Summary

The final node the tree leads to

<a name='P-DrawnUi-Maui-Infrastructure-VisualTreeChain-NodeIndices'></a>
### NodeIndices `property`

##### Summary

Perf cache for node indices

<a name='P-DrawnUi-Maui-Infrastructure-VisualTreeChain-Nodes'></a>
### Nodes `property`

##### Summary

Parents leading to the final node

<a name='P-DrawnUi-Maui-Infrastructure-VisualTreeChain-Transform'></a>
### Transform `property`

##### Summary

Final transform of the chain

<a name='T-DrawnUi-Maui-Draw-WindowParameters'></a>
## WindowParameters `type`

##### Namespace

DrawnUi.Maui.Draw

<a name='P-DrawnUi-Maui-Draw-WindowParameters-IsFixedSize'></a>
### IsFixedSize `property`

##### Summary

For desktop: if you set this to true the app window will not be allowed to be resized manually.

<a name='T-DrawnUi-Maui-Draw-ZoomContent'></a>
## ZoomContent `type`

##### Namespace

DrawnUi.Maui.Draw

##### Summary

Wrapper to zoom and pan content by changing the rendering scale so not affecting quality, this is not a transform.TODO add animated movement

<a name='F-DrawnUi-Maui-Draw-ZoomContent-LastValue'></a>
### LastValue `constants`

##### Summary

Last ViewportZoom value we are animating from

<a name='P-DrawnUi-Maui-Draw-ZoomContent-ZoomSpeed'></a>
### ZoomSpeed `property`

##### Summary

How much of finger movement will afect zoom change
