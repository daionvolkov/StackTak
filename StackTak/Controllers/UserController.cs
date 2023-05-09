using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO.Pipes;
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
            int accessRights = validUser.AccessRights;
            Session["AccessRights"] = accessRights;
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




            [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
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