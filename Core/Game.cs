using System.Linq;
using SadConsole;
using Microsoft.Xna.Framework;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.UserInterface.Windows;
using Emberpoint.Core.GameObjects.Managers;

namespace Emberpoint.Core
{
    public static class Game
    {
        private static MainMenuWindow _mainMenuWindow;
        public static Player Player { get; set; }

        private static void Main()
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(Constants.GameWindowWidth, Constants.GameWindowHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;
            // Hook the update event so we can check for key presses.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime gameTime)
        {
            if (_mainMenuWindow?.OptionsWindow != null && _mainMenuWindow.OptionsWindow.WaitingForAnyKeyPress)
            {
                if (Global.KeyboardState.KeysPressed.Any())
                {
                    _mainMenuWindow.OptionsWindow.ChangeKeybinding(Global.KeyboardState.KeysPressed.First().Key);
                }
            }

            if (!UserInterfaceManager.IsInitialized || UserInterfaceManager.IsPaused) return;

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                UserInterfaceManager.Get<DialogWindow>().ShowNext();
            }
        }

        public static void Reset()
        {
            UserInterfaceManager.IsInitialized = false;

            var skipInterfaces = new []
            {
                UserInterfaceManager.Get<MainMenuWindow>() as IUserInterface,
                UserInterfaceManager.Get<OptionsWindow>() as IUserInterface, 
            };

            foreach (var inf in UserInterfaceManager.GetAll<IUserInterface>())
            {
                if (skipInterfaces.Contains(inf)) continue;
                UserInterfaceManager.Remove(inf);
            }

            Player = null;
            EntityManager.Clear();
            ItemManager.Clear();
        }

        private static void Init()
        {
            // Makes buttons look better
            Settings.UseDefaultExtendedFont = true;

            // Shows the main menu
            _mainMenuWindow = MainMenuWindow.Show();
        }
    }
}
