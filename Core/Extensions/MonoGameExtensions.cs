using Microsoft.Xna.Framework;
using System;

namespace Emberpoint.Core.Extensions
{
    public static class MonoGameExtensions
    {
        public static Point Translate(this Point point, int x, int y)
        {
            return new Point(point.X + x, point.Y + y);
        }

        public static Point Translate(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        public static float SquaredDistance(this Point point, Point other)
        {
            return ((point.X - other.X) * (point.X - other.X)) + ((point.Y - other.Y) * (point.Y - other.Y));
        }

        public static float Distance(this Point point, Point other)
        {
            return (float)Math.Sqrt(point.SquaredDistance(other));
        }
    }
}
