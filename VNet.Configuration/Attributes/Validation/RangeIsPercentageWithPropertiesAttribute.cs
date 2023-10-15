using System.ComponentModel.DataAnnotations;

// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeIsPercentageWithPropertiesAttribute : ValidationAttribute
    {
        private readonly string[] _comparisonPropertyNames;

        public RangeIsPercentageWithPropertiesAttribute(string[] comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Select(s => s.Trim()).ToArray();
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty.PropertyType != typeof(Range))
            {
                throw new InvalidOperationException($"{nameof(FalseIfFalseAttribute)} can only be applied to System.Numerics.Vector2 properties.");
            }

            var range = (Range)value;
            var sumStart = Convert.ToDouble(range.Start);
            var sumEnd = Convert.ToDouble(range.End);
            foreach (var comparisonPropertyName in _comparisonPropertyNames)
            {
                var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonPropertyName);
                if (comparisonPropertyInfo == null)
                    return new ValidationResult($"Property {comparisonPropertyName} not found.");
                if(comparisonPropertyInfo.PropertyType != typeof(Range))
                    return new ValidationResult($"Property {comparisonPropertyName} must be of type Range.");

                var comparisonValue = (Range)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
                sumStart += Convert.ToDouble(comparisonValue.Start);
                sumEnd += Convert.ToDouble(comparisonValue.End);
            }

            return Math.Abs(sumStart - 100d) < double.Epsilon && Math.Abs(sumEnd - 100d) < double.Epsilon ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}