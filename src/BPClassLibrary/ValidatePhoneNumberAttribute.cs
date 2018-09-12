using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Resources;

namespace BPClassLibrary
{
    public class ValidatePhoneNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// ValidatePhoneNumber class validates the phone number by extracting the digits from the phone number and checks if the length is 10
        /// </summary>
        public ValidatePhoneNumberAttribute()
        {
            ErrorMessage = "Phone number must be numerical and 10 Digits";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Regex phoneValid = new Regex(@"^([^0-9]*\d){10}[^0-9]*$");
            string phone = "";

            for (int i = 0; i < value.ToString().Length; i++)
            {
                if (Char.IsDigit(value.ToString()[i]))
                {
                    phone += value.ToString()[i];
                }

            }
            if (phone.Length != 10)
            {
                return new ValidationResult(String.Format(ErrorMessage, validationContext.DisplayName));
            }

            else
            {
                value = phone;
                return ValidationResult.Success;
            }


        }
    }
}
