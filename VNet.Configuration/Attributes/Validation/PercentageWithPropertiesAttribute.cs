using System.ComponentModel.DataAnnotations;
// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8603 // Possible null reference return.

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PercentageWithPropertiesAttribute : ValidationAttribute
    {
        private readonly string[] _comparisonPropertyNames;

        public PercentageWithPropertiesAttribute(string[] comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Select(s => s.Trim()).ToArray();
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (!double.TryParse(value?.ToString(), out _))
            {
                return new ValidationResult($"Property value {value} could not be converted to a number.");
            }

            var sum = Convert.ToDouble(value);
            foreach (var comparisonPropertyName in _comparisonPropertyNames)
            {
                var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonPropertyName);
                if (comparisonPropertyInfo == null)
                    return new ValidationResult($"Property {comparisonPropertyName} not found.");


                var comparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
                if (double.TryParse(comparisonValue?.ToString(), out var dCompVal))
                {
                    sum += dCompVal;
                }
            }

            return Math.Abs(sum - 100d) < double.Epsilon ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}