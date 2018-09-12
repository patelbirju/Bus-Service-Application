using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BPClassLibrary
{
    /// <summary>
    /// PostalCodeValidationAttribute validates the postal code to make sure it is a valid Canadian postal code and 
    /// returns an error if not
    /// </summary>
    public class PostalCodeValidationAttribute : ValidationAttribute
    {
        public PostalCodeValidationAttribute()
        {
            ErrorMessage = "{0} is not a correct Canadian postal pattern";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Regex postalCodePattern = new Regex(@"^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}$", RegexOptions.IgnoreCase);
            if (value == null || postalCodePattern.IsMatch(value.ToString()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(String.Format(ErrorMessage, validationContext.DisplayName));
            }
            
        }


    }
}
