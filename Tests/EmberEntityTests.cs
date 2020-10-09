using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Tests.TestObjects.Entities;
using Tests.TestObjects.Grids;

namespace Tests
{
    [TestFixture]
    public class EmberEntityTests
    {
        protected BaseGrid _grid;

        [SetUp]
        public void SetUp()
        {
            _grid = BaseGrid.Create(10, 10);
        }

        [TearDown]
        public void TearDown()
        {
            EntityManager.Clear();
        }

        [Test]
        public void EntityObjectId_IncrementsCorrectly()
        {
            var entities = new[]
            {
                EntityManager.Create<BaseEntity>(new Point(0, 0)),
                EntityManager.Create<BaseEntity>(new Point(1, 0)),
            };

            Assert.AreEqual(0, entities[0].ObjectId);
            Assert.AreEqual(1, entities[1].ObjectId);
        }

        [Test]
        public void Entity_CanMoveTowards_IsCorrect()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), _grid);
            var cell = _grid.GetCell(0, 1);
            cell.CellProperties.Walkable = false;
            _grid.SetCell(cell, true);

            Assert.IsFalse(entity.CanMoveTowards(new Point(0, 1)));
            Assert.IsTrue(entity.CanMoveTowards(new Point(1, 0)));
        }

        [Test]
        public void Entity_MoveTowards_PositionChangeIsCorrect()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), _grid);
            var cell = _grid.GetCell(0, 1);
            cell.CellProperties.Walkable = false;
            _grid.SetCell(cell, true);

            entity.MoveTowards(new Point(0, 1));
            Assert.AreEqual(new Point(0, 0), entity.Position);
            entity.MoveTowards(new Point(1, 0));
            Assert.AreEqual(new Point(1, 0), entity.Position);
        }

        [Test]
        public void Entity_IsFieldOfView_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), _grid);
            entity.FieldOfViewRadius = 5;
            entity.FieldOfView.Calculate(entity.Position, entity.FieldOfViewRadius);

            Assert.IsTrue(entity.FieldOfView.BooleanFOV[3,0]);
            Assert.IsFalse(entity.FieldOfView.BooleanFOV[6, 0]);

            var cell = _grid.GetCell(2, 0);
            cell.CellProperties.BlocksFov = true;
            _grid.SetCell(cell, true);

            Assert.IsFalse(entity.FieldOfView.BooleanFOV[3, 0]);
        }

        [Test]
        public void Entity_CellContainsEntity_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), _grid);
            Assert.IsTrue(_grid.ContainsEntity(entity.Position));
            Assert.IsTrue(_grid.GetCell(entity.Position).ContainsEntity());
        }
    }
}
