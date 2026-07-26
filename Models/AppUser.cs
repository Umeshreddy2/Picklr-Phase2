using System.ComponentModel.DataAnnotations;

namespace Picklr.Models
{
    // Named AppUser to avoid future conflicts if ASP.NET Identity is added.
    public class AppUser
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Please enter a first name.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a last name.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "Client";

        // Computed display name for use in views
        public string FullName => $"{FirstName} {LastName}";
    }
}
