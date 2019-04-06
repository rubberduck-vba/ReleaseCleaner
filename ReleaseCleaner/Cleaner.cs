using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Octokit;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner
{
    internal class Cleaner
    {
        private readonly GitHubClient client;

        public Cleaner(GitHubClient client) 
        {
            this.client = client;
        }

        public List<Release> FindReleases(Arguments arguments)
        {
            // query must not be started explicitly, execution is already performed by the library
            var query = client.Repository.Release.GetAll(arguments.ProjectOwner, arguments.ProjectName);
            var tagQuery = client.Repository.GetAllTags(arguments.ProjectOwner, arguments.ProjectName);

            var compiledMatchers = arguments.CleanReleases.Select(specification => BuildRegularExpression(specification));
            // await query results before filtering
            query.Wait();
            tagQuery.Wait();
            var tags = tagQuery.Result.Select(repoTag => repoTag.Name).ToHashSet();
            return query.Result
                .Where(release => compiledMatchers.Any(matcher => MatchesRelease(matcher, release, arguments, tags)))
                .ToList();
        }

        private Regex BuildRegularExpression(string specification)
        {
            // FIXME implement some proper matching replacement here
            return new Regex(specification, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        private bool MatchesRelease(Regex matcher, Release release, Arguments filterArgs, HashSet<string> tags)
        {
            var hit = matcher.IsMatch(release.Name);
            if (filterArgs.InvertedMatching) hit = !hit;
            if (filterArgs.OrphanOnly) 
            {
                Debug.WriteLine($"Checking whether release {release.Name} is orphaned \r\n {release.TagName}, {release.Draft}\r\n");
                // Release is hit, draft AND associated tagname does not exist
                hit &= release.Draft && !tags.Contains(release.TagName);
            }
            // FIXME attempt to match tags as well?
            // FIXME add checks that use other parts in Arguments

            return filterArgs.InvertedMatching ? !hit : hit;
        }

        internal void DeleteTags(List<Release> matches)
        {
            // unimplemented as of yet
        }

        internal void DeleteReleases(List<Release> matches, Arguments metadata)
        {
            // FIXME check for interactive deletes somewhen
            foreach (var match in matches) {
                var task = client.Repository.Release.Delete(metadata.ProjectOwner, metadata.ProjectName, match.Id);
                // await task finishing, otherwise tasks overwrite one another
                task.Wait();
            }
        }
    }
}