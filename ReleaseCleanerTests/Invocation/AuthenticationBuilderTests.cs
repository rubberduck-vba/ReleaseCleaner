using System;
using NUnit.Framework;

namespace ReleaseCleaner.Invocation
{
    using CUT = AuthenticationBuilder;

    [TestFixture]
    [Category("UnitTest")]
    [Category("Argument Parsing")]
    public class AuthenticationBuilderTests 
    {
        private CUT cut;

        [SetUp]
        public void Setup() 
        {
            cut = new CUT();
        }

        [Test]
        public void SettingMultipleTokens_Throws()
        {
            cut.SetToken("");
            Assert.Throws<InvalidOperationException>(() => cut.SetToken(""));
        }

        [Test]
        public void SettingMultipleUsernames_Throws()
        {
            cut.SetUsername("");
            Assert.Throws<InvalidOperationException>(() => cut.SetUsername(""));
        }

        [Test]
        public void SettingMultiplePasswords_Throws()
        {
            cut.SetPassword("");
            Assert.Throws<InvalidOperationException>(() => cut.SetPassword(""));
        }

        [Test]
        public void SettingMultipleAuthMethods_IsInvalid()
        {
            cut.SetToken("");
            cut.SetUsername("");
            Assert.IsFalse(cut.IsValid());
        }

        [Test]
        public void SettingOnlyPassword_IsInvalid()
        {
            cut.SetPassword("");
            Assert.IsFalse(cut.IsValid());
        }

        [Test]
        public void SettingToken_MarksTokenAuth() 
        {
            cut.SetToken("");
            Assert.IsTrue(cut.IsTokenAuth);
        }

        [Test]
        public void SettingUsername_MarksPasswordAuth()
        {
            cut.SetUsername("");
            Assert.IsTrue(cut.IsPasswordAuth);
        }

        [Test]
        public void SettingPassword_MarksPasswordAuth()
        {
            cut.SetPassword("");
            Assert.IsTrue(cut.IsPasswordAuth);
        }
    }
}