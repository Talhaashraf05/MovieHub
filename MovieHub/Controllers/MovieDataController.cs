using MovieHub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieHub.Controllers
{
    public class MovieDataController : Controller
    {
        MovieHubDatabaseEntities1 db = new MovieHubDatabaseEntities1();

        // Add Movie by Admin
        [HttpGet]
        public ActionResult CreateMovie()
        {
                var screens = db.Screens.ToList();
                ViewBag.M_ScreenId = new SelectList(screens, "S_Id", "S_Name");
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovie(MovieData m)
        {
            if (m.M_StartDate.Date <= m.M_EndDate.Date)
            {
                bool movieNotExists = true; // Assume movie does not exist
                ViewBag.ErrorMessage = "Check 3 is incorrect.";

                // Check if db.MovieDatas is empty for the selected screen ID
                if (!db.MovieDatas.Any(x => x.M_ScreenId == m.M_ScreenId))
                {
                    movieNotExists = true; // Skip time validation if no movies exist for the 
                    ViewBag.ErrorMessage = "check 1 is incorrect.";
                }
                else
                {
                    movieNotExists = db.MovieDatas.Any(x => x.M_ScreenId == m.M_ScreenId &&
                        ((m.M_ShowtimeStart < x.M_ShowtimeStart && m.M_ShowtimeEnd < x.M_ShowtimeStart) ||
                        (m.M_ShowtimeStart > x.M_ShowtimeEnd && m.M_ShowtimeEnd > x.M_ShowtimeEnd)));
                    ViewBag.ErrorMessage = "Check2 is incorrect.";

                }

                if (movieNotExists)
                {
                    try
                            {
                                string imageFileName = Path.GetFileNameWithoutExtension(m.ImageFile.FileName);
                                string imageExtension = Path.GetExtension(m.ImageFile.FileName);
                                string imageFilePath = "~/Movies/Images/" + imageFileName + imageExtension;
                                string imagePhysicalPath = Server.MapPath(imageFilePath);
                                m.M_img = imageFilePath;
                                m.ImageFile.SaveAs(imagePhysicalPath);

                                string bannerFileName = Path.GetFileNameWithoutExtension(m.BannerFile.FileName);
                                string bannerExtension = Path.GetExtension(m.BannerFile.FileName);
                                string bannerFilePath = "~/Movies/Banner/" + bannerFileName + bannerExtension;
                                string bannerPhysicalPath = Server.MapPath(bannerFilePath);
                                m.M_Banner = bannerFilePath;
                                m.BannerFile.SaveAs(bannerPhysicalPath);

                                db.MovieDatas.Add(m);
                                db.SaveChanges();
                                return RedirectToAction("AdminPage", "Account");
                            }
                            catch (DbEntityValidationException ex)
                            {
                                // Handle validation errors
                                var errorMessages = ex.EntityValidationErrors
                                    .SelectMany(x => x.ValidationErrors)
                                    .Select(x => x.ErrorMessage);
                                string errorMessage = string.Join("; ", errorMessages);
                                ViewBag.ErrorMessage = "Validation errors: " + errorMessage;
                            }
                            catch (Exception ex)
                            {
                                ViewBag.ErrorMessage = "An error occurred while saving the movie data: " + ex.Message;
                            }
                }
                else
                {
                    ViewBag.ErrorMessage = "Movie already exists in this Screen.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Start Date must be earlier than End Date and Start Time must be earlier than End Time.";
            }



            var screens = db.Screens.ToList();
            ViewBag.M_ScreenId = new SelectList(screens, "S_Id", "S_Name");
            return View(m);
        }

        // Show Movie by Admin
        public ActionResult ShowMovieData()
        {
                List<MovieData> movies = db.MovieDatas.ToList();
                return View(movies);
        }

        // Delete Movie by Admin
        public ActionResult Delete(int id)
        {
                MovieData movie = db.MovieDatas.Find(id);
                if (movie != null)
                {
                    var seats = db.Seats.Where(s => s.Se_Movie_id == id);
                    db.Seats.RemoveRange(seats);
                    db.MovieDatas.Remove(movie);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = "Movie does not exist.";
                }
            

            return RedirectToAction("AdminPage", "Account");
        }
    }
}
