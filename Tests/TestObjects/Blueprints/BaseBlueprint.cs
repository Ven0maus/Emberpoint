using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Tests.TestObjects.Blueprints
{
    public class BaseBlueprint : Blueprint<EmberCell>
    {
        public override Blueprint<EmberCell> StairsUpBlueprint => new BaseBlueprintExtra();
        public BaseBlueprint() : base(Constants.Blueprint.Tests.TestBlueprintsPath)
        { }
    }
}
