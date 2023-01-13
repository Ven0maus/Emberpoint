using SadRogue.Primitives;
using System;
using System.Globalization;

namespace Emberpoint.Core.Extensions
{
    public static class MonoGameExtensions
    {
        public static Point[] Get4Neighbors(this Point point)
        {
            return new[] 
            { 
                new Point(point.X - 1, point.Y),
                new Point(point.X + 1, point.Y),
                new Point(point.X, point.Y - 1),
                new Point(point.X, point.Y + 1)
            };
        }

        public static Point[] Get8Neighbors(this Point point)
        {
            return new[]
            {
                new Point(point.X - 1, point.Y),
                new Point(point.X + 1, point.Y),
                new Point(point.X, point.Y - 1),
                new Point(point.X, point.Y + 1),
                new Point(point.X - 1, point.Y - 1),
                new Point(point.X + 1, point.Y + 1),
                new Point(point.X - 1, point.Y + 1),
                new Point(point.X + 1, point.Y - 1)
            };
        }

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

        public static Color GetColorByString(string value)
        {
            if (value.StartsWith("#"))
            {
                value = value.TrimStart('#');
                Color color;

                if (value.Length == 6)
                    color = new Color(
                                int.Parse(value.Substring(0, 2), NumberStyles.HexNumber),
                                int.Parse(value.Substring(2, 2), NumberStyles.HexNumber),
                                int.Parse(value.Substring(4, 2), NumberStyles.HexNumber),
                                255);
                else // assuming length of 8
                    color = new Color(
                                int.Parse(value.Substring(2, 2), NumberStyles.HexNumber),
                                int.Parse(value.Substring(4, 2), NumberStyles.HexNumber),
                                int.Parse(value.Substring(6, 2), NumberStyles.HexNumber),
                                int.Parse(value.Substring(0, 2), NumberStyles.HexNumber));

                return color;
            }

            var field = typeof(Color).GetField(value);
            if (field != null)
                return (Color)field.GetValue(null);
            return default;
        }
    }
}
