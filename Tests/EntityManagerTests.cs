using System;
using System.Collections.Generic;
using System.Text;
using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Entities.Items;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.UserInterface.Windows;
using GoRogue;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SadConsole.Entities;
using Console = SadConsole.Console;
using GameWindow = Microsoft.Xna.Framework.GameWindow;

namespace Tests
{
    [TestFixture]
    public class EntityManagerTests
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
        public void EntityManager_EntityExistsAt_True()
        {
            var entity = EntityManager.Create<EntityManagerTests.TestEntity>(new Point(0, 0));
            Assert.IsTrue(EntityManager.EntityExistsAt(0,0));
        }

        /// <summary>
        /// Mock of the entity class
        /// </summary>
        private class TestEntity : IEntity
        {
            public Point Position { get; set; }

            public int FieldOfViewRadius { get; set; } = 0;

            private FOV _fieldOfView;

            public FOV FieldOfView
            {
                get { return _fieldOfView ?? (_fieldOfView = new FOV(_grid.FieldOfView)); }
            }

            public int ObjectId { get; }

            public TestEntity()
            {
                ObjectId = EntityManager.GetUniqueId();
            }
            
            public void ResetFieldOfView()
            {
                _fieldOfView = null;
            }

            public void MoveTowards(Point position, bool checkCanMove = true)
            {
                throw new NotImplementedException();
            }

            public bool CanMoveTowards(Point position)
            {
                throw new NotImplementedException();
            }

            public void RenderObject(Console console)
            {
                throw new NotImplementedException();
            }

            public void UnRenderObject()
            {
                throw new NotImplementedException();
            }
        }

    }
}
