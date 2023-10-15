using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#pragma warning disable CS8603 // Possible null reference return.

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfFalseAttribute : ValidationAttribute
    {
        private readonly string _booleanPropertyName;

        public RequiredIfFalseAttribute(string booleanPropertyName)
        {
            _booleanPropertyName = booleanPropertyName;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var booleanProperty = validationContext.ObjectType.GetProperty(_booleanPropertyName);
            if (booleanProperty == null)
            {
                return new ValidationResult($"Unknown property: {_booleanPropertyName}");
            }

            var booleanValue = booleanProperty.GetValue(validationContext.ObjectInstance) as bool?;

            if ((booleanValue ?? true) && string.IsNullOrEmpty(Convert.ToString(value)))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}