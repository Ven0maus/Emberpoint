using Emberpoint.Core.UserInterface.Windows;
using System;
using System.Collections.Generic;

namespace Emberpoint.Core
{
    public class DeveloperCommands
    {
        public delegate TReturn CustomFunc<TReturn, TParam1, TOutValue>(TParam1 param1, out TOutValue output);

        public static Dictionary<string, CustomFunc<bool, DeveloperWindow, string>> Commands = CreateCommands();

        private static Dictionary<string, CustomFunc<bool, DeveloperWindow, string>> CreateCommands()
        {
            var commands = new Dictionary<string, CustomFunc<bool, DeveloperWindow, string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "clear", Clear },
            };
            return commands;
        }

        private static bool Clear(DeveloperWindow window, out string output)
        {
            output = "";
            window.ClearConsole();
            return true;
        }
    }
}
