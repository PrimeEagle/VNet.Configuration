using System.ComponentModel.DataAnnotations;
using System.Numerics;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable ParameterTypeCanBeEnumerable.Local


#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeIfFalseAttribute<T> : ValidationAttribute where T : INumber<T>
    {
        private readonly string _comparisonPropertyName;
        private readonly T _start;
        private readonly T _end;

        public RangeIfFalseAttribute(T start, T end, string comparisonPropertyName)
        {
            _comparisonPropertyName = comparisonPropertyName.Trim();
            _start = start;
            _end = end;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty == null || !typeof(IRange<T>).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeIfFalseAttribute<T>)} can only be applied to IRange properties.");
            }

            // Null-checking might still be relevant depending on your context
            var valRange = (IRange<T>)value;
            var valStart = valRange.Start;
            var valEnd = valRange.End;
            var comparisonValue = false;

            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparisonPropertyName);
            if (comparisonPropertyInfo == null)
            {
                return new ValidationResult($"Property {_comparisonPropertyName} not found.");
            }

            if (!typeof(bool).IsAssignableFrom(comparisonPropertyInfo.PropertyType))
            {
                return new ValidationResult($"Property {_comparisonPropertyName} must be of type bool.");
            }

            comparisonValue = (bool)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);

            // Now, we can use T's comparison thanks to INumber's capabilities.
            var isOutOfRange = !valStart.Equals(_start) || !valEnd.Equals(_end);

            return !comparisonValue && isOutOfRange
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}