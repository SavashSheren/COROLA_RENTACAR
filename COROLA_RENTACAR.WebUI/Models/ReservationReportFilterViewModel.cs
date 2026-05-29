using COROLA_RENTACAR.EntityLayer.Enums;

namespace COROLA_RENTACAR.WebUI.Models
{
    public class ReservationReportFilterViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ReservationStatus? Status { get; set; }

        public int? CarId { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? CustomerKeyword { get; set; }
    }
}