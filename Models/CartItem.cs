namespace Picklr.Models
{
    // A single line item held in the shopping cart. This is a plain POCO
    // (not an EF entity / not a DbSet) -- it is serialized to JSON and
    // stored in ASP.NET Core Session, not the database. It only becomes a
    // Reservation row once the user clicks "Pay & Confirm".
    public class CartItem
    {
        public int ProgramID { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public DateTime Date { get; set; }
    }
}
