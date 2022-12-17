using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Blueprints.Objects;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.Resources;
using Emberpoint.Core.UserInterface.Windows.ConsoleWindows;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Emberpoint.Core.GameObjects.Abstracts
{
    public abstract class CellBlueprint<T> : Blueprint
        where T : EmberCell, new()
    {
        public int ObjectId { get; private set; }
        public int GridSizeX { get; private set; }
        public int GridSizeY { get; private set; }

        public string BlueprintPath { get; private set; }

        public virtual CellBlueprint<T> StairsUpBlueprint { get; private set; }
        public virtual CellBlueprint<T> StairsDownBlueprint { get; private set; }

        public CellBlueprint()
        {
            ObjectId = BlueprintDatabase.GetUniqueId();
            InitializeBlueprint(Constants.Blueprint.CellBlueprintsDirectoryPath);
        }

        protected CellBlueprint(string customPath)
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
                        cell.EffectProperties.AddMovementEffect((entity) => AddStairsLogic(entity, cell, tile));
                    }
                    else if (cell.CellProperties.Name != null && cell.CellProperties.Name.Equals(Strings.StairsUp, StringComparison.OrdinalIgnoreCase) && StairsUpBlueprint != null)
                    {
                        cell.EffectProperties.AddMovementEffect((entity) => AddStairsLogic(entity, cell, tile));
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

        private void AddStairsLogic(IEntity entity, EmberCell cell, BlueprintTile tile)
        {
            string stairsName = cell.CellProperties.Name.Equals(Strings.StairsUp, StringComparison.OrdinalIgnoreCase) ? Strings.StairsDown : Strings.StairsUp;
            CellBlueprint<EmberCell> blueprint = cell.CellProperties.Name.Equals(Strings.StairsUp, StringComparison.OrdinalIgnoreCase) ? GridManager.ActiveBlueprint.StairsUpBlueprint : GridManager.ActiveBlueprint.StairsDownBlueprint;
            ItemBlueprint<EmberItem> itemBlueprint = new GenericItemBlueprint(blueprint.ObjectId, blueprint.GetType().Name.Replace("Cells", "Items"));
            // Initialize new blueprint with tracking of the previous
            GridManager.InitializeBlueprint(blueprint, itemBlueprint, true);

            // Sync entities to blueprint
            if (entity is Player)
                EntityManager.MovePlayerToBlueprint(blueprint);
            else
                entity.MoveToBlueprint(blueprint);

            // Move player
            var stairs = GridManager.Grid.GetCells(a => a.CellProperties.Name != null && a.CellProperties.Name.Equals(stairsName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (stairs == null)
            {
                throw new Exception($"[{GetType().Name}] No {stairsName} available for {tile.Name} at position: {cell.Position}");
            }

            if (entity is Player)
                GridManager.Grid.RenderObject(UserInterfaceManager.Get<MapWindow>());

            entity.MoveTowards(stairs.Position, false, null, false);

            if (entity is Player)
                Game.Player.Initialize(false);
        }

        private static class BlueprintDatabase
        {
            public static readonly Dictionary<int, CellBlueprint<T>> Blueprints = new Dictionary<int, CellBlueprint<T>>();

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
}
