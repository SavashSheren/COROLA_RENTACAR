using COROLA_RENTACAR.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactMessageController : Controller
    {
        private readonly IContactMessageService _contactMessageService;

        public ContactMessageController(IContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            var messages = await _contactMessageService.TGetAllAsync();

            var orderedMessages = messages
                .OrderBy(x => x.IsRead)
                .ThenByDescending(x => x.CreatedDate)
                .ToList();

            return View(orderedMessages);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var message = await _contactMessageService.TGetByIdAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _contactMessageService.TUpdateAsync(message);
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            var message = await _contactMessageService.TGetByIdAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            message.IsRead = false;
            await _contactMessageService.TUpdateAsync(message);

            return RedirectToAction("Inbox");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _contactMessageService.TGetByIdAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            await _contactMessageService.TDeleteAsync(id);

            return RedirectToAction("Inbox");
        }
    }
}