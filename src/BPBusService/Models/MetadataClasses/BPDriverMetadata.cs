using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BPClassLibrary;

/// <summary>
/// Partial class Driver which implements the IvalidateObject so that it is a self-validating model
/// Performs some formatting to the fields before writing to the databse
/// </summary>
namespace BPBusService.Models
{

    [ModelMetadataTypeAttribute(typeof(BPDriverMetadata))]
    public partial class Driver : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            FirstName = BPValidations.Capitalise(FirstName);
            LastName = BPValidations.Capitalise(LastName);
            FullName = LastName + "," + FirstName;

            if (ProvinceCode != null)
            {
                ProvinceCode = ProvinceCode.ToUpper();
            }
            if (PostalCode != null)
            {
                PostalCode = PostalCode.ToUpper();
                if (PostalCode[3] != ' ')
                {
                    PostalCode = PostalCode.Insert(3, " ");
                }
            }

            //HomePhone and WorkPhone Validation
            HomePhone = BPValidations.FormatPhoneNumber(HomePhone);
            if(WorkPhone != null)
            {
                WorkPhone = BPValidations.FormatPhoneNumber(WorkPhone);
            }

            yield return ValidationResult.Success;
        }
    }

    /// <summary>
    /// BPDriver Metadata class which has the validation rules for the Driver details which are written to the database 
    /// </summary>

    public class BPDriverMetadata
    {
        [Display ]
        public int DriverId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string FullName { get; set; }
        [Required]
        [Display(Name = "Home Phone")]
        [ValidatePhoneNumber]
        public string HomePhone { get; set; }
        [Display(Name = "Work Phone")]
        public string WorkPhone { get; set; }
        [Display(Name = "Street Address")]
        public string Street { get; set; }
        public string City { get; set; }
        [Required]
        [PostalCodeValidation]
        [Display(Name ="Postal")]
        public string PostalCode { get; set; }

        [Display(Name = "Province")]
        [Remote("checkProvinceCode", "BPRemotes")]
        public string ProvinceCode { get; set; }
        [Required]
        [DateNotInFuture]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Hired")]
        public DateTime? DateHired { get; set; }

        public virtual ICollection<Trip> Trip { get; set; }
        public virtual Province ProvinceCodeNavigation { get; set; }
    }

    
}
