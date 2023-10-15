using System.ComponentModel.DataAnnotations;
using System.Numerics;

#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Dimensions3D : ValidationAttribute
    {
        public Dimensions3D()
        {
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty.PropertyType != typeof(Vector3))
            {
                throw new InvalidOperationException($"{nameof(FalseIfFalseAttribute)} can only be applied to System.Numerics.Vector3 properties.");
            }

            var currentValue = (Vector3)value;

            if (currentValue.X <= 0 || currentValue.Y <= 0 || currentValue.Z <= 0)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}