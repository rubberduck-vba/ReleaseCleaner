using System;
using Octokit;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var (arguments, authentication) = CommandLine.Parse(args, new ReleaseCleaner.Input.Console());
            var client = new GitHubClient(new ProductHeaderValue("rubberduck-vba_ReleaseCleaner"));
            client.Credentials = authentication.Credentials;

            var cleaner = new Cleaner(client);
            var matches = cleaner.FindReleases(arguments);

            // DEBUGGING OUTPUT
            Console.WriteLine("Matching releases are:");
            foreach (var hit in matches)
            {
                Console.WriteLine($"{hit.Name}, pre: {hit.Prerelease}, draft: {hit.Draft}");
            }

            // FIXME are we sure we want to delete the tags?
            // cleaner.DeleteTags(matches);
            cleaner.DeleteReleases(matches, arguments);
        }
    }
}
