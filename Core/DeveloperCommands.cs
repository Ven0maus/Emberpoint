﻿using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core
{
    public class DeveloperCommands
    {
        public delegate TReturn CustomFunc<TReturn, TParam1, TParam2, TOutValue>(TParam1 param1, TParam2 param2, out TOutValue output);

        public static Dictionary<string, CustomFunc<bool, string, DeveloperWindow, string>> Commands =
            new Dictionary<string, CustomFunc<bool, string, DeveloperWindow, string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "clear", Clear },
                { "playerpos", GetPlayerPos },
                { "teleport", TeleportPlayer },
                { "setlanguage", SetLanguage }
            };

        private static bool SetLanguage(string text, DeveloperWindow window, out string output)
        {
            var lang = Constants.SupportedCultures.FirstOrDefault(m => m.Key.Equals(text, StringComparison.OrdinalIgnoreCase));
            if (lang.IsDefault())
            {
                output = "Invalid language ("+text+"), use any of the following: " + string.Join(", ", Constants.SupportedCultures.Select(a => "'" + a.Key + "'"));
                return false;
            }

            Constants.Language = lang.Key;

            // Update views
            var views = UserInterfaceManager.GetAll<IUserInterface>();
            foreach (var view in views)
                view.Update();

            output = "Language set to: " + lang.Key;
            return true;
        }

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
            if (args.Length != 2 || !int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y))
            {
                output = "Invalid command arguments.";
                return false;
            }
            var prevPosition = Game.Player.Position;
            Game.Player.MoveTowards(new Point(x, y), false);

            // Discover radius around player on teleport
            var prevRadius = Game.Player.FieldOfViewRadius;
            Game.Player.FieldOfViewRadius = Constants.Items.FlashlightRadius;
            EntityManager.RecalculatFieldOfView(Game.Player, true, true);
            Game.Player.FieldOfViewRadius = prevRadius;
            EntityManager.RecalculatFieldOfView(Game.Player, false);

            output = $"Teleported player from {prevPosition} to {Game.Player.Position}";
            return true;
        }
    }
}
