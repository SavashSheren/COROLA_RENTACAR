using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc;

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

            ViewBag.TotalRevenue = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Approved)
                .Sum(x => x.TotalPrice);

            ViewBag.MonthlyRevenue = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Approved
                            && x.PickupDate.Month == DateTime.Now.Month
                            && x.PickupDate.Year == DateTime.Now.Year)
                .Sum(x => x.TotalPrice);

            ViewBag.LatestCustomers = customers
                .OrderByDescending(x => x.CustomerId)
                .Take(5)
                .ToList();

            var latestReservations = reservations
                .OrderByDescending(x => x.ReservationId)
                .Take(6)
                .ToList();

            return View(latestReservations);
        }
    }
}