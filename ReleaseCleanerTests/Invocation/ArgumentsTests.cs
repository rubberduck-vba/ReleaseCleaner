using NUnit.Framework;

using System;
using ReleaseCleaner.Invocation;

namespace Tests.Invocation
{
    [TestFixture]
    public class ArgumentParserTests 
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
            Assert.Throws<ArgumentException>(() => Arguments.Parse(commandLine.Split(' ')));
        }

        [Test]
        [TestCase("rubberduck-vba/Rubberduck -m 2.2.0*", "rubberduck-vba", "Rubberduck", false, new string[]{"2.2.0*"})]
        public void ValidArgs_AreCorrectlyParsed(string commandLine, string owner, string repo, bool invert, string[] matchers)
        {
            var result = Arguments.Parse(commandLine.Split(' '));
            Assert.AreEqual(owner, result.ProjectOwner);
            Assert.AreEqual(repo, result.ProjectName);
            Assert.AreEqual(invert, result.InvertedMatching);
            Assert.AreEqual(matchers, result.CleanReleases);
        }
    }

}
