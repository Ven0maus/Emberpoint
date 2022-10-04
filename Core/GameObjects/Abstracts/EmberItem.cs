using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace Emberpoint.Core.GameObjects.Abstracts
{
    /// <summary>
    /// An item is also a renderable entity.
    /// </summary>
    public abstract class EmberItem : Entity, IItem
    {
        public int ObjectId { get; }
        public int Amount { get; set; }
        private readonly System.Func<string> _localizedName;
        public new string Name
        {
            get { return _localizedName?.Invoke() ?? GetType().Name; }
            private set { }
        }
        public int Glyph { get { return Appearance.Glyph; } set { Appearance.Glyph = value; } }
        public Color GlyphColor { get { return Appearance.Foreground; } set { Appearance.Foreground = value; } }

        public virtual string DisplayName { get { return string.Format(" {0} : {1} \r\n", Name, Amount); } }

        private Console _renderedConsole;

        public EmberItem(int glyph, Color foregroundColor, int zIndex = 0, System.Func<string> name = null) : base(foregroundColor, Color.Transparent, glyph, zIndex)
        {
            ObjectId = ItemManager.GetUniqueId();
            ItemManager.Add(this);

            Amount = 1;
            _localizedName = name;
            Appearance.Foreground = foregroundColor;
            Appearance.Background = Color.Transparent;
            Appearance.Glyph = glyph;
        }

        public void RenderObject(Console console)
        {
            _renderedConsole = console;
            console.Children.Add(this);
        }

        public void UnRenderObject()
        {
            if (_renderedConsole != null)
            {
                _renderedConsole.Children.Remove(this);
                _renderedConsole = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(IItem other)
        {
            // First two lines are just optimizations
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            return Equals((IItem)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public int CompareTo(IItem other)
        {
            return string.Compare(Name, other.Name);
        }

        /// <summary>
        /// Called when the object is picked up from the grid and placed in the inventory.
        /// </summary>
        public virtual void PickUp()
        {
            var inventory = Game.Player == null ? UserInterfaceManager.Get<InventoryWindow>() : Game.Player.Inventory;
            inventory.AddInventoryItem(this);
        }
    }
}
