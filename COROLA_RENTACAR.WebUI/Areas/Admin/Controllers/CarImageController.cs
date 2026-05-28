using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarImageController : Controller
    {
        private readonly ICarImageService _carImageService;
        private readonly ICarService _carService;

        public CarImageController(ICarImageService carImageService, ICarService carService)
        {
            _carImageService = carImageService;
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> CarImageList()
        {
            var values = await _carImageService.TGetAllCarImagesWithCarAsync();
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCarImage()
        {
            await LoadCarsAsync();

            return View(new CarImage
            {
                IsCoverImage = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCarImage(CarImage carImage)
        {
            try
            {
                await _carImageService.TInsertAsync(carImage);
                return Redirect("/Admin/CarImage/CarImageList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadCarsAsync();
                return View(carImage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCarImage(int id)
        {
            await _carImageService.TDeleteAsync(id);
            return Redirect("/Admin/CarImage/CarImageList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCoverImage(int id)
        {
            await _carImageService.TSetCoverImageAsync(id);
            return Redirect("/Admin/CarImage/CarImageList");
        }

        private async Task LoadCarsAsync()
        {
            ViewBag.Cars = await _carService.TGetAllCarsWithDetailsAsync();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCarImage(int id)
        {
            var value = await _carImageService.TGetByIdAsync(id);

            if (value == null)
            {
                return NotFound();
            }

            await LoadCarsAsync();
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCarImage(CarImage carImage)
        {
            try
            {
                await _carImageService.TUpdateAsync(carImage);
                return Redirect("/Admin/CarImage/CarImageList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadCarsAsync();
                return View(carImage);
            }
        }
    }
}