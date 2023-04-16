using NUnit.Framework;
using Version = LEGODeviceUnitySDK.Version;

namespace LEGO.LDSDK.Editor.Tests
{
    public class VersionTests
    {
        private static Version VersionFactory(int major, int minor, int bugfix) => new Version() { Major = major, Minor = minor, Bugfix = bugfix, Build = 0 };

        [Test]
        public void GivenCorrectVersions_CorrectComparison()
        {
            var version0 = VersionFactory(1, 2, 3);
            var version1 = VersionFactory(4, 5, 6);
            Assert.That(version1, Is.GreaterThan(version0));
            Assert.That(version0, Is.LessThan(version1));

            version0 = VersionFactory(4, 5, 6);
            Assert.AreEqual(version1, version0);
        }

        [Test]
        public void GivenIncorrectVersions_CorrectComparison()
        {
            Version version = null;
            Assert.IsNull(version);
            Assert.True(version == null);
        }
    }
}
