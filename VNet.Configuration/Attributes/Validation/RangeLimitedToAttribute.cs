using System.ComponentModel.DataAnnotations;

// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8605 // Unboxing a possibly null value.

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeLimitedToAttribute : ValidationAttribute
    {
        private readonly double _startLimit;
        private readonly double _endLimit;

        public RangeLimitedToAttribute(double startLimit, double endLimit)
        {
            _startLimit = startLimit;
            _endLimit = endLimit;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (currentProperty.PropertyType != typeof(Range))
            {
                throw new InvalidOperationException($"{nameof(FalseIfFalseAttribute)} can only be applied to System.Numerics.Vector2 properties.");
            }

            var range = (Range)value;
            var startVal = Convert.ToDouble(range.Start);
            var endVal = Convert.ToDouble(range.End);
            
            return startVal < _startLimit || endVal > _endLimit || _startLimit > _endLimit ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}