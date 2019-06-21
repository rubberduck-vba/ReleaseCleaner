namespace ReleaseCleaner.Invocation
{
    static class ArgumentsMockData
    {
        public const string Owner = "foo";
        public const string Project = "bar";

        public static ArgumentsBuilder BaseBuilder() 
        {
            var builder = new ArgumentsBuilder();
            builder.AddMatcher(".*");
            builder.SetOwner(Owner);
            builder.SetProject(Project);
            return builder;
        }
    }
}