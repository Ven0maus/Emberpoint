using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.GameObjects
{
    public class EmberCell : Cell
    {
        public Point Position { get; set; }
        public bool Walkable { get; set; }
        public string Name { get; set; }
        public Color NormalForeground { get; set; }
        public Color ForegroundFov { get; set; }
        public bool BlocksFov { get; set; }

        public EmberCell() 
        {
            NormalForeground = Color.White;
            Foreground = Color.White;
            ForegroundFov = Color.Gray;
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
                BlocksFov = this.BlocksFov
            };
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceTo(cell);
            return cell;
        }
    }
}
