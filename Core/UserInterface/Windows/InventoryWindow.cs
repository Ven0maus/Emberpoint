using Emberpoint.Core.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class InventoryWindow : SadConsole.Console, IUserInterface
    {
        private readonly SadConsole.Console _textConsole;

        private int _maxLineRows;
        private Dictionary<string, int> _inventoryDict;

        public InventoryWindow(int width, int height) : base(width, height)
        {
            DrawBorders(width, height, "O", "|", "-", Color.Gray);
            Print(3, 0, "Inventory", Color.Purple);

            _inventoryDict = new Dictionary<string, int>();
            _maxLineRows = Height - 2;
            _textConsole = new SadConsole.Console(Width - 2, Height - 2)
            {
                Position = new Point(2, 1),
            };

            Position = new Point(Constants.Map.Width + 7, 3);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void Initialize()
        {
            // Adding default Items to Inventory
            AddInventoryItem("Potion of Sanity", 3);
            AddInventoryItem("Potion of Health", 1);
            RemoveInventoryItem("Potion of Sanity", 2);
        }

        public void Update()
        {
        }

        public void AddInventoryItem(string name, int amount)
        {
            if (!_inventoryDict.ContainsKey(name))
            {
                _inventoryDict.Add(name, 0);
            }
            _inventoryDict[name] += amount;
            UpdateInventoryText();
        }

        public void RemoveInventoryItem(string name, int amount)
        {
            if (_inventoryDict.ContainsKey(name))
            {
                _inventoryDict[name] -= amount;
                if (_inventoryDict[name] < 1)
                {
                    _inventoryDict.Remove(name);
                }
            }
            UpdateInventoryText();
        }

        private void UpdateInventoryText()
        {
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);
            foreach (var item in _inventoryDict.OrderBy(x => x.Key))
            {
                _textConsole.Cursor.Print(string.Format("{0} : {1}\r\n", item.Value, item.Key));
            }
        }

        private void DrawBorders(int width, int height, string cornerGlyph, string horizontalBorderGlyph, string verticalBorderGlyph, Color borderColor)
        {
            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (int colIndex = 0; colIndex < width; colIndex++)
                {
                    // Drawing Corners
                    if ((rowIndex == 0 && colIndex == 0)
                        || (rowIndex == height - 1 && colIndex == 0)
                        || (rowIndex == height - 1 && colIndex == width - 1)
                        || (rowIndex == 0 && colIndex == width - 1))
                    {
                        Print(colIndex, rowIndex, cornerGlyph, borderColor);
                    }

                    if (rowIndex > 0 && rowIndex < height - 1 && (colIndex == 0 || colIndex == width - 1))
                    {
                        Print(colIndex, rowIndex, horizontalBorderGlyph, borderColor);
                    }

                    if (colIndex > 0 && colIndex < width - 1 && (rowIndex == 0 || rowIndex == height - 1))
                    {
                        Print(colIndex, rowIndex, verticalBorderGlyph, borderColor);
                    }
                }
            }
        }
    }
}