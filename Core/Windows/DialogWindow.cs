using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Windows
{
    public class DialogWindow : SadConsole.Console
    {
        private SadConsole.Console _textConsole;

        public DialogWindow(int width, int height) : base(width, height)
        {
            _textConsole = new SadConsole.Console(Constants.GameWindowWidth / 2 - 2, 4);
            _textConsole.Position = new Point(1, 1);
            _textConsole.DefaultBackground = Color.Black;

            Children.Add(_textConsole);
        }

        public void ShowDialog(string dialogTitle, string[] dialogLines)
        {
            Print(0, 0, dialogTitle,Color.Orange);
            _textConsole.Cursor.Position = new Point(0, 0);
            foreach (var line in dialogLines)
            {
                _textConsole.Cursor.Print(" " + line);
                _textConsole.Cursor.Print("\r\n ");
            }
            IsVisible = true;
        }

        public void CloseDialog()
        {
            IsVisible = false;
        }
    }
}
