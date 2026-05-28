using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class LocationManager : ILocationService
    {
        private readonly ILocationDal _locationDal;
        private readonly IValidator<Location> _validator;

        public LocationManager(ILocationDal locationDal, IValidator<Location> validator)
        {
            _locationDal = locationDal;
            _validator = validator;
        }

        public async Task<List<Location>> TGetAllAsync()
        {
            return await _locationDal.GetAllAsync();
        }

        public async Task<Location> TGetByIdAsync(int id)
        {
            return await _locationDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Location entity)
        {
            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _locationDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Location entity)
        {
            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _locationDal.UpdateAsync(entity);
        }

        public async Task TDeleteAsync(int id)
        {
            await _locationDal.DeleteAsync(id);
        }
    }
}