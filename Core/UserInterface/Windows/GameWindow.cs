using Emberpoint.Core.GameObjects.Interfaces;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class GameWindow : Console, IUserInterface
    {
        public GameWindow(int width, int height) : base(width, height)
        {
            // Set the XNA container's title
            SadConsole.Game.Instance.Window.Title = Constants.GameTitle;

            // Print the game title at the  top
            Print(Width / 2 - Constants.GameTitle.Length / 2, 1, Constants.GameTitle);

            // Set the current screen to the game window
            Global.CurrentScreen = this;
        }

        public void Update()
        {
            // Tell's sadconsole to redraw this console
            IsDirty = true;
        }
    }
}
