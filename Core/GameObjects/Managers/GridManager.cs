﻿using Emberpoint.Core.GameObjects.Abstracts;
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

        public static void InitializeBluePrint<T>(Blueprint<T> blueprint) where T : EmberCell, new()
        {
            Grid = new EmberGrid(blueprint.GridSizeX, blueprint.GridSizeY, blueprint.GetCells());

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
