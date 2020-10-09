using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emberpoint.Core.GameObjects.Map
{
    public class EmberCell : Cell, ILightable, IEquatable<EmberCell>
    {
        public EmberCellProperties CellProperties { get; set; }
        public LightEngineProperties LightProperties { get; set; }
        public EmberEffectProperties EffectProperties { get; set; }

        public Point Position { get; set; }

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
            EffectProperties = new EmberEffectProperties();
            Foreground = CellProperties.NormalForeground;
            Background = CellProperties.NormalBackground;
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
            Foreground = foreground;
            Background = Color.Black;
            LightProperties = new LightEngineProperties();
            EffectProperties = new EmberEffectProperties();
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
            Foreground = foreground;
            Background = background;
            LightProperties = new LightEngineProperties();
            EffectProperties = new EmberEffectProperties();
        }

        public void CopyFrom(EmberCell cell)
        {
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceFrom(cell);

            // Base properties
            Position = cell.Position;

            // Ember cell properties
            CellProperties.Name = cell.CellProperties.Name;
            CellProperties.NormalForeground = cell.CellProperties.NormalForeground;
            CellProperties.NormalBackground = cell.CellProperties.NormalBackground;
            CellProperties.ForegroundFov = cell.CellProperties.ForegroundFov;
            CellProperties.BackgroundFov = cell.CellProperties.BackgroundFov;
            CellProperties.Walkable = cell.CellProperties.Walkable;
            CellProperties.BlocksFov = cell.CellProperties.BlocksFov;
            CellProperties.IsExplored = cell.CellProperties.IsExplored;

            // Light engine properties
            LightProperties.Brightness = cell.LightProperties.Brightness;
            LightProperties.LightRadius = cell.LightProperties.LightRadius;
            LightProperties.EmitsLight = cell.LightProperties.EmitsLight;
            LightProperties.LightColor = cell.LightProperties.LightColor;
            LightProperties.LightSources = cell.LightProperties.LightSources;

            // Ember Effect Properties
            EffectProperties.EntityMovementEffects = cell.EffectProperties.EntityMovementEffects;
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

                LightProperties = new LightEngineProperties
                {
                    Brightness = this.LightProperties.Brightness,
                    LightRadius = this.LightProperties.LightRadius,
                    EmitsLight = this.LightProperties.EmitsLight,
                    LightColor = this.LightProperties.LightColor,
                    LightSources = this.LightProperties.LightSources
                },

                EffectProperties = new EmberEffectProperties
                {
                    EntityMovementEffects = this.EffectProperties.EntityMovementEffects,
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[EmberCell Information]");
            sb.AppendLine($"<Position>: [X] {Position.X} [Y] {Position.Y}");
            sb.AppendLine();
            sb.AppendLine("[CellProperties]");
            sb.AppendLine($"<Name>: {CellProperties.Name}");
            sb.AppendLine($"<Walkable>: {CellProperties.Walkable}");
            sb.AppendLine($"<BlocksFov>: {CellProperties.BlocksFov}");
            sb.AppendLine($"<IsExplored>: {CellProperties.IsExplored}");
            sb.AppendLine();
            sb.AppendLine("[LightProperties]");
            sb.AppendLine($"<EmitsLight>: {LightProperties.EmitsLight}");
            sb.AppendLine($"<Brightness>: {LightProperties.Brightness}");
            sb.AppendLine($"<LightRadius>: {LightProperties.LightRadius}");
            sb.AppendLine($"<LightColor>: {LightProperties.LightColor}");
            if (LightProperties.LightSources != null)
            {
                sb.AppendLine("<<LightSources>>");
                foreach (var lightSource in LightProperties.LightSources)
                {
                    sb.AppendLine($"<LightSource>: [X] {lightSource.Position.X} [Y] {lightSource.Position.Y}");
                }
            }
            sb.AppendLine();
            sb.AppendLine("[EffectProperties]");
            sb.AppendLine($"<MovementEffects>: {(EffectProperties.EntityMovementEffects != null ? EffectProperties.EntityMovementEffects.Count : 0)}");
            sb.AppendLine("[---------------------]");
            return sb.ToString();
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

        public class EmberEffectProperties
        {
            public List<Action<IEntity>> EntityMovementEffects { get; set; }

            public void AddMovementEffect(Action<IEntity> action)
            {
                if (EntityMovementEffects == null) EntityMovementEffects = new List<Action<IEntity>>();
                EntityMovementEffects.Add(action);
            }
        }
    }
}
