using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Blueprints.Objects;
using Emberpoint.Core.GameObjects.Interfaces;

namespace Tests.TestObjects.Blueprints
{
    public class GenericTestItemBlueprint : GenericItemBlueprint<IEntity>
    {
        public GenericTestItemBlueprint(int cellBlueprintId, string blueprintName)
            : base(cellBlueprintId, blueprintName, Constants.Blueprint.Tests.TestCellBlueprintsPath, 
                  Constants.Blueprint.Tests.TestItemBlueprintsPath)
        { }
    }
}
