using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Items;
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
        private EmberCell _previousFlashlightCellState = null;

        /// <summary>
        /// Call this method instead from EmberGrid.CalibrateLightEngine();
        /// </summary>
        /// <param name="cells"></param>
        public void Calibrate(T[] cells)
        {
            if (_isCalibrated) return;
            foreach (var cell in cells.Where(a => a.LightProperties.EmitsLight))
            {
                var original = GridManager.Grid.GetCell(cell.Position);
                original.LightProperties.EmitsLight = false;
                AdjustLights(cell, original);
            }
            _isCalibrated = true;
        }

        public void AdjustLights(EmberCell newCell, EmberCell oldCell)
        {
            if (newCell.LightProperties.EmitsLight == oldCell.LightProperties.EmitsLight) return;

            var fov = new FOV(GridManager.Grid.FieldOfView);

            if (newCell.LightProperties.EmitsLight)
            {
                // Cell emits light
                CalculateLightSource(newCell);
            }
            else
            {
                // No longer emits light
                var cells = GridManager.Grid
                    .GetCells(a => a.Position == newCell.Position || (a.LightProperties.LightSources != null && a.LightProperties.LightSources.Any(a => a.Position == newCell.Position)))
                    .ToList();

                var lightSourcesToRecalculate = new List<EmberCell>();
                foreach (var cell in cells)
                {
                    if (cell.LightProperties.EmitsLight)
                    {
                        // Unset ourself
                        if (cell.Position == newCell.Position)
                        {
                            cell.LightProperties = newCell.LightProperties;
                            cell.CellProperties = newCell.CellProperties;
                        }

                        // Remove ourself from any light sources
                        if (cell.LightProperties.LightSources != null)
                        {
                            RemoveFromCellLightSources(cell, newCell);
                        }
                    }
                    else
                    {
                        RemoveFromCellLightSources(cell, newCell);
                    }

                    if (cell.LightProperties.LightSources != null)
                    {
                        foreach (var source in cell.LightProperties.LightSources)
                        {
                            lightSourcesToRecalculate.Add(source);
                        }
                    }

                    if (!cell.LightProperties.EmitsLight)
                    {
                        cell.LightProperties.Brightness = 0f;
                        cell.LightProperties.LightColor = default;
                    }

                    GridManager.Grid.SetCellColors(cell);
                    //GridManager.Grid.SetCellColors(cell, cell.CellProperties.NormalForeground, cell.CellProperties.NormalBackground);
                    GridManager.Grid.SetCell(cell, false, false);
                }

                lightSourcesToRecalculate = lightSourcesToRecalculate
                    .Distinct()
                    .ToList();

                foreach (var lightSource in lightSourcesToRecalculate)
                {
                    CalculateLightSource(lightSource);
                }

                newCell.LightProperties = GridManager.Grid.GetCell(newCell.Position).LightProperties;
            }
        }

        private void RemoveFromCellLightSources(EmberCell cell, EmberCell newCell)
        {
            cell.LightProperties.LightSources.RemoveAll(a => a.Position == newCell.Position);
            if (!cell.LightProperties.LightSources.Any())
            {
                cell.LightProperties.Brightness = 0f;
                cell.LightProperties.LightColor = default;
                cell.LightProperties.LightRadius = 0;
                cell.LightProperties.LightSources = null;
            }
        }

        public void CalculateLightSource(EmberCell cell)
        {
            if (cell.LightProperties.EmitsLight)
            {
                var fov = new FOV(GridManager.Grid.FieldOfView);
                fov.Calculate(cell.Position, cell.LightProperties.LightRadius);
                var toChangeCells = new List<(EmberCell, float)>();
                for (int x = 0; x < GridManager.Grid.GridSizeX; x++)
                {
                    for (int y = 0; y < GridManager.Grid.GridSizeY; y++)
                    {
                        // If cell is in the field of view of the object
                        if (fov.BooleanFOV[x, y])
                        {
                            var pos = new Point(x, y);
                            var distanceOfCenter = cell.Position.SquaredDistance(pos);
                            var cellToAdd = GridManager.Grid.GetCell(x, y);
                            if (!cellToAdd.LightProperties.EmitsLight)
                            {
                                if (cellToAdd.LightProperties.LightSources == null)
                                    cellToAdd.LightProperties.LightSources = new List<EmberCell>();
                                cellToAdd.LightProperties.LightSources.RemoveAll(a => a.Position == cell.Position);
                                cellToAdd.LightProperties.LightSources.Add(cell);
                            }
                            toChangeCells.Add((cellToAdd, distanceOfCenter));
                        }
                    }
                }

                HandleBrightnessLayers(toChangeCells, cell);
            }
        }

        private void HandleBrightnessLayers(List<(EmberCell, float)> cells, EmberCell newCell)
        {
            var orderedCells = cells.OrderBy(a => a.Item2);
            var layers = orderedCells.Select(a => a.Item2).Distinct().ToList();
            var brightnessLayers = CalculateBrightnessLayers(newCell, layers);

            foreach (var lightedCell in orderedCells)
            {
                var brightness = brightnessLayers[lightedCell.Item2];
                if (lightedCell.Item1.LightProperties.Brightness < brightness)
                    lightedCell.Item1.LightProperties.Brightness = brightness;

                GridManager.Grid.SetCellColors(lightedCell.Item1);
                //GridManager.Grid.SetCellColors(lightedCell.Item1, newCell.LightProperties.LightColor, lightedCell.Item1.CellProperties.NormalBackground);
                GridManager.Grid.SetCell(lightedCell.Item1, false, false);
            }
        }

        private Dictionary<float, float> CalculateBrightnessLayers(EmberCell centerCell, List<float> layers)
        {
            float deductAmount = centerCell.LightProperties.Brightness / layers.Count;
            float maxBrightness = centerCell.LightProperties.Brightness + deductAmount;
            return layers.ToDictionary(a => a, a => maxBrightness -= deductAmount);
        }

        public void HandleFlashlight(Player entity)
        {
            // If the player has his flashlight enabled, we must set the current tile as emits light
            var flashlight = entity.Inventory.GetItemOfType<Flashlight>();
            if (_previousFlashlightCellState != null)
            {
                var previousCell = GridManager.Grid.GetCell(_previousFlashlightCellState.Position);
                previousCell.LightProperties.EmitsLight = _previousFlashlightCellState.LightProperties.EmitsLight;
                previousCell.LightProperties.Brightness = _previousFlashlightCellState.LightProperties.Brightness;
                previousCell.LightProperties.LightRadius = _previousFlashlightCellState.LightProperties.LightRadius;
                previousCell.LightProperties.LightColor = _previousFlashlightCellState.LightProperties.LightColor;
                GridManager.Grid.SetCell(previousCell);
                _previousFlashlightCellState = null;
            }

            if (flashlight != null)
            {
                if (flashlight.LightOn)
                {
                    _previousFlashlightCellState = GridManager.Grid.GetCell(entity.Position);
                    var currentTile = GridManager.Grid.GetCell(entity.Position);
                    currentTile.LightProperties.EmitsLight = true;
                    if (currentTile.LightProperties.Brightness < Constants.Items.FlashlightBrightness)
                    {
                        currentTile.LightProperties.Brightness = Constants.Items.FlashlightBrightness;
                    }
                    if (currentTile.LightProperties.LightRadius < Constants.Items.FlashlightRadius)
                    {
                        currentTile.LightProperties.LightRadius = Constants.Items.FlashlightRadius;
                    }
                    currentTile.LightProperties.LightColor = Color.BlanchedAlmond;
                    GridManager.Grid.SetCell(currentTile);
                }
            }
        }
    }
}
