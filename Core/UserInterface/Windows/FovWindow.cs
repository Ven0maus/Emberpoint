using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using SadConsole;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class FovWindow : Console, IUserInterface
    {
        public Console Console => this;

        private readonly int _maxLineRows;
        private readonly Console _textConsole;
        private readonly Dictionary<char, CharObj> _charObjects;
        private readonly Dictionary<char, BlueprintTile> _blueprintTiles;

        public FovWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);
            Print(3, 0, "Objects", Color.Orange);

            _charObjects = new Dictionary<char, CharObj>();
            _blueprintTiles = GetTilesFromConfig();
            _maxLineRows = Height - 2;

            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(2, 1),
            };

            Position = new Point(Constants.Map.Width + 7, 3 + 25);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        private Dictionary<char, BlueprintTile> GetTilesFromConfig()
        {
            var name = GridManager.Grid.Blueprint.GetType().Name;
            var blueprintPath = Path.Combine(Constants.Blueprint.BlueprintsPath, name + ".txt");
            var blueprintConfigPath = Path.Combine(Constants.Blueprint.BlueprintsConfigPath, Constants.Blueprint.BlueprintTiles + ".json");

            if (!File.Exists(blueprintPath) || !File.Exists(blueprintConfigPath) || !File.Exists(Constants.Blueprint.SpecialCharactersPath))
                return new Dictionary<char, BlueprintTile>();

            var specialConfig = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(Constants.Blueprint.SpecialCharactersPath));
            var specialChars = specialConfig.Tiles.ToDictionary(a => a.Glyph, a => a);

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var tiles = config.Tiles.ToDictionary(a => a.Glyph, a => a);

            foreach (var specialChar in specialChars)
            {
                tiles.Add(specialChar.Key, specialChar.Value);
            }

            return tiles;
        }

        public void Add(char character, bool updateText = true)
        {
            if (_charObjects.ContainsKey(character)) return;

            // Retrieve character name from the config
            if (!_blueprintTiles.TryGetValue(character, out BlueprintTile tile)) return;
            var glyphColor = GetColorByString(tile.Foreground);

            _charObjects.Add(character, new CharObj
            {
                Glyph = tile.Glyph,
                GlyphColor = glyphColor,
                Name = tile.Name
            });

            if (updateText)
            {
                UpdateText();
            }
        }

        private Color GetColorByString(string value)
        {
            var prop = typeof(Color).GetProperty(value);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return default;
        }

        public void RemoveAllExcept(List<char> characters)
        {
            var toBeRemoved = new List<char>();
            foreach (var c in _charObjects)
            {
                if (!characters.Contains(c.Key))
                    toBeRemoved.Add(c.Key);
            }

            foreach (var r in toBeRemoved)
                _charObjects.Remove(r);
        }

        public void UpdateText()
        {
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);

            if (_charObjects.Count > _maxLineRows)
            {
                foreach (var item in _charObjects.OrderBy(x => x.Key).Take(_maxLineRows - 1))
                {
                    _textConsole.Cursor.Print(new ColoredString("[" + item.Value.Glyph + "]:", item.Value.GlyphColor, Color.Transparent));
                    _textConsole.Cursor.Print(" " + item.Value.Name);
                    _textConsole.Cursor.CarriageReturn();
                    _textConsole.Cursor.LineFeed();
                }
                _textConsole.Cursor.Print("<More Objects..>");
            }
            else
            {
                foreach (var item in _charObjects.OrderBy(x => x.Key))
                {
                    _textConsole.Cursor.Print(new ColoredString("[" + item.Value.Glyph + "]:", item.Value.GlyphColor, Color.Transparent));
                    _textConsole.Cursor.Print(" " + item.Value.Name);
                    _textConsole.Cursor.CarriageReturn();
                    _textConsole.Cursor.LineFeed();
                }
            }
        }

        class CharObj
        {
            public char Glyph;
            public Color GlyphColor;
            public string Name;
        }
    }
}
