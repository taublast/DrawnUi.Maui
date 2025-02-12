

namespace AppoMobi.Touch
{
    public static class PointExtensions
    {
        /// <summary>
        /// Adds the coordinates of one Point to another.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Point Add(this Point first, Point second)
        {
            return new Point(first.X + second.X, first.Y + second.Y);
        }

        /// <summary>
        /// Gets the center of some touch points.
        /// </summary>
        /// <param name="touches"></param>
        /// <returns></returns>
        public static Point Center(this Point[] touches)
        {
            int num = (touches != null ? (int)touches.Length : 0);
            double x = 0;
            double y = 0;
            for (int i = 0; i < num; i++)
            {
                x += touches[i].X;
                y += touches[i].Y;
            }
            return new Point(x / (double)num, y / (double)num);
        }

        /// <summary>
        /// Subtracts the coordinates of one Point from another.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Point Subtract(this Point first, Point second)
        {
            return new Point(first.X - second.X, first.Y - second.Y);
        }
    }
}