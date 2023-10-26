using System.ComponentModel.DataAnnotations;
using System.Numerics;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8605 // Unboxing a possibly null value.


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeIfTrueAttribute<T> : ValidationAttribute where T : INumber<T>
    {
        private readonly string[] _comparisonPropertyNames;
        private readonly T[] _values;

        public RangeIfTrueAttribute(T[] values, string[] comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Select(s => s.Trim()).ToArray();
            _values = values;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty == null || !typeof(IRange<T>).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeIfTrueAttribute<T>)} can only be applied to IRange properties.");
            }

            if (value is IRange<T> valRange)
            {
                var valStart = valRange.Start;
                var valEnd = valRange.End;

                var anyComparisonTrue = false;

                foreach (var comparisonPropertyName in _comparisonPropertyNames)
                {
                    var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonPropertyName);
                    if (comparisonPropertyInfo == null)
                    {
                        return new ValidationResult($"Property {comparisonPropertyName} not found.");
                    }

                    if (!typeof(bool).IsAssignableFrom(comparisonPropertyInfo.PropertyType))
                    {
                        return new ValidationResult($"Property {comparisonPropertyName} must be of type bool.");
                    }

                    var comparisonValue = (bool)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
                    anyComparisonTrue |= comparisonValue;
                }

                // Assuming you want to validate against specific start and end values, and there's at least one comparison that is true.
                var isOutOfRange = !valStart.Equals(_values[0]) || !valEnd.Equals(_values[1]);

                return anyComparisonTrue && isOutOfRange
                    ? ValidationResult.Success
                    : new ValidationResult(ErrorMessage);
            }
            else
            {
                return new ValidationResult(ErrorMessage);
            }
        }
    }
}