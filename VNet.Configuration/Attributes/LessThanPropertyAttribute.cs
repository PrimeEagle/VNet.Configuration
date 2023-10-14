using System.ComponentModel.DataAnnotations;

namespace VNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class LessThanPropertyAttribute : ValidationAttribute
    {
        private readonly string _comparisonPropertyName;

        public LessThanPropertyAttribute(string comparisonPropertyName)
        {
            _comparisonPropertyName = comparisonPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparisonPropertyName);
            if (comparisonPropertyInfo == null)
                return new ValidationResult($"Property {_comparisonPropertyName} not found.");

            var comparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (value is IComparable baseComparable && comparisonValue is IComparable comparisonComparable)
            {
                if (baseComparable.CompareTo(comparisonComparable) >= 0)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} should be less than {_comparisonPropertyName}.";
        }
    }
}
