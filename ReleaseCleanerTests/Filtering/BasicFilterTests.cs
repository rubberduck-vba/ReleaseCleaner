using NUnit.Framework;
using Octokit;
using ReleaseCleaner.Invocation;
using Moq;

namespace ReleaseCleaner.Filtering 
{
    [TestFixture]
    [Category("Core Logic")]
    [Category("Filtering")]
    public class BasicFilterTests 
    {
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CheckAndFilter(bool a, bool b)
        {
            var filter = new AndFilter(new []{new ConstantValueFilter(a), new ConstantValueFilter(b)});
            Assert.AreEqual(a && b, filter.Matches(null));
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CheckOrFilter(bool a, bool b)
        {
            var filter = new OrFilter(new []{new ConstantValueFilter(a), new ConstantValueFilter(b)});
            Assert.AreEqual(a || b, filter.Matches(null));
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void ConditionalInversionFilter(bool invert, bool wrappedResult, bool expected) 
        {
            var builder = ArgumentsMockData.BaseBuilder();
            if (invert)
                builder.Inverted();
            
            var filter = new ConditionalInversion(new ConstantValueFilter(wrappedResult));
            filter.Prepare(builder.Build());
            Assert.AreEqual(expected, filter.Matches(null));
        }
    }


    class ConstantValueFilter : IReleasePredicate
    {
        private readonly bool value;
        public ConstantValueFilter(bool value) {
            this.value = value;
        }
        public bool Matches(Release releaseInfo)
        {
            return value;
        }

        public void Prepare(Arguments arguments)
        {
            // NO-OP
        }
    }
}