using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarService _carService;
        private readonly IContactMessageService _contactMessageService;

        public HomeController(
            ICarService carService,
            IContactMessageService contactMessageService)
        {
            _carService = carService;
            _contactMessageService = contactMessageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cars = await _carService.TGetAllCarsWithDetailsAsync();

            var featuredCars = cars
                .Where(x => x.IsAvailable)
                .OrderByDescending(x => x.CarId)
                .Take(6)
                .ToList();

            return View(featuredCars);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new PublicContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(PublicContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contactMessage = new ContactMessage
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
                Subject = model.Subject.Trim(),
                Message = model.Message.Trim(),
                CreatedDate = DateTime.Now,
                IsRead = false
            };

            await _contactMessageService.TCreateAsync(contactMessage);

            TempData["ContactSuccessMessage"] = "Your message has been sent successfully. Our team will review it shortly.";

            return RedirectToAction(nameof(Contact));
        }
    }
}