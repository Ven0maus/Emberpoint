using Emberpoint.Core.GameObjects.Map;
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
        public string BlueprintPath { get; private set; }

        public Blueprint()
        {
            InitializeBlueprint(Constants.Blueprint.BlueprintsPath);
        }

        protected Blueprint(string customPath)
        {
            InitializeBlueprint(customPath);
        }

        private void InitializeBlueprint(string path)
        {
            BlueprintPath = path;
            var blueprintPath = Path.Combine(BlueprintPath, GetType().Name + ".txt");
            var blueprintConfigPath = Path.Combine(Constants.Blueprint.BlueprintsConfigPath, Constants.Blueprint.BlueprintTiles + ".json");
            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));

            if (!File.Exists(blueprintConfigPath) || !File.Exists(blueprintPath))
                throw new Exception("Blueprint config file(s) were not found for " + Constants.Blueprint.BlueprintTiles);

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            GridSizeX = blueprint.Max(a => a.Length);
            GridSizeY = blueprint.Length;
        }

        /// <summary>
        /// Retrieves the cells from the blueprint.txt file and blueprint.json config file.
        /// Cells are not cached by default.
        /// </summary>
        /// <returns></returns>
        public T[] GetCells()
        {
            var name = GetType().Name;
            var blueprintPath = Path.Combine(BlueprintPath, name + ".txt");
            var blueprintConfigPath = Path.Combine(Constants.Blueprint.BlueprintsConfigPath, Constants.Blueprint.BlueprintTiles + ".json");

            if (!File.Exists(blueprintPath) || !File.Exists(blueprintConfigPath) || !File.Exists(Constants.Blueprint.SpecialCharactersPath)) 
                return Array.Empty<T>();

            var specialConfig = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(Constants.Blueprint.SpecialCharactersPath));
            var specialChars = specialConfig.Tiles.ToDictionary(a => a.Glyph, a => a);

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var tiles = config.Tiles.ToDictionary(a => (char?) a.Glyph, a => a);
            var nullTile = BlueprintTile.Null();

            foreach (var tile in tiles)
            {
                if (tile.Key == null) continue;
                if (specialChars.ContainsKey(tile.Key.Value))
                    throw new Exception("Glyph '" + tile.Key.Value + "': is reserved as a special character and cannot be used in " + name);
            }

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            var cells = new List<T>();
            for (int y=0; y < GridSizeY; y++)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    char? charValue;

                    if (y >= blueprint.Length || x >= blueprint[y].Length)
                    {
                        charValue = null;
                    } else {
                        charValue = blueprint[y][x];
                    }
                    
                    var position = new Point(x, y);
                    BlueprintTile tile = nullTile;
                    if (charValue !=null && !tiles.TryGetValue(charValue, out tile)) 
                        throw new Exception("Glyph '" + charValue + "' was not present in the config file for blueprint: " + name);
                    var foregroundColor = GetColorByString(tile.Foreground);
                    var backgroundColor = tile.Background != null ? GetColorByString(tile.Background) : Color.Black;
                    var cell = new T()
                    {
                        Glyph = tile.Glyph,
                        Position = position,
#pragma warning disable CS0618 // Type or member is obsolete
                        Foreground = foregroundColor,
                        Background = backgroundColor,
#pragma warning restore CS0618 // Type or member is obsolete
                        CellProperties = new EmberCell.EmberCellProperties
                        {
                            NormalForeground = foregroundColor,
                            NormalBackground = backgroundColor,
                            ForegroundFov = Color.Lerp(foregroundColor, Color.Black, .5f),
                            BackgroundFov = Color.Lerp(backgroundColor, Color.Black, .5f),
                            Walkable = tile.Walkable,
                            Name = tile.Name,
                            BlocksFov = tile.BlocksFov,
                        },
                        LightProperties = new EmberCell.LightEngineProperties
                        {
                            EmitsLight = tile.EmitsLight,
                            LightRadius = tile.LightRadius,
                            Brightness = tile.Brightness
                        }
                    };

                    if (!string.IsNullOrWhiteSpace(tile.LightColor))
                    {
                        cell.LightProperties.LightColor = GetColorByString(tile.LightColor);
                    }

                    cells.Add(cell);
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
    }

    [Serializable]
    internal class BlueprintConfig
    {
#pragma warning disable 0649
        public BlueprintTile[] Tiles;
#pragma warning restore 0649
    }

    [Serializable]
    internal class BlueprintTile
    {
#pragma warning disable 0649
        public char Glyph;
        public string Name;
        public bool Walkable;
        public string Foreground;
        public string Background;
        public bool BlocksFov;
        public bool EmitsLight;
        public string LightColor;
        public int LightRadius;
        public float Brightness;
#pragma warning restore 0649
        public static BlueprintTile Null()
        {
            var nullTile = new BlueprintTile
            {
                Glyph = ' ',
                Foreground = "BurlyWood",
                Background = "Black",
                Name = null,
                Walkable = false
            };

            return nullTile;
        }
    }
}
