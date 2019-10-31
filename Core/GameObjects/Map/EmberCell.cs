using Emberpoint.Core.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Emberpoint.Core.GameObjects.Map
{
    public class EmberCell : Cell, ILightable, IEquatable<EmberCell>
    {
        public Point Position { get; set; }
        public bool Walkable { get; set; }
        public string Name { get; set; }
        public Color NormalForeground { get; set; }
        public Color ForegroundFov { get; set; }
        public bool BlocksFov { get; set; }

        // Light properties
        public EmberCell LightSource { get; set; }
        public float Brightness { get; set; }
        public int LightRadius { get; set; }
        public bool EmitsLight { get; set; }
        public Color LightColor { get; set; }

        public EmberCell() 
        {
            NormalForeground = Color.White;
            Foreground = Color.White;
            ForegroundFov = Color.Lerp(Foreground, Color.Black, .5f);
            Background = Color.Black;
            Walkable = true;
            BlocksFov = false;
            Glyph = '.';
        }

        public EmberCell(Point position, int glyph, Color foreground, Color fov)
        {
            Position = position;
            Glyph = glyph;
            NormalForeground = foreground;
            Foreground = foreground;
            ForegroundFov = fov;
            Background = Color.Black;
            Walkable = true;
            BlocksFov = false;
        }

        public EmberCell(Point position, int glyph, Color foreground, Color fov, Color background)
        {
            Position = position;
            Glyph = glyph;
            NormalForeground = foreground;
            Foreground = foreground;
            ForegroundFov = fov;
            Background = background;
            Walkable = true;
            BlocksFov = false;
        }

        public void CopyFrom(EmberCell cell)
        {
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceFrom(cell);

            ForegroundFov = cell.ForegroundFov;
            IsVisible = cell.IsVisible;
            Name = cell.Name;
            NormalForeground = cell.NormalForeground;
            Position = cell.Position;
            Walkable = cell.Walkable;
            BlocksFov = cell.BlocksFov;
            Brightness = cell.Brightness;
            LightRadius = cell.LightRadius;
            EmitsLight = cell.EmitsLight;
            LightColor = cell.LightColor;
            LightSource = cell.LightSource;
        }

        public new EmberCell Clone()
        {
            var cell = new EmberCell()
            {
                ForegroundFov = this.ForegroundFov,
                IsVisible = this.IsVisible,
                Name = this.Name,
                NormalForeground = this.NormalForeground,
                Position = this.Position,
                Walkable = this.Walkable,
                BlocksFov = this.BlocksFov,
                Brightness = this.Brightness,
                LightRadius = this.LightRadius,
                EmitsLight = this.EmitsLight,
                LightColor = this.LightColor,
                LightSource = this.LightSource
            };
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceTo(cell);
            return cell;
        }

        public bool Equals([AllowNull] EmberCell other)
        {
            if (other == null) return false;
            return other.Position.Equals(Position);
        }
    }
}
