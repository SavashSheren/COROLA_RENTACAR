using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Controllers
{
    public class AdminLayout : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
