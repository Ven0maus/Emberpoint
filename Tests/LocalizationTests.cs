using Emberpoint.Core;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.Resources;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class LocalizationTests
    {
        private readonly string _defaultLanguage = Constants.Language;

        [SetUp]
        public void SetUp()
        {
            Constants.Language = _defaultLanguage;
            Constants.ResourceHelper.ResetCache();
        }

        [Test]
        public void ResourceHelper_ReadProperty_WorksAsExpected()
        {
            var translations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var culture in Constants.SupportedCultures)
            {
                Constants.Language = culture.Key;
                var property = Constants.ResourceHelper.ReadProperty("Strings.DoorStateOpen");
                translations.Add(property);
                Assert.AreEqual(Strings.DoorStateOpen, property);
            }
            Assert.AreEqual(Constants.SupportedCultures.Count, translations.Count);
        }

        [Test]
        public void ResourceHelper_AllLocalization_Covered()
        {
            var allResources = Constants.ResourceHelper.GetResources();

            // Check if all resources have a valid localization value
            foreach (var resource in allResources)
            {
                foreach (var kvp in resource.Value)
                {
                    Assert.IsNotNull(kvp.Value, "Localization value for '" + kvp.Key + "' was not defined for culture '"+resource.Key+"'.");
                    Assert.IsNotEmpty(kvp.Value, "Localization value for '" + kvp.Key + "' was empty for culture '" + resource.Key + "'.");
                }
            }

            // Check if defined tile config that uses localization, has strings defined
            var blueprintConfigs = Blueprint.GetTilesFromConfig();
            foreach (var tile in blueprintConfigs.Values)
            {
                if (tile.Name != null && tile.Name.StartsWith("Strings.", StringComparison.OrdinalIgnoreCase))
                {
                    var localizationValue = Constants.ResourceHelper.ReadProperty(tile.Name);
                    Assert.IsNotNull(localizationValue, "Localization not found for: " + tile.Name);
                    Assert.IsNotEmpty(localizationValue, "Localization was empty for: " + tile.Name);
                }
            }
        }
    }
}
