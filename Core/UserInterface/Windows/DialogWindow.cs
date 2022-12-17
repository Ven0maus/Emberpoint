using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Dialogs;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.Resources;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace Emberpoint.Core.UserInterface.Windows
{

    public class DialogWindow : Console, IUserInterface
    {
        private readonly Console _textConsole;
        private readonly Queue<DialogBuilder.Dialog> _queuedDialogs;

        private DialogBuilder.Dialog _displayedDialog;

        public Console Console
        {
            get { return this; }
        }

        public DialogWindow(int width, int height) : base(width, height)
        {
            _queuedDialogs = new Queue<DialogBuilder.Dialog>();
            _textConsole = new Console(Width, Height)
            {
                DefaultBackground = Color.Black
            };
            Children.Add(_textConsole);
            GameHost.Instance.Screen.Children.Add(this);
        }

        void DrawWindow(string title = "")
        {
            // draw borders
            this.DrawBorders(Width, Height, "O", "|", "-", Constants.Colors.WindowBorder);

            // calculate and apply position
            Position = new Point(5, Constants.GameWindowHeight - Height - 1);

            // print title
            Surface.Print(3, 0, title, Constants.Colors.WindowTitle);

            // print prompt in the bottom right corner
            int x = Width - Strings.PressEnterPrompt.Length - 4;
            Surface.Print(x, Height - 1, Strings.PressEnterPrompt, Constants.Colors.WindowTitle);
        }

        public void Refresh()
        {
            // Re-draw current dialog
            if (_displayedDialog != null)
            {
                int padding = 1;
                int contentHeight = _displayedDialog.Content.Length;
                int maxLineLength = Width - padding * 2 - 2;

                // resize the window to the amount of lines in the dialog
                (Surface as CellSurface).Resize(Width, contentHeight + padding * 2 + 2, true);
                DrawWindow(_displayedDialog.Title);

                // resize and reposition the text window
                (_textConsole.Surface as CellSurface).Resize(maxLineLength, contentHeight, true);
                _textConsole.Position = (1 + padding, 1 + padding);

                // display the dialog
                _textConsole.Cursor.Position = new Point(0, 0);
                for (int y = 0; y < contentHeight; y++)
                {
                    string line = _displayedDialog.Content[y];
                    // truncate lines of dialog that are too long
                    line = line.Length > maxLineLength ? line[..(maxLineLength - 3)] + "...": line;
                    _textConsole.Surface.Print(0, y, line);
                }
            }
        }

        public void AddDialogs(DialogBuilder builder)
        {
            foreach (var dialog in builder.Build())
                _queuedDialogs.Enqueue(dialog);
            if (_displayedDialog == null)
                ShowNext();
        }

        public void AddDialog(string dialogTitle, string[] dialogLines)
        {
            if (dialogLines == null || dialogLines.Length == 0)
                throw new System.ArgumentOutOfRangeException(nameof(dialogLines), "Missing dialog lines.");
            var dialog = new DialogBuilder.Dialog(dialogTitle, dialogLines);
            _queuedDialogs.Enqueue(dialog);
            if (_displayedDialog == null)
                ShowNext();
        }

        /// <summary>
        /// Show's all queued dialogs.
        /// </summary>
        public void ShowNext()
        {
            if (_queuedDialogs.Count == 0) 
            {
                _displayedDialog = null;
                IsVisible = false;
                return;
            }

            var dialog = _queuedDialogs.Dequeue();
            _displayedDialog = dialog;

            Refresh();
            IsVisible = true;
        }
    }
}