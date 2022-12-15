using SadConsole;
using SadConsole.Readers;
using SadRogue.Primitives;
using System;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    internal class DrawFontTitleWindow : ScreenSurface
    {
        readonly TheDrawFont _drawFont;

        public DrawFontTitleWindow(string title, ScreenObject parent, Point position, string fontFileName = "BIGICE_F.TDF") :
            base(Constants.GameWindowWidth, 9)
        {
            // draw font
            var fontEnumerable = TheDrawFont.ReadFonts($"./Resources/Fonts/{fontFileName}");
            if (fontEnumerable is null) throw new ArgumentException($"Font {fontFileName} couldn't be found.");
            _drawFont = fontEnumerable.ToArray()[0];
            Surface.PrintTheDraw(0, title, _drawFont, HorizontalAlignment.Center);

            // place it on the screen
            Parent = parent;
            Position = position;
        }
    }
}
