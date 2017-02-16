using System.ComponentModel.DataAnnotations;

namespace Movies.Model.ModelValidation.Account
{
    public class GenderValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string gender = (string)value;
                var result = gender.ToUpper().Equals("FEMALE") || gender.ToUpper().Equals("MALE");
                return result ? ValidationResult.Success : new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}