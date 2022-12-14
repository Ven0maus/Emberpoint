using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using Emberpoint.Core.UserInterface.Windows;
using SadConsole;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Emberpoint.Core
{
    public static class Game
    {
        private static MainMenuWindow _mainMenuWindow;
        public static Player Player { get; set; }

        public static CultureInfo CurrentCulture { get; internal set; }

        private static void Main()
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(Constants.GameWindowWidth, Constants.GameWindowHeight);

            // Set the MonoGame container's title
            Settings.WindowTitle = Strings.GameTitle;

            // Hook the start event so we can add consoles to the system.
            GameHost.Instance.OnStart = Init;
            // Hook the update event so we can check for key presses.
            GameHost.Instance.FrameUpdate += UpdateFrame;
            
            // Start the game.
            GameHost.Instance.Run();
            GameHost.Instance.Dispose();
        }

        private static void UpdateFrame(object sender, GameHost e)
        {
            if (_mainMenuWindow?.KeybindingsWindow != null && _mainMenuWindow.KeybindingsWindow.WaitingForAnyKeyPress)
            {
                if (GameHost.Instance.Keyboard.KeysPressed.Any())
                {
                    _mainMenuWindow.KeybindingsWindow.ChangeKeybinding(GameHost.Instance.Keyboard.KeysPressed.First().Key);
                }
            }
        }

        public static void Reset()
        {
            UserInterfaceManager.IsInitialized = false;

            var skipInterfaces = new []
            {
                UserInterfaceManager.Get<MainMenuWindow>() as IUserInterface,
                UserInterfaceManager.Get<KeybindingsWindow>() as IUserInterface, 
            };

            foreach (var inf in UserInterfaceManager.GetAll<IUserInterface>())
            {
                if (skipInterfaces.Contains(inf)) continue;
                UserInterfaceManager.Remove(inf);
            }

            Player = null;
            EntityManager.Clear(true);
            ItemManager.Clear();
        }

        private static void Init()
        {
            // Set default language to english
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Constants.Language);
            CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            // Makes buttons look better
            Settings.UseDefaultExtendedFont = true;
            Settings.AllowWindowResize = true;
            // It's ugly, but it's the best
            Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

            // Shows the main menu
            _mainMenuWindow = MainMenuWindow.Show();
        }
    }
}
