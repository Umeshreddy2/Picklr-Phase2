using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Picklr.Models
{
    // A saved (paid) booking. Rows are only written here after the user
    // completes the Checkout form -- before that, chosen programs live only
    // in Session as CartItem objects (see CartItem.cs).
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        [Required]
        public int ProgramID { get; set; }

        // Navigation property -- never posted from a form.
        [ValidateNever]
        public PicklProgram Program { get; set; } = null!;

        [Required]
        [Display(Name = "Program Date")]
        public DateTime ProgramDate { get; set; }

        // Fee snapshot at the time of booking, in case the program's fee
        // changes later in the admin portal.
        [Display(Name = "Fee Paid")]
        public decimal FeePaid { get; set; }

        [Display(Name = "Reserved On")]
        public DateTime ReservedOn { get; set; } = DateTime.Now;

        // ---- Phase 3: who made the booking -------------------------------
        // Captured on the Checkout form and copied onto every reservation in
        // that order. CustomerEmail is what the remote validation queries to
        // detect a duplicate booking.

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Display(Name = "Date of Birth")]
        public DateTime? CustomerDOB { get; set; }
    }
}
