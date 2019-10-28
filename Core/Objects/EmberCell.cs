using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.Objects
{
    public class EmberCell : Cell
    {
        public Point Position { get; set; }
        public bool Walkable { get; set; }
        public string Name { get; set; }

        public EmberCell() 
        {
            Foreground = Color.White;
            Background = Color.Black;
            Walkable = true;
            Glyph = '.';
        }

        public EmberCell(Point position, int glyph, Color foreground)
        {
            Position = position;
            Glyph = glyph;
            Foreground = foreground;
            Background = Color.Black;
            Walkable = true;
        }

        public EmberCell(Point position, int glyph, Color foreground, Color background)
        {
            Position = position;
            Glyph = glyph;
            Foreground = foreground;
            Background = background;
            Walkable = true;
        }
    }
}
