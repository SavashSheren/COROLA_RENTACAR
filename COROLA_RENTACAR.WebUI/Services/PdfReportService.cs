using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using COROLA_RENTACAR.WebUI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace COROLA_RENTACAR.WebUI.Services
{
    public class PdfReportService : IPdfReportService
    {
        private readonly IReservationService _reservationService;
        private readonly ICarService _carService;
        private readonly ICustomerService _customerService;

        public PdfReportService(
            IReservationService reservationService,
            ICarService carService,
            ICustomerService customerService)
        {
            _reservationService = reservationService;
            _carService = carService;
            _customerService = customerService;
        }

        public async Task<byte[]> GenerateReservationReportAsync(ReservationReportFilterViewModel filter)
        {
            NormalizeReservationFilter(filter);

            var reservations = await _reservationService.TGetAllReservationsWithDetailsAsync();

            var filteredReservations = ApplyReservationReportFilters(reservations, filter)
                .OrderByDescending(x => x.ReservationId)
                .ToList();

            var approvedReservations = filteredReservations
                .Where(x => x.ReservationStatus == ReservationStatus.Approved)
                .ToList();

            var totalRevenue = approvedReservations.Sum(x => x.TotalPrice);

            var averageValue = approvedReservations.Any()
                ? approvedReservations.Average(x => x.TotalPrice)
                : 0;

            var totalRentalDays = approvedReservations.Sum(x =>
            {
                var days = (x.ReturnDate.Date - x.PickupDate.Date).Days;
                return days < 1 ? 1 : days;
            });

            var filterSummary = BuildReservationFilterSummary(filter);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A3.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(8));

                    page.Header().Element(header =>
                    {
                        ComposeHeader(
                            header,
                            "Filtered Reservation Operation Report",
                            filterSummary);
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(14);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Element(c => ComposeMetric(c, "Total Requests", filteredReservations.Count.ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Approved Revenue", $"{totalRevenue:N0} TL"));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Average Value", $"{averageValue:N0} TL"));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Rental Days", totalRentalDays.ToString()));
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Element(c => ComposeMetric(c, "Pending", filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Pending).ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Approved", filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Approved).ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Rejected", filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Rejected).ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Cancelled", filteredReservations.Count(x => x.ReservationStatus == ReservationStatus.Cancelled).ToString()));
                        });

                        column.Item().Element(container =>
                        {
                            container
                                .Background(Colors.Grey.Lighten4)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(10)
                                .Column(inner =>
                                {
                                    inner.Item().Text("Applied Filters").Bold().FontSize(11);
                                    inner.Item().Text(filterSummary).FontSize(8).FontColor(Colors.Grey.Darken2);
                                });
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.RelativeColumn(1.15f);
                                columns.RelativeColumn(1.25f);
                                columns.RelativeColumn(.9f);
                                columns.RelativeColumn(.9f);
                                columns.RelativeColumn(.9f);
                                columns.RelativeColumn(.9f);
                                columns.RelativeColumn(.85f);
                                columns.RelativeColumn(.85f);
                                columns.RelativeColumn(.55f);
                                columns.RelativeColumn(.85f);
                                columns.RelativeColumn(.85f);
                                columns.RelativeColumn(1.25f);
                                columns.RelativeColumn(.9f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(TableHeaderStyle).Text("Code");
                                header.Cell().Element(TableHeaderStyle).Text("Customer");
                                header.Cell().Element(TableHeaderStyle).Text("Car");
                                header.Cell().Element(TableHeaderStyle).Text("Plate");
                                header.Cell().Element(TableHeaderStyle).Text("Category");
                                header.Cell().Element(TableHeaderStyle).Text("Fuel");
                                header.Cell().Element(TableHeaderStyle).Text("Transmission");
                                header.Cell().Element(TableHeaderStyle).Text("Pickup");
                                header.Cell().Element(TableHeaderStyle).Text("Return");
                                header.Cell().Element(TableHeaderStyle).Text("Days");
                                header.Cell().Element(TableHeaderStyle).Text("Status");
                                header.Cell().Element(TableHeaderStyle).Text("Price");
                                header.Cell().Element(TableHeaderStyle).Text("Email");
                                header.Cell().Element(TableHeaderStyle).Text("Phone");
                            });

                            foreach (var item in filteredReservations)
                            {
                                var customerName = $"{item.Customer?.FirstName} {item.Customer?.LastName}";
                                var carName = $"{item.Car?.Brand?.BrandName} {item.Car?.Model}";
                                var reservationCode = $"CRL-{DateTime.Now.Year}-{item.ReservationId:D6}";

                                var rentalDays = (item.ReturnDate.Date - item.PickupDate.Date).Days;

                                if (rentalDays < 1)
                                {
                                    rentalDays = 1;
                                }

                                table.Cell().Element(TableCellStyle).Text(reservationCode);
                                table.Cell().Element(TableCellStyle).Text(customerName);
                                table.Cell().Element(TableCellStyle).Text(carName);
                                table.Cell().Element(TableCellStyle).Text(item.Car?.PlateNumber ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.Car?.Category?.CategoryName ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.Car?.FuelType ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.Car?.TransmissionType ?? "-");
                                table.Cell().Element(TableCellStyle).Text(FormatDate(item.PickupDate));
                                table.Cell().Element(TableCellStyle).Text(FormatDate(item.ReturnDate));
                                table.Cell().Element(TableCellStyle).Text(rentalDays.ToString());
                                table.Cell().Element(TableCellStyle).Text(item.ReservationStatus.ToString());
                                table.Cell().Element(TableCellStyle).Text($"{item.TotalPrice:N0} TL");
                                table.Cell().Element(TableCellStyle).Text(item.Customer?.Email ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.Customer?.Phone ?? "-");
                            }
                        });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        public async Task<byte[]> GenerateCarReportAsync()
        {
            var cars = await _carService.TGetAllCarsWithDetailsAsync();

            var totalCars = cars.Count;
            var availableCars = cars.Count(x => x.IsAvailable);
            var unavailableCars = cars.Count(x => !x.IsAvailable);

            var averageDailyPrice = cars.Any()
                ? cars.Average(x => x.DailyPrice)
                : 0;

            var highestPriceCar = cars
                .OrderByDescending(x => x.DailyPrice)
                .FirstOrDefault();

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Element(header =>
                    {
                        ComposeHeader(
                            header,
                            "Car Report",
                            "Vehicle inventory, availability and pricing report");
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(14);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Element(c => ComposeMetric(c, "Total Cars", totalCars.ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Available", availableCars.ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Unavailable", unavailableCars.ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Avg Daily Price", $"{averageDailyPrice:N0} TL"));
                        });

                        if (highestPriceCar != null)
                        {
                            var highestCarName = $"{highestPriceCar.Brand?.BrandName} {highestPriceCar.Model}";

                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Grey.Lighten4)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(10)
                                    .Column(inner =>
                                    {
                                        inner.Item().Text("Highest Priced Car").Bold().FontSize(11);
                                        inner.Item().Text($"{highestCarName} - {highestPriceCar.DailyPrice:N0} TL / day");
                                    });
                            });
                        }

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(45);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1f);
                                columns.RelativeColumn(1f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(TableHeaderStyle).Text("#");
                                header.Cell().Element(TableHeaderStyle).Text("Car");
                                header.Cell().Element(TableHeaderStyle).Text("Plate");
                                header.Cell().Element(TableHeaderStyle).Text("Category");
                                header.Cell().Element(TableHeaderStyle).Text("Fuel");
                                header.Cell().Element(TableHeaderStyle).Text("Transmission");
                                header.Cell().Element(TableHeaderStyle).Text("Status");
                                header.Cell().Element(TableHeaderStyle).Text("Daily Price");
                            });

                            foreach (var item in cars.OrderByDescending(x => x.CarId))
                            {
                                var carName = $"{item.Brand?.BrandName} {item.Model}";
                                var status = item.IsAvailable ? "Available" : "Unavailable";

                                table.Cell().Element(TableCellStyle).Text($"#{item.CarId}");
                                table.Cell().Element(TableCellStyle).Text(carName);
                                table.Cell().Element(TableCellStyle).Text(item.PlateNumber ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.Category?.CategoryName ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.FuelType ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.TransmissionType ?? "-");
                                table.Cell().Element(TableCellStyle).Text(status);
                                table.Cell().Element(TableCellStyle).Text($"{item.DailyPrice:N0} TL");
                            }
                        });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        public async Task<byte[]> GenerateCustomerReportAsync()
        {
            var customers = await _customerService.TGetAllAsync();

            var totalCustomers = customers.Count;
            var pendingLicenses = customers.Count(x => x.DriverLicenseVerificationStatus == DriverLicenseVerificationStatus.Pending);
            var approvedLicenses = customers.Count(x => x.DriverLicenseVerificationStatus == DriverLicenseVerificationStatus.Approved);
            var rejectedLicenses = customers.Count(x => x.DriverLicenseVerificationStatus == DriverLicenseVerificationStatus.Rejected);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Element(header =>
                    {
                        ComposeHeader(
                            header,
                            "Customer Report",
                            "Customer and driver license verification report");
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(14);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Element(c => ComposeMetric(c, "Total Customers", totalCustomers.ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Pending Licenses", pendingLicenses.ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Approved Licenses", approvedLicenses.ToString()));
                            row.RelativeItem().Element(c => ComposeMetric(c, "Rejected Licenses", rejectedLicenses.ToString()));
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(45);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.6f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.3f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.2f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(TableHeaderStyle).Text("#");
                                header.Cell().Element(TableHeaderStyle).Text("Customer");
                                header.Cell().Element(TableHeaderStyle).Text("Email");
                                header.Cell().Element(TableHeaderStyle).Text("Phone");
                                header.Cell().Element(TableHeaderStyle).Text("License No");
                                header.Cell().Element(TableHeaderStyle).Text("Birth Date");
                                header.Cell().Element(TableHeaderStyle).Text("License Status");
                            });

                            foreach (var item in customers.OrderByDescending(x => x.CustomerId))
                            {
                                var fullName = $"{item.FirstName} {item.LastName}";

                                table.Cell().Element(TableCellStyle).Text($"#{item.CustomerId}");
                                table.Cell().Element(TableCellStyle).Text(fullName);
                                table.Cell().Element(TableCellStyle).Text(item.Email ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.Phone ?? "-");
                                table.Cell().Element(TableCellStyle).Text(item.DriverLicenseNumber ?? "-");
                                table.Cell().Element(TableCellStyle).Text(FormatDate(item.BirthDate));
                                table.Cell().Element(TableCellStyle).Text(item.DriverLicenseVerificationStatus.ToString());
                            }
                        });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        private static void NormalizeReservationFilter(ReservationReportFilterViewModel filter)
        {
            var today = DateTime.Today;

            filter.StartDate ??= today.AddMonths(-1);
            filter.EndDate ??= today;

            if (filter.EndDate.Value.Date < filter.StartDate.Value.Date)
            {
                filter.EndDate = filter.StartDate;
            }

            filter.CustomerKeyword = string.IsNullOrWhiteSpace(filter.CustomerKeyword)
                ? null
                : filter.CustomerKeyword.Trim();
        }

        private static List<Reservation> ApplyReservationReportFilters(
            List<Reservation> reservations,
            ReservationReportFilterViewModel filter)
        {
            var query = reservations.AsQueryable();

            if (filter.StartDate.HasValue)
            {
                query = query.Where(x => x.PickupDate.Date >= filter.StartDate.Value.Date);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(x => x.PickupDate.Date <= filter.EndDate.Value.Date);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.ReservationStatus == filter.Status.Value);
            }

            if (filter.CarId.HasValue)
            {
                query = query.Where(x => x.CarId == filter.CarId.Value);
            }

            if (filter.BrandId.HasValue)
            {
                query = query.Where(x => x.Car != null && x.Car.BrandId == filter.BrandId.Value);
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(x => x.Car != null && x.Car.CategoryId == filter.CategoryId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(x => x.TotalPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(x => x.TotalPrice <= filter.MaxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CustomerKeyword))
            {
                var keyword = filter.CustomerKeyword.ToLower();

                query = query.Where(x =>
                    x.Customer != null &&
                    (
                        ((x.Customer.FirstName ?? "").ToLower().Contains(keyword)) ||
                        ((x.Customer.LastName ?? "").ToLower().Contains(keyword)) ||
                        ((x.Customer.Email ?? "").ToLower().Contains(keyword)) ||
                        ((x.Customer.Phone ?? "").ToLower().Contains(keyword))
                    ));
            }

            return query.ToList();
        }

        private static string BuildReservationFilterSummary(ReservationReportFilterViewModel filter)
        {
            var parts = new List<string>
            {
                $"Date Range: {FormatDate(filter.StartDate!.Value)} - {FormatDate(filter.EndDate!.Value)}"
            };

            if (filter.Status.HasValue)
            {
                parts.Add($"Status: {filter.Status.Value}");
            }

            if (filter.CarId.HasValue)
            {
                parts.Add($"CarId: {filter.CarId.Value}");
            }

            if (filter.BrandId.HasValue)
            {
                parts.Add($"BrandId: {filter.BrandId.Value}");
            }

            if (filter.CategoryId.HasValue)
            {
                parts.Add($"CategoryId: {filter.CategoryId.Value}");
            }

            if (filter.MinPrice.HasValue)
            {
                parts.Add($"Min Price: {filter.MinPrice.Value:N0} TL");
            }

            if (filter.MaxPrice.HasValue)
            {
                parts.Add($"Max Price: {filter.MaxPrice.Value:N0} TL");
            }

            if (!string.IsNullOrWhiteSpace(filter.CustomerKeyword))
            {
                parts.Add($"Customer Search: {filter.CustomerKeyword}");
            }

            return string.Join(" | ", parts);
        }

        private static void ComposeHeader(IContainer container, string title, string subtitle)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text("COROLA Rent A Car").FontSize(18).Bold();
                        inner.Item().Text(title).FontSize(13).SemiBold().FontColor(Colors.Grey.Darken2);
                        inner.Item().Text(subtitle).FontSize(8).FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(180).AlignRight().Column(inner =>
                    {
                        inner.Item().Text($"Generated: {DateTime.Now.ToString("dd MMM yyyy HH:mm", CultureInfo.GetCultureInfo("en-US"))}")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);

                        inner.Item().Text("Admin PDF Report")
                            .FontSize(8)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken2);
                    });
                });

                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        }

        private static void ComposeMetric(IContainer container, string label, string value)
        {
            container
                .Background(Colors.Grey.Lighten4)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1).SemiBold();
                    column.Item().Text(value).FontSize(15).Bold();
                });
        }

        private static void ComposeFooter(IContainer container)
        {
            container
                .PaddingTop(10)
                .BorderTop(1)
                .BorderColor(Colors.Grey.Lighten2)
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("COROLA Rent A Car | Page ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
        }

        private static IContainer TableHeaderStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5)
                .DefaultTextStyle(x => x.SemiBold().FontSize(7));
        }

        private static IContainer TableCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten3)
                .Padding(5)
                .DefaultTextStyle(x => x.FontSize(7));
        }

        private static string FormatDate(DateTime date)
        {
            return date.ToString("dd MMM yyyy", CultureInfo.GetCultureInfo("en-US"));
        }
    }
}