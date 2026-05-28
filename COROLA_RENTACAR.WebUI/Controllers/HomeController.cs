using COROLA_RENTACAR.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarService _carService;

        public HomeController(ICarService carService)
        {
            _carService = carService;
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
            return View();
        }
    }
}