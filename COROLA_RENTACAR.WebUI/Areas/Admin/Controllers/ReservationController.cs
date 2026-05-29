using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using COROLA_RENTACAR.WebUI.Services;


namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ICarService _carService;
        private readonly ICustomerService _customerService;
        private readonly ILocationService _locationService;
        private readonly IEmailNotificationService _emailNotificationService;

        public ReservationController(
      IReservationService reservationService,
      ICarService carService,
      ICustomerService customerService,
      ILocationService locationService,
      IEmailNotificationService emailNotificationService)
        {
            _reservationService = reservationService;
            _carService = carService;
            _customerService = customerService;
            _locationService = locationService;
            _emailNotificationService = emailNotificationService;
        }

        [HttpGet]
        public async Task<IActionResult> ReservationList()
        {
            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            LoadReservationStats(reservations);

            ViewBag.PageTitle = "Reservation Management";
            ViewBag.PageDescription = "Manage all reservation requests and rental statuses.";

            return View(reservations.OrderByDescending(x => x.ReservationId).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> CreateReservation()
        {
            await LoadReservationDropdownsAsync();

            return View(new Reservation
            {
                PickupDate = DateTime.Today,
                ReturnDate = DateTime.Today.AddDays(1),
                ReservationStatus = ReservationStatus.Pending,
                Description = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReservation(Reservation reservation)
        {
            try
            {
                await _reservationService.TInsertAsync(reservation);
                return Redirect("/Admin/Reservation/ReservationList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                await LoadReservationDropdownsAsync();
                return View(reservation);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateReservation(int id)
        {
            var value = await _reservationService.TGetByIdAsync(id);

            if (value == null)
            {
                return NotFound();
            }

            await LoadReservationDropdownsAsync();
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReservation(Reservation reservation)
        {
            try
            {
                await _reservationService.TUpdateAsync(reservation);
                return Redirect("/Admin/Reservation/ReservationList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                await LoadReservationDropdownsAsync();
                return View(reservation);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            await _reservationService.TDeleteAsync(id);
            return Redirect("/Admin/Reservation/ReservationList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
   
        public async Task<IActionResult> ApproveReservation(int id)
        {
            try
            {
                await _reservationService.TApproveReservationAsync(id);

                var reservation = await _reservationService.TGetReservationWithDetailsByIdAsync(id);

                if (reservation != null)
                {
                    await _emailNotificationService.SendReservationApprovedEmailAsync(reservation);
                    TempData["ReservationSuccess"] = "Reservation approved and approval email has been sent to the customer.";
                }
            }
            catch (Exception ex)
            {
                TempData["ReservationError"] = ex.Message;
            }

            return Redirect("/Admin/Reservation/ReservationList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReservation(int id)
        {
            await _reservationService.TRejectReservationAsync(id);
            return Redirect("/Admin/Reservation/ReservationList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            await _reservationService.TCancelReservationAsync(id);
            return Redirect("/Admin/Reservation/ReservationList");
        }

        private async Task LoadReservationDropdownsAsync()
        {
            ViewBag.Cars = await _carService.TGetAllCarsWithDetailsAsync();
            ViewBag.Customers = await _customerService.TGetAllAsync();
            ViewBag.Locations = await _locationService.TGetAllAsync();
            ViewBag.Statuses = Enum.GetValues(typeof(ReservationStatus)).Cast<ReservationStatus>().ToList();
        }
        [HttpGet]
        public async Task<IActionResult> PendingReservations()
        {
            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            var filteredReservations = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Pending)
                .OrderByDescending(x => x.ReservationId)
                .ToList();

            LoadReservationStats(reservations);

            ViewBag.PageTitle = "Pending Reservations";
            ViewBag.PageDescription = "Reservations waiting for admin approval.";

            return View("ReservationList", filteredReservations);
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedReservations()
        {
            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            var filteredReservations = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Approved)
                .OrderByDescending(x => x.ReservationId)
                .ToList();

            LoadReservationStats(reservations);

            ViewBag.PageTitle = "Approved Reservations";
            ViewBag.PageDescription = "Confirmed and approved rental reservations.";

            return View("ReservationList", filteredReservations);
        }

        [HttpGet]
        public async Task<IActionResult> RejectedReservations()
        {
            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            var filteredReservations = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Rejected)
                .OrderByDescending(x => x.ReservationId)
                .ToList();

            LoadReservationStats(reservations);

            ViewBag.PageTitle = "Rejected Reservations";
            ViewBag.PageDescription = "Reservations rejected by admin.";

            return View("ReservationList", filteredReservations);
        }

        private void LoadReservationStats(List<COROLA_RENTACAR.EntityLayer.Entities.Reservation> reservations)
        {
            ViewBag.TotalReservations = reservations.Count;
            ViewBag.PendingReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Pending);
            ViewBag.ApprovedReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Approved);
            ViewBag.RejectedReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Rejected);
            ViewBag.CancelledReservations = reservations.Count(x => x.ReservationStatus == ReservationStatus.Cancelled);
        }
        [HttpGet]
        public async Task<IActionResult> CancelledReservations()
        {
            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            var filteredReservations = reservations
                .Where(x => x.ReservationStatus == ReservationStatus.Cancelled)
                .OrderByDescending(x => x.ReservationId)
                .ToList();

            LoadReservationStats(reservations);

            ViewBag.PageTitle = "Cancelled Reservations";
            ViewBag.PageDescription = "Reservations cancelled by admin.";

            return View("ReservationList", filteredReservations);
        }
    }
}