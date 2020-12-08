using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Console = SadConsole.Console;

namespace Emberpoint.Core
{
    public class DeveloperCommands
    {
        public delegate TReturn CustomFunc<TReturn, TParam1, TParam2, TOutValue>(TParam1 param1, TParam2 param2, out TOutValue output);

        public static Dictionary<string, CustomFunc<bool, Console, List<DeveloperWindow.Line>, string>> Commands = CreateCommands();

        private static Dictionary<string, CustomFunc<bool, Console, List<DeveloperWindow.Line>, string>> CreateCommands()
        {
            var commands = new Dictionary<string, CustomFunc<bool, Console, List<DeveloperWindow.Line>, string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "clear", Clear },
            };
            return commands;
        }

        private static bool Clear(Console textConsole, List<DeveloperWindow.Line> previousLines, out string output)
        {
            output = "";
            previousLines.Clear();
            textConsole.Clear();
            textConsole.Cursor.Position = new Point(0, 0);
            return true;
        }
    }
}
