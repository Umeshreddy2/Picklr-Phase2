namespace Picklr.Models
{
    // Utility class holding validation logic that has to query the database.
    //
    // It is singled out so that BOTH the remote validation (ValidationController)
    // and the server-side fallback (CartController) can call the same method,
    // instead of the rule being written twice and drifting apart.
    //
    // Since all it does is validate against the database, it lives in Models.
    public static class Check
    {
        // Returns an error message if this email already has a reservation for
        // any program/date combination currently sitting in the cart.
        // Returns an empty string when everything is fine.
        public static string DuplicateBooking(PicklrContext ctx,
            string email, List<CartItem> cart)
        {
            string msg = string.Empty;

            if (!string.IsNullOrEmpty(email) && cart != null)
            {
                foreach (var item in cart)
                {
                    bool exists = ctx.Reservations.Any(r =>
                        r.CustomerEmail.ToLower() == email.ToLower() &&
                        r.ProgramID == item.ProgramID &&
                        r.ProgramDate == item.Date);

                    if (exists)
                    {
                        msg = $"{email} already has a reservation for " +
                              $"{item.ProgramName} on " +
                              $"{item.Date.ToShortDateString()}.";
                        break;
                    }
                }
            }

            return msg;
        }
    }
}
