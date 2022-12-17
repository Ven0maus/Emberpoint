using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Blueprints.Objects;
using Emberpoint.Core.GameObjects.Items;
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

        public static void InitializeBlueprint<TCell, TItem>(CellBlueprint<TCell> cellBlueprint, ItemBlueprint<TItem> itemBlueprint, bool saveGridData) 
            where TCell : EmberCell, new()
            where TItem : EmberItem
        {
            if (!saveGridData)
            {
                Grid = new EmberGrid(cellBlueprint.GridSizeX, cellBlueprint.GridSizeY, cellBlueprint.GetCells(), 
                    cellBlueprint as CellBlueprint<EmberCell>, itemBlueprint as ItemBlueprint<EmberItem>);
                Grid.CalibrateLightEngine();
                return;
            }

            if (_blueprintGridCache.TryGetValue(cellBlueprint.GetType(), out EmberGrid grid))
            {
                Grid = grid;
            }
            else
            {
                Grid = new EmberGrid(cellBlueprint.GridSizeX, cellBlueprint.GridSizeY, cellBlueprint.GetCells(), 
                    cellBlueprint as CellBlueprint<EmberCell>, itemBlueprint as ItemBlueprint<EmberItem>);
                Grid.CalibrateLightEngine();

                _blueprintGridCache.Add(cellBlueprint.GetType(), Grid);
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
