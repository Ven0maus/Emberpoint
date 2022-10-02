using Emberpoint.Core.GameObjects.Interfaces;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class GameWindow : Console, IUserInterface
    {
        public GameWindow(int width, int height) : base(width, height)
        {
            // Set the XNA container's title
            SadConsole.Game.Instance.Window.Title = Resources.Strings.GameTitle;

            // Print the game title at the  top
            Print((int)System.Math.Round((Width / 2) / 1.5f) - Resources.Strings.GameTitle.Length / 2, 1, Resources.Strings.GameTitle);

            // Set the current screen to the game window
            Global.CurrentScreen = this;
        }

        public Console Console
        {
            get { return this; }
        }
    }
}
