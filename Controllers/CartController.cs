using Microsoft.AspNetCore.Mvc;
using Picklr.Models;

namespace Picklr.Controllers
{
    // The shopping cart. Selected programs live only in Session (as CartItem
    // objects) until the user clicks "Pay & Confirm" -- at that point each
    // item becomes a Reservation row in the database.
    public class CartController : Controller
    {
        private PicklrContext context;

        public CartController(PicklrContext ctx)
        {
            context = ctx;
        }

        // GET /Cart
        public IActionResult Index()
        {
            var cart = SessionCart.GetCart(HttpContext.Session);
            return View(cart);
        }

        // POST /Cart/Remove -- index is the item's position in the cart list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int index)
        {
            var cart = SessionCart.GetCart(HttpContext.Session);

            if (index >= 0 && index < cart.Count)
            {
                var removed = cart[index];
                cart.RemoveAt(index);
                SessionCart.SaveCart(HttpContext.Session, cart);
                TempData["message"] = $"{removed.ProgramName} removed from your cart.";
            }

            return RedirectToAction("Index");
        }

        // POST /Cart/ClearAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearAll()
        {
            SessionCart.ClearCart(HttpContext.Session);
            TempData["message"] = "Your cart was cleared.";
            return RedirectToAction("Index");
        }

        // POST /Cart/PayConfirm -- writes every cart item to the Reservations
        // table, then empties the Session cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayConfirm()
        {
            var cart = SessionCart.GetCart(HttpContext.Session);

            foreach (var item in cart)
            {
                context.Reservations.Add(new Reservation
                {
                    ProgramID = item.ProgramID,
                    ProgramDate = item.Date,
                    FeePaid = item.Fee,
                    ReservedOn = DateTime.Now
                });
            }
            context.SaveChanges();

            int count = cart.Count;
            SessionCart.ClearCart(HttpContext.Session);

            TempData["message"] = $"Payment confirmed! {count} reservation(s) saved.";
            return RedirectToAction("Index", "Home");
        }
    }
}
