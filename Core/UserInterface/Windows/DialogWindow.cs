using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Dialogs;
using Emberpoint.Core.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

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
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);

            _queuedDialogs = new Queue<DialogBuilder.Dialog>();
            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(1, 1),
                DefaultBackground = Color.Black
            };

            Position = new Point(5, Constants.GameWindowHeight - 7);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void Update()
        {
            // Re-draw current dialog
            if (_displayedDialog != null)
            {
                Clear();
                this.DrawBorders(Width, Height, "O", "|", "-", Color.Gray);
                Position = new Point(5, Constants.GameWindowHeight - 7);

                _textConsole.Clear();
                Print(3, 0, _displayedDialog.Title, Color.Orange);
                _textConsole.Clear();
                _textConsole.Cursor.Position = new Point(0, 0);
                foreach (var line in _displayedDialog.Content.Take(4))
                {
                    _textConsole.Cursor.Print(" " + line);
                    _textConsole.Cursor.Print("\r\n");
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
            if (dialogLines == null || dialogLines.Length == 0 || dialogLines.Length > 4)
                throw new System.Exception("Invalid dialog lines, must not be null & cannot be more than 4 lines.");
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

            Update();
            IsVisible = true;
        }
    }
}