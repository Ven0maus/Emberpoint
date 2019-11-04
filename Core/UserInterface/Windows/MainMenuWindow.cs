using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.SadConsoleHelpers;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class MainMenuWindow : ControlsConsole, IUserInterface
    {
        public SadConsole.Console Console
        {
            get { return this; }
        }

        public MainMenuWindow(int width, int height) : base(width, height)
        {
            // Set custom theme
            var consoleTheme = SadConsole.Themes.Library.Default.Clone();
            consoleTheme.Colors.ControlHostBack = Color.Black;
            consoleTheme.Colors.Text = Color.White;
            consoleTheme.ButtonTheme = new ButtonLinesThemeFixed
            {
                Colors = new SadConsole.Themes.Colors
                {
                    ControlBack = Color.Black,
                    Text = Color.Yellow,
                    TextDark = Color.Orange,
                    TextBright = Color.LightYellow,
                    TextFocused = Color.Yellow,
                    TextLight = Color.LightYellow,
                    TextSelected = Color.Yellow,
                    TextSelectedDark = Color.Orange,
                    TitleText = Color.Purple
                }
            };
            consoleTheme.ButtonTheme.Colors.RebuildAppearances();
            consoleTheme.Colors.RebuildAppearances();

            // Set the new theme
            Theme = consoleTheme;

            string[] titleFragments = @"
 _____             _                                _         _   
|  ___|           | |                              (_)       | |  
| |__   _ __ ___  | |__    ___  _ __  _ __    ___   _  _ __  | |_ 
|  __| | '_ ` _ \ | '_ \  / _ \| '__|| '_ \  / _ \ | || '_ \ | __|
| |___ | | | | | || |_) ||  __/| |   | |_) || (_) || || | | || |_ 
\____/ |_| |_| |_||_.__/  \___||_|   | .__/  \___/ |_||_| |_| \__|
                                     | |                          
                                     |_|                          
"
            .Replace("\r", string.Empty).Split('\n');

            int startPosX = (Constants.GameWindowWidth / 2) - (titleFragments.OrderByDescending(a => a.Length).First().Length / 2);
            int startPosY = 4;

            // Print title fragments
            for (int y=0; y < titleFragments.Length; y++)
            {
                for (int x=0; x < titleFragments[y].Length; x++)
                {
                    Print(startPosX + x, startPosY + y, new ColoredGlyph(titleFragments[y][x], Color.White, Color.Transparent));
                }
            }

            // Add it to the children of the main console
            Global.CurrentScreen.Children.Add(this);
        }

        public void InitializeButtons()
        {
            var playButton = new Button(20, 3)
            {
                Text = "Play",
                Position = new Point((Constants.GameWindowWidth / 2) - 10, (Constants.GameWindowHeight / 2) - 4),
                UseMouse = true,
                UseKeyboard = false,
            };
            playButton.Click += ButtonPressPlay;
            Add(playButton);

            var contributorsButton = new Button(20, 3)
            {
                Text = "Contributors",
                Position = new Point((Constants.GameWindowWidth / 2) - 10, (Constants.GameWindowHeight / 2)),
                UseMouse = true,
                UseKeyboard = false,
            };
            contributorsButton.Click += ButtonPressContributors;
            Add(contributorsButton);

            var optionsButton = new Button(20, 3)
            {
                Text = "Options",
                Position = new Point((Constants.GameWindowWidth / 2) - 10, (Constants.GameWindowHeight / 2) + 4),
                UseMouse = true,
                UseKeyboard = false,
            };
            optionsButton.Click += ButtonPressOptions;
            Add(optionsButton);

            var exitButton = new Button(20, 3)
            {
                Text = "Exit",
                Position = new Point((Constants.GameWindowWidth / 2) - 10, (Constants.GameWindowHeight / 2) + 8),
                UseMouse = true,
                UseKeyboard = false,
            };
            exitButton.Click += ButtonPressExit;
            Add(exitButton);
        }

        private void Transition(SadConsole.Console transitionConsole)
        {
            UserInterfaceManager.HideMainMenu(transitionConsole);
        }

        public void ButtonPressPlay(object sender, EventArgs args)
        {
            // Initialize user interface
            UserInterfaceManager.Initialize();

            // Remove mainmenu and transition
            Transition(UserInterfaceManager.Get<GameWindow>().Console);

            // Keep the dialog window in a global variable so we can check it in the game loop
            Game.DialogWindow = UserInterfaceManager.Get<DialogWindow>();

            // Instantiate player in the middle of the map
            Game.Player = EntityManager.Create<Player>(GridManager.Grid.GetFirstCell(a => a.LightProperties.Brightness > 0f && a.CellProperties.Walkable).Position);
            Game.Player.Initialize();

            // Show a tutorial dialog window.
            Game.DialogWindow.ShowDialog("Tutorial", new string[] { "Welcome to Emberpoint.", "To turn on your flashlight, press 'F'.", "Press 'Enter' to continue." });
        }

        public void ButtonPressContributors(object sender, EventArgs args)
        {
            // TODO
        }

        public void ButtonPressOptions(object sender, EventArgs args)
        {
            // TODO
        }

        public void ButtonPressExit(object sender, EventArgs args)
        {
            // TODO
        }

        public void Update()
        {
            IsDirty = true;
        }
    }
}
