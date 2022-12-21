using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows.ConsoleWindows
{
    public class InventoryWindow : BorderedWindow
    {
        private readonly List<IItem> _inventory;

        public InventoryWindow(int width, int height) : base(width, height)
        {
            _inventory = new List<IItem>();
            Title = Strings.Inventory;
            Position = (Constants.Map.Width + 7, 1);
        }

        public override void Refresh()
        {
            UpdateInventoryText();
        }

        public override void AfterCreate()
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

        // TODO: update this method to be able to display all items
        public void UpdateInventoryText()
        {
            Content.Clear();
            Content.Cursor.Position = new Point(0, 0);

            if (_inventory.Count > Content.Height)
            {
                foreach (var item in _inventory.OrderBy(x => x).Take(Content.Height - 1))
                    PrintItem(item);
                Content.Cursor.Print("<More Items..>");
            }
            else
            {
                foreach (var item in _inventory.OrderBy(x => x))
                    PrintItem(item);
            }
        }

        void PrintItem(IItem item)
        {
            Content.Cursor.Print(new ColoredString("[" + (char)item.Glyph + "]", item.GlyphColor,
                Color.Transparent)).Print(item.DisplayName).CarriageReturn().LineFeed();
        }
    }
}