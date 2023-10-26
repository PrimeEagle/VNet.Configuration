using System.ComponentModel.DataAnnotations;
using System.Numerics;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RangeLimitedToPercentAttribute<T> : ValidationAttribute
        where T : struct, INumber<T>
    {
        public RangeLimitedToPercentAttribute()
        {
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            if (currentProperty == null || !typeof(IRange<T>).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeLimitedToPercentAttribute<T>)} can only be applied to IRange properties.");
            }

            if (value is not IRange<T> range)
            {
                return new ValidationResult($"Invalid type. Expected an IRange but got {value?.GetType()}.");
            }

            var zero = T.Zero;
            var oneHundred = T.One * (T)Convert.ChangeType(100, typeof(T));
            var isStartOutOfRange = range.Start.CompareTo(zero) != 0;
            var isEndOutOfRange = range.End.CompareTo(oneHundred) != 0;

            return isStartOutOfRange || isEndOutOfRange
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}