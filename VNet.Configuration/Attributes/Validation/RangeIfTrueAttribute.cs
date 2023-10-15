using System.ComponentModel.DataAnnotations;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
#pragma warning disable CS8605 // Unboxing a possibly null value.

// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeIfTrueAttribute : ValidationAttribute
    {
        private readonly string[] _comparisonPropertyNames;
        private readonly double[] _values;

        public RangeIfTrueAttribute(object[] values, string[] comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Select(s => s.Trim()).ToArray();
            _values = values.Select(Convert.ToDouble).ToArray();
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (!typeof(IRange).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeIfTrueAttribute)} can only be applied to IRange properties.");
            }

            var valRange = (IRange)value;
            var valStart = Convert.ToDouble(valRange.Start);
            var valEnd = Convert.ToDouble(valRange.End);
            bool comparisonValue = false;

            foreach (var comparisonPropertyName in _comparisonPropertyNames)
            {
                var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonPropertyName);
                if (comparisonPropertyInfo == null)
                    return new ValidationResult($"Property {comparisonPropertyName} not found.");
                if (!typeof(IRange).IsAssignableFrom(comparisonPropertyInfo.PropertyType))
                    return new ValidationResult($"Property {comparisonPropertyName} must be of type IRange.");

                comparisonValue = (bool)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
            }

            return comparisonValue && (valStart != _values[0] || valEnd != _values[1])
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}