using Emberpoint.Core.GameObjects.Managers;
using NUnit.Framework;
using Tests.TestObjects.Blueprints;
using Tests.TestObjects.Grids;

namespace Tests
{
    [TestFixture]
    public class BlueprintTests : EmberGridTests
    {
        [SetUp]
        protected override void Setup()
        {
            // Setup a grid based on a blueprint
            _grid = BaseGrid.Create(new BaseBlueprint());
            GridManager.InitializeCustomGrid(_grid);
        }

        [Test]
        public void ConvertBlueprintToCells_DoesNotFail()
        {
            Assert.IsNotNull(_grid.Blueprint);

            var cells = _grid.Blueprint.GetCells();
            Assert.IsNotNull(cells);
            Assert.IsTrue(cells.Length > 0);
        }
    }
}
