using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ICarService _carService;
        private readonly ICustomerService _customerService;
        private readonly ILocationService _locationService;

        public ReservationController(
            IReservationService reservationService,
            ICarService carService,
            ICustomerService customerService,
            ILocationService locationService)
        {
            _reservationService = reservationService;
            _carService = carService;
            _customerService = customerService;
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> ReservationList()
        {
            var values = await _reservationService.TGetAllReservationsWithDetailsAsync();
            return View(values);
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
            await _reservationService.TApproveReservationAsync(id);
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
    }
}