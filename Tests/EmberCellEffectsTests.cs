using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Tests.TestObjects.Entities;
using Tests.TestObjects.Grids;

namespace Tests
{
    [TestFixture]
    public class EmberCellEffectsTests
    {
        private BaseGrid _grid;
        private IEntity _entity;

        [SetUp]
        public void SetUp()
        {
            _grid = BaseGrid.Create(10, 10);
            _entity = EntityManager.Create<BaseEntity>(new Point(0, 0), _grid);
            GridManager.InitializeCustomGrid(_grid);
        }

        [TearDown]
        public void TearDown()
        {
            EntityManager.Clear();
        }

        [Test]
        public void DeathEffect_Entity_IsKilledOnMovement()
        {
            Assert.IsTrue(_entity.Health > 0);

            // Set death effect on the cell
            var cell = _grid.GetCell(1, 0);
            cell.EffectProperties.AddMovementEffect((entity) =>
            {
                entity.TakeDamage(entity.Health);
            });
            _grid.SetCell(cell);

            Assert.IsTrue(_grid.GetCell(1,0).EffectProperties.EntityMovementEffects.Count == 1);
            Assert.IsTrue(_entity.Position == new Point(0, 0));
            _entity.MoveTowards(new Point(1, 0));
            Assert.IsTrue(_entity.Position == new Point(1, 0));
            Assert.IsTrue(_entity.Health == 0);

            Assert.IsFalse(EntityManager.EntityExistsAt(_entity.Position));
        }
    }
}
