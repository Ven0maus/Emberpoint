using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using SadRogue.Primitives;
using System;

namespace Emberpoint.Core.GameObjects.Items
{
    /// <summary>
    /// An item is also a renderable entity.
    /// </summary>
    public abstract class EmberItem : EmberEntity, IItem
    {
        public int Amount { get; set; }
        private readonly Func<string> _localizedName;
        public new string Name
        {
            get { return _localizedName?.Invoke() ?? GetType().Name; }
            private set { }
        }
        public Color GlyphColor { get { return Appearance.Foreground; } set { Appearance.Foreground = value; } }

        public virtual string DisplayName { get { return string.Format(" {0} : {1} \r\n", Name, Amount); } }

        public EmberItem(int glyph, Color foregroundColor, int zIndex = 0, Func<string> name = null) :
            base(foregroundColor, Color.Transparent, glyph, null, zIndex, false)
        {
            ItemManager.Add(this);

            Amount = 1;
            _localizedName = name;
        }

        public override void MoveToBlueprint(int blueprintId)
        {
            base.MoveToBlueprint(blueprintId);
            IsVisible = true;
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
            return obj != null && obj is IItem item && Equals(item);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position.X, Position.Y);
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
