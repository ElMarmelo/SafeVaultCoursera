using dotenv.net;
using NUnit.Framework;
using System.IO;

namespace SafeVault.Tests
{
    [SetUpFixture]
    public class TestEnvironmentSetup
    {
        [OneTimeSetUp]
        public void LoadEnv()
        {
            var root = Directory.GetParent(
                TestContext.CurrentContext.TestDirectory
            )?.Parent?.Parent?.Parent?.FullName;

            DotEnv.Load(options: new DotEnvOptions(
                envFilePaths: new[] { Path.Combine(root!, ".env") }
            ));
        }
    }
}
