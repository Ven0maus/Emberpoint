using Emberpoint.Core.Objects.Abstracts;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects.Blueprints
{
    public class HouseBlueprint : Blueprint<EmberCell>
    {
        public override EmberCell[] GetCells()
        {
            // TODO: Read from .txt file and .json config file
            int gridSizeX = 70;
            int gridSizeY = 30;
            var cells = new EmberCell[gridSizeX * gridSizeY];
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[y * gridSizeX + x] = new EmberCell(new Point(x, y), '.', Color.Green);
                }
            }
            return cells;
        }
    }
}
