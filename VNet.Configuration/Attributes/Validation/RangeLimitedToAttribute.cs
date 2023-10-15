using System.ComponentModel.DataAnnotations;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable EqualExpressionComparison

// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeLimitedToAttribute : ValidationAttribute
    {
        private readonly double _start;
        private readonly double _end;


        public RangeLimitedToAttribute(double start, double end)
        {
            _start = start;
            _end = end;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (!typeof(IRange).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeLimitedToAttribute)} can only be applied to IRange properties.");
            }

            var range = (IRange)value;
            var valStart = Convert.ToDouble(range.Start);
            var valEnd = Convert.ToDouble(range.End);

            return valStart < _start || valEnd > valEnd
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}