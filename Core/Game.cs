using SadConsole;
using Microsoft.Xna.Framework;
using Emberpoint.Core.Objects;
using Emberpoint.Core.Objects.Blueprints;
using Console = SadConsole.Console;
using Emberpoint.Core.Windows;

namespace Emberpoint.Core
{
    public static class Game
    {
        public static Console Map { get; private set; }
        public static EmberGrid Grid
        {
            get { return GridManager.Grid; }
        }
        public static Player Player { get; private set; }

        public static DialogWindow DialogWindow;

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
            if (DialogWindow.IsVisible)
            {
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    DialogWindow.CloseDialog();
                }
            }
            else
            {
                Player.CheckForMovement();
            }
        }

        private static void Init()
        {
            Map = new Console(Constants.Map.Width, Constants.Map.Height);
            GridManager.InitializeBlueprint<GroundFloorBlueprint>();
            Grid.RenderObject(Map);

            SadConsole.Game.Instance.Window.Title = Constants.GameTitle;

            var mainConsole = new Console(Constants.GameWindowWidth, Constants.GameWindowHeight);
            mainConsole.Children.Add(Map);
            mainConsole.Print(Constants.GameWindowWidth / 2 - Constants.GameTitle.Length / 2, 1, Constants.GameTitle);

            Map.Position = new Point(25, 3);

            // Instantiate player in the middle of the map
            Player = EntityManager.Create<Player>(new Point(Constants.Map.Width / 2, Constants.Map.Height / 2));
            Player.RenderObject(Map);

            Global.CurrentScreen = mainConsole;

            DialogWindow = new DialogWindow(Constants.GameWindowWidth / 2, 6);
            mainConsole.Children.Add(DialogWindow);

            DialogWindow.ShowDialog("Game", new string[] { "Game locked until Dialog is accepted.", "Press 'Enter' to continue." });
            DialogWindow.Position = new Point(2, Constants.GameWindowHeight - 7);
        }
    }
}
