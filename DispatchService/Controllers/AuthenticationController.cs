using DispatchService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DispatchService.Controllers
{
    public class AuthenticationController : Controller
    {
        public ActionResult Login()
        {            
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Main", "Request");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {         
            if (ModelState.IsValid)
            {
                using (HCSContext db = new HCSContext())
                {
                    User user = null;
                    user = db.Users.FirstOrDefault(u => u.login == model.login && u.password == model.password);
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.login, true);
                        return RedirectToAction("Main", "Request");
                    } else
                    {
                        ModelState.AddModelError("", "Неправильно введены логин или пароль");
                    }
                }
            }
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Authentication");
        }
    }
}