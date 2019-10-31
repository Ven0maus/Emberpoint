using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using GoRogue;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Map
{
    public class LightEngine<T> where T : EmberCell, ILightable
    {
        private bool _isCalibrated = false;
        public void Calibrate(T[] cells)
        {
            if (_isCalibrated) return;
            // Update light engine
            foreach (var cell in cells.Where(a => a.EmitsLight))
            {
                var oldCell = cell.Clone();
                oldCell.EmitsLight = false;
                AdjustLightLevels(cell, oldCell);
                GridManager.Grid.SetCell(cell);
            }
            _isCalibrated = true;
        }

        public void AdjustLightLevels(EmberCell newCell, EmberCell oldCell)
        {
            if (oldCell.EmitsLight == newCell.EmitsLight) return;

            var fov = new FOV(GridManager.Grid.FieldOfView);

            if (!newCell.EmitsLight)
            {
                fov.Calculate(newCell.Position, oldCell.LightRadius);

                var toChangeCells = new List<EmberCell> { newCell };
                for (int x = 0; x < GridManager.Grid.GridSizeX; x++)
                {
                    for (int y = 0; y < GridManager.Grid.GridSizeY; y++)
                    {
                        // If cell is in the field of view of the object
                        if (fov.BooleanFOV[x, y])
                        {
                            var pos = new Point(x, y);
                            var cellToAdd = newCell.Position == pos ? newCell : GridManager.Grid.GetCell(x, y);
                            if (cellToAdd.LightSource == newCell)
                                toChangeCells.Add(cellToAdd);
                        }
                    }
                }

                foreach (var cell in toChangeCells)
                {
                    cell.LightSource = null;
                    cell.Brightness = 0f;
                    cell.LightRadius = 0;
                    cell.LightColor = default;

                    GridManager.Grid.SetCellColors(cell, Game.Player, cell.NormalForeground, cell.ForegroundFov);
                    if (cell != newCell) // We don't need an infinite loop here :) It's passed by reference.
                        GridManager.Grid.SetCell(cell);
                }

                return;
            }

            var cells = new List<(EmberCell, float)>();
            fov.Calculate(newCell.Position, newCell.LightRadius);

            for (int x = 0; x < GridManager.Grid.GridSizeX; x++)
            {
                for (int y = 0; y < GridManager.Grid.GridSizeY; y++)
                {
                    // If cell is in the field of view of the object
                    if (fov.BooleanFOV[x, y])
                    {
                        var pos = new Point(x, y);
                        var distanceOfCenter = newCell.Position.SquaredDistance(pos);
                        var cellToAdd = newCell.Position == pos ? newCell : GridManager.Grid.GetCell(x, y);
                        cellToAdd.LightSource = newCell;
                        cells.Add((cellToAdd, distanceOfCenter));
                    }
                }
            }

            var orderedCells = cells.OrderBy(a => a.Item2);
            var layers = orderedCells.Select(a => a.Item2).Distinct().ToList();
            var brightnessLayers = CalculateBrightnessLayers(newCell, layers);

            foreach (var lightedCell in orderedCells)
            {
                var brightness = brightnessLayers[lightedCell.Item2];
                if (lightedCell.Item1.Brightness < brightness)
                    lightedCell.Item1.Brightness = brightness;

                GridManager.Grid.SetCellColors(lightedCell.Item1, Game.Player, newCell.LightColor, Color.Lerp(newCell.LightColor, Color.Black, .5f));
                if (lightedCell.Item1 != newCell) // We don't need an infinite loop here :) It's passed by reference.
                    GridManager.Grid.SetCell(lightedCell.Item1);
            }
        }

        private Dictionary<float, float> CalculateBrightnessLayers(EmberCell centerCell, List<float> layers)
        {
            float deductAmount = centerCell.Brightness / layers.Count;
            float maxBrightness = centerCell.Brightness + deductAmount;
            return layers.ToDictionary(a => a, a => maxBrightness -= deductAmount);
        }
    }
}
