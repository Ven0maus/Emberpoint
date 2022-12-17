using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.UserInterface.Windows;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
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
            // Set default control theme for buttons
            Library.Default.SetControlTheme(typeof(Button), new ButtonLinesTheme());

            // Initialize all game window interfaces
            var interfaces = new IUserInterface[]
            {
                new GameWindow(Constants.GameWindowWidth, Constants.GameWindowHeight),
                new MapWindow(Constants.Map.Width, Constants.Map.Height),
                new DialogWindow(Constants.Map.Width, 6),
                new GameOverWindow(Constants.GameWindowWidth, Constants.GameWindowHeight),
                new FovWindow(Constants.GameWindowWidth / 3, 12),
                new InventoryWindow(Constants.GameWindowWidth / 3, 15),
                new InteractionWindow(Constants.GameWindowWidth / 3, 7),
                new DeveloperWindow(Constants.Map.Width, 14),
                new WorldmapWindow(Constants.Map.Width, Constants.Map.Height)
            };

            foreach (var window in interfaces)
            {
                window.BeforeCreate();
                Add(window);
                window.AfterCreate();
            }

            IsInitialized = true;
        }

        public static void Add<T>(T userInterface) where T : IUserInterface
        {
            Interfaces.Add(userInterface);
        }

        public static T Get<T>() where T : IUserInterface
        {
            return (T)Interfaces.FirstOrDefault(a => a.GetType() == typeof(T));
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
