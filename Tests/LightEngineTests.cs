using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using GoRogue;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System.Linq;
using Tests.TestObjects.Blueprints;
using Tests.TestObjects.Entities;
using Tests.TestObjects.Grids;

namespace Tests
{
    [TestFixture]
    public class LightEngineTests
    {
        protected EmberGrid _grid;

        [SetUp]
        protected virtual void Setup()
        {
            _grid = BaseGrid.Create(20, 20);
            GridManager.InitializeCustomGrid(_grid);
        }

        [Test]
        public void CellsAreNotExplored_EvenWhen_AreaHasLights()
        {
            // Initialize a blueprint for testing
            _grid = BaseGrid.Create(new BaseBlueprint());
            GridManager.InitializeCustomGrid(_grid);

            // Create entity and calculate fov + draw it
            var entity = EntityManager.Create<BaseEntity>(new Point(1, 1), -1, _grid);
            EntityManager.RecalculatFieldOfView(entity);
            GridManager.Grid.DrawFieldOfView(entity);

            var cellsWithBrightness = _grid.GetCells(a => a.LightProperties.Brightness > 0f).ToList();

            foreach (var cell in cellsWithBrightness)
            {
                Assert.IsFalse(cell.CellProperties.IsExplored);
            }
        }

        [Test]
        public void CellsAreExplored_WhenEntityIsNear_LightSource()
        {
            // Initialize a blueprint for testing
            _grid = BaseGrid.Create(new BaseBlueprint());
            GridManager.InitializeCustomGrid(_grid);

            // Create entity and calculate fov + draw it
            var entity = EntityManager.Create<BaseEntity>(_grid.GetCell(a => a.LightProperties.Brightness > 0f && a.CellProperties.Walkable).Position, -1, _grid);
            EntityManager.RecalculatFieldOfView(entity);
            GridManager.Grid.DrawFieldOfView(entity);

            var cellsWithBrightness = _grid.GetCells(a => a.LightProperties.Brightness > 0f).ToList();

            foreach (var cell in cellsWithBrightness)
            {
                Assert.IsTrue(cell.CellProperties.IsExplored);
            }
        }

        [Test]
        public void EmittingCellsUnset_NoBrightnessCells_Left()
        {
            SetLightCell(1, 1, 5);
            Assert.IsTrue(_grid.GetCell(1, 1).LightProperties.Brightness > 0f);
            UnsetLightCell(1, 1);
            Assert.IsTrue(_grid.GetCell(1, 1).LightProperties.Brightness == 0f);
            Assert.IsTrue(_grid.GetCells(a => a.LightProperties.Brightness > 0f).Count() == 0);
        }

        [Test]
        public void SetEmittingCell_AdjacentCells_UpdatedCorrectly()
        {
            var cell = SetLightCell(10, 10, 4);

            var fov = new FOV(_grid.FieldOfView);
            fov.Calculate(cell.Position, cell.LightProperties.LightRadius);

            for (int x=0; x < _grid.GridSizeX; x++)
            {
                for (int y = 0; y < _grid.GridSizeY; y++)
                {
                    if (fov.BooleanFOV[x,y])
                    {
                        Assert.IsTrue(_grid.GetCell(x, y).LightProperties.Brightness > 0f);
                    }
                }
            }
        }

        [Test]
        public void RemoveEmittingCell_AdjacentCells_UpdatedCorrectly()
        {
            SetLightCell(10, 10, 4);
            UnsetLightCell(10, 10);

            var fov = new FOV(_grid.FieldOfView);
            fov.Calculate(new Point(10, 10), 4);

            for (int x = 0; x < _grid.GridSizeX; x++)
            {
                for (int y = 0; y < _grid.GridSizeY; y++)
                {
                    if (fov.BooleanFOV[x, y])
                    {
                        Assert.IsTrue(_grid.GetCell(x, y).LightProperties.Brightness == 0f);
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
                var fov = new FOV(_grid.FieldOfView);
                fov.Calculate(cell.Position, cell.LightProperties.LightRadius);

                for (int x = 0; x < _grid.GridSizeX; x++)
                {
                    for (int y = 0; y < _grid.GridSizeY; y++)
                    {
                        if (fov.BooleanFOV[x, y])
                        {
                            Assert.IsTrue(_grid.GetCell(x, y).LightProperties.Brightness > 0f);
                        }
                    }
                }
            }
        }

        [Test]
        public void RemoveMultipleEmittingCells_AdjacentCells_UpdatedCorrectly()
        {
            var positions = new[] { new Point(10, 10), new Point(13, 10), new Point(11, 13) };
            var radiuses = new int[] { 4, 6, 4 };
            for (int i=0; i < positions.Length; i++)
            {
                SetLightCell(positions[i].X, positions[i].Y, radiuses[i]);
                UnsetLightCell(positions[i].X, positions[i].Y);
            }

            for (int i=0; i < positions.Length; i++)
            {
                var fov = new FOV(_grid.FieldOfView);
                fov.Calculate(positions[i], radiuses[i]);

                for (int x = 0; x < _grid.GridSizeX; x++)
                {
                    for (int y = 0; y < _grid.GridSizeY; y++)
                    {
                        if (fov.BooleanFOV[x, y])
                        {
                            Assert.IsTrue(_grid.GetCell(x, y).LightProperties.Brightness == 0f);
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
                var fov = new FOV(_grid.FieldOfView);
                fov.Calculate(cell.Position, cell.LightProperties.LightRadius);

                for (int x = 0; x < _grid.GridSizeX; x++)
                {
                    for (int y = 0; y < _grid.GridSizeY; y++)
                    {
                        if (fov.BooleanFOV[x, y])
                        {
                            Assert.IsTrue(_grid.GetCell(x, y).LightProperties.Brightness > 0f);
                        }
                    }
                }
            }

            // Check unset cell for light sources = null has 0 brightness
            var fov2 = new FOV(_grid.FieldOfView);
            fov2.Calculate(unsetCells[0].Position, unsetCells[0].LightProperties.LightRadius);

            bool someCellsAreUnset = false;
            for (int x = 0; x < _grid.GridSizeX; x++)
            {
                for (int y = 0; y < _grid.GridSizeY; y++)
                {
                    if (fov2.BooleanFOV[x, y])
                    {
                        var value = _grid.GetCell(x, y);
                        if (value.LightProperties.LightSources == null && !value.LightProperties.EmitsLight)
                        {
                            someCellsAreUnset = true;
                            Assert.IsTrue(value.LightProperties.Brightness == 0f);
                        }
                    }
                }
            }
            Assert.IsTrue(someCellsAreUnset);
        }

        private EmberCell UnsetLightCell(int x, int y)
        {
            var cell = _grid.GetCell(x, y);
            cell.LightProperties.EmitsLight = false;
            _grid.SetCell(cell);
            return cell;
        }

        private EmberCell SetLightCell(int x, int y, int radius)
        {
            var cell = _grid.GetCell(x, y);
            cell.LightProperties.EmitsLight = true;
            cell.LightProperties.LightRadius = radius;
            cell.LightProperties.LightColor = Color.Red;
            cell.LightProperties.Brightness = 0.75f;
            _grid.SetCell(cell);
            return cell;
        }
    }
}
