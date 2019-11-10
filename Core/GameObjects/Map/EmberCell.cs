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
        public EmberCellProperties CellProperties { get; set; }
        public LightEngineProperties LightProperties { get; set; }

        public Point Position { get; set; }

        public EmberCell GetClosestLightSource()
        {
            if (LightProperties.LightSources == null) return null;
            if (LightProperties.LightSources.Count == 1) return LightProperties.LightSources[0];
            EmberCell closest = null;
            float smallestDistance = float.MaxValue;
            foreach (var source in LightProperties.LightSources)
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
            CellProperties = new EmberCellProperties
            {
                NormalForeground = Color.White,
                ForegroundFov = Color.Lerp(Foreground, Color.Black, .5f),
                Walkable = true,
                BlocksFov = false,
                IsExplored = false
            };

            Foreground = Color.White;
            Background = Color.Black;
            Glyph = '.';

            LightProperties = new LightEngineProperties();
        }

        public EmberCell(Point position, int glyph, Color foreground, Color fov)
        {
            CellProperties = new EmberCellProperties
            {
                NormalForeground = foreground,
                ForegroundFov = fov,
                Walkable = true,
                BlocksFov = false,
                IsExplored = false
            };

            Position = position;
            Glyph = glyph;
            Foreground = foreground;
            Background = Color.Black;

            LightProperties = new LightEngineProperties();
        }

        public EmberCell(Point position, int glyph, Color foreground, Color fov, Color background)
        {
            CellProperties = new EmberCellProperties
            {
                NormalForeground = foreground,
                ForegroundFov = fov,
                Walkable = true,
                BlocksFov = false,
                IsExplored = false
            };

            Position = position;
            Glyph = glyph;
            Foreground = foreground;
            Background = background;

            LightProperties = new LightEngineProperties();
        }

        public void CopyFrom(EmberCell cell)
        {
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceFrom(cell);

            CellProperties.ForegroundFov = cell.CellProperties.ForegroundFov;
            IsVisible = cell.IsVisible;
            CellProperties.Name = cell.CellProperties.Name;
            CellProperties.NormalForeground = cell.CellProperties.NormalForeground;
            Position = cell.Position;
            CellProperties.Walkable = cell.CellProperties.Walkable;
            CellProperties.BlocksFov = cell.CellProperties.BlocksFov;
            CellProperties.IsExplored = cell.CellProperties.IsExplored;

            LightProperties.Brightness = cell.LightProperties.Brightness;
            LightProperties.LightRadius = cell.LightProperties.LightRadius;
            LightProperties.EmitsLight = cell.LightProperties.EmitsLight;
            LightProperties.LightColor = cell.LightProperties.LightColor;
            LightProperties.LightSources = cell.LightProperties.LightSources;
        }

        public new EmberCell Clone()
        {
            var cell = new EmberCell()
            {
                CellProperties = new EmberCellProperties()
                {
                    ForegroundFov = this.CellProperties.ForegroundFov,

                    Name = this.CellProperties.Name,
                    NormalForeground = this.CellProperties.NormalForeground,
                    Walkable = this.CellProperties.Walkable,
                    BlocksFov = this.CellProperties.BlocksFov,
                    IsExplored = this.CellProperties.IsExplored
                },

                Position = this.Position,
                IsVisible = this.IsVisible,

                LightProperties = new LightEngineProperties
                {
                    Brightness = this.LightProperties.Brightness,
                    LightRadius = this.LightProperties.LightRadius,
                    EmitsLight = this.LightProperties.EmitsLight,
                    LightColor = this.LightProperties.LightColor,
                    LightSources = this.LightProperties.LightSources
                }
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

        public class LightEngineProperties
        {
            public List<EmberCell> LightSources { get; set; }
            public float Brightness { get; set; }
            public int LightRadius { get; set; }
            public bool EmitsLight { get; set; }
            public Color LightColor { get; set; }
        }

        public class EmberCellProperties
        {
            public bool Walkable { get; set; }
            public string Name { get; set; }
            public Color NormalForeground { get; set; }
            public Color ForegroundFov { get; set; }
            public bool BlocksFov { get; set; }
            public bool IsExplored { get; set; }
        }
    }
}
