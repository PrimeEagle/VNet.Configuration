using System.ComponentModel.DataAnnotations;

namespace VNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PercentageWithPropertiesAttribute : ValidationAttribute
    {
        private readonly string[] _comparisonPropertyNames;

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        public PercentageWithPropertiesAttribute(string[] comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Select(s => s.Trim()).ToArray();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!double.TryParse(value.ToString(), out _))
            {
                return new ValidationResult($"Property value {value} could not be converted to a number.");
            }

            double sum = 0;
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

            return Math.Abs(sum - 100d) < double.Epsilon ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName)) : ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Properties {string.Join(',', _comparisonPropertyNames)} should add up to a total of 100.";
        }
    }
}
