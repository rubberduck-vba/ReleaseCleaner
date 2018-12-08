

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
            var result = new CUT(empty, console.Object).Credentials;

            var expected = new Credentials(username, password);
            Assert.AreEqual(expected.Login, result.Login);
            Assert.AreEqual(expected.Password, result.Password);
            Assert.AreEqual(expected.AuthenticationType, result.AuthenticationType);
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

            var result = new CUT(builder, console.Object).Credentials;
            
            var expected = new Credentials(username, password);
            Assert.AreEqual(expected.Login, result.Login);
            Assert.AreEqual(expected.Password, result.Password);
            Assert.AreEqual(expected.AuthenticationType, result.AuthenticationType);
        }

        [Test]
        public void AuthBuilder_FullyInitialized_PromptsNothing()
        {
            const string username = "User";
            const string password = "Password";

            var builder = new AuthenticationBuilder();
            builder.SetUsername(username);
            builder.SetPassword(password);

            var result = new CUT(builder, console.Object).Credentials;

            var expected = new Credentials(username, password);
            Assert.AreEqual(expected.Login, result.Login);
            Assert.AreEqual(expected.Password, result.Password);
            Assert.AreEqual(expected.AuthenticationType, result.AuthenticationType);
        }

        [Test]
        public void AuthBuilder_WithToken_PromptsNothing()
        {
            const string token = "someTokenDescriptor";

            var builder = new AuthenticationBuilder();
            builder.SetToken(token);

            var result = new CUT(builder, console.Object).Credentials;

            var expected = new Credentials(token);
            Assert.AreEqual(expected.Login, result.Login);
            Assert.AreEqual(expected.Password, result.Password);
            Assert.AreEqual(expected.AuthenticationType, result.AuthenticationType);
        }

        [Test]
        public void InvalidAuthBuilder_ProvokesException()
        {
            var builder = new AuthenticationBuilder();
            // only setting a password makes the builder invalid
            builder.SetPassword("");

            Assert.Throws<InvalidOperationException>(() => new CUT(builder, console.Object));
        }
    }
}