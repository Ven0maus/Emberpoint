using Emberpoint.Core.GameObjects.Blueprints;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class BlueprintTests : EmberGridTests
    {
        [SetUp]
        protected override void Setup()
        {
            // Setup a grid based on a blueprint
            _grid = new EmberGrid(new GroundFloorBlueprint());
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
