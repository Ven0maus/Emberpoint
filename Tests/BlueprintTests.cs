using Emberpoint.Core.GameObjects.Managers;
using NUnit.Framework;
using System.IO;
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
            var cellBlueprint = new BaseBlueprintCells();
            _grid = BaseGrid.Create(cellBlueprint, new GenericTestItemBlueprint(cellBlueprint.ObjectId, 
                cellBlueprint.GetType().Name.Replace("Cells", "Items")));
            GridManager.InitializeCustomGrid(_grid);
        }

        [Test]
        public void ConvertBlueprintToCells_DoesNotFail()
        {
            Assert.IsNotNull(_grid.CellBlueprint);

            var cells = _grid.CellBlueprint.GetCells();
            Assert.IsNotNull(cells);
            Assert.IsTrue(cells.Length > 0);
        }

        [Test]
        public void BlueprintTextFile_MatchesConvertedGridLayout()
        {
            var testBlueprintFilePath = _grid.CellBlueprint.BlueprintPath;
            var fileContent = File.ReadAllText(testBlueprintFilePath);
            var blueprint = fileContent.Replace("\r", "").Split('\n');

            // Check if the loaded grid matches the content
            for (int y = 0; y < blueprint.Length; y++)
            {
                for (int x = 0; x < blueprint[y].Length; x++)
                {
                    Assert.AreEqual(blueprint[y][x], _grid.GetCell(x, y).Glyph);
                }
            }
        }
    }
}
