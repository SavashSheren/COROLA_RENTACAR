using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using COROLA_RENTACAR.WebUI.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using COROLA_RENTACAR.WebUI.Services;
namespace COROLA_RENTACAR.WebUI.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ICarService _carService;
        private readonly ICustomerService _customerService;
        private readonly ILocationService _locationService;
        private readonly IReservationService _reservationService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAiDriverLicenseVerificationService _aiDriverLicenseVerificationService;

        public ReservationController(
     ICarService carService,
     ICustomerService customerService,
     ILocationService locationService,
     IReservationService reservationService,
     IWebHostEnvironment webHostEnvironment,
     IAiDriverLicenseVerificationService aiDriverLicenseVerificationService)
        {
            _carService = carService;
            _customerService = customerService;
            _locationService = locationService;
            _reservationService = reservationService;
            _webHostEnvironment = webHostEnvironment;
            _aiDriverLicenseVerificationService = aiDriverLicenseVerificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int carId)
        {
            var car = await GetCarByIdAsync(carId);

            if (car == null || !car.IsAvailable)
            {
                return NotFound();
            }

            await LoadReservationPageDataAsync(car);

            return View(new PublicReservationViewModel
            {
                CarId = carId,
                PickupDate = DateTime.Today,
                ReturnDate = DateTime.Today.AddDays(1),
                BirthDate = DateTime.Today.AddYears(-25),
                DriverLicenseIssueDate = DateTime.Today.AddYears(-5),
                Description = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PublicReservationViewModel model)
        {
            var car = await GetCarByIdAsync(model.CarId);

            if (car == null || !car.IsAvailable)
            {
                return NotFound();
            }

            try
            {
                var hasReservationConflict = await _reservationService.THasReservationConflictAsync(
                    model.CarId,
                    model.PickupDate,
                    model.ReturnDate);

                if (hasReservationConflict)
                {
                    throw new Exception("This car is already reserved or pending approval for the selected date range. Please choose another date or another car.");
                }

                var systemVerificationMessage = await RunDriverLicenseSystemVerificationAsync(model);

                var savedLicenseImagePath = await SaveDriverLicenseImageAsync(model.DriverLicenseImage);
                model.DriverLicenseImageUrl = savedLicenseImagePath;

                var customer = await CreateOrUpdateCustomerAsync(model, systemVerificationMessage);

                var reservation = new Reservation
                {
                    CarId = model.CarId,
                    CustomerId = customer.CustomerId,
                    PickupDate = model.PickupDate,
                    ReturnDate = model.ReturnDate,
                    PickupLocationId = model.PickupLocationId,
                    ReturnLocationId = model.ReturnLocationId,
                    Description = model.Description ?? string.Empty,
                    ReservationStatus = ReservationStatus.Pending
                };

                await _reservationService.TCreatePublicReservationRequestAsync(reservation);

                return RedirectToAction(nameof(Success));
            }
            catch (ValidationException ex)
            {
                if (ex.Errors != null && ex.Errors.Any())
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

                await LoadReservationPageDataAsync(car);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadReservationPageDataAsync(car);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        private async Task<Car> GetCarByIdAsync(int carId)
        {
            var cars = await _carService.TGetAllCarsWithDetailsAsync();
            return cars.FirstOrDefault(x => x.CarId == carId);
        }

        private async Task LoadReservationPageDataAsync(Car car)
        {
            ViewBag.Car = car;
            ViewBag.Locations = await _locationService.TGetAllAsync();
        }

        private async Task<string> RunDriverLicenseSystemVerificationAsync(PublicReservationViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName))
                throw new Exception("First name is required.");

            if (string.IsNullOrWhiteSpace(model.LastName))
                throw new Exception("Last name is required.");

            if (string.IsNullOrWhiteSpace(model.Email))
                throw new Exception("Email is required.");

            if (string.IsNullOrWhiteSpace(model.Phone))
                throw new Exception("Phone is required.");

            if (string.IsNullOrWhiteSpace(model.DriverLicenseNumber))
                throw new Exception("Driver license number is required.");

            if (!model.DriverLicenseIssueDate.HasValue)
                throw new Exception("Driver license issue date is required.");

            if (model.DriverLicenseIssueDate.Value.Date > DateTime.Today)
                throw new Exception("Driver license issue date cannot be in the future.");

            if (model.BirthDate.Date > DateTime.Today.AddYears(-21))
                throw new Exception("Reservation cannot be created. Driver must be at least 21 years old.");

            if (model.DriverLicenseIssueDate.Value.Date > DateTime.Today.AddYears(-3))
                throw new Exception("Reservation cannot be created. Driver license must be at least 3 years old.");

            if (model.PickupLocationId <= 0)
                throw new Exception("Pickup location is required.");

            if (model.ReturnLocationId <= 0)
                throw new Exception("Return location is required.");

            if (model.ReturnDate.Date < model.PickupDate.Date)
                throw new Exception("Return date cannot be earlier than pickup date.");

            await ValidateDriverLicenseImageFileAsync(model.DriverLicenseImage);

            var aiResult = await _aiDriverLicenseVerificationService.VerifyAsync(model.DriverLicenseImage, model);

            if (!aiResult.IsPassed)
            {
                var reason = string.IsNullOrWhiteSpace(aiResult.RejectionReason)
                    ? "Uploaded driver license image could not pass AI verification."
                    : aiResult.RejectionReason;

                throw new Exception($"Reservation cannot be created. Driver license AI verification failed: {reason}");
            }

            return $"System verification passed. Basic checks passed. AI decision: {aiResult.Decision}. Risk: {aiResult.RiskLevel}. Confidence: {aiResult.Confidence}%. Detected text: {aiResult.DetectedTextSummary}";
        }

        private async Task ValidateDriverLicenseImageFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("Driver license image file is required.");
            }

            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new Exception("Driver license image file size can be at most 5 MB.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception("Driver license image must be JPG, JPEG, PNG or WEBP.");
            }

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                throw new Exception("Driver license file content type is not valid.");
            }

            var hasValidSignature = await HasValidImageSignatureAsync(file);

            if (!hasValidSignature)
            {
                throw new Exception("Driver license file is not a valid image.");
            }
        }

        private async Task<bool> HasValidImageSignatureAsync(IFormFile file)
        {
            var buffer = new byte[12];

            await using var stream = file.OpenReadStream();
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead < 4)
            {
                return false;
            }

            var isJpeg = buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF;

            var isPng =
                buffer[0] == 0x89 &&
                buffer[1] == 0x50 &&
                buffer[2] == 0x4E &&
                buffer[3] == 0x47;

            var isWebp =
                bytesRead >= 12 &&
                buffer[0] == 0x52 &&
                buffer[1] == 0x49 &&
                buffer[2] == 0x46 &&
                buffer[3] == 0x46 &&
                buffer[8] == 0x57 &&
                buffer[9] == 0x45 &&
                buffer[10] == 0x42 &&
                buffer[11] == 0x50;

            return isJpeg || isPng || isWebp;
        }

        private async Task<string> SaveDriverLicenseImageAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "uploads",
                "licenses");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"license_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/licenses/{fileName}";
        }

        private async Task<Customer> CreateOrUpdateCustomerAsync(PublicReservationViewModel model, string systemVerificationMessage)
        {
            var customers = await _customerService.TGetAllAsync();

            var customer = customers.FirstOrDefault(x =>
                x.Email.ToLower() == model.Email.ToLower());

            if (customer == null)
            {
                customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    BirthDate = model.BirthDate,
                    DriverLicenseNumber = model.DriverLicenseNumber,
                    DriverLicenseImageUrl = model.DriverLicenseImageUrl,
                    DriverLicenseIssueDate = model.DriverLicenseIssueDate,
                    IsDriverLicenseSystemVerified = true,
                    DriverLicenseSystemMessage = systemVerificationMessage,
                    DriverLicenseVerificationStatus = DriverLicenseVerificationStatus.Pending,
                    DriverLicenseVerifiedDate = null,
                    DriverLicenseRejectionReason = string.Empty
                };

                await _customerService.TInsertAsync(customer);
                return customer;
            }

            var licenseChanged =
                customer.DriverLicenseNumber != model.DriverLicenseNumber ||
                customer.DriverLicenseImageUrl != model.DriverLicenseImageUrl ||
                customer.DriverLicenseIssueDate != model.DriverLicenseIssueDate;

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Phone = model.Phone;
            customer.BirthDate = model.BirthDate;
            customer.DriverLicenseNumber = model.DriverLicenseNumber;
            customer.DriverLicenseImageUrl = model.DriverLicenseImageUrl;
            customer.DriverLicenseIssueDate = model.DriverLicenseIssueDate;
            customer.IsDriverLicenseSystemVerified = true;
            customer.DriverLicenseSystemMessage = systemVerificationMessage;

            if (licenseChanged || customer.DriverLicenseVerificationStatus == DriverLicenseVerificationStatus.Rejected)
            {
                customer.DriverLicenseVerificationStatus = DriverLicenseVerificationStatus.Pending;
                customer.DriverLicenseVerifiedDate = null;
                customer.DriverLicenseRejectionReason = string.Empty;
            }

            await _customerService.TUpdateAsync(customer);
            return customer;
        }
    }
}