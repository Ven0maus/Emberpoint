﻿using Emberpoint.Core.Resources;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows.ConsoleWindows
{
    public class InteractionWindow : BorderedWindow
    {
        private System.Func<string> _currentMessage;

        public InteractionWindow(int width, int height) : base(width, height)
        {
            Title = Strings.Interaction;
            Position = (Constants.Map.Width + 7, 18);
        }

        public override void Refresh()
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