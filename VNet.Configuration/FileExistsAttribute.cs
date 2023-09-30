using System.ComponentModel.DataAnnotations;


namespace VNet.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FileExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string filePath && !string.IsNullOrWhiteSpace(filePath))
            {
                if (!File.Exists(filePath))
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} does not exist as a file on the system.";
        }
    }

}