using System.ComponentModel.DataAnnotations;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable ParameterTypeCanBeEnumerable.Local


#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeIfFalseAttribute : ValidationAttribute
    {
        private readonly string _comparisonPropertyName;
        private readonly double _start;
        private readonly double _end;


        public RangeIfFalseAttribute(double start, double end, string comparisonPropertyName)
        {
            _comparisonPropertyName = comparisonPropertyName.Trim();
            _start = start;
            _end = end;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (!typeof(IRange).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeIfFalseAttribute)} can only be applied to IRange properties.");
            }

            var valRange = (IRange)value;
            var valStart = Convert.ToDouble(valRange.Start);
            var valEnd = Convert.ToDouble(valRange.End);
            var comparisonValue = false;


            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparisonPropertyName);
            if (comparisonPropertyInfo == null)
                return new ValidationResult($"Property {_comparisonPropertyName} not found.");
            if (!typeof(IRange).IsAssignableFrom(comparisonPropertyInfo.PropertyType))
                return new ValidationResult($"Property {_comparisonPropertyName} must be of type IRange.");

            comparisonValue = (bool)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);


            return !comparisonValue && (valStart != _start || valEnd != _end)
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}