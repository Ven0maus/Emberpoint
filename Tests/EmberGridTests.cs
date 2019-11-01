using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class EmberGridTests
    {
        protected EmberGrid _grid;

        public static EmberGrid BuildCustomGrid(int width, int height)
        {
            // Build a custom cell grid
            var cells = new EmberCell[width * height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    cells[y * width + x] = new EmberCell
                    {
                        Background = Color.Black,
                        Foreground = Color.Gray,
                        Glyph = '.',
                        Name = "floor",
                        Position = new Point(x, y),
                        Walkable = true
                    };
                }
            }
            return new EmberGrid(width, height, cells);
        }

        [SetUp]
        protected virtual void Setup()
        {
            _grid = BuildCustomGrid(10, 10);
            GridManager.InitializeCustomGrid(_grid);
        }

        [Test]
        public void CanRetrieveCell()
        {
            var cell = _grid.GetCell(0, 0);
            Assert.IsNotNull(cell);
            Assert.IsNotNull(_grid.GetCell(new Point(0, 0)));
        }

        [Test]
        public void CanModifyCell()
        {
            var cell = _grid.GetCell(0, 0);
            var prevValue = cell.Walkable;
            cell.Walkable = !cell.Walkable;
            _grid.SetCell(cell);

            Assert.AreEqual(!prevValue, _grid.GetCell(0, 0).Walkable);
        }

        [Test]
        public void CanRetrieveNeighborsOfCell()
        {
            var cell = _grid.GetCell(0, 0);
            Assert.AreEqual(_grid.GetNeighbors(cell).Length, 3);
        }

        [Test]
        public void InBoundsCheck_ReturnsCorrectResult() 
        {
            Assert.IsFalse(_grid.InBounds(-5, 10));
            Assert.IsTrue(_grid.InBounds(0, 0));
            Assert.IsFalse(_grid.InBounds(_grid.GridSizeX + 2, _grid.GridSizeY + 2));
        }
    }
}
