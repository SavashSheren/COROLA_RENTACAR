using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Controllers
{
    public class TestBrandController : Controller
    {
        private readonly IBrandService _brandService;

        public TestBrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public IActionResult CreateBrand()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {
            if (!ModelState.IsValid)
                return View(brand); // Hataları view'a geri döndür

            try
            {
                await _brandService.TInsertAsync(brand);
                return RedirectToAction("Index");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(brand);

            }
        }
    }
}
