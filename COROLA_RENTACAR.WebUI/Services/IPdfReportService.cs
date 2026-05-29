using COROLA_RENTACAR.WebUI.Models;

namespace COROLA_RENTACAR.WebUI.Services
{
    public interface IPdfReportService
    {
        Task<byte[]> GenerateReservationReportAsync(ReservationReportFilterViewModel filter);
        Task<byte[]> GenerateCarReportAsync();
        Task<byte[]> GenerateCustomerReportAsync();
    }
}