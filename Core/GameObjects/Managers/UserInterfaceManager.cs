using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.UserInterface.Windows;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class UserInterfaceManager
    {
        private static readonly List<IUserInterface> Interfaces = new List<IUserInterface>();

        public static bool IsPaused { get; set; }
        public static bool IsInitialized { get; private set; }

        public static void ShowMainMenu()
        {
            var mainMenu = Interfaces.OfType<MainMenuWindow>().SingleOrDefault();
            if (mainMenu == null)
            {
                mainMenu = new MainMenuWindow(Constants.GameWindowWidth, Constants.GameWindowHeight);
                mainMenu.InitializeButtons();
                Add(mainMenu);
            }
            else
            {
                mainMenu.IsVisible = true;
                mainMenu.IsFocused = true;
                mainMenu.IsCursorDisabled = false;
            }

            Global.CurrentScreen = mainMenu;

            foreach (var inf in Interfaces)
            {
                if (inf.Equals(mainMenu)) continue;
                Interfaces.Remove(inf);
            }
        }

        public static void HideMainMenu(Console transitionConsole)
        {
            var mainMenu = Interfaces.OfType<MainMenuWindow>().SingleOrDefault();
            if (mainMenu == null)
            {
                return;
            }
            else
            {
                mainMenu.IsVisible = false;
                mainMenu.IsFocused = false;
                mainMenu.IsCursorDisabled = true;
            }

            Global.CurrentScreen = transitionConsole;
        }

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

            // Initialize inventory
            var inventory = new InventoryWindow(Constants.GameWindowWidth / 3, 15);
            Add(inventory);
            inventory.Initialize();

            IsInitialized = true;
        }

        public static void Add<T>(T userInterface) where T : IUserInterface
        {
            Interfaces.Add(userInterface);
        }

        public static T Get<T>() where T : IUserInterface
        {
            return Interfaces.OfType<T>().Single();
        }    
    }
}
