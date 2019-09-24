using Octokit;
using ReleaseCleaner.Filtering;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new ReleaseCleaner.Input.Console();
            var (arguments, authentication) = CommandLine.Parse(args, console);
            var client = new GitHubClient(new ProductHeaderValue("rubberduck-vba_ReleaseCleaner"));
            client.Credentials = authentication.Credentials;

            var filter = new ConditionalInversion(
                new AndFilter(new IReleasePredicate[]{
                    new ReleaseNameMatcher(),
                    new OrphanMatcher(client)
                })
            );

            var cleaner = new Cleaner(client);
            var matches = cleaner.FindReleases(arguments, filter);

            if (arguments.DryRun)
            {
                console.Write("Matching releases are:");
                foreach (var hit in matches)
                {
                    console.Write($"{hit.Name}, pre: {hit.Prerelease}, draft: {hit.Draft}");
                }
            }
            else
            {
                cleaner.DeleteReleases(matches, arguments);
            }
        }
    }
}
