using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Octokit;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner.Filtering
{
    class ReleaseNameMatcher : IReleasePredicate
    {
        private List<Regex> matchers;
        
        public void Prepare(Arguments arguments)
        {
            matchers = arguments.CleanReleases.Select(specification => BuildRegularExpression(specification)).ToList();
        }

        public bool Matches(Release releaseInfo)
        {
            return matchers.Any(matcher => matcher.IsMatch(releaseInfo.Name));
        }

        private Regex BuildRegularExpression(string specification)
        {
            // FIXME implement some proper matching replacement here
            return new Regex(specification, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

    }
}