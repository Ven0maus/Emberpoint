using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Linq;
using SadConsole.Themes;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class MainMenuWindow : ControlsConsole, IUserInterface
    {
        public OptionsWindow OptionsWindow { get; private set; }

        public SadConsole.Console Console
        {
            get { return this; }
        }

        public MainMenuWindow(int width, int height) : base(width, height)
        {
            // Set the XNA container's title
            SadConsole.Game.Instance.Window.Title = Constants.GameTitle;

            // Set custom theme
            var consoleTheme = SadConsole.Themes.Library.Default.Clone();
            consoleTheme.Colors.ControlHostBack = Color.Black;
            consoleTheme.Colors.Text = Color.White;
            consoleTheme.ButtonTheme = new ButtonLinesTheme
            {
                Colors = new SadConsole.Themes.Colors
                {
                    ControlBack = Color.Black,
                    Text = Color.WhiteSmoke,
                    TextDark = Color.AntiqueWhite,
                    TextBright = Color.White,
                    TextFocused = Color.White,
                    TextLight = Color.WhiteSmoke,
                    TextSelected = Color.White,
                    TextSelectedDark = Color.AntiqueWhite,
                    TitleText = Color.WhiteSmoke
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

        public static MainMenuWindow Show()
        {
            var mainMenu = UserInterfaceManager.Get<MainMenuWindow>();
            if (mainMenu == null)
            {
                // Intialize default keybindings
                KeybindingsManager.InitializeDefaultKeybindings();

                mainMenu = new MainMenuWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                mainMenu.InitializeButtons();
                UserInterfaceManager.Add(mainMenu);
            }
            else
            {
                mainMenu.IsVisible = true;
                mainMenu.IsFocused = true;
                mainMenu.IsCursorDisabled = false;
            }
            Global.CurrentScreen = mainMenu;
            Game.Reset();
            return mainMenu;
        }

        public static void Hide(SadConsole.Console transitionConsole)
        {
            var mainMenu = UserInterfaceManager.Get<MainMenuWindow>();
            if (mainMenu == null)
            {
                return;
            }

            mainMenu.IsVisible = false;
            mainMenu.IsFocused = false;
            mainMenu.IsCursorDisabled = true;

            Global.CurrentScreen = transitionConsole;
        }

        private void Transition(SadConsole.Console transitionConsole)
        {
            Hide(transitionConsole);
        }

        public void ButtonPressPlay(object sender, EventArgs args)
        {
            // Initialize user interface
            UserInterfaceManager.Initialize();

            // Remove mainmenu and transition
            Transition(UserInterfaceManager.Get<GameWindow>().Console);

            // Instantiate player in the middle of the map
            Game.Player = EntityManager.Create<Player>(GridManager.Grid
                .GetFirstCell(a => a.LightProperties.Brightness > 0f && a.CellProperties.Walkable).Position);
            Game.Player.Initialize();

            // Show a tutorial dialog window.
            UserInterfaceManager.Get<DialogWindow>().ShowDialog("Tutorial", new[]
            {
                "Welcome to Emberpoint.",
                "To turn on your flashlight, press '" + KeybindingsManager.GetKeybinding(Keybindings.Flashlight) + "'.",
                "Press 'Enter' to continue."
            });
        }

        public void ButtonPressContributors(object sender, EventArgs args)
        {
            // TODO
        }

        public void ButtonPressOptions(object sender, EventArgs args)
        {
            if (OptionsWindow == null)
            {
                OptionsWindow = new OptionsWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                UserInterfaceManager.Add(OptionsWindow);
            }
            else
            {
                OptionsWindow.IsVisible = true;
            }

            // Transition to options window
            Hide(OptionsWindow);
        }

        public void ButtonPressExit(object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        public void Update()
        {
            IsDirty = true;
        }
    }
}
