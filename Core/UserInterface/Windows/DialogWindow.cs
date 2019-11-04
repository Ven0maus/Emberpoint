using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class DialogWindow : Console, IUserInterface
    {
        private readonly Console _textConsole;

        public Console Console
        {
            get { return this; }
        }

        public DialogWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);

            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(1, 1),
                DefaultBackground = Color.Black
            };

            Position = new Point(5, Constants.GameWindowHeight - 7);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void ShowDialog(string dialogTitle, string[] dialogLines)
        {
            Print(3, 0, dialogTitle, Color.Orange);
            _textConsole.Cursor.Position = new Point(0, 0);
            foreach (var line in dialogLines.Take(4))
            {
                _textConsole.Cursor.Print(" " + line);
                _textConsole.Cursor.Print("\r\n");
            }
            IsVisible = true;
        }

        public void CloseDialog()
        {
            IsVisible = false;
        }

        public void Update()
        {
            // Tell's sadconsole to redraw this console
            _textConsole.IsDirty = true;
            IsDirty = true;
        }
    }
}