using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
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
    public abstract class Blueprint<T> where T : EmberCell, new()
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
            InitializeBlueprint(Constants.Blueprint.BlueprintsPath);
        }

        protected Blueprint(string customPath)
        {
            ObjectId = BlueprintDatabase.GetUniqueId();
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
            var tiles = config.Tiles.ToDictionary(a => (char?)a.Glyph, a => a);
            var nullTile = BlueprintTile.Null();

            foreach (var tile in tiles)
            {
                if (tile.Key == null) continue;
                if (specialChars.ContainsKey(tile.Key.Value))
                    throw new Exception("Glyph '" + tile.Key.Value + "': is reserved as a special character and cannot be used in " + name);
            }

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

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
                    if (tile.Name != null && tile.Name.Equals("Stairs Down", StringComparison.OrdinalIgnoreCase) && StairsDownBlueprint != null)
                    {
                        cell.EffectProperties.AddMovementEffect((entity) => AddStairsLogic(cell, tile));
                    }
                    else if (tile.Name != null && tile.Name.Equals("Stairs Up", StringComparison.OrdinalIgnoreCase) && StairsUpBlueprint != null)
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
            string stairsName = tile.Name.Equals("Stairs Up", StringComparison.OrdinalIgnoreCase) ? "Stairs Down" : "Stairs Up";
            Blueprint<T> blueprint = tile.Name.Equals("Stairs Up", StringComparison.OrdinalIgnoreCase) ? StairsUpBlueprint : StairsDownBlueprint;

            // Make sure all entities are synced to correct blueprint
            EntityManager.MovePlayerToBlueprint(blueprint);

            // Initialize new blueprint with tracking of the previous
            GridManager.InitializeBlueprint(blueprint, true);

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
