using Emberpoint.Core.GameObjects.Dialogs;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class MainMenuWindow : ControlsConsole, IUserInterface
    {
        public ContributorsWindow ContributorsWindow { get; set; }
        public KeybindingsWindow KeybindingsWindow { get; private set; }
        public SettingsWindow SettingsWindow { get; private set; }

        // Wrapper that combines this window with the background animation
        public ScreenObject Container { get; private set; } = new ScreenObject();

        public SadConsole.Console Console
        {
            get { return this; }
        }

        public MainMenuWindow(int width, int height) : base(width, height)
        {
            // Set the XNA container's title
            Settings.WindowTitle = Strings.GameTitle;

            DefaultBackground = Color.Transparent;
            DefaultForeground = Color.White;

            // Display the background animation
            _ = new FullMoonBackgroundWindow(Container);

            // Display the game title
            _ = new DrawFontTitleWindow(Strings.GameTitle, this, (0, 2));

            // Add this console to the container
            Container.Children.Add(this);

            Refresh();
        }

        public void Refresh()
        {
            Controls.Clear();
            Surface.Clear();
            InitializeButtons();
            //DrawGameTitle();
        }

        private void DrawGameTitle()
        {
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
            for (int y = 0; y < titleFragments.Length; y++)
            {
                for (int x = 0; x < titleFragments[y].Length; x++)
                {
                    Surface.SetGlyph(startPosX + x, startPosY + y, titleFragments[y][x], Color.White, Color.Transparent);
                }
            }
        }

        public void InitializeButtons()
        {
            int w2 = Constants.GameWindowWidth / 2;
            int h2 = Constants.GameWindowHeight / 2;
            int x = 10;

            var playButton = new Button(20, 3)
            {
                Text = Strings.Play,
                Position = new Point(x, h2 - 4),
                UseMouse = true,
                UseKeyboard = false,
            };
            playButton.Click += ButtonPressPlay;
            Controls.Add(playButton);

            var contributorsButton = new Button(20, 3)
            {
                Text = Strings.Contributors,
                Position = new Point(x, h2),
                UseMouse = true,
                UseKeyboard = false,
            };
            contributorsButton.Click += ButtonPressContributors;
            Controls.Add(contributorsButton);

            var keybindingsButton = new Button(20, 3)
            {
                Text = Strings.Keybindings,
                Position = new Point(x, h2 + 4),
                UseMouse = true,
                UseKeyboard = false,
            };
            keybindingsButton.Click += ButtonPressKeybindings;
            Controls.Add(keybindingsButton);

            var settingsButton = new Button(20, 3)
            {
                Text = Strings.Settings,
                Position = new Point(x, h2 + 8),
                UseMouse = true,
                UseKeyboard = false,
            };
            settingsButton.Click += ButtonPressSettings;
            Controls.Add(settingsButton);

            var exitButton = new Button(20, 3)
            {
                Text = Strings.Exit,
                Position = new Point(x, h2 + 12),
                UseMouse = true,
                UseKeyboard = false,
            };
            exitButton.Click += ButtonPressExit;
            Controls.Add(exitButton);
        }

        public static MainMenuWindow Show()
        {
            var mainMenu = UserInterfaceManager.Get<MainMenuWindow>();
            if (mainMenu == null)
            {
                // Intialize default keybindings
                KeybindingsManager.InitializeDefaultKeybindings();

                mainMenu = new MainMenuWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                UserInterfaceManager.Add(mainMenu);
            }
            else
            {
                mainMenu.IsVisible = true;
                mainMenu.IsFocused = true;
                mainMenu.Cursor.IsEnabled = false;
            }
            GameHost.Instance.Screen = mainMenu.Container;
            Game.Reset();
            return mainMenu;
        }

        private static void Hide(SadConsole.Console transitionConsole)
        {
            var mainMenu = UserInterfaceManager.Get<MainMenuWindow>();
            if (mainMenu == null)
            {
                return;
            }

            mainMenu.IsVisible = false;
            mainMenu.IsFocused = false;
            mainMenu.Cursor.IsEnabled = true;

            GameHost.Instance.Screen = transitionConsole;
        }

        public static void Transition(SadConsole.Console transitionConsole)
        {
            Hide(transitionConsole);
        }

        public void ButtonPressPlay(object sender, EventArgs args)
        {
            // Set selected language
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Constants.Language);

            // Initialize user interface
            UserInterfaceManager.Initialize();

            // Remove mainmenu and transition
            Transition(UserInterfaceManager.Get<GameWindow>().Console);

            // Instantiate player in the middle of the map
            var spawnPosition = GridManager.Grid.GetCell(a => a.LightProperties.Brightness > 0.3f && a.CellProperties.Walkable);
            Game.Player = EntityManager.Create<Player>(spawnPosition.Position, GridManager.ActiveBlueprint.ObjectId);
            Game.Player.Initialize();

            // Show a tutorial dialog window.
            TutorialDialog.Start();
        }

        public void ButtonPressContributors(object sender, EventArgs args)
        {
            if (ContributorsWindow == null)
            {
                ContributorsWindow = new ContributorsWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                UserInterfaceManager.Add(ContributorsWindow);
            }
            else
            {
                ContributorsWindow.IsVisible = true;
            }

            // Transition to contributors window
            Transition(ContributorsWindow);
        }

        public void ButtonPressSettings(object sender, EventArgs args)
        {
            if (SettingsWindow == null)
            {
                SettingsWindow = new SettingsWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                UserInterfaceManager.Add(SettingsWindow);
            }
            else
            {
                SettingsWindow.IsVisible = true;
            }

            // Transition to options window
            Transition(SettingsWindow);
        }

        public void ButtonPressKeybindings(object sender, EventArgs args)
        {
            if (KeybindingsWindow == null)
            {
                KeybindingsWindow = new KeybindingsWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                UserInterfaceManager.Add(KeybindingsWindow);
            }
            else
            {
                KeybindingsWindow.IsVisible = true;
            }

            // Transition to options window
            Transition(KeybindingsWindow);
        }

        public void ButtonPressExit(object sender, EventArgs args)
        {
            Environment.Exit(0);
        }
    }
}
