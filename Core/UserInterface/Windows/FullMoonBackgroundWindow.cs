using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class FullMoonBackgroundWindow : ScreenSurface
    {
        public FullMoonBackgroundWindow(ScreenSurface parent) : base(Constants.GameWindowWidth, Constants.GameWindowHeight)
        {
            Parent = parent;
            string path = "./Resources/Images/";

            // load the image of the moon as a static 1 frame animation window
            var moon = AnimatedScreenSurface.FromImage("Full Moon", path + "fullmoon.png", (1, 1), 1f);

            // resize and reposition this surface according to the size of the moon surface
            (Surface as CellSurface).Resize(moon.Width, Surface.Height, moon.Width, Surface.Height, true);
            Position = (parent.Surface.Width - Surface.Width, 0);

            // add the moon to the children of this surface
            Children.Add(moon);
            int x = Surface.Width - moon.Width;
            moon.Position = (x, 2);

            // load the animation of the bat
            var bat = AnimatedScreenSurface.FromImage("Bat", path + "bat.png", (5, 10), 0.1f,
                pixelPadding: (1, 1), frameStartAndFinish: (0, 33), font: Constants.Fonts.ThickSquare8);

            // add the bat to the children of this surface
            Children.Add(bat);
            x = Surface.Width - bat.Width;
            bat.Position = (x, 14);

            // start the bat animation
            bat.Start();
            bat.Repeat = true;
        }
    }
}
