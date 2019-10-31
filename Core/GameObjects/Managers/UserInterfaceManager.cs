using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class UserInterfaceManager
    {
        private static readonly List<IUserInterface> Interfaces = new List<IUserInterface>();

        public static bool IsPaused { get; set; }

        public static void Initialize()
        {
            // Initialize game window, set's the Global.CurrentScreen
            var gameWindow = new UserInterface.Windows.GameWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
            Interfaces.Add(gameWindow);

            // Initialize map
            var map = new MapWindow(Constants.Map.Width, Constants.Map.Height);
            Interfaces.Add(map);
            map.Initialize();

            // Initializing Inventory window
            var inventory = new InventoryWindow(Constants.GameWindowWidth / 3, 15);
            Interfaces.Add(inventory);
            inventory.Initialize();

            // Initialize dialog window
            var dialogWindow = new DialogWindow(Constants.GameWindowWidth / 2, 5);
            Interfaces.Add(dialogWindow);
        }

        public static T Get<T>() where T : IUserInterface
        {
            return Interfaces.OfType<T>().Single();
        }
               
    }
}
