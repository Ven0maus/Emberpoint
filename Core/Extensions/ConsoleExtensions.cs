using SadConsole;
using SadRogue.Primitives;

namespace Emberpoint.Core.Extensions
{
    public static class ConsoleExtensions
    {
        public static void DrawBorders(this Console currentWindow, int width, int height, string cornerGlyph, string horizontalBorderGlyph, string verticalBorderGlyph, Color borderColor)
        {
            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (int colIndex = 0; colIndex < width; colIndex++)
                {
                    // Drawing Corners
                    if ((rowIndex == 0 && colIndex == 0)
                        || (rowIndex == height - 1 && colIndex == 0)
                        || (rowIndex == height - 1 && colIndex == width - 1)
                        || (rowIndex == 0 && colIndex == width - 1))
                    {
                        currentWindow.Print(colIndex, rowIndex, cornerGlyph, borderColor);
                    }

                    if (rowIndex > 0 && rowIndex < height - 1 && (colIndex == 0 || colIndex == width - 1))
                    {
                        currentWindow.Print(colIndex, rowIndex, horizontalBorderGlyph, borderColor);
                    }

                    if (colIndex > 0 && colIndex < width - 1 && (rowIndex == 0 || rowIndex == height - 1))
                    {
                        currentWindow.Print(colIndex, rowIndex, verticalBorderGlyph, borderColor);
                    }
                }
            }
        }

        public static void ColorFill(this Console currentWindow, Color color)
        {
            for (int x = 0; x < currentWindow.Width; x++)
            {
                for (int y = 0; y < currentWindow.Height; y++)
                {
                    currentWindow.SetGlyph(x, y, 0, color, color);
                }
            }
        }
    }
}