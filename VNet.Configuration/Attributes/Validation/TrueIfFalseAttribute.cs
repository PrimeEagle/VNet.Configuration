using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TrueIfFalseAttribute : ValidationAttribute
    {
        private readonly string _dependentPropertyName;

        public TrueIfFalseAttribute(string dependentPropertyName)
        {
            _dependentPropertyName = dependentPropertyName;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty.PropertyType != typeof(bool))
            {
                throw new InvalidOperationException($"{nameof(TrueIfFalseAttribute)} can only be applied to boolean properties.");
            }

            var dependentProperty = validationContext.ObjectType.GetProperty(_dependentPropertyName);
            if (dependentProperty == null)
            {
                return new ValidationResult($"Unknown property: {_dependentPropertyName}");
            }
            if (dependentProperty.PropertyType != typeof(bool))
            {
                return new ValidationResult($"{nameof(TrueIfFalseAttribute)} can only be used with a boolean dependent property.");
            }

            var currentValue = (bool)value;
            var dependentValue = (bool)dependentProperty.GetValue(validationContext.ObjectInstance);

            if (!dependentValue && !currentValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}