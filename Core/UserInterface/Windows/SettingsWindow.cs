using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Console = SadConsole.Console;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class SettingsWindow : ControlsConsole, IUserInterface
    {
        public Console Console => this;

        public SettingsWindow(int width, int height) : base(width, height)
        {
            DefaultBackground = Color.Black;
            DefaultForeground = Color.White;

            Refresh();
        }

        public void Refresh()
        {
            Controls.Clear();
            Surface.Clear();
            InitializeButtons();
            InitializeSettings();
            DrawWindowTitle();
            DrawButtonTitles();
        }

        private void InitializeSettings()
        {
            // Button name, (method, default value)
            var settings = new Dictionary<string, (EventHandler, string)> 
            {
                { Strings.Language, (ChangeLanguage, Constants.SupportedCultures[Constants.Language]()) }
            };

            // Introduce columns/paging when need arises

            var row = 12;
            foreach (var setting in settings)
            {
                var pos = new Point(35, row);
                var button = new Button(setting.Value.Item2.Length + 6, 3)
                {
                    Name = setting.Key + ":",
                    Text = setting.Value.Item2,
                    Position = pos,
                    UseMouse = true,
                    UseKeyboard = false,
                };

                button.Click += setting.Value.Item1;
                Controls.Add(button);
                row += 3;
            }
        }

        private readonly Queue<string> _langQueue = new Queue<string>(Constants.SupportedCultures.Keys);
        private void ChangeLanguage(object sender, EventArgs e)
        {
            var current = Constants.Language;
            _langQueue.Enqueue(current);

            var next = _langQueue.Dequeue();
            while (next == current)
                next = _langQueue.Dequeue();

            Constants.Language = next;

            var views = UserInterfaceManager.GetAll<IUserInterface>();
            foreach (var view in views)
                view.Refresh();
        }

        private void DrawButtonTitles()
        {
            var buttons = Controls.OfType<Button>();
            foreach (var button in buttons)
            {
                if (string.IsNullOrWhiteSpace(button.Name)) continue;
                Surface.Print(button.Position.X - (button.Name.Length + 1), button.Position.Y + 1, button.Name, Color.White);
            }
        }

        private void InitializeButtons()
        {
            // Add back button
            var backButton = new Button(20, 3)
            {
                Text = Strings.Back,
                Position = new Point(5, 3),
                UseMouse = true,
                UseKeyboard = false,
            };
            backButton.Click += (sender, args) =>
            {
                IsVisible = false;
                MainMenuWindow.Show();
            };
            Controls.Add(backButton);
        }

        private void DrawWindowTitle()
        {
            var titleFragments = @"
   _____      _   _   _                 
  / ____|    | | | | (_)                
 | (___   ___| |_| |_ _ _ __   __ _ ___ 
  \___ \ / _ \ __| __| | '_ \ / _` / __|
  ____) |  __/ |_| |_| | | | | (_| \__ \
 |_____/ \___|\__|\__|_|_| |_|\__, |___/
                               __/ |    
                              |___/     
".Replace("\r", string.Empty).Split('\n');

            int startPosX = (Constants.GameWindowWidth / 2) - (titleFragments.OrderByDescending(a => a.Length).First().Length / 2) + 6;
            int startPosY = 0;

            // Print title fragments
            for (var y = 0; y < titleFragments.Length; y++)
            {
                for (var x = 0; x < titleFragments[y].Length; x++)
                {
                    if (startPosX + x >= Constants.GameWindowWidth ||
                        startPosY + y >= Constants.GameWindowHeight) continue;
                    Surface.SetGlyph(startPosX + x, startPosY + y, titleFragments[y][x], Color.White, Color.Transparent);
                }
            }
        }
    }
}
