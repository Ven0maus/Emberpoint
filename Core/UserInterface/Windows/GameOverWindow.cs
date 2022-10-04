using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using Console = SadConsole.Console;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class GameOverWindow : ControlsConsole, IUserInterface
    {
        public GameOverWindow(int width, int height) : base(width, height)
        {
            DefaultBackground = Color.Black;
            DefaultForeground = Color.White;

            IsVisible = false;

            GameHost.Instance.Screen.Children.Add(this);

            DrawGameOverTitle();
            InitializeButtons();
        }

        public void Refresh()
        {
            Surface.Clear();
            Controls.Clear();
            InitializeButtons();
            DrawGameOverTitle();
        }

        public Console Console => this;
        public static void Show()
        {
            foreach (var inf in UserInterfaceManager.GetAll<IUserInterface>())
            {
                if (inf.Equals(UserInterfaceManager.Get<GameOverWindow>()) ||
                    inf.Equals(UserInterfaceManager.Get<GameWindow>()))
                {
                    inf.IsVisible = true;
                    continue;
                }

                inf.IsVisible = false;
            }
        }

        public static void Hide()
        {
            UserInterfaceManager.Get<GameOverWindow>().IsVisible = false;
        }
       
        private void InitializeButtons()
        {
            var returnToMainMenuButton = new Button(26, 3)
            {
                Text = "Return to main menu",
                Position = new Point(Constants.GameWindowWidth / 2 - 13, Constants.GameWindowHeight / 2 + 4),
                UseMouse = true,
                UseKeyboard = false
            };
            returnToMainMenuButton.Click += ButtonPressToMainMenu;
            Controls.Add(returnToMainMenuButton);

            var exitGameButton = new Button(26, 3)
            {
                Text = "Exit game",
                Position = new Point(Constants.GameWindowWidth / 2 - 13, Constants.GameWindowHeight / 2 + 8),
                UseMouse = true,
                UseKeyboard = false
            };
            exitGameButton.Click += ButtonPressExitGame;
            Controls.Add(exitGameButton);
        }

        private void ButtonPressExitGame(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ButtonPressToMainMenu(object sender, EventArgs e)
        {
            MainMenuWindow.Show();
        }

        private void DrawGameOverTitle()
        {
            var titleFragments = @"
 _______  _______  _______  _______    _______           _______  _______ 
(  ____ \(  ___  )(       )(  ____ \  (  ___  )|\     /|(  ____ \(  ____ )
| (    \/| (   ) || () () || (    \/  | (   ) || )   ( || (    \/| (    )|
| |      | (___) || || || || (__      | |   | || |   | || (__    | (____)|
| | ____ |  ___  || |(_)| ||  __)     | |   | |( (   ) )|  __)   |     __)
| | \_  )| (   ) || |   | || (        | |   | | \ \_/ / | (      | (\ (   
| (___) || )   ( || )   ( || (____/\  | (___) |  \   /  | (____/\| ) \ \__
(_______)|/     \||/     \|(_______/  (_______)   \_/   (_______/|/   \__/                        
"
                .Replace("\r", string.Empty).Split('\n');

            var startPosX = Constants.GameWindowWidth / 2 - 37;
            var startPosY = 10;

            // Print title fragments
            for (var y = 0; y < titleFragments.Length; y++)
            for (var x = 0; x < titleFragments[y].Length; x++)
                Surface.SetGlyph(startPosX + x, startPosY + y, titleFragments[y][x], Color.White, Color.Transparent);
        }
    }
}