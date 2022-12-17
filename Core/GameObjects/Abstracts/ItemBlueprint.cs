using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Emberpoint.Core.GameObjects.Abstracts
{
    public abstract class ItemBlueprint<T> : Blueprint
        where T : EmberItem
    {
        public int GridSizeX { get; private set; }
        public int GridSizeY { get; private set; }
        public string CellBlueprintPath { get; private set; }
        public string ItemBlueprintPath { get; private set; }

        public const char DefaultCellChar = '#';

        private readonly int _cellBlueprintId;

        protected ItemBlueprint(int cellBlueprintId, string blueprintName)
        {
            _cellBlueprintId = cellBlueprintId;
            InitializeBlueprint(blueprintName);
        }

        private void InitializeBlueprint(string blueprintName = null)
        {
            // Take the class name of the Blueprint class provided
            blueprintName ??= GetType().Name;
            ItemBlueprintPath = Path.Combine(Constants.Blueprint.ItemsBlueprintsDirectoryPath, blueprintName + ".txt");
            CellBlueprintPath = Path.Combine(Constants.Blueprint.CellBlueprintsDirectoryPath, 
                blueprintName.Replace("Items", "Cells") + ".txt");
            if (!File.Exists(ItemBlueprintPath))
                throw new Exception("Blueprint file was not found for '" + GetType().Name + "'.");

            // General configurations
            ValidateConfigurationPaths();

            var blueprint = File.ReadAllText(ItemBlueprintPath).Replace("\r", "").Split('\n');

            GridSizeX = blueprint.Max(a => a.Length);
            GridSizeY = blueprint.Length;
        }

        /// <summary>
        /// Retrieves the items from the blueprint.txt file and blueprint.json config file.
        /// Items are not cached by default.
        /// </summary>
        /// <returns></returns>
        public T[] GetCells()
        {
            var configs = GetConfigurationsDictionary();
            var specialChars = configs["SpecialCharacters"].ToDictionary(a => a.Glyph, a => a);
            var itemsNamespace = typeof(EmberItem).Namespace;
            var typeCache = configs["Items"].ToDictionary(a => a.Class, a => Type.GetType(itemsNamespace + "." + a.Class));
            var itemCache = configs["Items"].ToDictionary(a => a.Class, a => (EmberItem)Activator.CreateInstance(typeCache[a.Class]));
            var itemChars = configs["Items"].ToDictionary(a => (char)itemCache[a.Class].Glyph, a => a);
            var name = GetType().Name;

            // Check for special characters in blueprint
            foreach (var item in itemChars)
            {
                if (specialChars.ContainsKey(item.Key))
                    throw new Exception("Glyph '" + item.Key + "': is reserved as a special character and cannot be used in " + name);
            }

            var blueprint = File.ReadAllText(ItemBlueprintPath).Replace("\r", "").Split('\n');

            var items = new List<T>();
            for (int y = 0; y < GridSizeY; y++)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    char charValue;
                    if (y >= blueprint.Length || x >= blueprint[y].Length)
                    {
                        continue;
                    }
                    else
                    {
                        charValue = blueprint[y][x];
                    }

                    var position = new Point(x, y);

                    // Skip non item characters
                    if (charValue == DefaultCellChar || !itemChars.TryGetValue(charValue, out BlueprintTile config))
                        continue;

                    // Create actual item instance
                    var item = (T)EntityManager.Create(typeCache[config.Class], position, _cellBlueprintId);
                    items.Add(item);
                }
            }
            return items.ToArray();
        }
    }
}
