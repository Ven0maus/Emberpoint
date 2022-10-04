using SadRogue.Primitives;
using System;
using Console = SadConsole.Console;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IItem : IRenderable<Console>, IEquatable<IItem>, IComparable<IItem>
    {
        int ObjectId { get; }
        string Name { get; set; }
        int Glyph { get; set; }
        int Amount { get; set; }
        Point Position { get; set; }
        Color GlyphColor { get; set; }
        string DisplayName { get; }
        void PickUp();
    }
}
