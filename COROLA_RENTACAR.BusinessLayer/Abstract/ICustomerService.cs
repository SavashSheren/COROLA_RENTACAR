using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.BusinessLayer.Abstract
{
    public interface ICustomerService : IGenericService<Customer>
    {
        Task TApproveDriverLicenseAsync(int customerId);
        Task TRejectDriverLicenseAsync(int customerId, string rejectionReason);
        Task TMarkDriverLicensePendingAsync(int customerId);
    }
}