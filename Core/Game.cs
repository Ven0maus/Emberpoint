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
            if (!UserInterfaceManager.IsInitialized) return;

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                UserInterfaceManager.Get<DialogWindow>().CloseDialog();
            }

            if (UserInterfaceManager.IsPaused) return;

            Player.CheckForMovementKeys();
            Player.CheckForInteractionKeys();

            //Test trigger for game over state
            if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.P))
            {
                UserInterfaceManager.Get<GameOverWindow>().Show();
            }
        }

        public static void Reset()
        {
            UserInterfaceManager.IsInitialized = false;
            foreach (var inf in UserInterfaceManager.GetAll<IUserInterface>())
            {
                if (inf.Equals(UserInterfaceManager.Get<MainMenuWindow>())) continue;
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
            MainMenuWindow.Show();
        }
    }
}
