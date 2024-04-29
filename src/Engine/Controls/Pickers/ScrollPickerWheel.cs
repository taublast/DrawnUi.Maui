using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Draw;

namespace DrawnUi.Maui.Controls;

public class ScrollPickerWheel : SkiaLayout, ILayoutInsideViewport
{
    public ScrollPickerWheel()
    {
        // we turn this off because as we repeat views in loop those
        // visible become hidden at the same time and vice versa
        // se we cant recycle hidden cells at all
        RecyclingTemplate = RecyclingTemplate.Disabled;
    }

    protected override void OnLayoutChanged()
    {
        //this one constantly changes size, we avoid invoking onsizechanged on every frame..

        //base.OnLayoutReady();
    }


    public static readonly BindableProperty DistortionAngleProperty = BindableProperty.Create(nameof(DistortionAngle),
        typeof(double),
        typeof(ScrollPickerWheel),
        1.0, propertyChanged: OnNeedUpdate);


    public double DistortionAngle
    {
        get { return (double)GetValue(DistortionAngleProperty); }
        set { SetValue(DistortionAngleProperty, value); }
    }

    public static readonly BindableProperty DismorphProperty = BindableProperty.Create(nameof(Dismorph),
        typeof(double),
        typeof(ScrollPickerWheel),
        1.0, propertyChanged: OnNeedUpdate);


    public double Dismorph
    {
        get { return (double)GetValue(DismorphProperty); }
        set { SetValue(DismorphProperty, value); }
    }

    private static void OnNeedUpdate(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ScrollPickerWheel control)
        {
            control.Invalidate();
        }
    }


    public override void OnViewportWasChanged(ScaledRect viewport)
    {
        CurvedVisionAngle = Dismorph;
        var realHeight = ComputeViewportHeightFromTransformed(viewport.Pixels.Height, CurvedVisionAngle);
        mWheelHeight = realHeight;
        mHalfWheelHeight = mWheelHeight / 2.0f;
        mWheelCenterY = viewport.Pixels.MidY;
        mViewportHeightK = realHeight / viewport.Pixels.Height;

        base.OnViewportWasChanged(viewport);
    }

    private float ComputeSpace(float radians)
    {
        return (float)(mHalfWheelHeight * Math.Sin((double)radians));
    }
    private float ComputeDepth(float radians)
    {
        return (float)(mHalfWheelHeight * (1 - Math.Cos((double)radians)));
    }

    private float ComputeViewportHeight(float height, float radians)
    {
        var ret = height * Math.Cos(radians) / (Math.PI / 2.0);

        return (float)ret;
    }

    private float ComputeViewportHeightFromTransformed(double height, double radians)
    {
        var ret = height * (Math.PI / 2.0) / Math.Cos(radians);

        return (float)ret;
    }

    protected float mViewportHeightK;
    protected float mWheelHeight;
    protected float mHalfWheelHeight;
    protected float mWheelCenterY;
    protected double CurvedVisionAngle = 1.5707963267949; // Pi / 2

    protected override bool DrawChild(SkiaDrawingContext context, SKRect dest, ISkiaControl child, float scale)
    {
        if (child is SkiaControl control && mHalfWheelHeight > 0)
        {
            //child related
            var itemCenterY = dest.MidY;

            float single = (mWheelCenterY - itemCenterY) / mHalfWheelHeight;

            single = (float)Math.Min(Math.Max((double)single, -CurvedVisionAngle), CurvedVisionAngle);
            single = (float)Math.Max((double)single, -CurvedVisionAngle);

            var childViewAngle = (float)RadiansToDegrees(single) * (float)DistortionAngle;

            var distortionZ = 1.0f - DistortionAngle / 10.0f;

            var z = ComputeDepth(single) * (float)distortionZ;

            //draw transformed child

            var scroll = (SkiaScroll)Parent;

            var centerX = dest.Left + dest.Width * (float)TransformPivotPointX;
            var centerY = dest.Top + dest.Height * (float)TransformPivotPointY;

            var centerViewportY = scroll.Destination.Top + scroll.Viewport.Pixels.MidY;

            if (control.Helper3d == null)
            {
                control.Helper3d = new();
            }

#if SKIA3
            control.Helper3d.Reset();
            control.Helper3d.RotateXDegrees(childViewAngle);
            control.Helper3d.TranslateZ(z);
            var applyMatrix = control.Helper3d.GetMatrix();
#else
            control.Helper3d.Save();
            control.Helper3d.RotateXDegrees(childViewAngle);
            control.Helper3d.TranslateZ(z);
            var applyMatrix = control.Helper3d.Matrix;
            control.Helper3d.Restore();
#endif
            var saved = context.Canvas.Save();

            //set pivot point
            var DrawingMatrix = SKMatrix.CreateTranslation(-centerX, -centerY);

            //apply stuff
            DrawingMatrix = DrawingMatrix.PostConcat(applyMatrix);

            //restore coordinates back
            DrawingMatrix = DrawingMatrix.PostConcat(SKMatrix.CreateTranslation(centerX, centerY));

            //apply parent's transforms
            DrawingMatrix = DrawingMatrix.PostConcat(context.Canvas.TotalMatrix);

            context.Canvas.SetMatrix(DrawingMatrix);

            if (child is IInsideWheelStack cell)
            {
                var offset = 0f;
                if (dest.MidY > centerViewportY)
                {
                    offset = dest.MidY - centerViewportY;
                }
                else
                if (centerViewportY > dest.MidY)
                {
                    offset = centerViewportY - dest.MidY;
                }

                var offsetRatio = offset / mHalfWheelHeight;
                var isSelected = offset < dest.Height / 2;

                cell.OnPositionChanged(offsetRatio, isSelected);
            }

            var ret = base.DrawChild(context, dest, child, scale);

            context.Canvas.RestoreToCount(saved);

            return true;
        }
        else
        {
            return base.DrawChild(context, dest, child, scale);
        }


    }


}