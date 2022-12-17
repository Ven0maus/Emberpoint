using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Tests.TestObjects.Blueprints
{
    public class BaseBlueprintCells : CellBlueprint<EmberCell>
    {
        public override CellBlueprint<EmberCell> StairsUpBlueprint => new BaseBlueprintExtraCells();
        public BaseBlueprintCells() : base(Constants.Blueprint.Tests.TestCellBlueprintsPath)
        { }
    }
}
