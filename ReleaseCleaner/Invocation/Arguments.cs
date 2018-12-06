using System;
using System.Collections.Generic;
using System.Linq;

namespace ReleaseCleaner.Invocation 
{
    internal class Arguments 
    {
        // private ctor to disable instantiation except through the factory method
        private Arguments() {}

        public static Arguments Parse(string[] actualArgs)
        {
            var matchers = new List<string>();
            var result = new Arguments();
            for (int i = 0; i < actualArgs.Length; i++) {
                switch(actualArgs[i])
                {
                    case "-v": 
                    case "--invert":
                        result.InvertedMatching = true;
                        break;
                    case "-m":
                    case "--match":
                        if (i == actualArgs.Length) { throw new ArgumentException(nameof(CleanReleases)); }
                        matchers.Add(actualArgs[++i]);
                        break;
                    case "-o":
                    case "--owner":
                        if (i == actualArgs.Length) { throw new ArgumentException(nameof(ProjectOwner)); }
                        result.ProjectOwner = actualArgs[++i];
                        break;
                    case "-p":
                    case "--project":
                        if (i == actualArgs.Length) { throw new ArgumentException(nameof(ProjectName)); }
                        result.ProjectName = actualArgs[++i];
                        break;
                    default:
                        // attempt to parse given arg as owner/name
                        var spec = actualArgs[i].Split('/');
                        if (spec.Length != 2) { throw new ArgumentException("Owner Specification"); }
                        (result.ProjectOwner, result.ProjectName) = (spec[0], spec[1]);
                        break;
                }
            }
            result.CleanReleases = matchers.ToArray();
            // if any of the required properties is unset
            if (!result.IsValid())
            {
                throw new ArgumentException("Missing Required Argument(s)");
            }
            return result;
        }

        private bool IsValid()
        {
            return CleanReleases != default && CleanReleases.Any()
                && ProjectName != default
                && ProjectOwner != default;
        }

        public string[] CleanReleases { get; private set; }
        public bool InvertedMatching { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectOwner { get; private set; }
    }
}