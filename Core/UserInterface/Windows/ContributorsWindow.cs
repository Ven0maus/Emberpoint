using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.Resources;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System.Linq;
using Console = SadConsole.Console;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class ContributorsWindow : ControlsConsole, IUserInterface
    {
        public Console Console => this;

        public ContributorsWindow(int width, int height) : base(width, height)
        {
            DefaultBackground = Color.Black;
            DefaultForeground = Color.White;

            Refresh();
        }

        public void Refresh()
        {
            Controls.Clear();
            Surface.Clear();
            DrawWindowTitle();
            InitializeBackButton();
            DrawContributors();
        }

        private void InitializeBackButton()
        {
            // Add back button
            var backButton = new Button(20, 3)
            {
                Text = Strings.Back,
                Position = new Point(5, 3),
                UseMouse = true,
                UseKeyboard = false,
            };
            backButton.Click += (sender, args) =>
            {
                IsVisible = false;
                MainMenuWindow.Show();
            };
            Controls.Add(backButton);
        }

        private void DrawWindowTitle()
        {
            var titleFragments = @"
   _____            _        _ _           _                 
  / ____|          | |      (_) |         | |                
 | |     ___  _ __ | |_ _ __ _| |__  _   _| |_ ___  _ __ ___ 
 | |    / _ \| '_ \| __| '__| | '_ \| | | | __/ _ \| '__/ __|
 | |___| (_) | | | | |_| |  | | |_) | |_| | || (_) | |  \__ \
  \_____\___/|_| |_|\__|_|  |_|_.__/ \__,_|\__\___/|_|  |___/                                                       
".Replace("\r", string.Empty).Split('\n');

            int startPosX = (Constants.GameWindowWidth / 2) - (titleFragments.OrderByDescending(a => a.Length).First().Length / 2) + 45;
            int startPosY = 0;

            // Print title fragments
            for (var y = 0; y < titleFragments.Length; y++)
            {
                for (var x = 0; x < titleFragments[y].Length; x++)
                {
                    if (startPosX + x >= Constants.GameWindowWidth ||
                        startPosY + y >= Constants.GameWindowHeight) continue;
                    Surface.SetGlyph(startPosX + x, startPosY + y, titleFragments[y][x], Color.White, Color.Transparent);
                }
            }
        }

        private void DrawContributors()
        {
            string[] contributors = new[]
            {
                "Venom0us",
                "MattHaluska",
                "Smartis2812",
                "maratmugninov",
                "agit15",
                "otogyongyosi",
                "Rychu"
            };

            var listbox = new ListBox(contributors.Max(a => a.Length) + 5, Constants.GameWindowHeight / 2)
            {
                Position = new Point(49, 12)
            };

            foreach (var contributor in contributors)
                listbox.Items.Add(contributor);

            Controls.Add(listbox);
        }
    }
}
