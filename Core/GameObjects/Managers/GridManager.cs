using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Blueprints.Objects;
using Emberpoint.Core.GameObjects.Map;
using System;
using System.Collections.Generic;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class GridManager
    {
        public static EmberGrid Grid { get; private set; }
        public static CellBlueprint<EmberCell> ActiveBlueprint { get { return Grid?.CellBlueprint; } }

        private readonly static Dictionary<Type, EmberGrid> _blueprintGridCache = new Dictionary<Type, EmberGrid>();

        public static void ClearCache()
        {
            _blueprintGridCache.Clear();
        }

        public static void InitializeBlueprint<T>(bool saveGridData) where T : CellBlueprint<EmberCell>, new()
        {
            if (!saveGridData)
            {
                var cellBlueprint = new T();
                Grid = new EmberGrid(cellBlueprint, new GenericItemBlueprint(cellBlueprint.ObjectId, typeof(T).Name.Replace("Cells", "Items")));
                Grid.CalibrateLightEngine();
                return;
            }

            if (_blueprintGridCache.TryGetValue(typeof(T), out EmberGrid grid))
            {
                Grid = grid;
            }
            else
            {
                // Initialize the grid
                var cellBlueprint = new T();
                Grid = new EmberGrid(cellBlueprint, new GenericItemBlueprint(cellBlueprint.ObjectId, typeof(T).Name.Replace("Cells", "Items")));

                // Calibrate the lights
                Grid.CalibrateLightEngine();
                _blueprintGridCache.Add(typeof(T), Grid);
            }
        }

        public static void InitializeBlueprint<T>(CellBlueprint<T> blueprint, bool saveGridData) where T : EmberCell, new()
        {
            if (!saveGridData)
            {
                Grid = new EmberGrid(blueprint.GridSizeX, blueprint.GridSizeY, blueprint.GetCells(), blueprint as CellBlueprint<EmberCell>);
                Grid.CalibrateLightEngine();
                return;
            }

            if (_blueprintGridCache.TryGetValue(blueprint.GetType(), out EmberGrid grid))
            {
                Grid = grid;
            }
            else
            {
                Grid = new EmberGrid(blueprint.GridSizeX, blueprint.GridSizeY, blueprint.GetCells(), blueprint as CellBlueprint<EmberCell>);
                Grid.CalibrateLightEngine();

                _blueprintGridCache.Add(blueprint.GetType(), Grid);
            }
        }

        public static void InitializeCustomGrid(EmberGrid grid)
        {
            Grid = grid;

            // After map is created, we calibrate the light engine
            Grid.CalibrateLightEngine();
        }
    } 
}
