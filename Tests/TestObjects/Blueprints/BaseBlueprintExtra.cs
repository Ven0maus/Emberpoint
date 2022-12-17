using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Tests.TestObjects.Blueprints
{
    public class BaseBlueprintExtra : CellBlueprint<EmberCell>
    {
        public override CellBlueprint<EmberCell> StairsDownBlueprint => new BaseBlueprint();
        public BaseBlueprintExtra() : base(Constants.Blueprint.Tests.TestBlueprintsPath)
        { }
    }
}
