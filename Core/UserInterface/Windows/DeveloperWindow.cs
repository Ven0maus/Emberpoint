using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Input;
using SadConsole.Themes;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class DeveloperWindow : ControlsConsole, IUserInterface
    {
        /// <summary>
        /// Container for console text lines with color
        /// </summary>
        readonly struct Line
        {
            public readonly string Text;
            public readonly Color Color;

            public Line(string text, Color color)
            {
                Text = text;
                Color = color;
            }
        }

        public Console Console => this;
        private readonly Console _textConsole;
        private readonly TextBox _textInput;
        private readonly List<Line> _previousLines;
        private readonly int _maxLineRows, _maxLineLength;

        public DeveloperWindow(int width, int height) : base(width, height)
        {
            // Set custom theme
            var colors = Colors.CreateDefault();
            colors.ControlBack = Color.Lerp(Color.Black, Color.Transparent, 0.6f);
            colors.Text = Color.White;
            colors.TitleText = Color.White;
            colors.ControlHostBack = Color.White;
            colors.RebuildAppearances();

            // Set the new theme colors         
            ThemeColors = colors;

            // Add text area
            _previousLines = new List<Line>();
            _textConsole = new Console(width - 2, height - 3)
            {
                Position = new Point(1, 1)
            };

            _maxLineRows = _textConsole.Height -1;
            _maxLineLength = _textConsole.Width - 1;

            // Disable mouse, or it will steal mouse input from DeveloperConsole
            _textConsole.UseMouse = false;

            // Add input area
            _textInput = new TextBox(width - 2)
            {
                Position = _textConsole.Position + new Point(0, height - 3)
            };

            Add(_textInput);
            Children.Add(_textConsole);

            FocusedMode = ActiveBehavior.Push;

            // Middle of screen at the top
            Position = new Point(4, 2);

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

        private IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ');
            var line = words.First();
            foreach (var word in words.Skip(1))
            {
                var test = $"{line} {word}";
                if (test.Length > maximumLineLength)
                {
                    yield return line;
                    line = word;
                }
                else
                {
                    line = test;
                }
            }
            yield return line;
        }

        private void WriteText(string text, Color color)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);

            // Split text in multiple lines if it exceeds max line length
            if (text.Length > _maxLineLength)
            {
                _previousLines.AddRange(SplitToLines(text, _maxLineLength).Select(a => new Line(a, color)));
            }
            else
            {
                _previousLines.Add(new Line(text, color));
            }

            // Make sure we have only max line rows of text
            if (_previousLines.Count >= _maxLineRows)
            {
                var amountToRemove = _previousLines.Count - _maxLineRows;
                _previousLines.RemoveRange(0, amountToRemove);
            }

            foreach (var line in _previousLines.Take(_maxLineRows))
            {
                _textConsole.Cursor.Print(new ColoredString(line.Text, line.Color, Color.Transparent));
                _textConsole.Cursor.CarriageReturn();
                _textConsole.Cursor.LineFeed();
            }
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            var baseValue = base.ProcessKeyboard(info);

            if (_textInput.DisableKeyboard && info.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.DeveloperConsole)))
            {
                if (IsVisible)
                    Hide();
                else
                    Show();
                return true;
            }

            // Check for enter key press
            for (int i=0; i < info.KeysPressed.Count; i++)
            {
                if (info.KeysPressed[i].Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    if (!ParseCommand(_textInput.Text, out string output) && !string.IsNullOrWhiteSpace(output))
                        WriteText(output, Color.Red);
                    else
                        WriteText(output, Color.Green);

                    // Empty textfield but make sure we can keep typing
                    _textInput.Text = string.Empty;
                    _textInput.DisableKeyboard = false;
                    return true;
                }
            }

            return baseValue;
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            if (!_textInput.DisableKeyboard && state.Mouse.LeftClicked && !_textInput.MouseBounds.Contains(state.CellPosition))
            {
                _textInput.FocusLost();
                return true;
            }
            
            return base.ProcessMouse(state);
        }

        protected override void OnInvalidate()
        {
            base.OnInvalidate();

            // Draw borders for the controls console
            this.DrawBorders(Width, Height, "O", "|", "-", Color.Gray);
            Print(((Width / 2) - "Developer Console".Length / 2), 0, "Developer Console", Color.Orange);
        }

        public bool ParseCommand(string text, out string output)
        {
            output = "";
            if (Match(text, "clear"))
            {
                _previousLines.Clear();
                _textConsole.Clear();
                _textConsole.Cursor.Position = new Point(0, 0);
            }
            
            return false;
        }

        private bool Match(string text, string expected)
        {
            return text.Equals(expected, System.StringComparison.OrdinalIgnoreCase);
        }

        public void ClearConsole()
        {
            _previousLines.Clear();
        }
    }
}
