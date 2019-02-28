using System;
using System.Collections.Generic;
using ReleaseCleaner.Input;

namespace ReleaseCleaner.Invocation
{
    internal static class CommandLine
    {
        public static (Arguments, Authentication) Parse(string[] actualArgs, IConsole console)
        {
            var matchingBehavior = new ArgumentsBuilder();
            var auth = new AuthenticationBuilder(console);
            for (int i = 0; i < actualArgs.Length; i++)
            {
                switch (actualArgs[i])
                {
                    case "-v":
                    case "--invert":
                        matchingBehavior.Inverted();
                        break;
                    case "-m":
                    case "--match":
                        if (i == actualArgs.Length) { throw new ArgumentException(); }
                        matchingBehavior.AddMatcher(actualArgs[++i]);
                        break;
                    case "-o":
                    case "--owner":
                        if (i == actualArgs.Length) { throw new ArgumentException(); }
                        matchingBehavior.SetOwner(actualArgs[++i]);
                        break;
                    case "-p":
                    case "--project":
                        if (i == actualArgs.Length) { throw new ArgumentException(); }
                        matchingBehavior.SetProject(actualArgs[++i]);
                        break;
                    case "-u":
                    case "--user":
                        if (i == actualArgs.Length) { throw new ArgumentException(); }
                        auth.SetUsername(actualArgs[++i]);
                        break;
                    // -p is already taken for project
                    // we also want to prompt in the first place, this is just for scripting
                    case "--pass":
                    case "--password":
                        if (i == actualArgs.Length) { throw new ArgumentException(); }
                        auth.SetPassword(actualArgs[++i]);
                        break;
                    case "-t":
                    case "--token":
                        if (i == actualArgs.Length) { throw new ArgumentException(); }
                        auth.SetToken(actualArgs[++i]);
                        break;
                    case "--orphans":
                        matchingBehavior.Orphans();
                        break;
                    default:
                        // attempt to parse given arg as owner/name
                        var spec = actualArgs[i].Split('/');
                        if (spec.Length == 2)
                        {
                            matchingBehavior.SetOwner(spec[0]);
                            matchingBehavior.SetProject(spec[1]);
                        }
                        break;
                }
            }
            // if any of the required properties is unset
            if (!matchingBehavior.IsValid())
            {
                throw new ArgumentException("Missing Required Argument(s)");
            }
            return (matchingBehavior.Build(), auth.Build());
        }
    }
}