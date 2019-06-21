
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Octokit;
using ReleaseCleaner.Filtering;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner 
{
    [TestFixture]
    [Category("Core Logic")]
    [Category("Github Interaction")]
    public class CleanerTests 
    {
        private Cleaner cut;
        private Mock<IGitHubClient> clientMock = new Mock<IGitHubClient>();

        [SetUp]
        public void SetUp() {
            clientMock.Reset();
            cut = new Cleaner(clientMock.Object);
        }

        [Test]
        public void GivenSimpleArguments_FindReleases_RetrievesTagsAndReleases()
        {
            const string repoOwner = "foo";
            const string repoName = "bar";
            var repoMock = new Mock<IRepositoriesClient>();
            clientMock.Setup(client => client.Repository).Returns(repoMock.Object).Verifiable();
            var releasesMock = new Mock<IReleasesClient>();
            repoMock.SetupGet(repo => repo.Release).Returns(releasesMock.Object).Verifiable();
            releasesMock.Setup(releases => releases.GetAll(repoOwner, repoName))
                .Returns(Task.FromResult<IReadOnlyList<Release>>(new List<Release>()))
                .Verifiable();
            
            var argBuilder = new ArgumentsBuilder();
            argBuilder.AddMatcher(".*");
            argBuilder.SetOwner(repoOwner);
            argBuilder.SetProject(repoName);

            var results = cut.FindReleases(argBuilder.Build(), new ConstantValueFilter(true));

            Mock.VerifyAll(clientMock, repoMock, releasesMock);
            Assert.AreEqual(0, results.Count);
        }

    }
}