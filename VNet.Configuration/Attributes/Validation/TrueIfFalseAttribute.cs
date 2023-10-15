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
                throw new InvalidOperationException($"{nameof(FalseIfFalseAttribute)} can only be applied to boolean properties.");
            }

            // Get the object and property value
            var containerType = validationContext.ObjectType;
            var containerInstance = validationContext.ObjectInstance;
            var pathParts = _dependentPropertyName.Split('.');

            // Navigate the path to get to the dependent property
            foreach (var part in pathParts)
            {
                var property = containerType.GetProperty(part);
                if (property == null)
                {
                    return new ValidationResult($"Unknown property: {part}");
                }

                if (property.PropertyType != typeof(bool) && pathParts.Last() != part)
                {
                    containerType = property.PropertyType;
                    containerInstance = property.GetValue(containerInstance);
                    if (containerInstance == null)
                    {
                        return new ValidationResult($"Property path {part} returned null.");
                    }
                }
                else if (property.PropertyType != typeof(bool))
                {
                    return new ValidationResult($"{nameof(FalseIfFalseAttribute)} can only be used with a boolean dependent property.");
                }
                else
                {
                    var dependentValue = (bool)property.GetValue(containerInstance);

                    var currentValue = (bool)value;
                    if (!dependentValue && !currentValue)
                    {
                        return new ValidationResult(ErrorMessage);
                    }

                    return ValidationResult.Success;
                }
            }

            // If code execution reaches here, something has gone wrong with our path navigation
            return new ValidationResult($"Unknown error in {nameof(FalseIfFalseAttribute)}.");
        }
    }
}