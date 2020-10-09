using Emberpoint.Core.Extensions;
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
        private readonly Queue<Dialog> _queuedDialogs;

        public Console Console
        {
            get { return this; }
        }

        public DialogWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);

            _queuedDialogs = new Queue<Dialog>();
            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(1, 1),
                DefaultBackground = Color.Black
            };

            Position = new Point(5, Constants.GameWindowHeight - 7);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void AddDialog(string dialogTitle, string[] dialogLines)
        {
            var dialog = new Dialog(dialogTitle, dialogLines);
            _queuedDialogs.Enqueue(dialog);
        }

        /// <summary>
        /// Show's all queued dialogs.
        /// </summary>
        public void ShowNext()
        {
            if (_queuedDialogs.Count == 0) 
            {
                IsVisible = false;
                return;
            }

            var dialog = _queuedDialogs.Dequeue();

            Print(3, 0, dialog.Title, Color.Orange);
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);
            foreach (var line in dialog.Content.Take(4))
            {
                _textConsole.Cursor.Print(" " + line);
                _textConsole.Cursor.Print("\r\n");
            }
            IsVisible = true;
        }

        private class Dialog
        {
            public string Title;
            public string[] Content;

            public Dialog(string title, string[] content)
            {
                Title = title;
                Content = content;
            }
        }
    }
}