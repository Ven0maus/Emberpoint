using SadConsole;
using Microsoft.Xna.Framework;
using Emberpoint.Core.Objects;
using Emberpoint.Core.Objects.Blueprints;

namespace Emberpoint.Core
{
    public static class Game
    {
        public static Console Map { get; private set; }
        public static EmberGrid Grid { get; private set; }
        public static Player Player { get; private set; }

        private static void Main()
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(120, 40);

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
            Player.CheckForMovement();
        }

        private static void Init()
        {
            Map = new Console(70, 30);
            SetMapArea(70, 30);

            var mainConsole = new Console(120, 40);
            mainConsole.Children.Add(Map);
            mainConsole.Print(60 - "Emberpoint".Length / 2, 2, "Emberpoint");

            Map.Position = new Point(25, 5);

            // Instantiate player in the middle of the map
            Player = new Player(new Point(35, 15));
            Player.RenderObject(Map);

            Global.CurrentScreen = mainConsole;
        }

        private static void SetMapArea(int gridSizeX, int gridSizeY)
        {
            Grid = new EmberGrid(gridSizeX, gridSizeY, new HouseBlueprint());
            Grid.RenderObject(Map);
        }
    }
}
