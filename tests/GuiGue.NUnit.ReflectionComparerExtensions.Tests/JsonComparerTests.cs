using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GuiGue.NUnit.ReflectionComparerExtensions.Tests
{
    public class JsonComparerTests
    {
        [Test]
        public void Null_objects_should_be_equal()
        {
            Assert.That(null, Is.EqualTo(null).ByReflection());

            var ex = CheclAssertionFailure(() => Assert.That(null, Is.Not.EqualTo(null).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: not equal to null\r\n  But was:  null\r\n"));

            Assert.That(1, Has.One.EqualTo(1));


            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void Null_actual_should_not_be_equal_to_a_non_null_expected()
        {
            var ex = CheclAssertionFailure(() => Assert.That(null, Is.EqualTo(42).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: 42\r\n  But was:  null\r\n"));

            ex = CheclAssertionFailure(() => Assert.That(null, Is.EqualTo("FortyTwo").ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: \"FortyTwo\"\r\n  But was:  null\r\n"));

            Assert.That(null, Is.Not.EqualTo(42).ByReflection());
            Assert.That(null, Is.Not.EqualTo("FortyTwo").ByReflection());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void Non_null_expected_should_not_be_equal_to_a_null_expected()
        {
            var ex = CheclAssertionFailure(() => Assert.That(42, Is.EqualTo(null).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: null\r\n  But was:  42\r\n"));

            ex = CheclAssertionFailure(() => Assert.That("FortyTwo", Is.EqualTo(null).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: null\r\n  But was:  \"FortyTwo\"\r\n"));

            Assert.That(42, Is.Not.EqualTo(null).ByReflection());
            Assert.That("FortyTwo", Is.Not.EqualTo(null).ByReflection());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void Identical_struct_types_should_be_equal()
        {
            Assert.That(42, Is.EqualTo(42).ByReflection());
            Assert.That(42.42D, Is.EqualTo(42.42D).ByReflection());
            Assert.That(42.42M, Is.EqualTo(42.42M).ByReflection());
            Assert.That(42L, Is.EqualTo(42L).ByReflection());

            var ex = CheclAssertionFailure(() => Assert.That(42, Is.Not.EqualTo(42).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: not equal to 42\r\n  But was:  42\r\n"));

            ex = CheclAssertionFailure(() => Assert.That(42.42D, Is.Not.EqualTo(42.42D).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: not equal to 42.420000000000002d\r\n  But was:  42.420000000000002d\r\n"));

            ex = CheclAssertionFailure(() => Assert.That(42.42M, Is.Not.EqualTo(42.42M).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: not equal to 42.42m\r\n  But was:  42.42m\r\n"));

            ex = CheclAssertionFailure(() => Assert.That(42L, Is.Not.EqualTo(42L).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: not equal to 42\r\n  But was:  42\r\n"));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void Different_struct_types_should_not_be_equal()
        {
            var ex = CheclAssertionFailure(() => Assert.That(42, Is.EqualTo(1).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: 1\r\n  But was:  42\r\n"));

            ex = CheclAssertionFailure(() => Assert.That(42.42D, Is.EqualTo(-42.42D).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: -42.420000000000002d\r\n  But was:  42.420000000000002d\r\n"));

            ex = CheclAssertionFailure(() => Assert.That(42.42M, Is.EqualTo(-42.42M).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: -42.42m\r\n  But was:  42.42m\r\n"));

            ex = CheclAssertionFailure(() => Assert.That(42L, Is.EqualTo(-42L).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: -42\r\n  But was:  42\r\n"));
            
            Assert.That(42, Is.Not.EqualTo(-42).ByReflection());
            Assert.That(42.42D, Is.Not.EqualTo(-42.42D).ByReflection());
            Assert.That(42.42M, Is.Not.EqualTo(-42.42M).ByReflection());
            Assert.That(42L, Is.Not.EqualTo(-42L).ByReflection());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void Objects_with_same_members_should_be_equal()
        {
            var actual = new Level0Class()
            {
                _intField = 42,
                IntProperty = 42,
                StringProperty = "FortyTwo"
            };

            var expected = new Level0Class()
            {
                _intField = 42,
                IntProperty = 42,
                StringProperty = "FortyTwo"
            };

            Assert.That(actual, Is.EqualTo(expected).ByReflection());

            var ex = CheclAssertionFailure(() => Assert.That(actual, Is.Not.EqualTo(expected).ByReflection()));
            Assert.That(ex.Message, Is.EqualTo("  Expected: not equal to 42\r\n  But was:  42\r\n"));

            CheckForSpuriousAssertionResults();
        }

        private Exception CheclAssertionFailure(TestDelegate del)
        {
            Exception assertionException = null;
            using (new TestExecutionContext.IsolatedContext())
            {
                try
                {
                    del();
                }
                catch (Exception ex)
                {
                    assertionException = ex;
                }
            }

            Assert.That(assertionException, Is.Not.Null);

            return assertionException;
        }

        private static void CheckForSpuriousAssertionResults()
        {
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            Assert.That(result.AssertionResults.Count, Is.EqualTo(0),
                "Spurious result left by Assert.Fail()");
        }

        private class Level0Class
        {
            public int _intField;
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }

        private class Level1Class
        {
            public int _intField;
            public double DoubleProperty { get; set; }
            public long LongProperty { get; set; }
        }
    }
}