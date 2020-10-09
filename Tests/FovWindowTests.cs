using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Tests.TestObjects.Entities;
using Tests.TestObjects.Grids;
using Tests.TestObjects.Windows;

namespace Tests
{
    [TestFixture]
    public class FovWindowTests
    {
        private FovWindowMock _fovWindow;
        private BaseEntity _entity;
        private BaseGrid _grid;
        private const int _mapSize = 12;

        [SetUp]
        public void SetUp()
        {
            _fovWindow = new FovWindowMock();
            _grid = BaseGrid.Create(_mapSize, _mapSize);
            GridManager.InitializeCustomGrid(_grid);

            for (int x=0; x < _mapSize; x++)
            {
                for (int y=0; y < _mapSize; y++)
                {
                    if (x >= 1 && y >= 1 && x < _mapSize -1 && y < _mapSize -1)
                    {
                        var cell = _grid.GetCell(x, y);
                        cell.CellProperties.IsExplored = true;
                        _grid.SetCell(cell);
                        continue;
                    }

                    // Add walls around the map borders
                    AddWallToGrid(x, y);
                }
            }

            // Add lamp at pos 10x10y
            AddLampToGrid(10, 10);

            // Create entity on the grid
            _entity = EntityManager.Create<BaseEntity>(new Point(1, 1), _grid);
            _entity.FieldOfViewRadius = Constants.Player.FieldOfViewRadius;
        }

        [TearDown]
        public void TearDown()
        {
            EntityManager.Clear();
        }

        [Test]
        public void CorrectGlyphs_AreShown()
        {
            var expectedResult = new[] { '#' };
            _fovWindow.Update(_entity);
            Assert.IsTrue(_fovWindow.SequenceEqual(expectedResult, out IEnumerable<char> result), GetExpectedMessage(expectedResult, result));

            AddGlyphToGrid('G', "Gargoyle Statue", 3, 2);

            _fovWindow.Update(_entity);
            expectedResult = new[] { '#', 'G' };
            Assert.IsTrue(_fovWindow.SequenceEqual(expectedResult, out result), GetExpectedMessage(expectedResult, result));

            AddGlyphToGrid('>', "Stairs up", 4, 3);

            _fovWindow.Update(_entity);
            expectedResult = new[] { '#', 'G', '>' };
            Assert.IsTrue(_fovWindow.SequenceEqual(expectedResult, out result), GetExpectedMessage(expectedResult, result));
        }

        [Test]
        public void CorrectGlyphs_AreShown_WhenExploredLightNearby()
        {
            var expectedResult = new[] { '#' };
            _entity.MoveTowards(new Point(10, 1));

            _fovWindow.Update(_entity);
            Assert.IsTrue(_fovWindow.SequenceEqual(expectedResult, out IEnumerable<char> result), GetExpectedMessage(expectedResult, result));

            expectedResult = new[] { '#', '.' };
            _entity.MoveTowards(new Point(10, 2));

            _fovWindow.Update(_entity);
            Assert.IsTrue(_fovWindow.SequenceEqual(expectedResult, out result), GetExpectedMessage(expectedResult, result));
        }

        private string GetExpectedMessage(IEnumerable<char> expected, IEnumerable<char> result)
        {
            return "Expected " + string.Join(",", expected.Select(a => "[" + a + "]")) + " but got: " + string.Join(",", result.Select(a => "[" + a + "]"));
        }

        private void AddLampToGrid(int posX, int posY)
        {
            // Add a lamp
            var bottomRightCorner = _grid.GetCell(posX, posY);
            bottomRightCorner.Glyph = '.';
            bottomRightCorner.CellProperties.NormalForeground = Color.BlanchedAlmond;
            bottomRightCorner.CellProperties.Name = "Lamp";
            bottomRightCorner.CellProperties.BlocksFov = false;
            bottomRightCorner.CellProperties.Walkable = true;
            bottomRightCorner.LightProperties.EmitsLight = true;
            bottomRightCorner.LightProperties.LightRadius = 5;
            bottomRightCorner.LightProperties.LightColor = Color.BlanchedAlmond;
            bottomRightCorner.LightProperties.Brightness = 0.5f;
            bottomRightCorner.CellProperties.IsExplored = true;
            _grid.SetCell(bottomRightCorner);
        }

        private void AddWallToGrid(int posX, int posY)
        {
            var sideCell = _grid.GetCell(posX, posY);
            sideCell.Glyph = '#';
            sideCell.CellProperties.NormalForeground = Color.White;
            sideCell.CellProperties.Name = "Wall";
            sideCell.CellProperties.BlocksFov = true;
            sideCell.CellProperties.Walkable = false;
            sideCell.CellProperties.IsExplored = true;
            _grid.SetCell(sideCell);
        }

        private void AddGlyphToGrid(char glyph, string name, int posX, int posY)
        {
            var cell = _grid.GetCell(posX, posY);
            cell.Glyph = glyph;
            cell.CellProperties.NormalForeground = Color.White;
            cell.CellProperties.Name = name;
            cell.CellProperties.Walkable = true;
            cell.CellProperties.BlocksFov = false;
            cell.CellProperties.IsExplored = true;
            _grid.SetCell(cell);
        }
    }
}
