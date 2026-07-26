using System.ComponentModel.DataAnnotations;

namespace Picklr.Models
{
    public class Club
    {
        public int ClubID { get; set; }

        [Required(ErrorMessage = "Please enter a club name.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a location.")]
        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
