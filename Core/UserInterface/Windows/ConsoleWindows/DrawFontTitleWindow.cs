using SadConsole;
using SadConsole.Readers;
using SadRogue.Primitives;

namespace Emberpoint.Core.UserInterface.Windows.ConsoleWindows
{
    internal class DrawFontTitleWindow : ScreenSurface
    {
        public DrawFontTitleWindow(string title, ScreenObject parent, Point position, TheDrawFont df) :
            base(Constants.GameWindowWidth, 9)
        {
            Surface.PrintTheDraw(0, title, df, HorizontalAlignment.Center);
            Parent = parent;
            Position = position;
        }
    }
}
