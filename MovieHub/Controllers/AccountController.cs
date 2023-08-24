using MovieHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieHub.Controllers
{
    public class AccountController : Controller
    {
        MovieHubDatabaseEntities1 db = new MovieHubDatabaseEntities1();

        // Registeration
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(User u)
        {
            if (u.Username != null && u.UserPassword != null && u.UserRole == null)
            {
                u.UserRole = "Customer";
                db.Users.Add(u);
                db.SaveChanges();
                return Redirect("Login");
            }
            else
            {
                ViewBag.ErrorMessage = "User name and Password should not be null.";
                return View();

            }
        }

        // Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User u)
        {
            string userName = Request.Cookies["UserName"]?.Value;

            bool b1 = db.Users.Any(x => x.Username == u.Username && x.UserPassword == u.UserPassword);
            if (u.Username != null && u.UserPassword != null)
            {
                if (b1)
                {
                    if (db.Users.Any(x => x.Username == u.Username && x.UserRole == "Customer"))
                    {
                        Response.Cookies["UserName"].Value = u.Username;
                        return RedirectToAction("LandigPage", "Account");
                    }
                    else
                    {
                        return RedirectToAction("AdminPage", "Account");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "User name or Password is incorrect.";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "User name and Password should not be null.";
            }
            return View();
        }

        // Landing
        [HttpGet]
        public ActionResult LandigPage()
        {
            //if (Response.Cookies["UserName"].Value != null)
            //{
                return View(db.MovieDatas.ToList());
            //}
            //return Redirect("Login");
        }

        [HttpGet]
        public ActionResult AdminPage()
        {
            return View();
        }



    }

}