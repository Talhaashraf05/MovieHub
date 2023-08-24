using MovieHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieHub.Controllers
{
    public class ScreenController : Controller
    {
        MovieHubDatabaseEntities1 db = new MovieHubDatabaseEntities1();

        // Add Screen by Admin
        [HttpGet]
        public ActionResult AddScreen()
        {
                var cinema = db.Cinemas.ToList();
                ViewData["S_Cinema_Id"] = new SelectList(cinema, "C_Id", "C_Name");
                return View();
        }
        [HttpPost]
        public ActionResult AddScreen(Screen s)
        {
                db.Screens.Add(s);
                db.SaveChanges();
                return Redirect("Admin");
        }
        public ActionResult ShowScreenData()
        {
                return View(db.Screens.ToList());
        }
        public ActionResult DeleteScreen(int id)
        {
                Screen screen = db.Screens.Find(id);
                if (screen != null)
                {
                    var movieDataList = db.MovieDatas.Where(m => m.M_ScreenId == id);
                    foreach (var movieData in movieDataList)
                    {
                        var reservedSeats = db.Seats.Where(s => s.Se_Movie_id == movieData.M_Id);
                        db.Seats.RemoveRange(reservedSeats);
                        db.MovieDatas.Remove(movieData);
                    }
                    db.Screens.Remove(screen);
                    db.SaveChanges();
                }
                ViewBag.ErrorMessage = "Screen don't Exist.";
                return RedirectToAction("Admin");
        }
    }
}