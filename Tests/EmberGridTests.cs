using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System.Linq;
using Tests.TestObjects.Blueprints;
using Tests.TestObjects.Entities;
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
        public void CanChangeBlueprintByCellEffect()
        {
            GridManager.InitializeBlueprint<BaseBlueprint>(true);
            var currentBlueprint = GridManager.ActiveBlueprint;

            Assert.IsFalse(GridManager.Grid.GetCell(1, 1).LightProperties.EmitsLight);
            var exploreACell = GridManager.Grid.GetCell(0, 0);
            exploreACell.CellProperties.BlocksFov = true;
            GridManager.Grid.SetCell(exploreACell, false, false);
            Assert.IsTrue(GridManager.Grid.GetCell(0, 0).CellProperties.BlocksFov);

            var up = GridManager.Grid.GetCells(a => a.Glyph == '>').FirstOrDefault();
            Assert.IsNotNull(up);

            // Add effect to go up
            up.EffectProperties.EntityMovementEffects.Clear();
            up.EffectProperties.AddMovementEffect((e) =>
            {
                // Initialize new blueprint with tracking of the previous
                GridManager.InitializeBlueprint<BaseBlueprintExtra>(true);
                ((BaseEntity)e).MoveToBlueprint(GridManager.ActiveBlueprint);
            });

            var entity = EntityManager.Create<BaseEntity>(new Point(2, 3), GridManager.Grid);

            Assert.IsTrue(entity.CurrentBlueprintId == currentBlueprint.ObjectId);
            Assert.IsTrue(entity.CurrentBlueprintId == GridManager.ActiveBlueprint.ObjectId);

            // Move entity into new blueprint by stairs logic
            entity.MoveTowards(up.Position, false, triggerMovementEffects: true);

            Assert.IsTrue(entity.CurrentBlueprintId != currentBlueprint.ObjectId);
            Assert.IsTrue(entity.CurrentBlueprintId == GridManager.ActiveBlueprint.ObjectId);

            // Map is reloaded at this point, find way down
            var down = GridManager.Grid.GetCells(a => a.Glyph == '<').FirstOrDefault();
            Assert.IsNotNull(down);

            // Add effect to go back down
            down.EffectProperties.EntityMovementEffects.Clear();
            down.EffectProperties.AddMovementEffect((e) =>
            {
                // Initialize new blueprint with tracking of the previous
                GridManager.InitializeBlueprint<BaseBlueprint>(true);
                ((BaseEntity)e).MoveToBlueprint(GridManager.ActiveBlueprint);
            });

            entity.MoveTowards(down.Position, false, triggerMovementEffects: false);
            entity.ChangeGrid(GridManager.Grid);

            // Check if a cell corresponds to the correct grid after change
            Assert.IsTrue(GridManager.Grid.GetCell(1, 1).LightProperties.EmitsLight);

            // Move away from stairs first
            entity.MoveTowards(new Point(2, 3), false, triggerMovementEffects: false);
            // Move back downwards
            entity.MoveTowards(down.Position, false, triggerMovementEffects: true);

            Assert.IsTrue(entity.CurrentBlueprintId == currentBlueprint.ObjectId);
            Assert.IsTrue(entity.CurrentBlueprintId == GridManager.ActiveBlueprint.ObjectId);

            // Verify if map changed
            Assert.IsFalse(GridManager.Grid.GetCell(1, 1).LightProperties.EmitsLight);

            // Verify if changes are tracked
            Assert.IsTrue(GridManager.Grid.GetCell(0, 0).CellProperties.BlocksFov);
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
            Assert.AreEqual(_grid.GetNeighbors(cell).Count(), 3);
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
