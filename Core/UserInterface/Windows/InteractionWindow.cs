using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.Resources;
using SadConsole;
using SadRogue.Primitives;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class InteractionWindow : Window, IUserInterface
    {
        private System.Func<string> _currentMessage;

        public Console Console => this;

        public InteractionWindow(int width, int height) : base(width, height)
        {
            Title = Strings.Interaction;
            Position = (Constants.Map.Width + 7, 18);
            GameHost.Instance.Screen.Children.Add(this);
            Draw();
        }

        public void Refresh()
        {
            if (!string.IsNullOrWhiteSpace(_currentMessage?.Invoke()))
                PrintMessage(_currentMessage);
            else
                ClearMessage();
        }

        public void PrintMessage(System.Func<string> message)
        {
            _currentMessage = message;
            Content.Clear();
            Content.Print(0, 0, message());
        }

        public void ClearMessage()
        {
            Content.Clear();
        }
    }
}