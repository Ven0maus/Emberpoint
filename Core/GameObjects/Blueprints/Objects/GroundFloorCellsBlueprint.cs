using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Blueprints.Objects;
using Emberpoint.Core.GameObjects.Map;

namespace Emberpoint.Core.GameObjects.Blueprints
{
    public class GroundFloorCellsBlueprint : CellBlueprint<EmberCell>
    {
        public override CellBlueprint<EmberCell> StairsDownBlueprint { get { return new BasementCellsBlueprint(); } }
    }
}
