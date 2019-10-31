using Emberpoint.Core.Extensions;
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
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);
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
            IsDirty = true;
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

            if (_inventoryDict.Count > _maxLineRows)
            {
                foreach (var item in _inventoryDict.OrderBy(x => x.Key).Take(_maxLineRows - 1))
                {
                    _textConsole.Cursor.Print(string.Format("{0} : {1}\r\n", item.Value, item.Key));
                }
                _textConsole.Cursor.Print("<More Items..>");
            }
            else
            {
                foreach (var item in _inventoryDict.OrderBy(x => x.Key))
                {
                    _textConsole.Cursor.Print(string.Format("{0} : {1}\r\n", item.Value, item.Key));
                }
            }
        }
    }
}