using Emberpoint.Core.Resources;
using SadConsole;
using SadConsole.Readers;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Emberpoint.Core
{
    public sealed class Constants
    {
        public const int GameWindowWidth = 120;
        public const int GameWindowHeight = 41;

        private static string _language;
        public static string Language
        {
            get { return _language ?? (Language = "en-US"); }
            set
            {
                if (value == null) return;
                _language = value;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(value);
                Game.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }

        public static readonly Dictionary<string, Func<string>> SupportedCultures = new Dictionary<string, Func<string>>
        {
            { "en-US", () => Strings.English },
            { "nl-BE", () => Strings.Dutch },
        };

        public static readonly string ApplicationRoot = GetApplicationRoot();
        public static readonly ResourceHelper ResourceHelper = new ResourceHelper();

        public static class Map
        {
            public const int Width = 70;
            public const int Height = 30;

            // Note: changing the size, means you must also adapt the map's viewport rectangle size.
            public const IFont.Sizes Size = IFont.Sizes.Three;
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
            public static string BlueprintsDirectoryPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints");
            public static string BlueprintsConfigDirectoryPath = Path.Combine(ApplicationRoot, "Core", "GameObjects", "Blueprints", "Config");

            public static class Prints
            {
                public const string BlueprintTiles = "BlueprintTiles";
                public const string SpecialCharacters = "SpecialCharacters";
            }

            public static class Tests
            {
                public static string TestBlueprintsPath = Path.Combine(ApplicationRoot, "Tests", "TestObjects", "Blueprints", "BlueprintTexts");
            }
        }

        private static string GetApplicationRoot()
        {
            var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("\\bin"));
            return appRoot.Substring(0, appRoot.LastIndexOf("\\") + 1);
        }

        public static class Fonts
        {
            static readonly string FontsDirectoryPath = "./Resources/Fonts/";
            static readonly List<TheDrawFont> s_drawFonts = new();

            public static IFont Default => GameHost.Instance.DefaultFont;
            public static IFont ThickSquare8 => GetFont("thick_square_8x8.font");
            public static TheDrawFont BigIce => GetDrawFont("BIGICE_F.TDF");
            

            static IFont GetFont(string fontName)
            {
                if (GameHost.Instance.Fonts.ContainsKey(fontName)) 
                    return GameHost.Instance.Fonts[fontName];
                else
                {
                    try
                    {
                        var font = GameHost.Instance.LoadFont(FontsDirectoryPath + fontName);
                        return font;
                    }
                    catch (System.Runtime.Serialization.SerializationException)
                    {
                        throw new ArgumentException($"There has been a problem while loading the font {fontName}.");
                    }
                }
            }

            static TheDrawFont GetDrawFont(string fontName)
            {
                if (s_drawFonts.Find(f => f.Title == fontName) is TheDrawFont df)
                    return df;
                else
                {
                    var fontEnumerable = TheDrawFont.ReadFonts(FontsDirectoryPath + fontName);
                    if (fontEnumerable is null)
                    {
                        throw new ArgumentException($"There has been a problem while loading the DrawFont {fontName}.");
                    }
                    df = fontEnumerable.First();
                    s_drawFonts.Add(df);
                    return df;
                }
            }
        }
    }
}
