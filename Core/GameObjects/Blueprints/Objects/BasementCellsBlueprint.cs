using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;

namespace Emberpoint.Core.GameObjects.Blueprints.Objects
{
    public class BasementCellsBlueprint : CellBlueprint<EmberCell>
    {
        public override CellBlueprint<EmberCell> StairsUpBlueprint { get { return new GroundFloorCellsBlueprint(); } }
    }
}
