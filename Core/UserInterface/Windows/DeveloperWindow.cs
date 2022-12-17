using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class DeveloperWindow : ControlsConsole, IUserInterface
    {
        /// <summary>
        /// Container for console text lines with color
        /// </summary>
        private readonly struct Line
        {
            public readonly string Text;
            public readonly Color Color;

            public Line(string text, Color color)
            {
                Text = text;
                Color = color;
            }
        }

        public Console Content => this;
        private readonly Console _textConsole;
        private readonly TextBox _textInput;
        private readonly List<Line> _previousLines;
        private readonly int _maxLineRows, _maxLineLength;

        private bool _textInputInUse = false;

        public DeveloperWindow(int width, int height) : base(width, height)
        {
            DefaultBackground = Color.Lerp(Color.Black, Color.Transparent, 0.6f);
            DefaultForeground = Color.White;

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

            // Textbox colors
            var newColors = new Colors();
            newColors.Appearance_ControlMouseDown.Background = Color.Lerp(Color.Green, Color.Transparent, 0.4f);
            newColors.Appearance_ControlMouseDown.Foreground = Color.GreenYellow;
            newColors.Appearance_ControlFocused.Background = Color.Lerp(Color.Green, Color.Transparent, 0.4f);
            newColors.Appearance_ControlFocused.Foreground = Color.GreenYellow;
            newColors.Appearance_ControlDisabled.Background = Color.Lerp(Color.Green, Color.Transparent, 0.4f);
            newColors.Appearance_ControlDisabled.Foreground = Color.GreenYellow;
            newColors.Appearance_ControlNormal.Background = Color.Lerp(Color.Green, Color.Transparent, 0.4f);
            newColors.Appearance_ControlNormal.Foreground = Color.GreenYellow;
            newColors.Appearance_ControlOver.Background = Color.Lerp(Color.Green, Color.Transparent, 0.4f);
            newColors.Appearance_ControlOver.Foreground = Color.GreenYellow;
            newColors.Appearance_ControlSelected.Background = Color.Lerp(Color.Green, Color.Transparent, 0.4f);
            newColors.Appearance_ControlSelected.Foreground = Color.GreenYellow;
            _textInput.SetThemeColors(newColors);

            Controls.Add(_textInput);

            // Make sure we have focus through parent console
            _textInput.IsFocused = true;
            // TODO: ^ [CONVERSION V9] CHECK IF THIS IS CORRECT!
            //_textInput.Parent.FocusedControl = _textInput;

            Children.Add(_textConsole);

            FocusedMode = FocusBehavior.Push;

            // Middle of screen at the top
            Position = new Point(4, 2);

            Refresh();

            GameHost.Instance.Screen.Children.Add(this);
        }

        public void BeforeCreate()
        {
            IsVisible = false;
        }

        public void Refresh()
        {
            Surface.Clear();
            // Draw borders for the controls console
            this.DrawBorders(Width, Height, "O", "|", "-", Color.Gray);
            Surface.Print(((Width / 2) - "Developer Console".Length / 2), 0, "Developer Console", Color.Orange);
        }

        public void Show()
        {
            // TODO: Animate entrance
            IsVisible = true;
            IsFocused = true;
            _textInput.Text = "Type here..";
            _textInputInUse = false;
            _textInput.DisableKeyboard = true;
            Game.Player.IsFocused = false;
        }

        public void Hide()
        {
            // TODO: Animate departure
            IsVisible = false;
            IsFocused = false;
            _textInput.Text = "Type here..";
            _textInputInUse = false;
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
                if (info.KeysPressed[i].Key == Keys.Enter && _textInputInUse)
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

        public void ClearConsole()
        {
            _previousLines.Clear();
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            // Lose focus if we click outside of the textbox
            if (!_textInput.DisableKeyboard && state.Mouse.LeftClicked && !_textInput.Bounds.Contains(state.CellPosition))
            {
                _textInput.Text = "Type here..";
                _textInput.FocusLost();
                _textInputInUse = false;
                return true;
            }

            // Clear input when clicking on the field
            if (_textInput.DisableKeyboard && state.Mouse.LeftClicked && _textInput.Bounds.Contains(state.CellPosition))
            {
                _textInput.Text = string.Empty;
                _textInputInUse = true;
            }
            
            return base.ProcessMouse(state);
        }

        private bool ParseCommand(string text, out string output)
        {
            foreach (var command in DeveloperCommands.Commands)
            {
                if (text.StartsWith(command.Key, System.StringComparison.OrdinalIgnoreCase))
                {
                    var sub = text.Substring(command.Key.Length).TrimStart();
                    return command.Value(sub, this, out output);
                }
            }
            output = "";
            return false;
        }
    }
}
