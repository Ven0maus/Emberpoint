using Emberpoint.Core.GameObjects.Map;
using Microsoft.Xna.Framework;

namespace Tests.TestObjects.Grids
{
    public class BaseGrid : EmberGrid
    {
        private BaseGrid(int gridSizeX, int gridSizeY, EmberCell[] cells) : base(gridSizeX, gridSizeY, cells)
        { }

        private BaseGrid(Emberpoint.Core.GameObjects.Abstracts.Blueprint<EmberCell> blueprint) : base(blueprint)
        { }

        public static BaseGrid Create(int width, int height)
        {
            // Build a custom cell grid
            var cells = new EmberCell[width * height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var cell = new EmberCell
                    {
                        Background = Color.Black,
                        Foreground = Color.Gray,
                        Glyph = ' ',
                        Position = new Point(x, y)
                    };
                    cell.CellProperties = new EmberCell.EmberCellProperties
                    {
                        Name = null,
                        Walkable = true,
                        BlocksFov = false,
                        NormalForeground = cell.Foreground,
                        NormalBackground = cell.Background,
                        ForegroundFov = Color.Lerp(cell.Foreground, Color.Black, .5f),
                        BackgroundFov = cell.Background
                    };
                    cells[y * width + x] = cell;
                }
            }
            return new BaseGrid(width, height, cells);
        }

        public static BaseGrid Create(Emberpoint.Core.GameObjects.Abstracts.Blueprint<EmberCell> blueprint)
        {
            return new BaseGrid(blueprint);
        }
    }
}
