﻿using SadConsole.Entities;
using SadRogue.Primitives;
using System;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IItem : IRenderable<Renderer>, IEquatable<IItem>, IComparable<IItem>
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
