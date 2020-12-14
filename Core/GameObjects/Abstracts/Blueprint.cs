using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
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
        public int GridSizeX { get; private set; }
        public int GridSizeY { get; private set; }
        public string BlueprintPath { get; private set; }

        public virtual Blueprint<T> StairsUpBlueprint { get; private set; }
        public virtual Blueprint<T> StairsDownBlueprint { get; private set; }

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
                        cell.EffectProperties.AddMovementEffect((entity) =>
                        {
                            EntityManager.ClearExceptPlayer();
                            GridManager.InitializeCustomCells(StairsDownBlueprint.GridSizeX, StairsDownBlueprint.GridSizeY, StairsDownBlueprint.GetCells());

                            // Move player
                            var stairsUp = GridManager.Grid.GetCells(a => a.CellProperties.Name != null && a.CellProperties.Name.Equals("Stairs Up")).FirstOrDefault();
                            if (stairsUp == null)
                            {
                                throw new Exception("[" + GetType().Name + "] No stairs up available for stairs down at position: " + cell.Position);
                            }
                            var walkableCell = GridManager.Grid.GetNeighbors(stairsUp).FirstOrDefault(a => a.CellProperties.Walkable);
                            if (walkableCell == null)
                                throw new Exception("No suitable spot near stairs! found!");
                            Game.Player.MoveTowards(walkableCell.Position, false);
                            Game.Player.Initialize(false);
                            // TODO: Re-initialize map?
                        });
                    }
                    else if (tile.Name != null && tile.Name.Equals("Stairs Up", StringComparison.OrdinalIgnoreCase) && StairsUpBlueprint != null)
                    {
                        cell.EffectProperties.AddMovementEffect((entity) =>
                        {
                            EntityManager.ClearExceptPlayer();
                            GridManager.InitializeCustomCells(StairsUpBlueprint.GridSizeX, StairsUpBlueprint.GridSizeY, StairsUpBlueprint.GetCells());

                            // Move player
                            var stairsDown = GridManager.Grid.GetCells(a => a.CellProperties.Name != null && a.CellProperties.Name.Equals("Stairs Down")).FirstOrDefault();
                            if (stairsDown == null)
                            {
                                throw new Exception("["+GetType().Name+"] No stairs down available for stairs up at position: " + cell.Position);
                            }
                            var walkableCell = GridManager.Grid.GetNeighbors(stairsDown).FirstOrDefault(a => a.CellProperties.Walkable);
                            if (walkableCell == null)
                                throw new Exception("No suitable spot near stairs! found!");
                            Game.Player.MoveTowards(walkableCell.Position, false);
                            Game.Player.Initialize(false);
                            // TODO: Re-initialize map?
                        });
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
