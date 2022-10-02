using Emberpoint.Core;
using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using System.Collections.Generic;
using System.Linq;

namespace Tests.TestObjects.Windows
{
    public class FovWindowMock
    {
        private Dictionary<char, CharObj> _charObjects;
        private readonly Dictionary<char, BlueprintTile> _blueprintTiles;

        public FovWindowMock()
        {
            _blueprintTiles = Blueprint.GetTilesFromConfig();
        }

        private void ReinitializeCharObjects(IEnumerable<char> characters)
        {
            _charObjects = new Dictionary<char, CharObj>(GetCharObjectPairs(characters));
        }

        private IEnumerable<KeyValuePair<char, CharObj>> GetCharObjectPairs(IEnumerable<char> characters)
        {
            foreach (var character in characters)
            {
                if (!_blueprintTiles.TryGetValue(character, out var tile) || tile.Name == null) continue;
                var glyphColor = MonoGameExtensions.GetColorByString(tile.Foreground);
                if (glyphColor.A == 0) continue; // Don't render transparent tiles on the fov window
                yield return new KeyValuePair<char, CharObj>(character, new CharObj(tile.Glyph, tile.Name));
            }
        }

        public void Update(IEntity entity)
        {
            int radius = entity is Player ? Constants.Player.FieldOfViewRadius : entity.FieldOfViewRadius;
            var farBrightCells = GetBrightCellsInFov(entity, radius + 3).ToList();

            // Gets cells player can see after FOV refresh.
            var cells = GridManager.Grid.GetExploredCellsInFov(entity)
                .Select(a => (char)a.Glyph)
                //Merge in bright cells before FOV refresh.
                .Union(farBrightCells)
                //Take only unique cells as an array.
                .Distinct();

            // Draw visible cells to the FOV window
            ReinitializeCharObjects(characters: cells);
        }

        /// <summary>
        /// Check if the sequence equals the passed glyphs.
        /// </summary>
        /// <param name="expectedGlyphs"></param>
        /// <returns></returns>
        public bool SequenceEqual(IEnumerable<char> expectedGlyphs, out IEnumerable<char> result)
        {
            result = _charObjects?.Values.Select(a => a.Glyph);
            return _charObjects != null && _charObjects.Values.Select(a => a.Glyph).SequenceEqual(expectedGlyphs);
        }

        private IEnumerable<char> GetBrightCellsInFov(IEntity entity, int fovRadius)
        {
            return GridManager.Grid.GetExploredCellsInFov(entity, fovRadius)
                .Where(a => a.LightProperties.Brightness > 0f)
                .Select(a => (char)a.Glyph);
        }

        private readonly struct CharObj
        {
            public readonly char Glyph;
            public readonly string Name;

            public CharObj(char glyph, string name)
            {
                Glyph = glyph;
                Name = name;
            }
        }
    }
}
