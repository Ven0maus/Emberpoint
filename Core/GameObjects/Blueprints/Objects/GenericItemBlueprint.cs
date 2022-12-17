using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Items;
using System.IO;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Blueprints.Objects
{
    public class GenericItemBlueprint<T> : ItemBlueprint<T>
        where T : IEntity
    {
        public GenericItemBlueprint(int cellBlueprintId, string blueprintName) 
            : base(cellBlueprintId, blueprintName)
        {
            UpdateItemBlueprint();
        }

        public GenericItemBlueprint(int cellBlueprintId, string blueprintName, string customCellsPath, string customItemsPath) 
            : base(cellBlueprintId, blueprintName, customCellsPath, customItemsPath)
        {
            UpdateItemBlueprint();
        }

        private void UpdateItemBlueprint()
        {
            // Copies the cell blueprint text files over the item blueprints, without modifying the actual items.
            // Maps all the cells into a specific character defined in the constants
            var cellBlueprintText = File.ReadAllText(CellBlueprintPath);
            var itemBlueprintText = File.ReadAllText(ItemBlueprintPath);

            var cellBlueprint = cellBlueprintText.Replace("\r", "").Split('\n');
            var itemBlueprint = itemBlueprintText.Replace("\r", "").Split('\n');

            bool modified = false;
            if (cellBlueprint.Length != itemBlueprint.Length &&
                cellBlueprint.Max(a => a) != itemBlueprint.Max(a => a))
            {
                // Invalid item blueprint, does not match layout of cell blueprint.
                // So we copy the cell blueprint layout into it
                itemBlueprint = cellBlueprint;
                modified = true;
            }

            for (int y = 0; y < GridSizeY; y++)
            {
                var cArray = itemBlueprint[y].ToCharArray();
                for (int x = 0; x < GridSizeX; x++)
                {
                    char itemCharValue, cellCharValue;
                    if (y >= itemBlueprint.Length || x >= itemBlueprint[y].Length)
                    {
                        continue;
                    }
                    else
                    {
                        itemCharValue = itemBlueprint[y][x];
                        cellCharValue = cellBlueprint[y][x];
                    }

                    if (itemCharValue == cellCharValue && itemCharValue != DefaultCellChar && itemCharValue != ' ')
                    {
                        cArray[x] = DefaultCellChar;
                        modified = true;
                    }
                }
                itemBlueprint[y] = new string(cArray);
            }

            if (modified)
                File.WriteAllText(ItemBlueprintPath, string.Join("\n", itemBlueprint));
        }
    }

    public class GenericItemBlueprint : GenericItemBlueprint<EmberItem>
    {
        public GenericItemBlueprint(int cellBlueprintId, string blueprintName)
            : base(cellBlueprintId, blueprintName)
        { }

        public GenericItemBlueprint(int cellBlueprintId, string blueprintName, string customCellsPath, string customItemsPath)
            : base(cellBlueprintId, blueprintName, customCellsPath, customItemsPath)
        { }
    }
}
