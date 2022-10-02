using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.Resources;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Emberpoint.Core.GameObjects.Abstracts
{
    public abstract class Blueprint<T> : Blueprint
        where T : EmberCell, new()
    {
        public int ObjectId { get; private set; }
        public int GridSizeX { get; private set; }
        public int GridSizeY { get; private set; }

        public string BlueprintPath { get; private set; }

        public virtual Blueprint<T> StairsUpBlueprint { get; private set; }
        public virtual Blueprint<T> StairsDownBlueprint { get; private set; }

        public Blueprint()
        {
            ObjectId = BlueprintDatabase.GetUniqueId();
            InitializeBlueprint(Constants.Blueprint.BlueprintsDirectoryPath);
        }

        protected Blueprint(string customPath)
        {
            ObjectId = BlueprintDatabase.GetUniqueId();
            InitializeBlueprint(customPath);
        }

        private void InitializeBlueprint(string blueprintDirectoryPath)
        {
            BlueprintPath = Path.Combine(blueprintDirectoryPath, GetType().Name + ".txt");
            if (!File.Exists(BlueprintPath))
                throw new Exception("Blueprint file was not found for '" + GetType().Name + "'.");

            // General configurations
            ValidateConfigurationPaths();

            var blueprint = File.ReadAllText(BlueprintPath).Replace("\r", "").Split('\n');

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
            var configs = GetConfigurationsDictionary();
            var specialChars = configs["SpecialCharacters"].ToDictionary(a => a.Glyph, a => a);
            var tiles = configs["BlueprintTiles"].ToDictionary(a => (char?)a.Glyph, a => a);

            var nullTile = BlueprintTile.Null();
            var name = GetType().Name;

            // Check for special characters in blueprint
            foreach (var tile in tiles)
            {
                if (tile.Key == null) continue;
                if (specialChars.ContainsKey(tile.Key.Value))
                    throw new Exception("Glyph '" + tile.Key.Value + "': is reserved as a special character and cannot be used in " + name);
            }

            var blueprint = File.ReadAllText(BlueprintPath).Replace("\r", "").Split('\n');

            var cells = new List<T>();
            for (int y = 0; y < GridSizeY; y++)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    char? charValue;

                    if (y >= blueprint.Length || x >= blueprint[y].Length)
                    {
                        charValue = null;
                    }
                    else
                    {
                        charValue = blueprint[y][x];
                    }

                    var position = new Point(x, y);
                    BlueprintTile tile = nullTile;
                    if (charValue != null && !tiles.TryGetValue(charValue, out tile))
                        throw new Exception("Glyph '" + charValue + "' was not present in the config file for blueprint: " + name);
                    var foregroundColor = MonoGameExtensions.GetColorByString(tile.Foreground);
                    var backgroundColor = tile.Background != null ? MonoGameExtensions.GetColorByString(tile.Background) : Color.Black;
                    var cell = new T()
                    {
                        Glyph = tile.Glyph,
                        Position = position,
                        Foreground = foregroundColor,
                        Background = backgroundColor,
                        CellProperties = new EmberCell.EmberCellProperties
                        {
                            NormalForeground = foregroundColor,
                            NormalBackground = backgroundColor,
                            ForegroundFov = foregroundColor == Color.Transparent ? Color.Transparent : Color.Lerp(foregroundColor, Color.Black, .5f),
                            BackgroundFov = backgroundColor == Color.Transparent ? Color.Transparent : Color.Lerp(backgroundColor, Color.Black, .5f),
                            Walkable = tile.Walkable,
                            Interactable = tile.Interactable,
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

                    // Set cell effect for stairs
                    if (cell.CellProperties.Name != null && cell.CellProperties.Name.Equals(Strings.StairsDown, StringComparison.OrdinalIgnoreCase) && StairsDownBlueprint != null)
                    {
                        cell.EffectProperties.AddMovementEffect((entity) => AddStairsLogic(cell, tile));
                    }
                    else if (cell.CellProperties.Name != null && cell.CellProperties.Name.Equals(Strings.StairsUp, StringComparison.OrdinalIgnoreCase) && StairsUpBlueprint != null)
                    {
                        cell.EffectProperties.AddMovementEffect((entity) => AddStairsLogic(cell, tile));
                    }

                    if (!string.IsNullOrWhiteSpace(tile.LightColor))
                    {
                        cell.LightProperties.LightColor = MonoGameExtensions.GetColorByString(tile.LightColor);
                    }

                    cells.Add(cell);
                }
            }
            return cells.ToArray();
        }

        private void AddStairsLogic(T cell, BlueprintTile tile)
        {
            string stairsName = tile.Name.Equals(Strings.StairsUp, StringComparison.OrdinalIgnoreCase) ? Strings.StairsDown : Strings.StairsUp;
            Blueprint<T> blueprint = tile.Name.Equals(Strings.StairsUp, StringComparison.OrdinalIgnoreCase) ? StairsUpBlueprint : StairsDownBlueprint;

            // Initialize new blueprint with tracking of the previous
            GridManager.InitializeBlueprint(blueprint, true);

            // Make sure all entities are synced to correct blueprint
            EntityManager.MovePlayerToBlueprint(blueprint);

            // Move player
            var stairs = GridManager.Grid.GetCells(a => a.CellProperties.Name != null && a.CellProperties.Name.Equals(stairsName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (stairs == null)
            {
                throw new Exception($"[{GetType().Name}] No {stairsName} available for {tile.Name} at position: {cell.Position}");
            }

            GridManager.Grid.RenderObject(UserInterfaceManager.Get<MapWindow>());
            Game.Player.MoveTowards(stairs.Position, false, null, false);
            Game.Player.Initialize(false);
        }

        private static class BlueprintDatabase
        {
            public static readonly Dictionary<int, Blueprint<T>> Blueprints = new Dictionary<int, Blueprint<T>>();

            private static int _currentId;
            public static int GetUniqueId()
            {
                return _currentId++;
            }

            public static void Reset()
            {
                Blueprints.Clear();
                _currentId = 0;
            }

            public static void ResetExcept(params int[] ids)
            {
                var toRemove = new List<int>();
                foreach (var entity in Blueprints)
                {
                    toRemove.Add(entity.Key);
                }

                toRemove = toRemove.Except(ids).ToList();
                foreach (var id in toRemove)
                    Blueprints.Remove(id);

                _currentId = Blueprints.Count == 0 ? 0 : (Blueprints.Max(a => a.Key) + 1);
            }
        }
    }

    public class Blueprint
    {
        protected Blueprint() 
        { }

        internal static Dictionary<char, BlueprintTile> GetTilesFromConfig()
        {
            var configs = GetConfigurations();
            var tiles = GetBlueprintConfigValuePairs(configs);

            return new Dictionary<char, BlueprintTile>(tiles);
        }

        public static void ValidateConfigurationPaths()
        {
            var configPaths = GetConfigurationPaths();
            foreach (var path in configPaths)
            {
                if (!File.Exists(path))
                    throw new Exception("Invalid configuration path defined: " + path);
            }
        }

        internal static string[] GetConfigurationPaths() => new[]
        {
            Path.Combine(Constants.Blueprint.BlueprintsConfigDirectoryPath, Constants.Blueprint.Prints.BlueprintTiles + ".json"),
            Path.Combine(Constants.Blueprint.BlueprintsConfigDirectoryPath, Constants.Blueprint.Prints.SpecialCharacters + ".json"),
        };

        internal static IEnumerable<BlueprintConfig> GetConfigurations(IEnumerable<string> configPaths) =>
            configPaths.Select(path => JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(path)));

        internal static IEnumerable<BlueprintConfig> GetConfigurations() =>
            GetConfigurationPaths().Where(a => File.Exists(a))
            .Select(path => JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(path)));

        internal static Dictionary<string, BlueprintTile[]> GetConfigurationsDictionary() =>
            GetConfigurations(GetConfigurationPaths().Where(a => File.Exists(a)))
            .ToDictionary(a => a.Name, a => a.Tiles);

        private static IEnumerable<KeyValuePair<char, BlueprintTile>> GetBlueprintConfigValuePairs(IEnumerable<BlueprintConfig> configs) =>
            configs.SelectMany(config => config.Tiles.Select(a => new KeyValuePair<char, BlueprintTile>(a.Glyph, a)));
    }

    [Serializable]
    internal class BlueprintConfig
    {
#pragma warning disable 0649
        public string Name;
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
        public bool Interactable;
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
