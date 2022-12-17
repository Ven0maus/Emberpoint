using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.Resources;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class GameWindow : Console, IUserInterface
    {
        public GameWindow(int width, int height) : base(width, height)
        {
            // Set the current screen to the game window
            GameHost.Instance.Screen = this;
            Initialize();
        }

        public Console Content
        {
            get { return this; }
        }

        public void Refresh()
        {
            Surface.Clear();
            Initialize();
        }

        void Initialize()
        {
            // Print the game title at the top
            int x = (int) System.Math.Round(Width / 2 / 1.5f) - Strings.GameTitle.Length / 2;
            Surface.Print(x, 1, Strings.GameTitle);

        }
    }
}
