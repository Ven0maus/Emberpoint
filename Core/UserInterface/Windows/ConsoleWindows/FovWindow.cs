using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.Resources;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows.ConsoleWindows
{
    public class FovWindow : BorderedWindow
    {
        private ILookup<char, CharObj> _charObjects;
        private readonly Dictionary<char, BlueprintTile> _blueprintTiles;

        public FovWindow(int width, int height) : base(width, height)
        {
            _blueprintTiles = Blueprint.GetTilesFromConfig();
            Title = Strings.ObjectsInView;
            Position = (Constants.Map.Width + 7, 3 + 24);
            GameHost.Instance.Screen.Children.Add(this);
        }

        private void ReinitializeCharObjects(EmberCell[] cells, bool updateText = true)
        {
            _charObjects = GetCellCharObjectPairs(cells).ToLookup(a => a.Glyph, a => a);

            if (updateText)
                UpdateText();
        }

        private IEnumerable<CharObj> GetCellCharObjectPairs(EmberCell[] cells)
        {
            var cellObjects = cells
                .Where(a => a.EmberItem == null)
                .Select(a => (char)a.Glyph)
                .Distinct();

            foreach (var character in cellObjects)
            {
                if (!_blueprintTiles.TryGetValue(character, out var tile) || tile.Name == null) continue;
                var glyphColor = MonoGameExtensions.GetColorByString(tile.Foreground);
                if (glyphColor.A == 0) continue; // Don't render transparent tiles on the fov window
                yield return new CharObj(tile.Glyph, glyphColor, () => Constants.ResourceHelper.ReadProperty(tile.Name, tile.Name ?? ""), false);
            }

            var itemObjects = cells
                .Where(a => a.EmberItem != null)
                .Select(a => a.EmberItem)
                .DistinctBy(a => (char)a.Glyph);

            foreach (var item in itemObjects)
            {
                var glyphColor = item.Appearance.Foreground;
                if (glyphColor.A == 0) continue; // Don't render transparent tiles on the fov window
                yield return new CharObj((char)item.Glyph, glyphColor, () => item.Name, true);
            }
        }

        private void UpdateText()
        {
            Content.Clear();
            Content.Cursor.Position = new Point(0, 0);

            var orderedValues = _charObjects
                .OrderBy(x => x.Key)
                .SelectMany(pair => pair);

            foreach (var charObj in orderedValues.Take(Content.Height - 1))
                DrawCharObj(charObj);

            if (_charObjects.Count > Content.Height)
                Content.Cursor.Print("<More Objects..>");
        }

        private void DrawCharObj(CharObj charObj)
        {
            Content.Cursor.Print(new ColoredString("[" + charObj.Glyph + "]:", charObj.GlyphColor, Color.Transparent));
            Content.Cursor.Print(' ' + charObj.Name()).CarriageReturn().LineFeed();
        }

        public void Update(IEntity entity)
        {
            int radius = entity is Player ? Constants.Player.FieldOfViewRadius : entity.FieldOfViewRadius;
            var farBrightCells = GetBrightCellsInFov(entity, radius + 3);

            // Gets cells player can see after FOV refresh.
            var cellCollection = GridManager.Grid.GetExploredCellsInFov(entity)
                .Union(farBrightCells)
                .ToArray();

            // Draw visible cells to the FOV window
            ReinitializeCharObjects(cells: cellCollection, updateText: false);
            UpdateText();
        }

        public override void Refresh()
        {
            if (Game.Player != null)
                Update(Game.Player);
        }

        private static IEnumerable<EmberCell> GetBrightCellsInFov(IEntity entity, int fovRadius)
        {
            return GridManager.Grid.GetExploredCellsInFov(entity, fovRadius)
                .Where(a => a.LightProperties.Brightness > 0f);
        }

        private readonly struct CharObj
        {
            public readonly char Glyph;
            public readonly Color GlyphColor;
            public readonly Func<string> Name;
            public readonly bool IsItemChar;

            public CharObj(char glyph, Color glyphColor, Func<string> name, bool isItemChar)
            {
                Glyph = glyph;
                GlyphColor = glyphColor;
                Name = name;
                IsItemChar = isItemChar;
            }
        }

    }
}
