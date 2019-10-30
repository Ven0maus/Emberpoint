using Emberpoint.Core.GameObjects;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using GoRogue;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using static SadConsole.Entities.Entity;
using Console = SadConsole.Console;

namespace Tests
{
    [TestFixture]
    public class EmberEntityTests
    {
        private static EmberGrid _grid;

        [SetUp]
        public void SetUp()
        {
            _grid = EmberGridTests.BuildCustomGrid(10, 10);
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
                EntityManager.Create<TestEntity>(new Point(0, 0)),
                EntityManager.Create<TestEntity>(new Point(1, 0)),
            };

            Assert.AreEqual(0, entities[0].ObjectId);
            Assert.AreEqual(1, entities[1].ObjectId);
        }

        [Test]
        public void Entity_CanMoveTowards_IsCorrect()
        {
            var entity = EntityManager.Create<TestEntity>(new Point(0, 0));
            var cell = _grid.GetCell(0, 1);
            cell.Walkable = false;
            _grid.SetCell(cell, true);

            Assert.IsFalse(entity.CanMoveTowards(new Point(0, 1)));
            Assert.IsTrue(entity.CanMoveTowards(new Point(1, 0)));
        }

        [Test]
        public void Entity_MoveTowards_PositionChangeIsCorrect()
        {
            var entity = EntityManager.Create<TestEntity>(new Point(0, 0));
            var cell = _grid.GetCell(0, 1);
            cell.Walkable = false;
            _grid.SetCell(cell, true);

            entity.MoveTowards(new Point(0, 1));
            Assert.AreEqual(new Point(0, 0), entity.Position);
            entity.MoveTowards(new Point(1, 0));
            Assert.AreEqual(new Point(1, 0), entity.Position);
        }

        [Test]
        public void Entity_IsFieldOfView_Correct()
        {
            var entity = EntityManager.Create<TestEntity>(new Point(0, 0));
            entity.FieldOfViewRadius = 5;
            entity.FieldOfView.Calculate(entity.Position, entity.FieldOfViewRadius);

            Assert.IsTrue(entity.FieldOfView.BooleanFOV[3,0]);
            Assert.IsFalse(entity.FieldOfView.BooleanFOV[6, 0]);

            var cell = _grid.GetCell(2, 0);
            cell.BlocksFov = true;
            _grid.SetCell(cell, true);

            Assert.IsFalse(entity.FieldOfView.BooleanFOV[3, 0]);
        }

        /// <summary>
        /// Mock of the entity class
        /// </summary>
        private class TestEntity : IEntity
        {
            public Point Position { get; set; }

            public EventHandler<EntityMovedEventArgs> Moved;

            public int FieldOfViewRadius { get; set; } = 0;

            private FOV _fieldOfView;
            public FOV FieldOfView
            {
                get
                {
                    return _fieldOfView ?? (_fieldOfView = new FOV(_grid.FieldOfView));
                }
            }

            public int ObjectId { get; }

            public TestEntity()
            {
                ObjectId = EntityManager.GetUniqueId();
                Moved += OnMove;
            }

            private void OnMove(object sender, EntityMovedEventArgs args)
            {
                if (FieldOfViewRadius > 0)
                {
                    // Re-calculate the field of view
                    FieldOfView.Calculate(Position, FieldOfViewRadius);
                }
            }

            public bool CanMoveTowards(Point position)
            {
                return _grid.InBounds(position) && _grid.GetCell(position).Walkable && !EntityManager.EntityExistsAt(position);
            }

            public void MoveTowards(Point position, bool checkCanMove = true)
            {
                if (checkCanMove && !CanMoveTowards(position)) return;
                var prevPos = Position;
                Position = position;
                Moved.Invoke(this, new EntityMovedEventArgs(null, prevPos));
            }

            public void RenderObject(Console console)
            {
                throw new NotImplementedException();
            }

            public void ResetFieldOfView()
            {
                _fieldOfView = null;
            }
        }
    }
}
