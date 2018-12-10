using System;
using NUnit.Framework;

namespace ReleaseCleaner.Invocation
{

    using CUT = ReleaseCleaner.Invocation.ArgumentsBuilder;

    [TestFixture]
    [Category("UnitTest")]
    [Category("Argument Parsing")]
    public class ArgumentsBuilderTests
    {
        [Test]
        public void ArgumentBuilder_WithOwner_IsInvalid()
        {
            var instance = new CUT();
            instance.SetOwner("");

            Assert.IsFalse(instance.IsValid());

            instance.Inverted();
            Assert.IsFalse(instance.IsValid());
            Assert.Throws<InvalidOperationException>(() => instance.Build());
        }

        [Test]
        public void ArgumentBuilder_WithProject_IsInvalid()
        {
            var instance = new CUT();
            instance.SetProject("");

            Assert.IsFalse(instance.IsValid());

            instance.Inverted();
            Assert.IsFalse(instance.IsValid());
            Assert.Throws<InvalidOperationException>(() => instance.Build());
        }

        [Test]
        public void ArgumentBuilder_WithMatcher_IsInvalid()
        {
            var instance = new CUT();
            instance.AddMatcher("");

            Assert.IsFalse(instance.IsValid());

            instance.Inverted();
            Assert.IsFalse(instance.IsValid());
            Assert.Throws<InvalidOperationException>(() => instance.Build());
        }


        [Test]
        public void ArgumentBuilder_WithOwnerAndProject_IsInvalid()
        {
            var instance = new CUT();
            instance.SetOwner("");
            instance.SetProject("");

            Assert.IsFalse(instance.IsValid());

            instance.Inverted();
            Assert.IsFalse(instance.IsValid());
            Assert.Throws<InvalidOperationException>(() => instance.Build());
        }

        [Test]
        public void ArgumentBuilder_WithOwnerAndMatcher_IsInvalid()
        {
            var instance = new CUT();
            instance.SetOwner("");
            instance.AddMatcher("");

            Assert.IsFalse(instance.IsValid());

            instance.Inverted();
            Assert.IsFalse(instance.IsValid());
            Assert.Throws<InvalidOperationException>(() => instance.Build());
        }

        [Test]
        public void ArgumentBuilder_WithMatcherAndProject_IsInvalid()
        {
            var instance = new CUT();
            instance.AddMatcher("");
            instance.SetProject("");

            Assert.IsFalse(instance.IsValid());

            instance.Inverted();
            Assert.IsFalse(instance.IsValid());
            Assert.Throws<InvalidOperationException>(() => instance.Build());
        }

        [Test]
        public void ArgumentBuilder_WithOwnerAndProjectAndMatcher_IsValid()
        {
            var instance = new CUT();
            instance.SetOwner("");
            instance.SetProject("");
            instance.AddMatcher("");

            Assert.IsTrue(instance.IsValid());

            instance.Inverted();
            Assert.IsTrue(instance.IsValid());
        }

        [Test]
        public void ArgumentBuilder_WithMultipleOwners_ThrowsException()
        {
            var instance = new CUT();
            instance.SetOwner("");
            Assert.Throws<InvalidOperationException>(() => instance.SetOwner(""));
        }


        [Test]
        public void ArgumentBuilder_WithMultipleProjects_ThrowsException()
        {
            var instance = new CUT();
            instance.SetProject("");
            Assert.Throws<InvalidOperationException>(() => instance.SetProject(""));
        }

        [Test]
        public void ArgumentBuilder_WithMultipleMatchers_Works()
        {
            var instance = new CUT();
            instance.AddMatcher("");
            instance.AddMatcher("");
            // assertion is not throwing an exception
        }

        [Test]
        public void ArgumentBuilder_WithMultipleInverts_StaysInverted()
        {
            int[] repetitions = { 2, 3, 12, 152, 61, 6, 125, 62 };
            foreach (var reps in repetitions)
            {
                var instance = new CUT();
                for (int i = 0; i < reps; i++)
                {
                    instance.Inverted();
                }
                Assert.IsTrue(instance.InvertedMatching);
            }
        }

    }
}