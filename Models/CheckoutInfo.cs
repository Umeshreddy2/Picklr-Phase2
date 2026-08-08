using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Picklr.Models
{
    // The model bound to the Checkout form. It isn't a DbSet -- once it
    // validates, its values are copied onto each Reservation being saved.
    //
    // Implements IValidatableObject so the class can also carry a
    // model-level rule that spans more than one property (Ch11, slide 43).
    public class CheckoutInfo : IValidatableObject
    {
        // ---- Standard validation -----------------------------------------

        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(50, ErrorMessage =
            "Name must be 50 characters or less.")]
        [RegularExpression(@"^[a-zA-Z .'-]+$", ErrorMessage =
            "Name may not contain numbers or special characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        // ---- Remote validation -------------------------------------------
        // [Remote] points MVC at ValidationController.CheckEmail, which runs
        // on the server but is called by jQuery from the browser as soon as
        // the user leaves this field.
        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Remote("CheckEmail", "Validation")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your email address.")]
        [Compare("Email", ErrorMessage = "The two email addresses must match.")]
        [Display(Name = "Confirm Email")]
        public string ConfirmEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a phone number.")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage =
            "Please enter the phone number as 123-456-7890.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        // ---- Custom validation (server-side + client-side) ---------------
        // MinimumAge is our own ValidationAttribute. Players must be at least
        // 13 to book a program on their own.
        [Required(ErrorMessage = "Please enter a date of birth.")]
        [MinimumAge(13, ErrorMessage =
            "You must be at least 13 years old to book a program.")]
        [Display(Name = "Date of Birth")]
        public DateTime? DOB { get; set; }

        [Display(Name = "I agree to the club rules")]
        public bool AgreeToRules { get; set; }

        // ---- Model-level validation --------------------------------------
        // Runs after the property-level rules above. Model-level rules can't
        // be checked by client-side JavaScript, so this one always runs on
        // the server -- which is exactly why the checkbox is validated here.
        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (!AgreeToRules)
            {
                yield return new ValidationResult(
                    "You must agree to the club rules before booking.",
                    new[] { nameof(AgreeToRules) });
            }

            if (!string.IsNullOrEmpty(Email) &&
                Email.Equals(FullName, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(
                    "Name and email address cannot be the same.",
                    new[] { nameof(FullName), nameof(Email) });
            }
        }
    }
}
