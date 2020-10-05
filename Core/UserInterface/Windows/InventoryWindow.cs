using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Entities.Items;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class InventoryWindow : Console, IUserInterface
    {
        private readonly Console _textConsole;

        public Console Console
        {
            get { return this; }
        }

        private readonly int _maxLineRows;
        private readonly List<IItem> _inventory;

        public InventoryWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);
            Print(3, 0, "Inventory", Color.Orange);

            _inventory = new List<IItem>();
            _maxLineRows = Height - 2;
            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(2, 1),
            };

            Position = new Point(Constants.Map.Width + 7, 1);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void Initialize()
        {
            // Adding default Items to Inventory
            var items = new IItem[]
            {
                new Flashlight(),
                new Battery() { Amount = 2 }
            };

            foreach (var item in items)
            {
                item.PickUp();
            }
        }

        public void AddInventoryItem(IItem item)
        {
            if (!_inventory.Contains(item))
            {
                _inventory.Add(item);
            }
            else
            {
                var invItem = _inventory.Single(a => a.Equals(item));
                if (invItem.Amount != item.Amount)
                    invItem.Amount += item.Amount;
            }
            UpdateInventoryText();
        }

        public void RemoveInventoryItem(IItem item)
        {
            if (_inventory.Contains(item))
            {
                var invItem = _inventory.Single(a => a.Equals(item));
                invItem.Amount -= item.Amount;
                if (invItem.Amount < 1)
                {
                    _inventory.Remove(invItem);
                    ItemManager.Remove(invItem);
                }
                UpdateInventoryText();
            }
        }

        public void RemoveInventoryItem<T>(int amount) where T : IItem
        {
            var item = GetItemOfType<T>();
            if (item != null)
            {
                if (_inventory.Contains(item))
                {
                    var invItem = _inventory.Single(a => a.Equals(item));
                    invItem.Amount -= amount;
                    if (invItem.Amount < 1)
                    {
                        _inventory.Remove(invItem);
                        ItemManager.Remove(invItem);
                    }
                    UpdateInventoryText();
                }
            }
        }

        public T GetItemOfType<T>() where T : IItem
        {
            return _inventory.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetItemsOfType<T>() where T : IItem
        {
            return _inventory.OfType<T>();
        }

        public bool HasItemOfType<T>() where T : IItem
        {
            return _inventory.OfType<T>().Any();
        }

        public void UpdateInventoryText()
        {
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);

            if (_inventory.Count > _maxLineRows)
            {
                foreach (var item in _inventory.OrderBy(x => x).Take(_maxLineRows - 1))
                {
                    _textConsole.Cursor.Print(new ColoredString("[" + (char)item.Glyph + "]", item.GlyphColor, Color.Transparent));
                    _textConsole.Cursor.Print(item.DisplayName);
                }
                _textConsole.Cursor.Print("<More Items..>");
            }
            else
            {
                foreach (var item in _inventory.OrderBy(x => x))
                {
                    _textConsole.Cursor.Print(new ColoredString("[" + (char)item.Glyph + "]", item.GlyphColor, Color.Transparent));
                    _textConsole.Cursor.Print(item.DisplayName);
                }
            }
        }
    }
}