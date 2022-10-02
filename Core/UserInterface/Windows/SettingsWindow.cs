using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;
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
            // Set custom theme
            var colors = Colors.CreateDefault();
            colors.ControlBack = Color.Black;
            colors.Text = Color.White;
            colors.TitleText = Color.White;
            colors.ControlHostBack = Color.White;
            Library.Default.SetControlTheme(typeof(Button), new ButtonLinesTheme());
            colors.RebuildAppearances();

            // Set the new theme colors         
            ThemeColors = colors;

            InitializeButtons();
            InitializeSettings();
        }

        public void Update()
        {
            RemoveAll();
            InitializeButtons();
            InitializeSettings();
            Invalidate();
        }

        protected override void OnInvalidate()
        {
            base.OnInvalidate();

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
                Add(button);
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
                view.Update();
        }

        private void DrawButtonTitles()
        {
            var buttons = Controls.OfType<Button>();
            foreach (var button in buttons)
            {
                if (string.IsNullOrWhiteSpace(button.Name)) continue;
                Print(button.Position.X - (button.Name.Length + 1), button.Position.Y + 1, button.Name, Color.White);
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
            Add(backButton);
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
                    Print(startPosX + x, startPosY + y, new ColoredGlyph(titleFragments[y][x], Color.White, Color.Transparent));
                }
            }
        }
    }
}
