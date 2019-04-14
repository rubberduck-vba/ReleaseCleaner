using Octokit;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner.Filtering
{
    internal interface IReleasePredicate
    {
        void Prepare(Arguments arguments);
        bool Matches(Release releaseInfo);
    }
}