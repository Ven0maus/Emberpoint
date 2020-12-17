using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Tests.TestObjects.Blueprints
{
    public class BaseBlueprintExtra : Blueprint<EmberCell>
    {
        public override Blueprint<EmberCell> StairsDownBlueprint => new BaseBlueprint();
        public BaseBlueprintExtra() : base(Constants.Blueprint.Tests.TestBlueprintsPath)
        { }
    }
}
