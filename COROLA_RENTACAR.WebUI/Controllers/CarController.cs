using COROLA_RENTACAR.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly ICarImageService _carImageService;

        public CarController(
            ICarService carService,
            IBrandService brandService,
            ICategoryService categoryService,
            ICarImageService carImageService)
        {
            _carService = carService;
            _brandService = brandService;
            _categoryService = categoryService;
            _carImageService = carImageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm,
            int? brandId,
            int? categoryId,
            string? fuelType,
            string? transmissionType,
            decimal? minPrice,
            decimal? maxPrice,
            int? seatCount,
            int? luggageCapacity,
            bool availableOnly = true)
        {
            var allCars = await _carService.TGetAllCarsWithDetailsAsync();

            var cars = allCars.AsEnumerable();

            if (availableOnly)
            {
                cars = cars.Where(x => x.IsAvailable);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = searchTerm.Trim();

                cars = cars.Where(x =>
                    (!string.IsNullOrWhiteSpace(x.Model) &&
                     x.Model.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||

                    (x.Brand != null &&
                     !string.IsNullOrWhiteSpace(x.Brand.BrandName) &&
                     x.Brand.BrandName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||

                    (x.Category != null &&
                     !string.IsNullOrWhiteSpace(x.Category.CategoryName) &&
                     x.Category.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||

                    (!string.IsNullOrWhiteSpace(x.FuelType) &&
                     x.FuelType.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||

                    (!string.IsNullOrWhiteSpace(x.TransmissionType) &&
                     x.TransmissionType.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (brandId.HasValue && brandId.Value > 0)
            {
                cars = cars.Where(x => x.BrandId == brandId.Value);
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                cars = cars.Where(x => x.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(fuelType))
            {
                cars = cars.Where(x =>
                    !string.IsNullOrWhiteSpace(x.FuelType) &&
                    x.FuelType.Equals(fuelType.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(transmissionType))
            {
                cars = cars.Where(x =>
                    !string.IsNullOrWhiteSpace(x.TransmissionType) &&
                    x.TransmissionType.Equals(transmissionType.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (minPrice.HasValue && minPrice.Value > 0)
            {
                cars = cars.Where(x => x.DailyPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                cars = cars.Where(x => x.DailyPrice <= maxPrice.Value);
            }

            if (seatCount.HasValue && seatCount.Value > 0)
            {
                cars = cars.Where(x => x.SeatCount >= seatCount.Value);
            }

            if (luggageCapacity.HasValue && luggageCapacity.Value > 0)
            {
                cars = cars.Where(x => x.LuggageCapacity >= luggageCapacity.Value);
            }

            var filteredCars = cars
                .OrderByDescending(x => x.IsAvailable)
                .ThenByDescending(x => x.CarId)
                .ToList();

            ViewBag.Brands = (await _brandService.TGetAllAsync())
                .Where(x => x.Status)
                .OrderBy(x => x.BrandName)
                .ToList();

            ViewBag.Categories = (await _categoryService.TGetAllAsync())
                .OrderBy(x => x.CategoryName)
                .ToList();

            ViewBag.FuelTypes = allCars
                .Where(x => !string.IsNullOrWhiteSpace(x.FuelType))
                .Select(x => x.FuelType.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            ViewBag.TransmissionTypes = allCars
                .Where(x => !string.IsNullOrWhiteSpace(x.TransmissionType))
                .Select(x => x.TransmissionType.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            ViewBag.TotalCarCount = allCars.Count;
            ViewBag.FilteredCarCount = filteredCars.Count;

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedBrandId = brandId;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedFuelType = fuelType;
            ViewBag.SelectedTransmissionType = transmissionType;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SeatCount = seatCount;
            ViewBag.LuggageCapacity = luggageCapacity;
            ViewBag.AvailableOnly = availableOnly;

            return View(filteredCars);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var cars = await _carService.TGetAllCarsWithDetailsAsync();
            var car = cars.FirstOrDefault(x => x.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.CarImages = await _carImageService.TGetCarImagesByCarIdAsync(id);

            return View(car);
        }
    }
}