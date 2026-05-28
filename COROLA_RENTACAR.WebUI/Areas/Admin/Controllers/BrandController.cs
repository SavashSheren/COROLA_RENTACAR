using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> BrandList()
        {
            var values = await _brandService.TGetAllAsync();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateBrand()
        {
            var brand = new Brand
            {
                Status = true
            };

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {
            try
            {
                await _brandService.TInsertAsync(brand);
                return Redirect("/Admin/Brand/BrandList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(brand);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBrand(int id)
        {
            var value = await _brandService.TGetByIdAsync(id);

            if (value == null)
            {
                return NotFound();
            }

            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBrand(Brand brand)
        {
            try
            {
                await _brandService.TUpdateAsync(brand);
                return Redirect("/Admin/Brand/BrandList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(brand);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _brandService.TDeleteAsync(id);
            return Redirect("/Admin/Brand/BrandList");
        }
    }
}