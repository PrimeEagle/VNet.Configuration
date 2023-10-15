using System.ComponentModel.DataAnnotations;
using System.Numerics;

#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Dimensions2D : ValidationAttribute
    {
        public Dimensions2D()
        {
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty.PropertyType != typeof(Vector2))
            {
                throw new InvalidOperationException($"{nameof(FalseIfFalseAttribute)} can only be applied to System.Numerics.Vector2 properties.");
            }

            var currentValue = (Vector2)value;

            if (currentValue.X <= 0 || currentValue.Y <= 0)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}