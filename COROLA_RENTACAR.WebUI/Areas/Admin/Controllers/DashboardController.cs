using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ICarService _carService;
        private readonly ICustomerService _customerService;
        private readonly IReservationService _reservationService;

        public DashboardController(
            ICarService carService,
            ICustomerService customerService,
            IReservationService reservationService)
        {
            _carService = carService;
            _customerService = customerService;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cars = await _carService.TGetAllCarsWithDetailsAsync();
            var customers = await _customerService.TGetAllAsync();
            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            var approvedReservations = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Approved)
                .ToList();

            var today = DateTime.Today;
            var culture = CultureInfo.GetCultureInfo("en-US");

            ViewBag.TotalCars = cars.Count;
            ViewBag.AvailableCars = cars.Count(x => x.IsAvailable);
            ViewBag.UnavailableCars = cars.Count(x => !x.IsAvailable);

            ViewBag.TotalCustomers = customers.Count;
            ViewBag.PendingLicenses = customers.Count(x => x.DriverLicenseVerificationStatus == DriverLicenseVerificationStatus.Pending);
            ViewBag.ApprovedLicenses = customers.Count(x => x.DriverLicenseVerificationStatus == DriverLicenseVerificationStatus.Approved);
            ViewBag.RejectedLicenses = customers.Count(x => x.DriverLicenseVerificationStatus == DriverLicenseVerificationStatus.Rejected);

            ViewBag.TotalReservations = reservations.Count;
            ViewBag.PendingReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Pending);
            ViewBag.ApprovedReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Approved);
            ViewBag.RejectedReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Rejected);
            ViewBag.CancelledReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Cancelled);

            ViewBag.TotalRevenue = approvedReservations.Sum(x => x.TotalPrice);

            ViewBag.MonthlyRevenue = approvedReservations
                .Where(x => x.PickupDate.Month == today.Month && x.PickupDate.Year == today.Year)
                .Sum(x => x.TotalPrice);

            ViewBag.TodayReservations = reservations.Count(x => x.PickupDate.Date == today);
            ViewBag.ActiveReservations = reservations.Count(x =>
                x.ReservationStatus == ReservationStatus.Approved &&
                x.PickupDate.Date <= today &&
                x.ReturnDate.Date >= today);

            var lastSixMonths = Enumerable.Range(0, 6)
                .Select(i => today.AddMonths(-5 + i))
                .ToList();

            ViewBag.MonthLabels = lastSixMonths
                .Select(x => x.ToString("MMM yyyy", culture))
                .ToList();

            ViewBag.MonthlyRevenueData = lastSixMonths
                .Select(month => approvedReservations
                    .Where(x => x.PickupDate.Month == month.Month && x.PickupDate.Year == month.Year)
                    .Sum(x => x.TotalPrice))
                .ToList();

            ViewBag.MonthlyReservationData = lastSixMonths
                .Select(month => reservations
                    .Count(x => x.PickupDate.Month == month.Month && x.PickupDate.Year == month.Year))
                .ToList();

            ViewBag.ReservationStatusLabels = new List<string>
            {
                "Pending",
                "Approved",
                "Rejected",
                "Cancelled"
            };

            ViewBag.ReservationStatusData = new List<int>
            {
                ViewBag.PendingReservations,
                ViewBag.ApprovedReservations,
                ViewBag.RejectedReservations,
                ViewBag.CancelledReservations
            };

            ViewBag.CarAvailabilityLabels = new List<string>
            {
                "Available",
                "Unavailable"
            };

            ViewBag.CarAvailabilityData = new List<int>
            {
                ViewBag.AvailableCars,
                ViewBag.UnavailableCars
            };

            var categoryGroups = cars
                .GroupBy(x => x.Category != null ? x.Category.CategoryName : "Uncategorized")
                .OrderByDescending(x => x.Count())
                .ToList();

            ViewBag.CategoryLabels = categoryGroups.Select(x => x.Key).ToList();
            ViewBag.CategoryData = categoryGroups.Select(x => x.Count()).ToList();

            var latestReservations = reservations
                .OrderByDescending(x => x.ReservationId)
                .Take(6)
                .ToList();

            ViewBag.LatestCustomers = customers
                .OrderByDescending(x => x.CustomerId)
                .Take(5)
                .ToList();

            return View(latestReservations);
        }
    }
}