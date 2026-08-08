using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Picklr.Models;

namespace Picklr.Controllers
{
    // The shopping cart. Selected programs live only in Session (as CartItem
    // objects) until the user completes the Checkout form -- at that point
    // each item becomes a Reservation row in the database.
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

        // GET /Cart/Checkout -- shows the form that collects customer details
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = SessionCart.GetCart(HttpContext.Session);

            if (cart.Count == 0)
            {
                TempData["message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            ViewBag.Total = cart.Sum(c => c.Fee);
            return View(new CheckoutInfo());
        }

        // POST /Cart/Checkout -- validates, then saves the reservations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutInfo info)
        {
            var cart = SessionCart.GetCart(HttpContext.Session);

            if (cart.Count == 0)
            {
                TempData["message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            // Second layer of the remote validation. If JavaScript is disabled
            // the browser never called ValidationController.CheckEmail, so
            // TempData["okEmail"] is null and we run the same check here.
            // Both paths call Check.DuplicateBooking, so the rule lives in
            // exactly one place.
            if (TempData["okEmail"] == null)
            {
                string msg = Check.DuplicateBooking(context, info.Email, cart);
                if (!string.IsNullOrEmpty(msg))
                {
                    ModelState.AddModelError(nameof(CheckoutInfo.Email), msg);
                }
            }

            // Custom message added straight to ModelState (Ch11, slide 23):
            // a date of birth in the future is nonsense even if it passes
            // the minimum-age rule.
            string dobKey = nameof(CheckoutInfo.DOB);
            if (ModelState.GetValidationState(dobKey) == ModelValidationState.Valid)
            {
                if (info.DOB > DateTime.Today)
                {
                    ModelState.AddModelError(dobKey,
                        "Date of birth must not be a future date.");
                }
            }

            if (ModelState.IsValid)
            {
                foreach (var item in cart)
                {
                    context.Reservations.Add(new Reservation
                    {
                        ProgramID = item.ProgramID,
                        ProgramDate = item.Date,
                        FeePaid = item.Fee,
                        ReservedOn = DateTime.Now,
                        CustomerName = info.FullName,
                        CustomerEmail = info.Email,
                        CustomerPhone = info.Phone,
                        CustomerDOB = info.DOB
                    });
                }
                context.SaveChanges();

                int count = cart.Count;
                SessionCart.ClearCart(HttpContext.Session);

                TempData["message"] =
                    $"Payment confirmed! {count} reservation(s) saved for {info.FullName}.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Model-level message shown by asp-validation-summary="ModelOnly"
                ModelState.AddModelError("",
                    "There are errors in the form. Please correct them and try again.");

                ViewBag.Total = cart.Sum(c => c.Fee);
                return View(info);
            }
        }
    }
}
