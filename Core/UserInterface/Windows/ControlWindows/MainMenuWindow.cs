using Emberpoint.Core.GameObjects.Conversation;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using Emberpoint.Core.UserInterface.Windows.ConsoleWindows;
using SadConsole;
using SadConsole.Readers;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Globalization;
using System.Threading;

namespace Emberpoint.Core.UserInterface.Windows.ControlWindows
{
    public class MainMenuWindow : ControlsConsole, IUserInterface
    {
        public ContributorsWindow ContributorsWindow { get; set; }
        public SettingsWindow SettingsWindow { get; private set; }

        public SadConsole.Console Content
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
            _ = new FullMoonBackgroundWindow(this);

            // Display the game title
            _ = new DrawFontTitleWindow(Strings.GameTitle, this, (0, 2), Constants.Fonts.BigIce);

            Refresh();
        }

        public void Refresh()
        {
            Controls.Clear();
            Surface.Clear();
            InitializeButtons();
        }

        public void InitializeButtons()
        {
            int halfWindowHeight = Constants.GameWindowHeight / 2;
            int x = 10;

            var playButton = new Button(20, 3)
            {
                Text = Strings.Play,
                Position = new Point(x, halfWindowHeight - 4),
                UseMouse = true,
                UseKeyboard = false,
            };
            playButton.Click += ButtonPressPlay;
            Controls.Add(playButton);

            var contributorsButton = new Button(20, 3)
            {
                Text = Strings.Contributors,
                Position = new Point(x, halfWindowHeight),
                UseMouse = true,
                UseKeyboard = false,
            };
            contributorsButton.Click += ButtonPressContributors;
            Controls.Add(contributorsButton);

            var settingsButton = new Button(20, 3)
            {
                Text = Strings.Settings,
                Position = new Point(x, halfWindowHeight + 4),
                UseMouse = true,
                UseKeyboard = false,
            };
            settingsButton.Click += ButtonPressSettings;
            Controls.Add(settingsButton);

            var exitButton = new Button(20, 3)
            {
                Text = Strings.Exit,
                Position = new Point(x, halfWindowHeight + 8),
                UseMouse = true,
                UseKeyboard = false,
            };
            exitButton.Click += ButtonPressExit;
            Controls.Add(exitButton);
        }

        public static void Show(bool resetGameState = false)
        {
            var mainMenu = UserInterfaceManager.Get<MainMenuWindow>();
            if (mainMenu == null)
            {
                mainMenu = new MainMenuWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                UserInterfaceManager.Add(mainMenu);
            }
            else
            {
                mainMenu.IsVisible = true;
                mainMenu.IsFocused = true;
                mainMenu.Cursor.IsEnabled = false;
            }
            GameHost.Instance.Screen = mainMenu;
            if (resetGameState)
                Game.Reset();
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
            Transition(UserInterfaceManager.Get<GameWindow>());

            // Instantiate player in the middle of the map
            var spawnPosition = GridManager.Grid.GetCell(a => a.LightProperties.Brightness > 0.3f && a.CellProperties.Walkable);
            Game.Player = EntityManager.Create<Player>(spawnPosition.Position, GridManager.ActiveBlueprint.ObjectId);
            Game.Player.Initialize();

            // Show the first room dialogue
            DialogueManager.Load(Dialogues.FirstRoom);
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

        public void ButtonPressExit(object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        class FullMoonBackgroundWindow : ScreenSurface
        {
            public FullMoonBackgroundWindow(ScreenSurface parent) : base(Constants.GameWindowWidth, Constants.GameWindowHeight)
            {
                Parent = parent;
                string path = "./Resources/Images/";

                // load the image of the moon as a static 1 frame animation window
                var moon = AnimatedScreenSurface.FromImage("Full Moon", path + "fullmoon.png", (1, 1), 1f);

                // resize and reposition this surface according to the size of the moon surface
                (Surface as CellSurface).Resize(moon.Width, Surface.Height, moon.Width, Surface.Height, true);
                Position = (parent.Surface.Width - Surface.Width, 0);

                // add the moon to the children of this surface
                Children.Add(moon);
                int x = Surface.Width - moon.Width;
                moon.Position = (x, 2);

                // load the animation of the bat
                var bat = AnimatedScreenSurface.FromImage("Bat", path + "bat.png", (5, 10), 0.1f,
                    pixelPadding: (1, 1), frameStartAndFinish: (0, 33), font: Constants.Fonts.ThickSquare8);

                // add the bat to the children of this surface
                Children.Add(bat);
                x = Surface.Width - bat.Width;
                bat.Position = (x, 14);

                // start the bat animation
                bat.Start();
                bat.Repeat = true;
            }
        }

        class DrawFontTitleWindow : ScreenSurface
        {
            public DrawFontTitleWindow(string title, ScreenObject parent, Point position, TheDrawFont df) :
                base(Constants.GameWindowWidth, 9)
            {
                Surface.PrintTheDraw(0, title, df, HorizontalAlignment.Center);
                Parent = parent;
                Position = position;
            }
        }
    }
}
