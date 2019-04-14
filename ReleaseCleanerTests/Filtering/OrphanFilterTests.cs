using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Octokit;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner.Filtering
{
    [TestFixture]
    [Category("Core Logic")]
    [Category("Filtering")]
    public class OrphanMatcherTests 
    {
        private readonly Mock<IGitHubClient> clientMock = new Mock<IGitHubClient>();
        private OrphanMatcher cut;

        [SetUp]
        public void TestSetup() 
        {
            clientMock.Reset();
            cut = new OrphanMatcher(clientMock.Object);
        }

        [Test]
        public void NoOrphanFlag_DoesNotIncurRequest()
        {
            var args = ArgumentsMockData.BaseBuilder();
            // OrphanOnly defaults to false
            cut.Prepare(args.Build());
            var result = cut.Matches(null);
            Assert.IsTrue(result);
            clientMock.VerifyNoOtherCalls();
        }

        [Test]
        public void PrepareWithOrphanFlag_HitsApi()
        {
            var args = ArgumentsMockData.BaseBuilder();
            args.Orphans();

            var repositoryMock = new Mock<IRepositoriesClient>();
            clientMock.SetupGet(client => client.Repository).Returns(repositoryMock.Object);
            repositoryMock.Setup(repository => repository.GetAllTags(ArgumentsMockData.Owner, ArgumentsMockData.Project))
                .Returns(Task.FromResult<IReadOnlyList<RepositoryTag>>(new List<RepositoryTag>()))
                .Verifiable();
            cut.Prepare(args.Build());

            Mock.VerifyAll(clientMock, repositoryMock);
            clientMock.VerifyNoOtherCalls();
            repositoryMock.VerifyNoOtherCalls();
        }

        [TestCase(true, "exists", false, Description = "Draft and existing Tag is not orphan")]
        [TestCase(true, "missing", true, Description = "Draft and missing Tag is an orphan")]
        [TestCase(false, "exists", false, Description = "NonDraft and existing Tag is not orphan")]
        [TestCase(false, "missing", false, Description = "NonDraft and missing Tag is not orphan")]
        public void OrphaningLogic(bool draftStatus, string tagName, bool expected)
        {
            var release = new Release(
                null, null, null, null, 0, null, 
                tagName: tagName,
                null, null, null,
                draft: draftStatus, false, default, null, default, null, null, new List<ReleaseAsset>()
            );
            
            var args = ArgumentsMockData.BaseBuilder();
            args.Orphans();

            var repositoryMock = new Mock<IRepositoriesClient>();
            clientMock.SetupGet(client => client.Repository).Returns(repositoryMock.Object);
            repositoryMock.Setup(repository => repository.GetAllTags(ArgumentsMockData.Owner, ArgumentsMockData.Project))
                .Returns(Task.FromResult<IReadOnlyList<RepositoryTag>>(new List<RepositoryTag>(new [] { new RepositoryTag("exists", default, default, default, default)})))
                .Verifiable();
            cut.Prepare(args.Build());

            Assert.AreEqual(expected, cut.Matches(release));

            Mock.VerifyAll(clientMock, repositoryMock);
            clientMock.VerifyNoOtherCalls();
            repositoryMock.VerifyNoOtherCalls();
        }
    }
}