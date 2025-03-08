using System.Drawing;
using Size = System.Drawing.Size;

namespace AppoMobi.Maui.Gestures;

public static class PointFExtensions
{
    public static float Length(this PointF point)
    {
        return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
    }

    public static PointF Add(this PointF lhs, PointF rhs)
    {
        return new PointF(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }

    public static SizeF Distance(this PointF lhs, PointF rhs)
    {
        return new(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static PointF Subtract(this PointF lhs, PointF rhs)
    {
        return new PointF(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static PointF Multiply(float lhs, PointF rhs)
    {
        return new PointF(lhs * rhs.X, lhs * rhs.Y);
    }

    public static PointF Divide(this PointF lhs, float rhs)
    {
        return new PointF(lhs.X / rhs, lhs.Y / rhs);
    }
}