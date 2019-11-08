﻿using System;
using System.Linq;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;
using Console = SadConsole.Console;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class OptionsWindow : ControlsConsole, IUserInterface
    {
        public Console Console => this;

        public OptionsWindow(int width, int height) : base(width, height)
        {
            // Set custom theme
            var consoleTheme = Library.Default.Clone();
            consoleTheme.Colors.ControlHostBack = Color.Black;
            consoleTheme.Colors.Text = Color.White;
            consoleTheme.ButtonTheme = new ButtonLinesTheme
            {
                Colors = new Colors
                {
                    ControlBack = Color.Black,
                    Text = Color.WhiteSmoke,
                    TextDark = Color.AntiqueWhite,
                    TextBright = Color.White,
                    TextFocused = Color.White,
                    TextLight = Color.WhiteSmoke,
                    TextSelected = Color.White,
                    TextSelectedDark = Color.AntiqueWhite,
                    TitleText = Color.WhiteSmoke
                }
            };
            consoleTheme.ButtonTheme.Colors.RebuildAppearances();
            consoleTheme.Colors.RebuildAppearances();

            // Set the new theme
            Theme = consoleTheme;

            // Setup UI for the buttons
            CreateKeybindingButtons();

            // Add back button
            var backButton = new Button(20, 3)
            {
                Text = "Back",
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

            var titleFragments = @"
 _   __           _     _           _ _                 
| | / /          | |   (_)         | (_)                
| |/ /  ___ _   _| |__  _ _ __   __| |_ _ __   __ _ ___ 
|    \ / _ \ | | | '_ \| | '_ \ / _` | | '_ \ / _` / __|
| |\  \  __/ |_| | |_) | | | | | (_| | | | | | (_| \__ \
\_| \_/\___|\__, |_.__/|_|_| |_|\__,_|_|_| |_|\__, |___/
             __/ |                             __/ |    
            |___/                             |___/     
".Replace("\r", string.Empty).Split('\n');

            int startPosX = (Constants.GameWindowWidth / 2) - (titleFragments.OrderByDescending(a => a.Length).First().Length / 2) + 15;
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

        private void CreateKeybindingButtons()
        {
            var bindings = KeybindingsManager.GetKeybindings();

            var row = 12;
            const int maxPerColumn = 8;
            const int maxColumns = 2;
            var total = 0;
            var columns = 0;
            foreach (var (key, value) in bindings)
            {
                if (total == maxPerColumn)
                {
                    columns++;
                    total = 0;
                    row = 12;

                    if (columns == maxColumns)
                    {
                        // Show paging when the need arises?
                        throw new System.Exception("Exceeded max keybindings limit (16)");
                    }
                }

                var pos = columns == 0 ? new Point(25, row) : new Point(65, row);
                var button = new Button(10, 3)
                {
                    Name = key.ToString(),
                    Text = value.ToString(),
                    Position = pos,
                    UseMouse = true,
                    UseKeyboard = false,
                };

                // Add key re-arrange method
                button.Click += TriggerKeybindingChangeCheck;
                Add(button);

                Print(pos.X + 12, row + 1, key.ToString(), Color.White);
                row += 3;
                total++;
            }
        }

        public bool WaitingForAnyKeyPress { get; private set; }
        private Button _buttonPressed;

        private void TriggerKeybindingChangeCheck(object sender, EventArgs args)
        {
            if (WaitingForAnyKeyPress) return;
            WaitingForAnyKeyPress = true;
            UseMouse = false;
            _buttonPressed = (Button)sender;
        }

        public void ChangeKeybinding(Keys newKey)
        {
            if (!WaitingForAnyKeyPress) return;
            if (_buttonPressed == null) throw new Exception("Oops?");

            KeybindingsManager.EditKeybinding((Keybindings)Enum.Parse(typeof(Keybindings), _buttonPressed.Name), newKey);
            
            _buttonPressed.Text = newKey.ToString();
            _buttonPressed.IsDirty = true;
            _buttonPressed = null;
            WaitingForAnyKeyPress = false;
            UseMouse = true;
        }

        public void Update()
        {
            IsDirty = true;
        }
    }
}
