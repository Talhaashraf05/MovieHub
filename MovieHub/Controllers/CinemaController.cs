using MovieHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieHub.Controllers
{
    public class CinemaController : Controller
    {
        MovieHubDatabaseEntities1 db = new MovieHubDatabaseEntities1();

        // Add Cinema by Admin
        [HttpGet]
        public ActionResult AddCinema()
        {
                return View();
        }
        [HttpPost]
        public ActionResult AddCinema(Cinema c)
        {
                db.Cinemas.Add(c);
                db.SaveChanges();
                return Redirect("Admin");
        }
    
    public ActionResult ShowCinemaData()
        {
                return View(db.Cinemas.ToList());

        }


        // Delete Cinema by Admin
        public ActionResult DeleteCinema(int id)
        {
                Cinema cinema = db.Cinemas.Find(id);
                if (cinema != null)
                {
                    var screens = db.Screens.Where(s => s.S_Cinema_Id == id).ToList();

                    foreach (var screen in screens)
                    {
                        var movieDataList = db.MovieDatas.Where(m => m.M_ScreenId == screen.S_Id).ToList();
                        foreach (var movieData in movieDataList)
                        {
                            var reservedSeats = db.Seats.Where(s => s.Se_Movie_id == movieData.M_Id).ToList();
                            db.Seats.RemoveRange(reservedSeats);
                            db.MovieDatas.Remove(movieData);
                        }
                        db.Screens.Remove(screen);
                    }
                    db.Cinemas.Remove(cinema);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = "Cimena not Exist.";
                }
                return RedirectToAction("AdminPage", "Account");
        }

    }
}