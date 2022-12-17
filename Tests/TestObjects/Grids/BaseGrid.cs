using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using SadRogue.Primitives;
using Tests.TestObjects.Entities;

namespace Tests.TestObjects.Grids
{
    public class BaseGrid : EmberGrid
    {
        private BaseGrid(int gridSizeX, int gridSizeY, EmberCell[] cells) : base(gridSizeX, gridSizeY, cells)
        { }

        private BaseGrid(CellBlueprint<EmberCell> cellBlueprint) 
            : base(cellBlueprint, null)
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
                        ForegroundFov = cell.Foreground == Color.Transparent ? Color.Transparent : Color.Lerp(cell.Foreground, Color.Black, .5f),
                        BackgroundFov = cell.Background == Color.Transparent ? Color.Transparent : Color.Lerp(cell.Foreground, Color.Black, .5f)
                    };
                    cells[y * width + x] = cell;
                }
            }
            return new BaseGrid(width, height, cells);
        }

        public static BaseGrid Create(CellBlueprint<EmberCell> cellBlueprint, ItemBlueprint<IEntity> itemBlueprint)
        {
            var grid = new BaseGrid(cellBlueprint);
            if (itemBlueprint != null)
            {
                foreach (var item in itemBlueprint.GetCells())
                {
                    var entity = EntityManager.Create<BaseEntity>(item.Position, -1, grid);
                    entity.Glyph = item.Glyph;
                    entity.MoveToBlueprint(grid.CellBlueprint);
                    entity.ChangeGrid(grid);
                }
            }
            return grid;
        }
    }
}
