using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.WebUI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using System.Globalization;
using System.Net;

namespace COROLA_RENTACAR.WebUI.Services
{
    public class MailKitEmailNotificationService : IEmailNotificationService
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MailKitEmailNotificationService(
            IOptions<MailSettings> mailSettings,
            IWebHostEnvironment webHostEnvironment)
        {
            _mailSettings = mailSettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendReservationApprovedEmailAsync(Reservation reservation)
        {
            if (reservation.Customer == null || string.IsNullOrWhiteSpace(reservation.Customer.Email))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_mailSettings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_mailSettings.SenderPassword))
            {
                throw new Exception("Mail settings are not configured. Sender email or password is missing.");
            }

            var culture = CultureInfo.GetCultureInfo("en-US");

            var customerFullName = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}";
            var carName = $"{reservation.Car?.Brand?.BrandName} {reservation.Car?.Model}";
            var pickupLocation = reservation.PickupLocation?.LocationName ?? "-";
            var returnLocation = reservation.ReturnLocation?.LocationName ?? "-";

            var pickupDateText = reservation.PickupDate.ToString("dd MMM yyyy", culture);
            var returnDateText = reservation.ReturnDate.ToString("dd MMM yyyy", culture);

            var reservationCode = $"CRL-{DateTime.Now.Year}-{reservation.ReservationId.ToString("D6")}";

            var rentalDays = (reservation.ReturnDate.Date - reservation.PickupDate.Date).Days;

            if (rentalDays < 1)
            {
                rentalDays = 1;
            }

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(customerFullName, reservation.Customer.Email));
            message.Subject = $"Your Reservation Has Been Approved - {reservationCode}";

            var bodyBuilder = new BodyBuilder();

            var carImageHtml = BuildCarImageHtml(reservation, bodyBuilder);

            bodyBuilder.HtmlBody = $@"
                <div style='font-family:Arial,sans-serif;background:#f4f4f4;padding:30px;'>
                    <div style='max-width:680px;margin:0 auto;background:#ffffff;border-radius:16px;overflow:hidden;border:1px solid #eeeeee;'>

                        <div style='background:#111111;color:#ffffff;padding:28px 32px;'>
                            <h2 style='margin:0;font-size:24px;'>COROLA Rent A Car</h2>
                            <p style='margin:8px 0 0;color:#cccccc;font-size:14px;'>Reservation Approved</p>
                        </div>

                        <div style='padding:32px;'>

                            <h3 style='margin-top:0;color:#111111;font-size:22px;'>
                                Hello {customerFullName},
                            </h3>

                            <p style='color:#555555;line-height:1.8;font-size:15px;'>
                                Your reservation request has been approved by our admin team.
                                Your rental process is now confirmed.
                            </p>

                            <div style='background:#fff7e6;border:1px solid #ffd98a;border-radius:12px;padding:18px;margin:24px 0;'>
                                <p style='margin:0;color:#111111;font-size:15px;'>
                                    <strong>Reservation Code:</strong>
                                    <span style='font-size:18px;color:#d97706;font-weight:bold;'>{reservationCode}</span>
                                </p>
                            </div>

                            {carImageHtml}

                            <div style='background:#f9f9f9;border:1px solid #eeeeee;border-radius:12px;padding:22px;margin:24px 0;'>

                                <h4 style='margin-top:0;margin-bottom:16px;color:#111111;'>Reservation Details</h4>

                                <table style='width:100%;border-collapse:collapse;font-size:14px;color:#333333;'>
                                    <tr>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;'><strong>Customer Name</strong></td>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;text-align:right;'>{customerFullName}</td>
                                    </tr>

                                    <tr>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;'><strong>Car</strong></td>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;text-align:right;'>{carName}</td>
                                    </tr>

                                    <tr>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;'><strong>Pickup Date</strong></td>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;text-align:right;'>{pickupDateText}</td>
                                    </tr>

                                    <tr>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;'><strong>Return Date</strong></td>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;text-align:right;'>{returnDateText}</td>
                                    </tr>

                                    <tr>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;'><strong>Rental Days</strong></td>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;text-align:right;'>{rentalDays} day(s)</td>
                                    </tr>

                                    <tr>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;'><strong>Pickup Location</strong></td>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;text-align:right;'>{pickupLocation}</td>
                                    </tr>

                                    <tr>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;'><strong>Return Location</strong></td>
                                        <td style='padding:10px 0;border-bottom:1px solid #eeeeee;text-align:right;'>{returnLocation}</td>
                                    </tr>

                                    <tr>
                                        <td style='padding:14px 0 0;'><strong>Total Price</strong></td>
                                        <td style='padding:14px 0 0;text-align:right;font-size:18px;color:#d97706;font-weight:bold;'>{reservation.TotalPrice:N0} ₺</td>
                                    </tr>
                                </table>
                            </div>

                            <p style='color:#555555;line-height:1.8;font-size:15px;'>
                                Please keep your reservation code with you during vehicle pickup.
                                You may be asked to show your driver license and personal information.
                            </p>

                            <p style='margin-top:30px;color:#111111;font-size:15px;'>
                                Best regards,<br>
                                <strong>COROLA Rent A Car Team</strong>
                            </p>
                        </div>
                    </div>
                </div>";

            message.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                _mailSettings.SmtpHost,
                _mailSettings.SmtpPort,
                SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(
                _mailSettings.SenderEmail,
                _mailSettings.SenderPassword);

            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }

        private string BuildCarImageHtml(Reservation reservation, BodyBuilder bodyBuilder)
        {
            var imageUrl = reservation.Car?.ImageUrl;

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return string.Empty;
            }

            if (imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var safeImageUrl = WebUtility.HtmlEncode(imageUrl);

                return $@"
                    <div style='margin:24px 0;border-radius:14px;overflow:hidden;border:1px solid #eeeeee;'>
                        <img src='{safeImageUrl}' alt='Rental car image' style='width:100%;max-height:280px;object-fit:cover;display:block;'>
                    </div>";
            }

            if (imageUrl.StartsWith("/"))
            {
                var relativePath = imageUrl
                    .TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar);

                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    var linkedImage = bodyBuilder.LinkedResources.Add(filePath);
                    linkedImage.ContentId = MimeUtils.GenerateMessageId();

                    return $@"
                        <div style='margin:24px 0;border-radius:14px;overflow:hidden;border:1px solid #eeeeee;'>
                            <img src='cid:{linkedImage.ContentId}' alt='Rental car image' style='width:100%;max-height:280px;object-fit:cover;display:block;'>
                        </div>";
                }
            }

            return string.Empty;
        }
    }
}