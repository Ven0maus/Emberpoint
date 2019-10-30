using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace Emberpoint.Core
{
    public sealed class Constants
    {
        public static string ApplicationRoot = GetApplicationRoot();
        public const string GameTitle = "Emberpoint";
        public const int GameWindowWidth = 120;
        public const int GameWindowHeight = 40;

        public static class Map
        {
            public const int Width = 70;
            public const int Height = 30;
        }

        public static class Player
        {
            public const char Character = '@';
            public static Color Foreground = Color.White;
            public const int FieldOfViewRadius = 4;
        }

        public static class Blueprint
        {
            public static string SpecialCharactersPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints", "SpecialCharactersConfig.json");
            public static string BlueprintsPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints");
            public static string BlueprintsConfigPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints", "Config");
        }

        private static string GetApplicationRoot()
        {
            var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("\\bin"));
            return appRoot.Substring(0, appRoot.LastIndexOf("\\") + 1);
        }
    }
}
