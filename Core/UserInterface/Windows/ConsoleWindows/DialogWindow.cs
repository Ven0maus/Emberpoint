using Emberpoint.Core.GameObjects.Dialogs;
using Emberpoint.Core.Resources;
using SadConsole;
using System.Collections.Generic;

namespace Emberpoint.Core.UserInterface.Windows.ConsoleWindows
{
    public class DialogWindow : Window
    {
        private readonly Queue<DialogBuilder.Dialog> _queuedDialogs;
        private DialogBuilder.Dialog _displayedDialog;

        public DialogWindow(int width, int height) : base(width, height)
        {
            _queuedDialogs = new Queue<DialogBuilder.Dialog>();
            GameHost.Instance.Screen.Children.Add(this);
            Position = (5, Constants.GameWindowHeight - Height - 1);
            Prompt = Strings.PressEnterPrompt;
        }

        public override void Refresh()
        {
            // Re-draw current dialog
            if (_displayedDialog != null)
            {
                int lineCount = _displayedDialog.Content.Length;
                if (Content.Height != lineCount)
                {
                    ResizeContentHeight(lineCount);
                    Position = (5, Constants.GameWindowHeight - Height - 1);
                }

                Content.Clear();
                Title = _displayedDialog.Title;

                // display the dialog
                for (int y = 0; y < lineCount; y++)
                {
                    string line = _displayedDialog.Content[y];
                    Content.Print(0, y, line);
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