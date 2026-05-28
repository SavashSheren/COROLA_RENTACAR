using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;

        public CarController(
            ICarService carService,
            IBrandService brandService,
            ICategoryService categoryService)
        {
            _carService = carService;
            _brandService = brandService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> CarList()
        {
            var values = await _carService.TGetAllCarsWithDetailsAsync();
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCar()
        {
            await LoadCarDropdownsAsync();

            return View(new Car
            {
                IsAvailable = true,
                ModelYear = DateTime.Now.Year,
                SeatCount = 5,
                LuggageCapacity = 2,
                Mileage = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCar(Car car)
        {
            try
            {
                await _carService.TInsertAsync(car);
                return Redirect("/Admin/Car/CarList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadCarDropdownsAsync();
                return View(car);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCar(int id)
        {
            var value = await _carService.TGetByIdAsync(id);

            if (value == null)
            {
                return NotFound();
            }

            await LoadCarDropdownsAsync();
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCar(Car car)
        {
            try
            {
                await _carService.TUpdateAsync(car);
                return Redirect("/Admin/Car/CarList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadCarDropdownsAsync();
                return View(car);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int id)
        {
            await _carService.TDeleteAsync(id);
            return Redirect("/Admin/Car/CarList");
        }

        private async Task LoadCarDropdownsAsync()
        {
            ViewBag.Brands = await _brandService.TGetAllAsync();
            ViewBag.Categories = await _categoryService.TGetAllAsync();
        }
    }
}