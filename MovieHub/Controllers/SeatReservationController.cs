using MovieHub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace MovieHub.Controllers
{
    public class SeatReservationController : Controller
    {
        MovieHubDatabaseEntities1 db = new MovieHubDatabaseEntities1();

        // Seat Reservation by Customer
        [HttpGet]
        public ActionResult Reservation(int? movieId)
        {
                if (movieId == null)
                {
                    return RedirectToAction("Error");
                }

                List<string> reservedSeatNumbers;

                reservedSeatNumbers = db.Seats
                .Where(s => s.Se_Movie_id == movieId.Value)
                .Select(s => s.Se_Number.ToString()) // Convert to string explicitly
                .ToList();
                // Generate the list of available seat numbers
                List<string> availableSeatNumbers = Enumerable.Range(1, 200)
                    .Where(seatNumber => !reservedSeatNumbers.Contains(seatNumber.ToString()))
                    .Select(seatNumber => seatNumber.ToString())
                    .ToList();

                // Create a SelectList for the dropdown
                ViewBag.AvailableSeatNumbers = new SelectList(availableSeatNumbers);
                return View();
        }

        [HttpPost]
        public ActionResult Reservation(Seat se, int movieId)
        {
            string userName = HttpContext.Request.Cookies["UserName"]?.Value;
            bool seNumberExists = db.Seats.Any(s => s.Se_Number == se.Se_Number && s.Se_Movie_id == movieId);

                if (!seNumberExists)
                {
                    se.Se_Movie_id = movieId;
                    User user = db.Users.FirstOrDefault(u => u.Username == userName);

                    if (user != null)
                    {
                        se.Se_User_id = user.Id;
                        se.Se_UserName = user.FirstName + " " + user.LastName;
                        db.Seats.Add(se);
                        try
                        {
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Seat created successfully.";

                            return RedirectToAction("LandigPage","Account");
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var errorMessages = ex.EntityValidationErrors
                                .SelectMany(e => e.ValidationErrors)
                                .Select(e => e.ErrorMessage);

                            ModelState.AddModelError("", "Validation errors occurred:");

                            foreach (var errorMessage in errorMessages)
                            {
                                ModelState.AddModelError("", errorMessage);
                            }
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "User not found.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Seat Already Booked.";
                    return View();
                }
        }

        // Show Seat Data for admin
        public ActionResult ShowSeatData()
        {
                List<string> reservedSeatNumbers = db.Seats
                .Select(s => s.Se_Number)
                .ToList();
                var seats = db.Seats.Include(s => s.MovieData).ToList();
                return View(seats);
        }


        // Delete any Seat by Admin
        public ActionResult Delete(int id)
        {
                Seat p = db.Seats.Find(id);
                if (p != null)
                {
                    db.Seats.Remove(p);
                    db.SaveChanges();
                    return View();
                }
                ViewBag.ErrorMessage = "Seat not found.";
                return View();
        }


        [HttpGet]
        public ActionResult SearchSeat()
        {
                return View();
        }

        [HttpPost]
        public ActionResult SearchSeat(string username)
        {
                var matchingSeats = db.Seats.Where(s => s.Se_UserName == username).ToList();

                if (matchingSeats.Count > 0)
                {
                    return View("SearchSeatResults", matchingSeats);
                }
                else
                {
                    ViewBag.ErrorMessage = "No seat reservations found for the given username.";
                    return View("SearchSeat");
                }
        }

        [HttpPost]
        public ActionResult DeleteSeat(int id)
        {
                Seat seat = db.Seats.Find(id);

                if (seat != null)
                {
                    db.Seats.Remove(seat);
                    db.SaveChanges();
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = "Seat reservation not found.";
                    return View();
                }
        }


    }
}
