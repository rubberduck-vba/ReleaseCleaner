

namespace ReleaseCleaner.Invocation
{
    using System;
    using Moq;
    using NUnit.Framework;
    using Octokit;
    using ReleaseCleaner.Input;
    using CUT = ReleaseCleaner.Invocation.Authentication;

    [TestFixture]
    [Category("UnitTest")]
    [Category("Argument Parsing")]
    public class AuthenticationTests
    {
        Mock<IConsole> console = new Mock<IConsole>();

        [SetUp]
        public void Setup()
        {
            console.Reset();
        }

        [TearDown]
        public void EnforcedVerify()
        {
            console.VerifyAll();
            console.VerifyNoOtherCalls();
        }


        [Test]
        public void EmptyAuthBuilder_PromptsForBoth()
        {
            const string username = "User";
            const string password = "Password";

            console.Setup(c => c.ReadUsername()).Returns(username).Verifiable();
            console.Setup(c => c.ReadPassword(It.Is<string>(u => u == username)))
                .Returns(password)
                .Verifiable();

            var empty = new AuthenticationBuilder();
            var result = new CUT(empty, console.Object);

            Assert.AreEqual(new Credentials(username, password), result.Credentials);
        }

        [Test]
        public void AuthBuilder_WithUsername_PromptsPassword()
        {
            const string username = "User";
            const string password = "Password";

            console.Setup(c => c.ReadPassword(It.Is<string>(u => u == username)))
                .Returns(password)
                .Verifiable();
            
            var builder = new AuthenticationBuilder();
            builder.SetUsername(username);

            var result = new CUT(builder, console.Object);

            Assert.AreEqual(new Credentials(username, password), result.Credentials);
        }

        [Test]
        public void AuthBuilder_FullyInitialized_PromptsNothing()
        {
            const string username = "User";
            const string password = "Password";

            var builder = new AuthenticationBuilder();
            builder.SetUsername(username);
            builder.SetPassword(password);

            var result = new CUT(builder, console.Object);

            Assert.AreEqual(new Credentials(username, password), result.Credentials);
        }

        [Test]
        public void AuthBuilder_WithToken_PromptsNothing()
        {
            const string token = "someTokenDescriptor";

            var builder = new AuthenticationBuilder();
            builder.SetToken(token);

            var result = new CUT(builder, console.Object);

            Assert.AreEqual(new Credentials(token), result.Credentials);
        }

        [Test]
        public void InvalidAuthBuilder_ProvokesException()
        {
            var builder = new Mock<AuthenticationBuilder>();
            builder.Setup(b => b.IsValid()).Returns(false).Verifiable();

            Assert.Throws<InvalidOperationException>(() => new CUT(builder.Object, console.Object));

            builder.VerifyAll();
            builder.VerifyNoOtherCalls();
        }
    }
}