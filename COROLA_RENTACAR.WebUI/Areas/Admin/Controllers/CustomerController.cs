using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace COROLA_RENTACAR.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> CustomerList()
        {
            var values = await _customerService.TGetAllAsync();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View(new Customer
            {
                BirthDate = DateTime.Today.AddYears(-25),
                DriverLicenseVerificationStatus = DriverLicenseVerificationStatus.Pending,
                DriverLicenseImageUrl = string.Empty,
                DriverLicenseRejectionReason = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            try
            {
                await _customerService.TInsertAsync(customer);
                return Redirect("/Admin/Customer/CustomerList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(customer);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomer(int id)
        {
            var value = await _customerService.TGetByIdAsync(id);

            if (value == null)
            {
                return NotFound();
            }

            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            try
            {
                await _customerService.TUpdateAsync(customer);
                return Redirect("/Admin/Customer/CustomerList");
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(customer);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customerService.TDeleteAsync(id);
            return Redirect("/Admin/Customer/CustomerList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDriverLicense(int id)
        {
            try
            {
                await _customerService.TApproveDriverLicenseAsync(id);
            }
            catch (ValidationException)
            {
                TempData["CustomerError"] = "Driver license image is required before approval.";
            }

            return Redirect("/Admin/Customer/CustomerList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectDriverLicense(int id, string rejectionReason)
        {
            await _customerService.TRejectDriverLicenseAsync(id, rejectionReason);
            return Redirect("/Admin/Customer/CustomerList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDriverLicensePending(int id)
        {
            await _customerService.TMarkDriverLicensePendingAsync(id);
            return Redirect("/Admin/Customer/CustomerList");
        }
    }
}