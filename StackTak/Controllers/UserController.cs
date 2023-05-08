using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StackTak.Controllers
{
    public class UserController : Controller
    {
        private readonly InventoryDbContext db = new InventoryDbContext();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User_List user)
        {
            var validUser = db.Users.FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
            if (validUser != null)
            {
                FormsAuthentication.SetAuthCookie(validUser.Login, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(user);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}