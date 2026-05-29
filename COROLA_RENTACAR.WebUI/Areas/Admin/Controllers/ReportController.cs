using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using COROLA_RENTACAR.WebUI.Models;
using COROLA_RENTACAR.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ICarService _carService;
        private readonly ICustomerService _customerService;
        private readonly IPdfReportService _pdfReportService;

        public ReportController(
            IReservationService reservationService,
            ICarService carService,
            ICustomerService customerService,
            IPdfReportService pdfReportService)
        {
            _reservationService = reservationService;
            _carService = carService;
            _customerService = customerService;
            _pdfReportService = pdfReportService;
        }

        [HttpGet]
        public async Task<IActionResult> ReservationReports(ReservationReportFilterViewModel filter)
        {
            NormalizeReservationFilter(filter);

            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();
            var cars = await _carService.TGetAllCarsWithDetailsAsync();

            var filteredReservations = ApplyReservationReportFilters(reservations, filter)
                .OrderByDescending(x => x.ReservationId)
                .ToList();

            var approvedReservations = filteredReservations
                .Where(x => x.ReservationStatus == ReservationStatus.Approved)
                .ToList();

            ViewBag.Filter = filter;

            LoadReservationReportDropdowns(cars);

            ViewBag.StartDate = filter.StartDate!.Value;
            ViewBag.EndDate = filter.EndDate!.Value;

            ViewBag.TotalReservations = filteredReservations.Count;
            ViewBag.PendingReservations = filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Pending);
            ViewBag.ApprovedReservations = filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Approved);
            ViewBag.RejectedReservations = filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Rejected);
            ViewBag.CancelledReservations = filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Cancelled);

            ViewBag.TotalRevenue = approvedReservations.Sum(x => x.TotalPrice);

            ViewBag.AverageReservationValue = approvedReservations.Any()
                ? approvedReservations.Average(x => x.TotalPrice)
                : 0;

            ViewBag.TotalRentalDays = approvedReservations.Sum(x =>
            {
                var days = (x.ReturnDate.Date - x.PickupDate.Date).Days;
                return days < 1 ? 1 : days;
            });

            var culture = CultureInfo.GetCultureInfo("en-US");

            var groupedByDay = filteredReservations
                .GroupBy(x => x.PickupDate.Date)
                .OrderBy(x => x.Key)
                .ToList();

            ViewBag.DayLabels = groupedByDay
                .Select(x => x.Key.ToString("dd MMM", culture))
                .ToList();

            ViewBag.DailyReservationData = groupedByDay
                .Select(x => x.Count())
                .ToList();

            ViewBag.DailyRevenueData = groupedByDay
                .Select(x => x
                    .Where(r => r.ReservationStatus == ReservationStatus.Approved)
                    .Sum(r => r.TotalPrice))
                .ToList();

            ViewBag.StatusLabels = new List<string>
            {
                "Pending",
                "Approved",
                "Rejected",
                "Cancelled"
            };

            ViewBag.StatusData = new List<int>
            {
                ViewBag.PendingReservations,
                ViewBag.ApprovedReservations,
                ViewBag.RejectedReservations,
                ViewBag.CancelledReservations
            };

            return View(filteredReservations);
        }

        [HttpGet]
        public async Task<IActionResult> CarReports()
        {
            var cars = await _carService.TGetAllCarsWithDetailsAsync();
            return View(cars);
        }

        [HttpGet]
        public IActionResult PdfReports()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReservationPdf(ReservationReportFilterViewModel filter)
        {
            NormalizeReservationFilter(filter);

            var pdfBytes = await _pdfReportService.GenerateReservationReportAsync(filter);
            var fileName = $"reservation-report-{DateTime.Now:yyyyMMddHHmm}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadCarPdf()
        {
            var pdfBytes = await _pdfReportService.GenerateCarReportAsync();
            var fileName = $"car-report-{DateTime.Now:yyyyMMddHHmm}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadCustomerPdf()
        {
            var pdfBytes = await _pdfReportService.GenerateCustomerReportAsync();
            var fileName = $"customer-report-{DateTime.Now:yyyyMMddHHmm}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        private void NormalizeReservationFilter(ReservationReportFilterViewModel filter)
        {
            var today = DateTime.Today;

            filter.StartDate ??= today.AddMonths(-1);
            filter.EndDate ??= today;

            if (filter.EndDate.Value.Date < filter.StartDate.Value.Date)
            {
                filter.EndDate = filter.StartDate;
            }

            filter.CustomerKeyword = string.IsNullOrWhiteSpace(filter.CustomerKeyword)
                ? null
                : filter.CustomerKeyword.Trim();
        }

        private List<Reservation> ApplyReservationReportFilters(
            List<Reservation> reservations,
            ReservationReportFilterViewModel filter)
        {
            var query = reservations.AsQueryable();

            if (filter.StartDate.HasValue)
            {
                query = query.Where(x => x.PickupDate.Date >= filter.StartDate.Value.Date);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(x => x.PickupDate.Date <= filter.EndDate.Value.Date);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.ReservationStatus == filter.Status.Value);
            }

            if (filter.CarId.HasValue)
            {
                query = query.Where(x => x.CarId == filter.CarId.Value);
            }

            if (filter.BrandId.HasValue)
            {
                query = query.Where(x => x.Car != null && x.Car.BrandId == filter.BrandId.Value);
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(x => x.Car != null && x.Car.CategoryId == filter.CategoryId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(x => x.TotalPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(x => x.TotalPrice <= filter.MaxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CustomerKeyword))
            {
                var keyword = filter.CustomerKeyword.ToLower();

                query = query.Where(x =>
                    x.Customer != null &&
                    (
                        ((x.Customer.FirstName ?? "").ToLower().Contains(keyword)) ||
                        ((x.Customer.LastName ?? "").ToLower().Contains(keyword)) ||
                        ((x.Customer.Email ?? "").ToLower().Contains(keyword)) ||
                        ((x.Customer.Phone ?? "").ToLower().Contains(keyword))
                    ));
            }

            return query.ToList();
        }

        private void LoadReservationReportDropdowns(List<Car> cars)
        {
            ViewBag.Cars = cars
                .OrderBy(x => x.Brand != null ? x.Brand.BrandName : "")
                .ThenBy(x => x.Model)
                .ToList();

            ViewBag.Brands = cars
                .Where(x => x.Brand != null)
                .GroupBy(x => new { x.BrandId, x.Brand!.BrandName })
                .Select(x => new ReportDropdownItemViewModel
                {
                    Id = x.Key.BrandId,
                    Name = x.Key.BrandName
                })
                .OrderBy(x => x.Name)
                .ToList();

            ViewBag.Categories = cars
                .Where(x => x.Category != null)
                .GroupBy(x => new { x.CategoryId, x.Category!.CategoryName })
                .Select(x => new ReportDropdownItemViewModel
                {
                    Id = x.Key.CategoryId,
                    Name = x.Key.CategoryName
                })
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}