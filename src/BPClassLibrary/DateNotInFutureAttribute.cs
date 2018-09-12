using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BPClassLibrary
{
    /// <summary>
    /// DateNotInTheFuture Attribute validates the Date to make sure it is not in the future
    /// </summary>
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        public  DateNotInFutureAttribute()
        {
            ErrorMessage = "Date must be greater then current Date/Time, {0}";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            if (value == null || ((DateTime)value < DateTime.Today))
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
