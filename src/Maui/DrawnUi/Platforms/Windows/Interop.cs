using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKImageFilterHandle = System.IntPtr;

namespace DrawnUi.Draw
{
    public partial class SkiaControl
    {
        public static SKRect GetShadowRect(SKRect baseRect, SKImageFilter filter)
        {
            if (filter == null)
                return baseRect; // No effect, return base rect

            SKMatrix IdentityMatrix = new SKMatrix
            {
                ScaleX = 1,
                SkewX = 0,
                TransX = 0,
                SkewY = 0,
                ScaleY = 1,
                TransY = 0,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };

            IntPtr filterHandle = filter.Handle; // Publicly exposed!
            SKRect src = baseRect;
            SKRect dst;

            sk_imagefilter_filter_bounds(filterHandle, ref src, ref IdentityMatrix, 0, IntPtr.Zero, out dst);

            return dst;
        }

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void sk_imagefilter_get_output_bounds(
            SKImageFilterHandle filter, ref SKRect src, out SKRect dst);

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern SKImageFilterHandle sk_imagefilter_new_drop_shadow(
            float dx, float dy, float sigmaX, float sigmaY, uint color, SKImageFilterHandle input);

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void sk_imagefilter_filter_bounds(
            SKImageFilterHandle filter, ref SKRect src, ref SKMatrix ctm, int direction, IntPtr inputRect,
            out SKRect dst);
    }
}
