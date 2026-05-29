using COROLA_RENTACAR.WebUI.Areas.Admin.Models;
using COROLA_RENTACAR.WebUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminAiToolsController : Controller
    {
        private readonly IAiCarDescriptionService _aiCarDescriptionService;

        public AdminAiToolsController(IAiCarDescriptionService aiCarDescriptionService)
        {
            _aiCarDescriptionService = aiCarDescriptionService;
        }

        [HttpGet]
        public IActionResult CarDescription()
        {
            return View(new AiCarDescriptionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CarDescription(AiCarDescriptionViewModel model)
        {
            model.GeneratedDescription = await _aiCarDescriptionService.GenerateDescriptionAsync(
                model.Brand,
                model.Model,
                model.Category,
                model.FuelType,
                model.Transmission,
                model.SeatCount,
                model.DailyPrice,
                model.Features);

            return View(model);
        }
    }
}