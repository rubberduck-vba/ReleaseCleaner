using Octokit;
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

            var cleaner = new Cleaner(client);
            var matches = cleaner.FindReleases(arguments);

            if (arguments.DryRun)
            {
                // DEBUGGING OUTPUT
                console.Write("Matching releases are:");
                foreach (var hit in matches)
                {
                    console.Write($"{hit.Name}, pre: {hit.Prerelease}, draft: {hit.Draft}");
                }
            }
            else
            {
                // FIXME are we sure we want to delete the tags?
                // cleaner.DeleteTags(matches);
                cleaner.DeleteReleases(matches, arguments);
            }
        }
    }
}
