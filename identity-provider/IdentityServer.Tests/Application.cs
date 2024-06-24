using Alba;

namespace IdentityServer.Tests
{
    [SetUpFixture]
    public class Application
    {
        [OneTimeSetUp]
        public async Task Init()
        {
            Host = await AlbaHost.For<Program>();
        }

        public static IAlbaHost Host { get; private set; } = null!;

        // Make sure that NUnit will shut down the AlbaHost when
        // all the projects are finished
        [OneTimeTearDown]
        public void Teardown()
        {
            Host.Dispose();
        }
    }
}