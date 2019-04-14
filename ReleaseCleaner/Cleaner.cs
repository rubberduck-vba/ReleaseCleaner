using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Octokit;
using ReleaseCleaner.Filtering;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner
{
    internal class Cleaner
    {
        private readonly IGitHubClient client;

        public Cleaner(IGitHubClient client) 
        {
            this.client = client;
        }

        public List<Release> FindReleases(Arguments arguments, IReleasePredicate filter)
        {
            // query must not be started explicitly, execution is already performed by the library
            var query = client.Repository.Release.GetAll(arguments.ProjectOwner, arguments.ProjectName);
        
            filter.Prepare(arguments);
            // await query results before filtering
            query.Wait();
            return query.Result
                .Where(release => filter.Matches(release))
                .ToList();
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