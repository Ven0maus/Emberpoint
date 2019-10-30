using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Abstracts
{
    public abstract class Blueprint<T> where T : EmberCell, new()
    {
        public int GridSizeX { get; private set; }
        public int GridSizeY { get; private set; }

        public Blueprint()
        {
            var blueprintConfigPath = Path.Combine(Constants.Blueprint.BlueprintsConfigPath, GetType().Name + ".json");
            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));

            if (!File.Exists(blueprintConfigPath))
                throw new Exception("Blueprint config file was not found for " + GetType().Name);

            GridSizeX = config.GridSizeX;
            GridSizeY = config.GridSizeY;
        }

        /// <summary>
        /// Retrieves the cells from the blueprint.txt file and blueprint.json config file.
        /// Cells are not cached by default.
        /// </summary>
        /// <returns></returns>
        public T[] GetCells()
        {
            var name = GetType().Name;
            var blueprintPath = Path.Combine(Constants.Blueprint.BlueprintsPath, name + ".txt");
            var blueprintConfigPath = Path.Combine(Constants.Blueprint.BlueprintsConfigPath, name + ".json");

            if (!File.Exists(blueprintPath) || !File.Exists(blueprintConfigPath) || !File.Exists(Constants.Blueprint.SpecialCharactersPath)) 
                return Array.Empty<T>();

            var specialConfig = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(Constants.Blueprint.SpecialCharactersPath));
            var specialChars = specialConfig.Tiles.ToDictionary(a => a.Glyph, a => a);

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var tiles = config.Tiles.ToDictionary(a => a.Glyph, a => a);

            foreach (var tile in tiles)
            {
                if (specialChars.ContainsKey(tile.Key))
                    throw new Exception("Glyph '" + tile.Key + "': is reserved as a special character and cannot be used in " + name);
            }

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            var cells = new List<T>();
            for (int y=0; y < config.GridSizeY; y++)
            {
                for (int x = 0; x < config.GridSizeX; x++)
                {
                    var charValue = blueprint[y][x];
                    var position = new Point(x, y);
                    BlueprintTile tile;
                    if (!tiles.TryGetValue(charValue, out tile)) 
                        throw new Exception("Glyph '" + charValue + "' was not present in the config file for blueprint: " + name);
                    var foregroundColor = GetColorByString(tile.Foreground);
                    var foregroundFovColor = GetColorByString(tile.ForegroundFov);
                    cells.Add(new T() 
                    { 
                        Glyph = tile.Glyph, 
                        Position = position, 
                        Foreground = foregroundColor, 
                        NormalForeground = foregroundColor,
                        ForegroundFov = foregroundFovColor,
                        Walkable = tile.Walkable,
                        Name = tile.Name,
                        BlocksFov = tile.BlocksFov
                    });
                }
            }
            return cells.ToArray();
        }

        private Color GetColorByString(string value)
        {
            var prop = typeof(Color).GetProperty(value);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return default;
        }

        [Serializable]
        private class BlueprintConfig
        {
#pragma warning disable 0649
            public int GridSizeX;
            public int GridSizeY;
            public BlueprintTile[] Tiles;
#pragma warning restore 0649
        }

        [Serializable]
        private class BlueprintTile
        {
#pragma warning disable 0649
            public char Glyph;
            public string Name;
            public bool Walkable;
            public string Foreground;
            public string ForegroundFov;
            public bool BlocksFov;
#pragma warning restore 0649
        }
    }
}
