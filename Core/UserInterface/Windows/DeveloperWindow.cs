using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Input;
using SadConsole.Themes;
using System.Collections.Generic;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class DeveloperWindow : ControlsConsole, IUserInterface
    {
        public Console Console => this;
        private readonly Console _textConsole;
        private readonly TextBox _textInput;

        private readonly List<string> _previousLines;

        public DeveloperWindow(int width, int height) : base(width, height)
        {
            // Set custom theme
            var colors = Colors.CreateDefault();
            colors.ControlBack = Color.Black;
            colors.Text = Color.White;
            colors.TitleText = Color.White;
            colors.ControlHostBack = Color.White;
            colors.RebuildAppearances();

            // Set the new theme colors         
            ThemeColors = colors;

            // Add text area
            _previousLines = new List<string>();
            _textConsole = new Console(width - 2, height - 3)
            {
                Position = new Point(2, 1)
            };

            // Add input area
            _textInput = new TextBox(width - 2)
            {
                Position = _textConsole.Position + new Point(-1, height - 3)
            };

            Add(_textInput);
            Children.Add(_textConsole);

            // Middle of screen at the top
            Position = new Point((Constants.GameWindowWidth / 2) - width / 2, 1);

            Global.CurrentScreen.Children.Add(this);
        }

        public void Show()
        {
            IsVisible = true;
            IsFocused = true;
            Game.Player.IsFocused = false;
        }

        public void Hide()
        {
            IsVisible = false;
            IsFocused = false;
            Game.Player.IsFocused = true;
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            if (_textInput.DisableKeyboard && info.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.DeveloperConsole)))
            {
                if (IsVisible)
                    Hide();
                else
                    Show();
                return true;
            }

            return base.ProcessKeyboard(info);
        }

        protected override void OnInvalidate()
        {
            base.OnInvalidate();

            // Draw borders for the controls console
            this.DrawBorders(Width, Height, "O", "|", "-", Color.Gray);
            Print(((Width / 2) - "Developer Console".Length / 2), 0, "Developer Console", Color.Orange);
        }

        public void ParseCommand(string text)
        {

        }

        public void ClearConsole()
        {
            _previousLines.Clear();
        }
    }
}
