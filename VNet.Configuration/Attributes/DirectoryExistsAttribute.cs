using System.ComponentModel.DataAnnotations;


namespace VNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DirectoryExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not string filePath || string.IsNullOrWhiteSpace(filePath)) return ValidationResult.Success;
            return !Directory.Exists(filePath) ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName)) : ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} does not exist as a directory on the system.";
        }
    }
}