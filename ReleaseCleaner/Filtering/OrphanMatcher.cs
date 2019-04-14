using System.Collections.Generic;
using System.Linq;
using Octokit;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner.Filtering 
{
    class OrphanMatcher : IReleasePredicate 
    {
        private readonly IGitHubClient client;
        private HashSet<string> tags = new HashSet<string>();
        private bool skipMatcher = false;

        public OrphanMatcher(IGitHubClient client)
        {
            this.client = client;
        }

        public void Prepare(Arguments arguments)
        {
            skipMatcher = !arguments.OrphanOnly;
            if (skipMatcher) return;
            var tagQuery = client.Repository.GetAllTags(arguments.ProjectOwner, arguments.ProjectName);
            tagQuery.Wait();
            tags = tagQuery.Result.Select(repoTag => repoTag.Name).ToHashSet();
        }

        public bool Matches(Release releaseInfo)
        {
            if (skipMatcher) 
            {
                return true;
            }
            return releaseInfo.Draft && !tags.Contains(releaseInfo.TagName);
        }

    }
}