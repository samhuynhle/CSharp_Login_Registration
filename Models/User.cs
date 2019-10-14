using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace login_reg.Models
{
    public class User
    {
        [Key]
        public int UserId {get; set;}
        [Required]
        [MinLength(2, ErrorMessage="First Name must be 2 or more characters")]
        public string FirstName {get; set;}
        [Required]
        [MinLength(2, ErrorMessage="Last Name must be 2 or more characters")]
        public string LastName {get; set;}
        [Required]
        [EmailAddress]
        public string Email {get; set;}
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer")]
        public string Password {get; set;}
        [Required]
        [DataType(DataType.Date)]
        [AgeValidation]
        public DateTime DateOfBirth {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;

        [NotMapped]
        [Compare("Password", ErrorMessage="Passwords must match")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
    }
    public class AgeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime DoB = (DateTime)value; 
            DateTime today = DateTime.Now;
            if (today < DoB)
            {
                return new ValidationResult("You can't be a time traveler!");    
            }
            
            DateTime zeroTime = new DateTime(1,1,1);
            TimeSpan span = today - DoB;
            int years = (zeroTime + span).Year -1;
            if(years < 18)
            {
                return new ValidationResult("You must be 18 years old!");
            }
            return ValidationResult.Success;
        }
    }
}