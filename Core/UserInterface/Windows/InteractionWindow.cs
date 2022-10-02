using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.Resources;
using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class InteractionWindow : Console, IUserInterface
    {
        private readonly Console _textConsole;
        private System.Func<string> _currentMessage;

        public Console Console
        {
            get { return this; }
        }


        public InteractionWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);
            Print(3, 0, Strings.Interaction, Color.Orange);

            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(2, 1),
            };

            Position = new Point(Constants.Map.Width + 7, 18);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void Update()
        {
            Clear();
            this.DrawBorders(Width, Height, "O", "|", "-", Color.Gray);
            Print(3, 0, Strings.Interaction, Color.Orange);

            if (!string.IsNullOrWhiteSpace(_currentMessage?.Invoke()))
                PrintMessage(_currentMessage);
            else
                ClearMessage();
        }

        public void PrintMessage(System.Func<string> message)
        {
            _currentMessage = message;
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);
            _textConsole.Cursor.Print(message());
        }

        public void ClearMessage()
        {
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);
        }
    }
}