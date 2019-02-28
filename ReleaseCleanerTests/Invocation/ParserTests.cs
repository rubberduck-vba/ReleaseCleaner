using NUnit.Framework;

using System;
using ReleaseCleaner.Invocation;
using CUT = ReleaseCleaner.Invocation.CommandLine;
using Moq;
using ReleaseCleaner.Input;

namespace ReleaseCleaner.Invocation
{
    [TestFixture]
    [Category("UnitTest")]
    [Category("Argument Parsing")]
    public class ParserTests 
    {
        [Test]
        [TestCase("")]
        [TestCase("-v")]
        [TestCase("rubberduck-vba/Rubberduck")]
        [TestCase("rubberduck-vba/Rubberduck -v")]
        [TestCase("-m v2.2.0.*")]
        [TestCase("-m v2.2.0.* -v")]
        [TestCase("-m v2.2.0.* -v -o rubberduck-vba")]
        [TestCase("-m v2.2.0.* -v -p Rubberduck")]
        [TestCase("--invert")]
        [TestCase("rubberduck-vba/Rubberduck")]
        [TestCase("rubberduck-vba/Rubberduck --invert")]
        [TestCase("--match v2.2.0.*")]
        [TestCase("--match v2.2.0.* --invert")]
        [TestCase("--match v2.2.0.* --invert --owner rubberduck-vba")]
        [TestCase("--match v2.2.0.* --invert --project Rubberduck")]
        public void EmptyArgs_IsRejected(string commandLine) 
        {
            Assert.Throws<ArgumentException>(() => CUT.Parse(commandLine.Split(' '), null));
        }

        [Test]
        [TestCase("rubberduck-vba/Rubberduck -m 2.2.0*", "rubberduck-vba", "Rubberduck", false, new string[]{"2.2.0*"})]
        public void ValidArgs_AreCorrectlyParsed(string commandLine, string owner, string repo, bool invert, string[] matchers)
        {
            var console = new Mock<IConsole>();
            console.Setup(c => c.ReadPassword(It.IsAny<string>())).Returns("PW");
            console.Setup(c => c.ReadUsername()).Returns("User");
            var (result, _) = CUT.Parse(commandLine.Split(' '), console.Object);
            Assert.AreEqual(owner, result.ProjectOwner);
            Assert.AreEqual(repo, result.ProjectName);
            Assert.AreEqual(invert, result.InvertedMatching);
            Assert.AreEqual(matchers, result.CleanReleases);
        }
    }

}
