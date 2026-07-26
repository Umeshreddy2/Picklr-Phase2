using Microsoft.AspNetCore.Mvc;
using Picklr.Models;

namespace Picklr.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClubController : Controller
    {
        private PicklrContext context;

        public ClubController(PicklrContext ctx)
        {
            context = ctx;
        }

        // GET /Admin/Club/List
        public IActionResult List()
        {
            var clubs = context.Clubs.OrderBy(c => c.Name).ToList();
            return View(clubs);
        }

        // GET /Admin/Club/AddEdit        — shows a blank form (Add)
        // GET /Admin/Club/AddEdit/3      — loads the Club with id=3 (Edit)
        [HttpGet]
        public IActionResult AddEdit(int? id)
        {
            var club = (id == null)
                ? new Club()
                : context.Clubs.Find(id) ?? new Club();

            ViewBag.Action = (id == null) ? "Add" : "Edit";
            return View(club);
        }

        // POST — handle both Add and Edit based on ClubID
        [HttpPost]
        public IActionResult AddEdit(Club club)
        {
            if (ModelState.IsValid)
            {
                if (club.ClubID == 0)
                    context.Clubs.Add(club);
                else
                    context.Clubs.Update(club);

                context.SaveChanges();
                TempData["message"] = $"'{club.Name}' was saved successfully.";
                return RedirectToAction("List"); // PRG: redirect after POST
            }

            // Validation failed — redisplay the form
            ViewBag.Action = (club.ClubID == 0) ? "Add" : "Edit";
            return View(club);
        }

        // GET /Admin/Club/Delete/3 — confirmation page
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var club = context.Clubs.Find(id) ?? new Club();
            return View(club);
        }

        // POST — perform the delete
        [HttpPost]
        public IActionResult Delete(Club club)
        {
            context.Clubs.Remove(club);
            context.SaveChanges();
            TempData["message"] = $"'{club.Name}' was deleted.";
            return RedirectToAction("List"); // PRG
        }
    }
}
