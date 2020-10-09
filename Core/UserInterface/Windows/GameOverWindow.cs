using System;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;
using Console = SadConsole.Console;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class GameOverWindow : ControlsConsole, IUserInterface
    {
        public GameOverWindow(int width, int height) : base(width, height)
        {
            var colors = Colors.CreateDefault();
            colors.ControlBack = Color.Black;
            colors.Text = Color.White;
            colors.TitleText = Color.White;
            colors.ControlHostBack = Color.White;
            Library.Default.SetControlTheme(typeof(Button), new ButtonLinesTheme());
            colors.RebuildAppearances();

            // Set the new theme colors         
            ThemeColors = colors;

            IsVisible = false;

            Global.CurrentScreen.Children.Add(this);

            InitializeButtons();
        }

        protected override void OnInvalidate()
        {
            base.OnInvalidate();

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
            Add(returnToMainMenuButton);

            var exitGameButton = new Button(26, 3)
            {
                Text = "Exit game",
                Position = new Point(Constants.GameWindowWidth / 2 - 13, Constants.GameWindowHeight / 2 + 8),
                UseMouse = true,
                UseKeyboard = false
            };
            exitGameButton.Click += ButtonPressExitGame;
            Add(exitGameButton);
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
                Print(startPosX + x, startPosY + y,
                    new ColoredGlyph(titleFragments[y][x], Color.White, Color.Transparent));
        }
    }
}