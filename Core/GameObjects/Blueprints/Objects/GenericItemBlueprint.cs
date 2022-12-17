﻿using Emberpoint.Core.GameObjects.Abstracts;
using System.IO;
using System;
using System.Linq;
using Emberpoint.Core.GameObjects.Items;

namespace Emberpoint.Core.GameObjects.Blueprints.Objects
{
    public class GenericItemBlueprint : ItemBlueprint<EmberItem>
    {
        private readonly string _blueprintName;

        public GenericItemBlueprint(int cellBlueprintId, string blueprintName) : 
            base(cellBlueprintId, blueprintName)
        {
            _blueprintName = blueprintName;
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

            if (cellBlueprint.Length != itemBlueprint.Length &&
                cellBlueprint.Max(a => a) != itemBlueprint.Max(a => a))
                throw new Exception($"Invalid item blueprint({_blueprintName}), does not match layout of cell blueprint.");

            bool modified = false;
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
}
