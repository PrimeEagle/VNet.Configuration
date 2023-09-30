using System.ComponentModel.DataAnnotations;

namespace VNet.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PercentageWithPropertiesAttribute : ValidationAttribute
    {
        private readonly string[] _comparisonPropertyNames;

        public PercentageWithPropertiesAttribute(string comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Split(',').Select(s => s.Trim()).ToArray();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            double dVal;
            if (!double.TryParse(value.ToString(), out dVal))
            {
                return new ValidationResult($"Property value {value.ToString()} could not be converted to a number.");
            }

            double sum = 0;
            for (var i = 0; i < _comparisonPropertyNames.Length; i++)
            {
                var comparisonPropertyName = _comparisonPropertyNames[i];
                var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonPropertyName);
                if (comparisonPropertyInfo == null)
                    return new ValidationResult($"Property {comparisonPropertyName} not found.");


                var comparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
                double dCompVal;
                if (double.TryParse(comparisonValue.ToString(), out dCompVal))
                {
                    sum += dCompVal;
                }
            }

            if (Math.Abs(sum - 100d) < double.Epsilon)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Properties {string.Join(',', _comparisonPropertyNames)} should add up to a total of 100.";
        }
    }
}
