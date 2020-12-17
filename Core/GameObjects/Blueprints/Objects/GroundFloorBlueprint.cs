using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Blueprints.Objects;
using Emberpoint.Core.GameObjects.Map;

namespace Emberpoint.Core.GameObjects.Blueprints
{
    public class GroundFloorBlueprint : Blueprint<EmberCell>
    {
        public override Blueprint<EmberCell> StairsDownBlueprint { get { return new BasementBlueprint(); } }
    }
}
