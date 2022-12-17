using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Abstracts
{
    public class Blueprint
    {
        protected Blueprint()
        { }

        internal static Dictionary<char, BlueprintTile> GetTilesFromConfig()
        {
            var configs = GetConfigurations();
            var tiles = GetBlueprintConfigValuePairs(configs.Where(a => !a.Name.Equals("Items", StringComparison.OrdinalIgnoreCase)));

            return new Dictionary<char, BlueprintTile>(tiles);
        }

        public static void ValidateConfigurationPaths()
        {
            var configPaths = GetConfigurationPaths();
            foreach (var path in configPaths)
            {
                if (!File.Exists(path))
                    throw new Exception("Invalid configuration path defined: " + path);
            }
        }

        internal static string[] GetConfigurationPaths() => new[]
        {
            Path.Combine(Constants.Blueprint.CellBlueprintsConfigDirectoryPath, Constants.Blueprint.Prints.Items + ".json"),
            Path.Combine(Constants.Blueprint.CellBlueprintsConfigDirectoryPath, Constants.Blueprint.Prints.BlueprintTiles + ".json"),
            Path.Combine(Constants.Blueprint.CellBlueprintsConfigDirectoryPath, Constants.Blueprint.Prints.SpecialCharacters + ".json"),
        };

        internal static IEnumerable<BlueprintConfig> GetConfigurations(IEnumerable<string> configPaths) =>
            configPaths.Select(path => JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(path)));

        internal static IEnumerable<BlueprintConfig> GetConfigurations() =>
            GetConfigurationPaths().Where(a => File.Exists(a))
            .Select(path => JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(path)));

        internal static Dictionary<string, BlueprintTile[]> GetConfigurationsDictionary() =>
            GetConfigurations(GetConfigurationPaths().Where(a => File.Exists(a)))
            .ToDictionary(a => a.Name, a => a.Tiles);

        private static IEnumerable<KeyValuePair<char, BlueprintTile>> GetBlueprintConfigValuePairs(IEnumerable<BlueprintConfig> configs) =>
            configs.SelectMany(config => config.Tiles.Select(a => new KeyValuePair<char, BlueprintTile>(a.Glyph, a)));
    }

    [Serializable]
    internal class BlueprintConfig
    {
#pragma warning disable 0649
        public string Name;
        public BlueprintTile[] Tiles;
#pragma warning restore 0649
    }

    [Serializable]
    internal class BlueprintTile
    {
#pragma warning disable 0649
        public string Class;
        public char Glyph;
        public string Name;
        public bool Walkable;
        public bool Interactable;
        public string Foreground;
        public string Background;
        public bool BlocksFov;
        public bool EmitsLight;
        public string LightColor;
        public int LightRadius;
        public float Brightness;
#pragma warning restore 0649
        public static BlueprintTile Null()
        {
            var nullTile = new BlueprintTile
            {
                Glyph = ' ',
                Foreground = "BurlyWood",
                Background = "Black",
                Name = null,
                Walkable = false
            };

            return nullTile;
        }
    }
}
