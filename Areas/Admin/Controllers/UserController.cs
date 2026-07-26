using Microsoft.AspNetCore.Mvc;
using Picklr.Models;

namespace Picklr.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private PicklrContext context;

        public UserController(PicklrContext ctx)
        {
            context = ctx;
        }

        // GET /Admin/User/List
        public IActionResult List()
        {
            var users = context.Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
            return View(users);
        }

        // GET /Admin/User/AddEdit        — blank form (Add)
        // GET /Admin/User/AddEdit/1      — loads existing record (Edit)
        [HttpGet]
        public IActionResult AddEdit(int? id)
        {
            var user = (id == null)
                ? new AppUser()
                : context.Users.Find(id) ?? new AppUser();

            ViewBag.Action = (id == null) ? "Add" : "Edit";
            return View(user);
        }

        [HttpPost]
        public IActionResult AddEdit(AppUser user)
        {
            if (ModelState.IsValid)
            {
                if (user.UserID == 0)
                    context.Users.Add(user);
                else
                    context.Users.Update(user);

                context.SaveChanges();
                TempData["message"] = $"'{user.FullName}' was saved successfully.";
                return RedirectToAction("List"); // PRG
            }

            ViewBag.Action = (user.UserID == 0) ? "Add" : "Edit";
            return View(user);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = context.Users.Find(id) ?? new AppUser();
            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(AppUser user)
        {
            context.Users.Remove(user);
            context.SaveChanges();
            TempData["message"] = $"'{user.FullName}' was deleted.";
            return RedirectToAction("List"); // PRG
        }
    }
}
