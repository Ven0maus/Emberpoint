using Emberpoint.Core.GameObjects.Abstracts;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class GridManager
    {
        public static EmberGrid Grid { get; private set; }

        public static void InitializeBlueprint<T>() where T : Blueprint<EmberCell>, new()
        {
            Grid = new EmberGrid(new T());
        }

        public static void InitializeCustomCells(int width, int height, EmberCell[] cells)
        {
            Grid = new EmberGrid(width, height, cells);
        }

        public static void InitializeCustomGrid(EmberGrid grid)
        {
            Grid = grid;
        }
    } 
}
