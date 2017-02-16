using System.ComponentModel.DataAnnotations;

namespace Movies.Model.ModelValidation.Client
{
    public class ApplicationTypeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string appType = (string)value;
                var result = appType.ToUpper().Equals("NATIVE CONFIDENTIAL") || appType.ToUpper().Equals("JAVA SCRIPT");
                return result ? ValidationResult.Success : new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}