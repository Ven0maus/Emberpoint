using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.UserInterface.Windows;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class UserInterfaceManager
    {
        private static readonly List<IUserInterface> Interfaces = new List<IUserInterface>();

        public static bool IsPaused { get; set; }
        public static bool IsInitialized { get; set; }

        public static void Initialize()
        {
            // Initialize game window, set's the Global.CurrentScreen
            var gameWindow = new GameWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
            Add(gameWindow);

            // Initialize map
            var map = new MapWindow(Constants.Map.Width, Constants.Map.Height);
            Add(map);
            map.Initialize();

            // Initialize dialog window
            var dialogWindow = new DialogWindow(Constants.Map.Width, 6);
            Add(dialogWindow);

            // Initialize game over window
            var gameOverWindow = new GameOverWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
            Add(gameOverWindow);

            // Initialize inventory
            var inventory = new InventoryWindow(Constants.GameWindowWidth / 3, 15);
            Add(inventory);
            inventory.Initialize();

            var fovWindow = new FovWindow(Constants.GameWindowWidth / 3, 12);
            Add(fovWindow);

            IsInitialized = true;
        }

        public static void Add<T>(T userInterface) where T : IUserInterface
        {
            Interfaces.Add(userInterface);
        }

        public static T Get<T>() where T : IUserInterface
        {
            return Interfaces.OfType<T>().SingleOrDefault();
        }

        public static void Remove<T>(T userInterface) where T : IUserInterface
        {
            Interfaces.Remove(userInterface);
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return Interfaces.OfType<T>().ToArray();
        }
    }
}
