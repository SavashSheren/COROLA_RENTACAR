using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class CustomerManager : ICustomerService
    {
        private readonly ICustomerDal _customerDal;
        private readonly IValidator<Customer> _validator;

        public CustomerManager(ICustomerDal customerDal, IValidator<Customer> validator)
        {
            _customerDal = customerDal;
            _validator = validator;
        }

        public async Task<List<Customer>> TGetAllAsync()
        {
            return await _customerDal.GetAllAsync();
        }

        public async Task<Customer> TGetByIdAsync(int id)
        {
            return await _customerDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Customer entity)
        {
            NormalizeCustomer(entity);

            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _customerDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Customer entity)
        {
            NormalizeCustomer(entity);

            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _customerDal.UpdateAsync(entity);
        }

        public async Task TDeleteAsync(int id)
        {
            await _customerDal.DeleteAsync(id);
        }

        public async Task TApproveDriverLicenseAsync(int customerId)
        {
            var customer = await _customerDal.GetByIdAsync(customerId);

            if (customer == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(customer.DriverLicenseImageUrl))
            {
                throw new ValidationException("Driver license image is required before approval.");
            }

            customer.DriverLicenseVerificationStatus = DriverLicenseVerificationStatus.Approved;
            customer.DriverLicenseVerifiedDate = DateTime.Now;
            customer.DriverLicenseRejectionReason = string.Empty;

            await _customerDal.UpdateAsync(customer);
        }

        public async Task TRejectDriverLicenseAsync(int customerId, string rejectionReason)
        {
            var customer = await _customerDal.GetByIdAsync(customerId);

            if (customer == null)
            {
                return;
            }

            customer.DriverLicenseVerificationStatus = DriverLicenseVerificationStatus.Rejected;
            customer.DriverLicenseVerifiedDate = null;
            customer.DriverLicenseRejectionReason = rejectionReason ?? string.Empty;

            await _customerDal.UpdateAsync(customer);
        }

        public async Task TMarkDriverLicensePendingAsync(int customerId)
        {
            var customer = await _customerDal.GetByIdAsync(customerId);

            if (customer == null)
            {
                return;
            }

            customer.DriverLicenseVerificationStatus = DriverLicenseVerificationStatus.Pending;
            customer.DriverLicenseVerifiedDate = null;
            customer.DriverLicenseRejectionReason = string.Empty;

            await _customerDal.UpdateAsync(customer);
        }

        private void NormalizeCustomer(Customer customer)
        {
            customer.DriverLicenseImageUrl ??= string.Empty;
            customer.DriverLicenseRejectionReason ??= string.Empty;
            customer.DriverLicenseSystemMessage ??= string.Empty;

            if (customer.CustomerId == 0)
            {
                customer.DriverLicenseVerificationStatus = DriverLicenseVerificationStatus.Pending;
                customer.DriverLicenseVerifiedDate = null;
            }
        }
    }
}