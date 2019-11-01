using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;

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
        public List<EmberCell> LightSources { get; set; }
        public float Brightness { get; set; }
        public int LightRadius { get; set; }
        public bool EmitsLight { get; set; }
        public Color LightColor { get; set; }

        public EmberCell GetClosestLightSource()
        {
            if (LightSources == null) return null;
            if (LightSources.Count == 1) return LightSources[0];
            EmberCell closest = null;
            float smallestDistance = float.MaxValue;
            foreach (var source in LightSources)
            {
                var sqdistance = source.Position.SquaredDistance(Position);
                if (smallestDistance > sqdistance)
                {
                    smallestDistance = sqdistance;
                    closest = source;
                }
            }
            return closest;
        }

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
            LightSources = cell.LightSources;
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
                LightSources = this.LightSources
            };
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceTo(cell);
            return cell;
        }

        public bool Equals(EmberCell other)
        {
            return other.Position.Equals(Position);
        }

        public override bool Equals(object obj)
        {
            return Equals((EmberCell)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(EmberCell lhs, EmberCell rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(EmberCell lhs, EmberCell rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
