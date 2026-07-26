using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Picklr.Models;

namespace Picklr.Controllers
{
    public class HomeController : Controller
    {
        private PicklrContext context;

        public HomeController(PicklrContext ctx)
        {
            context = ctx;
        }

        // GET /                              -> today, all clubs
        // GET /?clubId=1&date=2026-07-18      -> model binding pulls clubId and
        // date straight off the query string, the same way Ch10's ToDo List
        // Index(string id) action lets MVC bind a primitive parameter instead
        // of reading Request.Query["..."] by hand.
        public IActionResult Index(int? clubId, DateTime? date)
        {
            DateTime searchDate = date ?? DateTime.Today;

            ViewBag.SelectedClubId = clubId;
            ViewBag.SelectedDate = searchDate;

            // Club dropdown options
            ViewBag.Clubs = context.Clubs.OrderBy(c => c.Name).ToList();

            // Date dropdown: today + the next 7 days. This is a small computed
            // list built in the controller, the same idea as the Filters
            // .DueFilterValues static dictionary in Ch10.
            ViewBag.DateOptions = Enumerable.Range(0, 7)
                .Select(offset => DateTime.Today.AddDays(offset))
                .ToList();

            IQueryable<PicklProgram> query = context.Programs.Include(p => p.Club);

            if (clubId.HasValue)
            {
                query = query.Where(p => p.ClubID == clubId.Value);
            }

            // IsAvailableOn() is plain C# (splits the AvailableDays string), so
            // it can't be translated to SQL -- pull the (already club-filtered)
            // rows into memory first with AsEnumerable(), then finish filtering.
            var programs = query
                .AsEnumerable()
                .Where(p => p.IsAvailableOn(searchDate))
                .OrderBy(p => p.Name)
                .ToList();

            return View(programs);
        }

        // POST -- handles the "Reserve" button on each row of the results
        // table. programId, date, and clubId are posted as hidden fields
        // with the same names as these parameters, so MVC binds them the
        // same way the ToDo List sample's MarkComplete(...) action does
        // (Ch10, slide 47).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reserve(int programId, DateTime date, int? clubId)
        {
            var program = context.Programs.Include(p => p.Club)
                .FirstOrDefault(p => p.ProgramID == programId);

            if (program != null)
            {
                var cart = SessionCart.GetCart(HttpContext.Session);
                cart.Add(new CartItem
                {
                    ProgramID = program.ProgramID,
                    ProgramName = program.Name,
                    ClubName = program.Club.Name,
                    Fee = program.Fee,
                    Date = date
                });
                SessionCart.SaveCart(HttpContext.Session, cart);

                TempData["message"] = $"{program.Name} on {date:MMM d} added to your cart.";
            }

            // PRG: redirect back to Index, preserving the filters the user had.
            return RedirectToAction("Index", new { clubId, date = date.ToString("yyyy-MM-dd") });
        }

        // Placeholder pages -- out of scope for Phase 2.
        public ContentResult About() => Content("About page — under construction.");
        public ContentResult Club() => Content("Club page — under construction.");
        public ContentResult Program() => Content("Program page — under construction.");
        public ContentResult Shop() => Content("Shop page — under construction.");
    }
}
