using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Tests.TestObjects.Blueprints
{
    public class BaseBlueprint : Blueprint<EmberCell>
    { 
        public BaseBlueprint() : base(Constants.Blueprint.TestBlueprintsPath)
        { }
    }
}
