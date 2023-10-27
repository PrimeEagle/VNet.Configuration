using System.ComponentModel.DataAnnotations;

namespace VNet.Configuration.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FileExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not string filePath || string.IsNullOrWhiteSpace(filePath)) return ValidationResult.Success;
            return (!File.Exists(filePath) ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName)) : ValidationResult.Success) ?? throw new InvalidOperationException();
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} does not exist as a file on the system.";
        }
    }
}