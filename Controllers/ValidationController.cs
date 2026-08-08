using Microsoft.AspNetCore.Mvc;
using Picklr.Models;

namespace Picklr.Controllers
{
    // Controller that exists purely to serve remote validation requests.
    // jQuery calls CheckEmail from the browser as soon as the user leaves the
    // email field on the Checkout form, and MVC returns JSON that the
    // unobtrusive validation library turns into an error message.
    public class ValidationController : Controller
    {
        private PicklrContext context;

        public ValidationController(PicklrContext ctx) => context = ctx;

        // Called by the [Remote("CheckEmail", "Validation")] attribute on
        // CheckoutInfo.Email. The parameter name must match the property name.
        //
        // Return Json(true)  -> valid
        // Return Json(msg)   -> invalid, and msg becomes the error shown
        public JsonResult CheckEmail(string email)
        {
            // The rule depends on what's in the cart, which lives in Session --
            // so we read it here rather than passing it up from the browser.
            var cart = SessionCart.GetCart(HttpContext.Session);

            string msg = Check.DuplicateBooking(context, email, cart);

            if (string.IsNullOrEmpty(msg))
            {
                // Flag that the check already passed on the client side, so
                // CartController doesn't run the same database query twice.
                TempData["okEmail"] = true;
                return Json(true);
            }
            else
            {
                return Json(msg);
            }
        }
    }
}
