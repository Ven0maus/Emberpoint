using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Emberpoint.Core.Objects.Abstracts
{
    public abstract class Blueprint<T> where T : EmberCell, new()
    {
        public virtual T[] GetCells()
        {
            var name = GetType().Name;
            var root = GetApplicationRoot();
            var blueprintPath = Path.Combine(root, "Core", "Objects", "Blueprints", name + ".txt");
            var blueprintConfigPath = Path.Combine(root, "Core", "Objects", "Blueprints", "Config", name + ".json");

            if (!File.Exists(blueprintPath) || !File.Exists(blueprintConfigPath)) return Array.Empty<T>();

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            var cells = new List<T>();
            var tiles = config.Tiles.ToDictionary(a => a.Glyph, a => a);
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
                    cells.Add(new T() 
                    { 
                        Glyph = tile.Glyph, 
                        Position = position, 
                        Foreground = foregroundColor, 
                        Walkable = tile.Walkable,
                        Name = tile.Name
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

        private string GetApplicationRoot()
        {
            var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("\\bin"));
            return appRoot.Substring(0, appRoot.LastIndexOf("\\") + 1);
        }

        [Serializable]
        private class BlueprintConfig
        {
            public int GridSizeX;
            public int GridSizeY;
            public BlueprintTile[] Tiles;
        }

        [Serializable]
        private class BlueprintTile
        {
            public char Glyph;
            public string Name;
            public bool Walkable;
            public string Foreground;
        }
    }
}
