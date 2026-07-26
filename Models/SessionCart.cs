using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Picklr.Models
{
    // A small static helper that reads/writes the shopping cart (a
    // List<CartItem>) as JSON in ASP.NET Core Session. Session state isn't
    // covered in our textbook chapters, so this class keeps the JSON
    // serialize/deserialize plumbing in one place instead of repeating it
    // in HomeController and CartController.
    public static class SessionCart
    {
        private const string SessionKey = "Cart";

        public static List<CartItem> GetCart(ISession session)
        {
            var json = session.GetString(SessionKey);
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        public static void SaveCart(ISession session, List<CartItem> cart)
        {
            session.SetString(SessionKey, JsonSerializer.Serialize(cart));
        }

        public static void ClearCart(ISession session)
        {
            session.Remove(SessionKey);
        }
    }
}
