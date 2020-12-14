using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Emberpoint.Core
{
    public class DeveloperCommands
    {
        public delegate TReturn CustomFunc<TReturn, TParam1, TParam2, TOutValue>(TParam1 param1, TParam2 param2, out TOutValue output);

        public static Dictionary<string, CustomFunc<bool, string,  DeveloperWindow, string>> Commands = 
            new Dictionary<string, CustomFunc<bool, string, DeveloperWindow, string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "clear", Clear },
                { "playerpos", GetPlayerPos },
                { "teleport", TeleportPlayer }
            };

        private static bool Clear(string text, DeveloperWindow window, out string output)
        {
            output = "";
            window.ClearConsole();
            return true;
        }

        private static bool GetPlayerPos(string text, DeveloperWindow window, out string output)
        {
            output = $"Player pos: {Game.Player.Position}";
            return true;
        }

        private static bool TeleportPlayer(string text, DeveloperWindow window, out string output)
        {
            var args = text.Split(' ');
            if (args.Length != 3 || !int.TryParse(args[1], out int x) || !int.TryParse(args[2], out int y))
            {
                output = "Invalid command arguments.";
                return false;
            }
            var position = new Point(x, y);
            Game.Player.MoveTowards(position, false);
            output = $"Teleported player from {Game.Player.Position} to {position}";
            return true;
        }
    }
}
