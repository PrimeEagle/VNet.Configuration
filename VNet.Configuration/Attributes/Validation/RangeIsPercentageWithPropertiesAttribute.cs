using System.ComponentModel.DataAnnotations;
using System.Numerics;

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CA1822


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RangeIsPercentageWithPropertiesAttribute<T> : ValidationAttribute
    where T : struct, IFloatingPoint<T>
    {
        private readonly string[] _comparisonPropertyNames;

        public RangeIsPercentageWithPropertiesAttribute(string[] comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Select(s => s.Trim()).ToArray();
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty == null || !typeof(IRange<T>).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeIsPercentageWithPropertiesAttribute<T>)} can only be applied to IRange properties.");
            }

            var range = (IRange<T>)value;
            var sumStart = range.Start;
            var sumEnd = range.End;

            foreach (var comparisonPropertyName in _comparisonPropertyNames)
            {
                var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonPropertyName);
                if (comparisonPropertyInfo == null)
                    return new ValidationResult($"Property {comparisonPropertyName} not found.");
                if (!typeof(IRange<T>).IsAssignableFrom(comparisonPropertyInfo.PropertyType))
                    return new ValidationResult($"Property {comparisonPropertyName} must be of type IRange.");

                var comparisonRange = (IRange<T>)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
                sumStart += comparisonRange.Start;
                sumEnd += comparisonRange.End;
            }

            var oneHundred = MultiplyByInteger(T.One, 100);
            var tolerance = MultiplyByInteger(T.One, 1) / (T)Convert.ChangeType(1000000, typeof(T));
            var isValidStart = T.Abs(sumStart - oneHundred) < tolerance;
            var isValidEnd = T.Abs(sumEnd - oneHundred) < tolerance;

            return isValidStart && isValidEnd
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }

        private T MultiplyByInteger(T value, int multiplier)
        {
            var method = typeof(T).GetMethod("op_Multiply", new Type[] { typeof(T), typeof(T) });
            var result = value;
            for (var i = 0; i < multiplier; i++)
            {
                result = (T)(method.Invoke(null, new object[] { result, T.One }) ?? throw new InvalidOperationException());
            }
            return result;
        }
    }
}