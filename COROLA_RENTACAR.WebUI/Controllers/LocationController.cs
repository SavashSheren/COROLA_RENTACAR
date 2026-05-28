using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.WebUI.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task<IActionResult> LocationList()
        {
            var values = await _locationService.TGetAllAsync();
            return View(values);
        }
        public IActionResult CreateLocation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateLocation(Location location)
        {
            await _locationService.TInsertAsync(location);
            return RedirectToAction("LocationList"); 
        }

        public async Task<IActionResult> DeleteLocation(int id)
        {
            await _locationService.TDeleteAsync(id);
            return RedirectToAction("LocationList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateLocation(int id)
        {
            var value = await _locationService.TGetByIdAsync(id);
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation(Location location)
        {
            await _locationService.TUpdateAsync(location);
            return RedirectToAction("LocationList");
        }
    }
}
