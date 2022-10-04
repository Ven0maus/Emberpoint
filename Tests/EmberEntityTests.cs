﻿using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using NUnit.Framework;
using SadRogue.Primitives;
using Tests.TestObjects.Blueprints;
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
            GridManager.ClearCache();
            _grid = BaseGrid.Create(10, 10);
        }

        [TearDown]
        public void TearDown()
        {
            EntityManager.Clear(false);
        }

        [Test]
        public void EntitiesSynced_AfterPlayerMoves_ToNewBlueprint()
        {
            GridManager.InitializeBlueprint<BaseBlueprintExtra>(false);

            var entity1 = EntityManager.Create<BaseEntity>(new Point(1, 1), -1, _grid);
            var entity2 = EntityManager.Create<BaseEntity>(new Point(2, 1), -1, _grid);
            Assert.AreEqual(entity1.CurrentBlueprintId, entity2.CurrentBlueprintId);

            // Set entity1's blueprint layer
            entity1.MoveToBlueprint(GridManager.ActiveBlueprint);
            entity1.ChangeGrid(GridManager.Grid);

            // Sync entities
            var entities = EntityManager.GetEntities<BaseEntity>();
            foreach (var entity in entities)
            {
                if (entity.CurrentBlueprintId == GridManager.ActiveBlueprint.ObjectId)
                {
                    // Make all entities on current blueprint visible
                    entity.IsVisible = true;
                }
                else
                {
                    // Make all entities on another blueprint invisible
                    entity.IsVisible = false;
                }
            }

            Assert.AreEqual(GridManager.ActiveBlueprint.ObjectId, entity1.CurrentBlueprintId);
            Assert.AreNotEqual(GridManager.ActiveBlueprint.ObjectId, entity2.CurrentBlueprintId);
            Assert.AreEqual(true, entity1.IsVisible);
            Assert.AreEqual(false, entity2.IsVisible);

            var stairsDown = GridManager.Grid.GetCell(a => a.CellProperties.Name == Strings.StairsDown);
            Assert.IsNotNull(stairsDown);

            // Add an additional effect to adjust grid for this entity
            stairsDown.EffectProperties.AddMovementEffect((e) =>
            {
                ((BaseEntity)e).ChangeGrid(GridManager.Grid);
            });

            // Move onto stairs down, and trigger its effect
            var prevBlueprint = GridManager.ActiveBlueprint.ObjectId;
            entity1.MoveTowards(stairsDown.Position, false, triggerMovementEffects: true);
            Assert.AreNotEqual(GridManager.ActiveBlueprint.ObjectId, prevBlueprint);
            Assert.AreEqual(GridManager.ActiveBlueprint.ObjectId, entity1.CurrentBlueprintId);
        }

        [Test]
        public void EntityObjectId_IncrementsCorrectly()
        {
            var entities = new[]
            {
                EntityManager.Create<BaseEntity>(new Point(0, 0), -1),
                EntityManager.Create<BaseEntity>(new Point(1, 0), -1),
            };

            Assert.AreEqual(0, entities[0].ObjectId);
            Assert.AreEqual(1, entities[1].ObjectId);
        }

        [Test]
        public void Entity_CanMoveTowards_IsCorrect()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
            var cell = _grid.GetCell(0, 1);
            cell.CellProperties.Walkable = false;
            _grid.SetCell(cell, true);

            Assert.IsFalse(entity.CanMoveTowards(new Point(0, 1)));
            Assert.IsTrue(entity.CanMoveTowards(new Point(1, 0)));
        }

        [Test]
        public void Entity_MoveTowards_PositionChangeIsCorrect()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
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
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
            entity.FieldOfViewRadius = 5;
            entity.FieldOfView.Calculate(entity.Position, entity.FieldOfViewRadius);

            Assert.IsTrue(entity.FieldOfView.BooleanResultView[3,0]);
            Assert.IsFalse(entity.FieldOfView.BooleanResultView[6, 0]);

            var cell = _grid.GetCell(2, 0);
            cell.CellProperties.BlocksFov = true;
            _grid.SetCell(cell, true);

            Assert.IsFalse(entity.FieldOfView.BooleanResultView[3, 0]);
        }

        [Test]
        public void Entity_CellContainsEntity_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
            Assert.IsTrue(_grid.ContainsEntity(entity.Position, -1));
            Assert.IsTrue(_grid.GetCell(entity.Position).ContainsEntity(-1));
        }

        [Test]
        public void Entity_CellIntegrationFacingRight_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
            entity.MoveTowards(new Point(1, 0), false);
            var cell = _grid.GetCell(2, 0);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;
            _grid.SetCell(cell);

            // We are facing the interactable cell, we should be able to interact
            Assert.IsTrue(entity.CheckInteraction());

            // Unset interactable
            cell.CellProperties.Interactable = false;
            _grid.SetCell(cell);

            // Set tile to the left interactable, but don't face left
            cell = _grid.GetCell(0, 0);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;

            // We are not facing the interactable cell, we should not be able to interact
            Assert.IsFalse(entity.CheckInteraction());
        }

        [Test]
        public void Entity_CellIntegrationFacingLeft_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(2, 0), -1, _grid);
            entity.MoveTowards(new Point(1, 0), false);
            var cell = _grid.GetCell(0, 0);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;
            _grid.SetCell(cell);

            // We are facing the interactable cell, we should be able to interact
            Assert.IsTrue(entity.CheckInteraction());

            // Unset interactable
            cell.CellProperties.Interactable = false;
            _grid.SetCell(cell);

            // Set tile to the right interactable, but don't face right
            cell = _grid.GetCell(2, 0);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;

            // We are not facing the interactable cell, we should not be able to interact
            Assert.IsFalse(entity.CheckInteraction());
        }

        [Test]
        public void Entity_CellIntegrationFacingUp_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 2), -1, _grid);
            entity.MoveTowards(new Point(0, 1), false);
            var cell = _grid.GetCell(0, 0);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;
            _grid.SetCell(cell);

            // We are facing the interactable cell, we should be able to interact
            Assert.IsTrue(entity.CheckInteraction());

            // Unset interactable
            cell.CellProperties.Interactable = false;
            _grid.SetCell(cell);

            // Set tile downwards interactable, but don't face down
            cell = _grid.GetCell(0, 2);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;

            // We are not facing the interactable cell, we should not be able to interact
            Assert.IsFalse(entity.CheckInteraction());
        }

        [Test]
        public void Entity_CellIntegrationFacingDown_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
            entity.MoveTowards(new Point(0, 1), false);
            var cell = _grid.GetCell(0, 2);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;
            _grid.SetCell(cell);

            // We are facing the interactable cell, we should be able to interact
            Assert.IsTrue(entity.CheckInteraction());

            // Unset interactable
            cell.CellProperties.Interactable = false;
            _grid.SetCell(cell);

            // Set tile downwards interactable, but don't face down
            cell = _grid.GetCell(0, 0);
            cell.CellProperties.Interactable = true;
            cell.CellProperties.IsExplored = true;

            // We are not facing the interactable cell, we should not be able to interact
            Assert.IsFalse(entity.CheckInteraction());
        }

        [Test]
        public void Entity_CellIntegrationNotExplored_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
            entity.MoveTowards(new Point(1, 0)); // facing right
            var cell = _grid.GetCell(2, 0);
            cell.CellProperties.Interactable = true;
            _grid.SetCell(cell);
            Assert.IsFalse(entity.CheckInteraction());
        }

        [Test]
        public void Entity_CellIntegrationNotInteractable_Correct()
        {
            var entity = EntityManager.Create<BaseEntity>(new Point(0, 0), -1, _grid);
            entity.MoveTowards(new Point(1, 0)); // facing right
            var cell = _grid.GetCell(2, 0);
            cell.CellProperties.Interactable = false;
            cell.CellProperties.IsExplored = true;
            _grid.SetCell(cell);
            Assert.IsFalse(entity.CheckInteraction());
        }
    }
}
