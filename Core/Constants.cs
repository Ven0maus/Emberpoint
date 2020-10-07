using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.IO;

namespace Emberpoint.Core
{
    public sealed class Constants
    {
        public static string ApplicationRoot = GetApplicationRoot();
        public const string GameTitle = "Emberpoint";
        public const int GameWindowWidth = 120;
        public const int GameWindowHeight = 41;

        public static class Map
        {
            public const int Width = 70;
            public const int Height = 30;

            // Note: changing the size, means you must also adapt the map's viewport rectangle size.
            public const Font.FontSizes Size = Font.FontSizes.Three;
        }

        public static class Player
        {
            public const char Character = '@';
            public static Color Foreground = Color.White;
            public const int FieldOfViewRadius = Items.FlashlightRadius;
            public const int DiscoverLightsRadius = 8;
        }

        public static class Items
        {
            public const int BatteryMaxPower = 60;
            public const float FlashlightBrightness = 0.5f;
            public const int FlashlightRadius = 5;
        }

        public static class Blueprint
        {
            public static string SpecialCharactersPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints", "SpecialCharactersConfig.json");
            public static string BlueprintsPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints");
            public static string TestBlueprintsPath = Path.Combine(ApplicationRoot, "Tests", "TestObjects", "Blueprints", "BlueprintTexts");
            public static string BlueprintsConfigPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints", "Config");
            public const string BlueprintTiles = "BlueprintTiles";
        }

        private static string GetApplicationRoot()
        {
            var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("\\bin"));
            return appRoot.Substring(0, appRoot.LastIndexOf("\\") + 1);
        }
    }
}
