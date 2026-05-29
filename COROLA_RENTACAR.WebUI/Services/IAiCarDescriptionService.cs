namespace COROLA_RENTACAR.WebUI.Services
{
    public interface IAiCarDescriptionService
    {
        Task<string> GenerateDescriptionAsync(
            string brand,
            string model,
            string category,
            string fuelType,
            string transmission,
            int seatCount,
            decimal dailyPrice,
            string features);
    }
}