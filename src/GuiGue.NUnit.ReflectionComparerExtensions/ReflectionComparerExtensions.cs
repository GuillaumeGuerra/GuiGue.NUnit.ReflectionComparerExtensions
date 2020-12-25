using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace GuiGue.NUnit.ReflectionComparerExtensions
{
    public static class ReflectionComparerExtensions
    {
        public static IConstraint DeepEqualTo(this Is @is, object expected)
        {
            return new DeepEqualConstraint(expected);
        }

        public static EqualConstraint ByReflection(this EqualConstraint constraint)
        {
            return constraint.Using<object>(JsonSerializationComparison);
        }

        private static bool JsonSerializationComparison(object actual, object expected)
        {
            return JsonConvert.SerializeObject(actual).Equals(JsonConvert.SerializeObject(expected));
        }
    }

    public class DeepEqualConstraint : IConstraint
    {
        private readonly EqualConstraint _innerConstraint;

        public DeepEqualConstraint(object expected)
        {
            _innerConstraint = new EqualConstraint(JsonConvert.SerializeObject(expected));
        }

        public IConstraint Resolve()
        {
            return ((IResolveConstraint) _innerConstraint).Resolve();
        }

        public ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return _innerConstraint.ApplyTo(JsonConvert.SerializeObject(actual));
        }

        public ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            return _innerConstraint.ApplyTo(()=> JsonConvert.SerializeObject(del()));
        }

        public ConstraintResult ApplyTo<TActual>(ref TActual actual)
        {
            return ApplyTo(actual);
        }

        public string DisplayName { get; } = "MonculDisplayName";
        public string Description { get; } = "MonculDescription";
        public object[] Arguments { get; }

        public ConstraintBuilder Builder { get; set; }
    }
}
