using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StackTak.Controllers
{
    public class AccountController : Controller
    {
        private readonly InventoryDbContext db = new InventoryDbContext();

        [Authorize]
        public ActionResult Index()
        {
            string currentUserLogin = User.Identity.Name;
            User_List user = db.Users.FirstOrDefault(u => u.Login == currentUserLogin);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }
        [Authorize]
        public ActionResult Edit()
        {
            string currentUserLogin = User.Identity.Name;
            User_List user = db.Users.FirstOrDefault(u => u.Login == currentUserLogin);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(User_List user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }
    }

}