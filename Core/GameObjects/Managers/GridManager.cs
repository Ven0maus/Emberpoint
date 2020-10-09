using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class GridManager
    {
        public static EmberGrid Grid { get; private set; }

        public static void InitializeBlueprint<T>() where T : Blueprint<EmberCell>, new()
        {
            Grid = new EmberGrid(new T());

            // After map is created, we calibrate the light engine
            Grid.CalibrateLightEngine();
        }

        public static void InitializeCustomCells(int width, int height, EmberCell[] cells)
        {
            Grid = new EmberGrid(width, height, cells);

            // After map is created, we calibrate the light engine
            Grid.CalibrateLightEngine();
        }

        public static void InitializeCustomGrid(EmberGrid grid)
        {
            Grid = grid;

            // After map is created, we calibrate the light engine
            Grid.CalibrateLightEngine();
        }
    } 
}
