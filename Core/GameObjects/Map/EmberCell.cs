using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
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

        // Force obsoletion, so we use CellProperties instead.
        [Obsolete("Use CellProperties.NormalForeground instead.")]
        public new Color Foreground;
        [Obsolete("Use CellProperties.NormalBackground instead.")]
        public new Color Background;

        public EmberCell() 
        {
            CellProperties = new EmberCellProperties
            {
                NormalForeground = Color.White,
                NormalBackground = Color.Black,
                ForegroundFov = Color.Lerp(Color.White, Color.Black, .5f),
                BackgroundFov = Color.Black,
                Walkable = true,
                BlocksFov = false,
                IsExplored = false
            };

            Glyph = ' ';
            LightProperties = new LightEngineProperties();

#pragma warning disable CS0618 // Type or member is obsolete
            Foreground = CellProperties.NormalForeground;
            Background = CellProperties.NormalBackground;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public EmberCell(Point position, int glyph, Color foreground, Color fov)
        {
            CellProperties = new EmberCellProperties
            {
                NormalForeground = foreground,
                NormalBackground = Color.Black,
                ForegroundFov = fov,
                BackgroundFov = Color.Black,
                Walkable = true,
                BlocksFov = false,
                IsExplored = false
            };

            Position = position;
            Glyph = glyph;

#pragma warning disable CS0618 // Type or member is obsolete
            Foreground = foreground;
            Background = Color.Black;
#pragma warning restore CS0618 // Type or member is obsolete

            LightProperties = new LightEngineProperties();
        }

        public EmberCell(Point position, int glyph, Color foreground, Color foregroundFov, Color background, Color backgroundFov)
        {
            CellProperties = new EmberCellProperties
            {
                NormalForeground = foreground,
                NormalBackground = background,
                ForegroundFov = foregroundFov,
                BackgroundFov = backgroundFov,
                Walkable = true,
                BlocksFov = false,
                IsExplored = false
            };

            Position = position;
            Glyph = glyph;

#pragma warning disable CS0618 // Type or member is obsolete
            Foreground = foreground;
            Background = background;
#pragma warning restore CS0618 // Type or member is obsolete

            LightProperties = new LightEngineProperties();
        }

        public void CopyFrom(EmberCell cell)
        {
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceFrom(cell);

            IsVisible = cell.IsVisible;
            CellProperties.Name = cell.CellProperties.Name;
            CellProperties.NormalForeground = cell.CellProperties.NormalForeground;
            CellProperties.NormalBackground = cell.CellProperties.NormalBackground;
            CellProperties.ForegroundFov = cell.CellProperties.ForegroundFov;
            CellProperties.BackgroundFov = cell.CellProperties.BackgroundFov;
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
                    Name = this.CellProperties.Name,
                    NormalForeground = this.CellProperties.NormalForeground,
                    NormalBackground = this.CellProperties.NormalBackground,
                    ForegroundFov = this.CellProperties.ForegroundFov,
                    BackgroundFov = this.CellProperties.BackgroundFov,
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

        public EmberCell GetClosestLightSource()
        {
            // Return itself when this is a light source.
            if (LightProperties.EmitsLight) return this;
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

        public bool ContainsEntity()
        {
            return EntityManager.EntityExistsAt(Position);
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
            public Color NormalBackground { get; set; }
            public Color BackgroundFov { get; set; }
            public bool BlocksFov { get; set; }
            public bool IsExplored { get; set; }
        }
    }
}
