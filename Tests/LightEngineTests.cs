using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using GoRogue;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class LightEngineTests
    {
        protected EmberGrid _grid;

        [SetUp]
        protected virtual void Setup()
        {
            _grid = EmberGridTests.BuildCustomGrid(20, 20);
            GridManager.InitializeCustomGrid(_grid);
        }

        [Test]
        public void SetEmittingCell_AdjacentCells_UpdatedCorrectly()
        {
            var cell = SetLightCell(10, 10, 4);

            var area = new List<EmberCell>();
            GetRadiusNeighbors(cell, cell.LightRadius, area);
            var points = area.ToDictionary(a => a.Position, a => a);

            var fov = new FOV(_grid.FieldOfView);
            fov.Calculate(cell.Position, cell.LightRadius);

            for (int x=0; x < _grid.GridSizeX; x++)
            {
                for (int y = 0; y < _grid.GridSizeY; y++)
                {
                    if (fov.BooleanFOV[x,y])
                    {
                        Assert.IsTrue(points.TryGetValue(new Point(x, y), out EmberCell value));
                        Assert.IsTrue(value.Brightness > 0f);
                    }
                }
            }
        }

        [Test]
        public void RemoveEmittingCell_AdjacentCells_UpdatedCorrectly()
        {
            var cell = SetLightCell(10, 10, 4);
            cell = UnsetLightCell(10, 10);

            var area = new List<EmberCell>();
            GetRadiusNeighbors(cell, 4, area);
            var points = area.ToDictionary(a => a.Position, a => a);

            var fov = new FOV(_grid.FieldOfView);
            fov.Calculate(cell.Position, 4);

            for (int x = 0; x < _grid.GridSizeX; x++)
            {
                for (int y = 0; y < _grid.GridSizeY; y++)
                {
                    if (fov.BooleanFOV[x, y])
                    {
                        Assert.IsTrue(points.TryGetValue(new Point(x, y), out EmberCell value));
                        Assert.IsTrue(value.Brightness == 0f);
                    }
                }
            }
        }

        [Test]
        public void SetMultipleEmittingCells_AdjacentCells_UpdatedCorrectly()
        {
            var cells = new[] { SetLightCell(10, 10, 4), SetLightCell(13, 10, 6), SetLightCell(11, 13, 4) };

            foreach (var cell in cells)
            {
                var area = new List<EmberCell>();
                GetRadiusNeighbors(cell, cell.LightRadius, area);
                var points = area.ToDictionary(a => a.Position, a => a);

                var fov = new FOV(_grid.FieldOfView);
                fov.Calculate(cell.Position, cell.LightRadius);

                for (int x = 0; x < _grid.GridSizeX; x++)
                {
                    for (int y = 0; y < _grid.GridSizeY; y++)
                    {
                        if (fov.BooleanFOV[x, y])
                        {
                            Assert.IsTrue(points.TryGetValue(new Point(x, y), out EmberCell value));
                            Assert.IsTrue(value.Brightness > 0f);
                        }
                    }
                }
            }
        }

        [Test]
        public void RemoveMultipleEmittingCells_AdjacentCells_UpdatedCorrectly()
        {
            var cells = new[] { SetLightCell(10, 10, 4), SetLightCell(13, 10, 6), SetLightCell(11, 13, 4) };
            cells = new[] { UnsetLightCell(10, 10), UnsetLightCell(13, 10), UnsetLightCell(11, 13) };
            var radiuses = new int[] { 4, 6, 4 };

            for (int i=0; i < cells.Length; i++)
            {
                var area = new List<EmberCell>();
                GetRadiusNeighbors(cells[i], radiuses[i], area);
                var points = area.ToDictionary(a => a.Position, a => a);

                var fov = new FOV(_grid.FieldOfView);
                fov.Calculate(cells[i].Position, radiuses[i]);

                for (int x = 0; x < _grid.GridSizeX; x++)
                {
                    for (int y = 0; y < _grid.GridSizeY; y++)
                    {
                        if (fov.BooleanFOV[x, y])
                        {
                            Assert.IsTrue(points.TryGetValue(new Point(x, y), out EmberCell value));
                            Assert.IsTrue(value.Brightness == 0f);
                        }
                    }
                }
            }
        }

        [Test]
        public void RemoveOneEmittingCell_FromManyEmittingCells_AdjacentCells_UpdatedCorrectly()
        {
            var setCells = new[] { SetLightCell(10, 10, 4), SetLightCell(13, 10, 6), SetLightCell(11, 13, 4) };
            var unsetCells = new[] { UnsetLightCell(13, 10) };

            foreach (var cell in setCells.Except(unsetCells))
            {
                var area = new List<EmberCell>();
                GetRadiusNeighbors(cell, cell.LightRadius, area);
                var points = area.ToDictionary(a => a.Position, a => a);

                var fov = new FOV(_grid.FieldOfView);
                fov.Calculate(cell.Position, cell.LightRadius);

                for (int x = 0; x < _grid.GridSizeX; x++)
                {
                    for (int y = 0; y < _grid.GridSizeY; y++)
                    {
                        if (fov.BooleanFOV[x, y])
                        {
                            Assert.IsTrue(points.TryGetValue(new Point(x, y), out EmberCell value));
                            Assert.IsTrue(value.Brightness > 0f);
                        }
                    }
                }
            }

            // Check unset cell for light sources = null has 0 brightness
            var area2 = new List<EmberCell>();
            GetRadiusNeighbors(unsetCells[0], unsetCells[0].LightRadius, area2);
            var points2 = area2.ToDictionary(a => a.Position, a => a);

            var fov2 = new FOV(_grid.FieldOfView);
            fov2.Calculate(unsetCells[0].Position, unsetCells[0].LightRadius);

            bool someCellsAreUnset = false;
            for (int x = 0; x < _grid.GridSizeX; x++)
            {
                for (int y = 0; y < _grid.GridSizeY; y++)
                {
                    if (fov2.BooleanFOV[x, y])
                    {
                        Assert.IsTrue(points2.TryGetValue(new Point(x, y), out EmberCell value));
                        if (value.LightSources == null)
                        {
                            someCellsAreUnset = true;
                            Assert.IsTrue(value.Brightness == 0f);
                        }
                    }
                }
            }
            Assert.IsTrue(someCellsAreUnset);
        }

        private EmberCell UnsetLightCell(int x, int y)
        {
            var cell = _grid.GetCell(x, y);
            cell.EmitsLight = false;
            _grid.SetCell(cell);
            return cell;
        }

        private EmberCell SetLightCell(int x, int y, int radius)
        {
            var cell = _grid.GetCell(x, y);
            cell.EmitsLight = true;
            cell.LightRadius = radius;
            cell.LightColor = Color.Red;
            cell.Brightness = 0.75f;
            _grid.SetCell(cell);
            return cell;
        }

        private void GetRadiusNeighbors(EmberCell cell, int radius, List<EmberCell> cells)
        {
            if (radius == 0) return;
            var neighbors = _grid.GetNeighbors(cell);
            foreach (var neighbor in neighbors)
            {
                if (!cells.Contains(neighbor)) 
                    cells.Add(neighbor);

                GetRadiusNeighbors(neighbor, radius - 1, cells);
            }
        }
    }
}
