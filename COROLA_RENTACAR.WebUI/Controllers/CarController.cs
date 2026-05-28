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
            int? brandId,
            int? categoryId,
            string fuelType,
            string transmissionType,
            decimal? minPrice,
            decimal? maxPrice,
            bool availableOnly = true)
        {
            var cars = await _carService.TGetAllCarsWithDetailsAsync();

            if (availableOnly)
            {
                cars = cars.Where(x => x.IsAvailable).ToList();
            }

            if (brandId.HasValue && brandId.Value > 0)
            {
                cars = cars.Where(x => x.BrandId == brandId.Value).ToList();
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                cars = cars.Where(x => x.CategoryId == categoryId.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(fuelType))
            {
                cars = cars.Where(x => x.FuelType == fuelType).ToList();
            }

            if (!string.IsNullOrWhiteSpace(transmissionType))
            {
                cars = cars.Where(x => x.TransmissionType == transmissionType).ToList();
            }

            if (minPrice.HasValue)
            {
                cars = cars.Where(x => x.DailyPrice >= minPrice.Value).ToList();
            }

            if (maxPrice.HasValue)
            {
                cars = cars.Where(x => x.DailyPrice <= maxPrice.Value).ToList();
            }

            ViewBag.Brands = await _brandService.TGetAllAsync();
            ViewBag.Categories = await _categoryService.TGetAllAsync();

            ViewBag.SelectedBrandId = brandId;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedFuelType = fuelType;
            ViewBag.SelectedTransmissionType = transmissionType;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.AvailableOnly = availableOnly;

            return View(cars);
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