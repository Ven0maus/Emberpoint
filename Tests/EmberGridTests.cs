using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Tests.TestObjects.Grids;

namespace Tests
{
    [TestFixture]
    public class EmberGridTests
    {
        protected BaseGrid _grid;

        [SetUp]
        protected virtual void Setup()
        {
            _grid = BaseGrid.Create(10, 10);
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
            var prevValue = cell.CellProperties.Walkable;
            cell.CellProperties.Walkable = !cell.CellProperties.Walkable;
            _grid.SetCell(cell);

            Assert.AreEqual(!prevValue, _grid.GetCell(0, 0).CellProperties.Walkable);
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
