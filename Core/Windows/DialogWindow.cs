using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Windows
{
    public class DialogWindow : Window
    {

        private SadConsole.Console _textConsole;

        public DialogWindow(int width, int height) : base(width, height)
        {
            CanDrag = false;

            _textConsole = new SadConsole.Console(Constants.GameWindowWidth / 2 - 2, 4);
            _textConsole.Position = new Point(1, 1);
            _textConsole.DefaultBackground = Color.Black;

            Children.Add(_textConsole);
        }

        public void ShowDialog(string dialogTitle, string[] dialogLines)
        {
            Title = dialogTitle.Align(HorizontalAlignment.Left, Width);
            _textConsole.Cursor.Position = new Point(0, 0);
            foreach (var line in dialogLines)
            {
                _textConsole.Cursor.Print(" " + line);
                _textConsole.Cursor.Print("\r\n ");
            }
            Show();
        }

        public void CloseDialog()
        {
            Title = "";
            this.Hide();
        }
    }
}
