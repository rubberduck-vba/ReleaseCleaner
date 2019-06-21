using System.Collections.Generic;
using System.Linq;
using Octokit;
using ReleaseCleaner.Invocation;

namespace ReleaseCleaner.Filtering
{
    abstract class BaseNaryFilter : IReleasePredicate
    {
        protected readonly IEnumerable<IReleasePredicate> PredicateChain;
        
        public BaseNaryFilter(IEnumerable<IReleasePredicate> predicateChain) 
        {
            PredicateChain = predicateChain;
        }

        public void Prepare(Arguments arguments)
        {
            foreach (var predicate in PredicateChain)
            {
                predicate.Prepare(arguments);
            }
        }

        public abstract bool Matches(Release releaseInfo);
    }

    class AndFilter : BaseNaryFilter
    {
        public AndFilter(IEnumerable<IReleasePredicate> predicateChain) : base(predicateChain) { }

        public override bool Matches(Release releaseInfo)
        {
            return PredicateChain.All(matcher => matcher.Matches(releaseInfo));
        }
    }

    class OrFilter : BaseNaryFilter
    {
        public OrFilter(IEnumerable<IReleasePredicate> predicateChain) : base(predicateChain) { }

        public override bool Matches(Release releaseInfo)
        {
            return PredicateChain.Any(matcher => matcher.Matches(releaseInfo));
        }
    }

    class ConditionalInversion : IReleasePredicate
    {
        private readonly IReleasePredicate wrappedPredicate;
        private bool performInversion = false;

        public ConditionalInversion(IReleasePredicate predicate) 
        {
            wrappedPredicate = predicate;
        }

        public void Prepare(Arguments args) 
        {
            performInversion = args.InvertedMatching;
        }

        public bool Matches(Release releaseInfo) 
        {
            var wrappedResult = wrappedPredicate.Matches(releaseInfo);
            return performInversion ? !wrappedResult : wrappedResult;
        }

    }
}