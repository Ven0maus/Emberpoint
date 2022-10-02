using Emberpoint.Core;
using Emberpoint.Core.Resources;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class LocalizationTests
    {
        private string _defaultLanguage = Constants.Language;

        [SetUp]
        public void SetUp()
        {
            Constants.Language = _defaultLanguage;
        }

        [Test]
        public void ResourceHelper_ReadProperty_WorksAsExpected()
        {
            var translations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var culture in Constants.SupportedCultures)
            {
                Constants.Language = culture;
                var property = Constants.ResourceHelper.ReadProperty("DoorStateOpen");
                translations.Add(property);
                Assert.AreEqual(Strings.DoorStateOpen, property);
            }
            Assert.AreEqual(Constants.SupportedCultures.Length, translations.Count);
        }
    }
}
