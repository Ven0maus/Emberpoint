using SadConsole;
using Microsoft.Xna.Framework;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.UserInterface.Windows;
using Emberpoint.Core.GameObjects.Managers;

namespace Emberpoint.Core
{
    public static class Game
    {
        public static Player Player { get; set; }
        public static DialogWindow DialogWindow { get; set; }

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
            if (!UserInterfaceManager.IsInitialized) return;

            if (DialogWindow.IsVisible)
            {
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    DialogWindow.CloseDialog();
                }
            }

            if (UserInterfaceManager.IsPaused) return;

            Player.CheckForMovementKeys();
            Player.CheckForInteractionKeys();
        }

        private static void Init()
        {
            UserInterfaceManager.ShowMainMenu();
        }
    }
}
