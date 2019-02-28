using System;
using System.Collections.Generic;
using System.Linq;

namespace ReleaseCleaner.Invocation
{
    internal class Arguments
    {
        public Arguments(ArgumentsBuilder builder)
        {
            if (!builder.IsValid())
            {
                // BAD PROGRAMMER
                throw new InvalidOperationException();
            }
            CleanReleases = builder.CleanReleases.ToArray();
            InvertedMatching = builder.InvertedMatching;
            OrphanOnly = builder.OrphanOnly;
            ProjectName = builder.Project;
            ProjectOwner = builder.Owner;
        }

        public string[] CleanReleases { get; private set; }
        public bool InvertedMatching { get; private set; }
        public bool OrphanOnly { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectOwner { get; private set; }

    }
    public class ArgumentsBuilder
    {

        private List<string> cleanReleases = new List<string>();
        public IReadOnlyList<string> CleanReleases
        {
            get
            {
                return cleanReleases.AsReadOnly();
            }
        }
        public bool InvertedMatching { get; private set; }
        public bool OrphanOnly { get; private set; }
        public string Project { get; set; }
        public string Owner { get; set; }
        public ArgumentsBuilder() { }

        internal bool IsValid()
        {
            return CleanReleases.Any()
                && Project != default
                && Owner != default;
        }

        internal void SetOwner(string owner)
        {
            if (Owner != default)
            {
                // disallow setting that multiple times
                throw new InvalidOperationException();
            }
            Owner = owner;
        }

        internal void AddMatcher(string releaseMatcher)
        {
            cleanReleases.Add(releaseMatcher);
        }

        internal void SetProject(string projectName)
        {
            if (Project != default)
            {
                // disallow setting that multiple times
                throw new InvalidOperationException();
            }
            Project = projectName;
        }

        internal void Inverted()
        {
            // ignore multiple sets
            InvertedMatching = true;
        }

        internal void Orphans() 
        {
            OrphanOnly = true;
        }

        internal Arguments Build()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException();
            }
            return new Arguments(this);
        }
    }
}