﻿using System.Linq;
using SadConsole;
using Microsoft.Xna.Framework;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.UserInterface.Windows;
using Emberpoint.Core.GameObjects.Managers;
using System;

namespace Emberpoint.Core
{
    public static class Game
    {
        private static MainMenuWindow _mainMenuWindow;
        private static DialogWindow _dialogWindow;
        private static DeveloperWindow _developerWindow;

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

            if (_dialogWindow.IsVisible && !_developerWindow.IsVisible && Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                _dialogWindow.ShowNext();
            }
        }

        private static void InitializeWindows(object sender, EventArgs args)
        {
            _dialogWindow = UserInterfaceManager.Get<DialogWindow>();
            _developerWindow = UserInterfaceManager.Get<DeveloperWindow>();
            UserInterfaceManager.OnInitalized -= InitializeWindows;
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

            UserInterfaceManager.OnInitalized += InitializeWindows;

            // Shows the main menu
            _mainMenuWindow = MainMenuWindow.Show();
        }
    }
}
