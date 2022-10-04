using Emberpoint.Core.Extensions;
using NUnit.Framework;
using SadRogue.Primitives;
using System;

namespace Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void MonoGameExtension_PointTranslate_Correct()
        {
            Point testPoint = new Point(10,10);
            Point  expectedPoint = new Point(20,20);
            Assert.AreEqual(expectedPoint, testPoint.Translate(10,10));
            Assert.AreNotEqual(0, testPoint.Translate(10, 10)); 
        }

        [Test]
        public void MonoGameExtension_PointPointTranslate_Correct()
        {
            Point testPoint = new Point(10, 10);
            Point otherTestPoint = new Point(10, 10);
            Point expectedPoint = new Point(20, 20);
            Assert.AreEqual(expectedPoint, testPoint.Translate(otherTestPoint));
            Assert.AreNotEqual(0, testPoint.Translate(otherTestPoint));
        }

        [Test]
        public void MonoGameExtension_FloatSquaredDistance_Correct()
        {
            Point testPoint = new Point(10, 10);
            Point otherTestPoint = new Point(20, 20);
            Assert.AreEqual(200, testPoint.SquaredDistance(otherTestPoint));
            Assert.AreNotEqual(0, testPoint.SquaredDistance(otherTestPoint));
        }

        [Test]
        public void MonoGameExtension_Distance_Correct()
        {
            Point testPoint = new Point(10, 10);
            Point otherTestPoint = new Point(20, 20);
            float expectedValue = (float)Math.Sqrt(200);
            Assert.AreEqual(expectedValue, testPoint.Distance(otherTestPoint));
            Assert.AreNotEqual(0, testPoint.Distance(otherTestPoint));
        }
    }
}
