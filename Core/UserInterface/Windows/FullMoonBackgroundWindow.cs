using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class FullMoonBackgroundWindow : ScreenSurface
    {
        public FullMoonBackgroundWindow(ScreenObject parent) : base(Constants.GameWindowWidth, Constants.GameWindowHeight)
        {
            Parent = parent;

            string r = "Resources/";
            string i = "Images/";
            string f = "Fonts/";

            // square font is required to make the animation of the bat look correctly
            GameHost.Instance.LoadFont(r + f + "thick_square_8x8.font");

            // load the image of the moon as a static 1 frame animation window
            var moon = AnimatedScreenSurface.FromImage("Full Moon", r + i + "fullmoon.png", (1, 1), 1f);

            // add the moon to the children of this surface
            Children.Add(moon);
            int x = Surface.Width - moon.Width;
            moon.Position = (x, 2);

            // load the animation of the bat
            var bat = AnimatedScreenSurface.FromImage("Bat", r + i + "bat.png", (5, 10), 0.1f,
                pixelPadding: (1, 1), frameStartAndFinish: (0, 33), font: GameHost.Instance.Fonts["ThickSquare8"]);
                //action: (c) => { if (c.Foreground.R > 10) c.Background = c.Background.FillAlpha(); });


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
