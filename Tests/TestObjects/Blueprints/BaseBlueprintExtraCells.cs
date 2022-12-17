using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Tests.TestObjects.Blueprints
{
    public class BaseBlueprintExtraCells : CellBlueprint<EmberCell>
    {
        public override CellBlueprint<EmberCell> StairsDownBlueprint => new BaseBlueprintCells();
        public BaseBlueprintExtraCells() : base(Constants.Blueprint.Tests.TestCellBlueprintsPath)
        { }
    }
}
