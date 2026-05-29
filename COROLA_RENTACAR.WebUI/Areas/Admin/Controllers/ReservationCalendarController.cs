using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReservationCalendarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IReservationService _reservationService;

        public ReservationCalendarController(
            ICarService carService,
            IReservationService reservationService)
        {
            _carService = carService;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? year, int? month, string status = "active")
        {
            var today = DateTime.Today;

            var selectedYear = year ?? today.Year;
            var selectedMonth = month ?? today.Month;

            if (selectedMonth < 1 || selectedMonth > 12)
            {
                selectedMonth = today.Month;
            }

            var monthStart = new DateTime(selectedYear, selectedMonth, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var cars = await _carService.TGetAllCarsWithDetailsAsync();
            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            reservations = reservations
                .Where(x =>
                    x.PickupDate.Date <= monthEnd.Date &&
                    x.ReturnDate.Date >= monthStart.Date)
                .ToList();

            status = string.IsNullOrWhiteSpace(status) ? "active" : status.ToLower();

            reservations = status switch
            {
                "pending" => reservations
                    .Where(x => x.ReservationStatus == ReservationStatus.Pending)
                    .ToList(),

                "approved" => reservations
                    .Where(x => x.ReservationStatus == ReservationStatus.Approved)
                    .ToList(),

                "rejected" => reservations
                    .Where(x => x.ReservationStatus == ReservationStatus.Rejected)
                    .ToList(),

                "cancelled" => reservations
                    .Where(x => x.ReservationStatus == ReservationStatus.Cancelled)
                    .ToList(),

                "all" => reservations.ToList(),

                _ => reservations
                    .Where(x =>
                        x.ReservationStatus == ReservationStatus.Pending ||
                        x.ReservationStatus == ReservationStatus.Approved)
                    .ToList()
            };

            var previousMonth = monthStart.AddMonths(-1);
            var nextMonth = monthStart.AddMonths(1);

            ViewBag.Cars = cars;
            ViewBag.MonthStart = monthStart;
            ViewBag.MonthEnd = monthEnd;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedStatus = status;
            ViewBag.MonthName = monthStart.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("en-US"));

            ViewBag.PreviousYear = previousMonth.Year;
            ViewBag.PreviousMonth = previousMonth.Month;

            ViewBag.NextYear = nextMonth.Year;
            ViewBag.NextMonth = nextMonth.Month;

            ViewBag.TotalReservations = reservations.Count;
            ViewBag.PendingReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Pending);
            ViewBag.ApprovedReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Approved);
            ViewBag.MonthRevenue = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Approved)
                .Sum(x => x.TotalPrice);

            return View(reservations.OrderBy(x => x.PickupDate).ToList());
        }
    }
}