﻿using System.ComponentModel.DataAnnotations;

// ReSharper disable ParameterTypeCanBeEnumerable.Local
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RangeIsPercentageWithPropertiesAttribute : ValidationAttribute
    {
        private readonly string[] _comparisonPropertyNames;

        public RangeIsPercentageWithPropertiesAttribute(string[] comparisonPropertyNames)
        {
            _comparisonPropertyNames = comparisonPropertyNames.Select(s => s.Trim()).ToArray();
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (!typeof(IRange).IsAssignableFrom(currentProperty.PropertyType))
            {
                throw new InvalidOperationException($"{nameof(RangeIsPercentageWithPropertiesAttribute)} can only be applied to IRange properties.");
            }

            var range = (IRange)value;
            var sumStart = Convert.ToDouble(range.Start);
            var sumEnd = Convert.ToDouble(range.End);

            foreach (var comparisonPropertyName in _comparisonPropertyNames)
            {
                var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(comparisonPropertyName);
                if (comparisonPropertyInfo == null)
                    return new ValidationResult($"Property {comparisonPropertyName} not found.");
                if (!typeof(IRange).IsAssignableFrom(comparisonPropertyInfo.PropertyType))
                    return new ValidationResult($"Property {comparisonPropertyName} must be of type IRange.");

                var comparisonValue = (IRange)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
                sumStart += Convert.ToDouble(comparisonValue.Start);
                sumEnd += Convert.ToDouble(comparisonValue.End);
            }

            return Math.Abs(sumStart - 100d) < double.Epsilon && Math.Abs(sumEnd - 100d) < double.Epsilon
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}