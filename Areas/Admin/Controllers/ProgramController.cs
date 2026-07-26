using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Picklr.Models;

namespace Picklr.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProgramController : Controller
    {
        private PicklrContext context;

        public ProgramController(PicklrContext ctx)
        {
            context = ctx;
        }

        // GET /Admin/Program/List
        public IActionResult List()
        {
            var programs = context.Programs
                .Include(p => p.Club)
                .OrderBy(p => p.Name)
                .ToList();
            return View(programs);
        }

        // GET /Admin/Program/AddEdit        — blank form (Add)
        // GET /Admin/Program/AddEdit/2      — loads existing record (Edit)
        [HttpGet]
        public IActionResult AddEdit(int? id)
        {
            ViewBag.Clubs = context.Clubs.OrderBy(c => c.Name).ToList();

            var program = (id == null)
                ? new PicklProgram()
                : context.Programs.Find(id) ?? new PicklProgram();

            ViewBag.Action = (id == null) ? "Add" : "Edit";
            return View(program);
        }

        // Available Days is posted as a string array of checked checkbox
        // values (all sharing name="availableDays") -- the same "post an
        // array to an action" technique Ch10 (slide 23) uses for the
        // ToDo List's Filter(string[] filter) action. It can't be part of
        // the PicklProgram model-bind itself because AvailableDays on the
        // model is a single delimited string, not an array.
        [HttpPost]
        public IActionResult AddEdit(PicklProgram program, string[] availableDays)
        {
            program.AvailableDays = string.Join(",", availableDays ?? Array.Empty<string>());

            // The Club navigation property isn't posted from the form (only
            // ClubID is), so remove it from validation the same way Ch10's
            // ToDo sample uses [ValidateNever] on Category/Status.
            ModelState.Remove(nameof(PicklProgram.Club));

            if (ModelState.IsValid)
            {
                if (program.ProgramID == 0)
                    context.Programs.Add(program);
                else
                    context.Programs.Update(program);

                context.SaveChanges();
                TempData["message"] = $"'{program.Name}' was saved successfully.";
                return RedirectToAction("List"); // PRG
            }

            ViewBag.Clubs = context.Clubs.OrderBy(c => c.Name).ToList();
            ViewBag.Action = (program.ProgramID == 0) ? "Add" : "Edit";
            return View(program);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var program = context.Programs.Include(p => p.Club).FirstOrDefault(p => p.ProgramID == id)
                ?? new PicklProgram();
            return View(program);
        }

        [HttpPost]
        public IActionResult Delete(PicklProgram program)
        {
            var toDelete = context.Programs.Find(program.ProgramID);
            if (toDelete != null)
            {
                context.Programs.Remove(toDelete);
                context.SaveChanges();
                TempData["message"] = $"'{toDelete.Name}' was deleted.";
            }
            return RedirectToAction("List"); // PRG
        }
    }
}
