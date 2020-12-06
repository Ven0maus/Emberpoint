using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class InteractionWindow : Console, IUserInterface
    {
        private readonly Console _textConsole;

        public Console Console
        {
            get { return this; }
        }

        private readonly int _maxLineRows;

        public InteractionWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);
            Print(3, 0, "Interaction", Color.Orange);

            _maxLineRows = Height - 2;
            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(2, 1),
            };

            Position = new Point(Constants.Map.Width + 7, 18);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void PrintMessage(string Message)
        {
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);
            _textConsole.Cursor.Print(Message);
        }
    }
}