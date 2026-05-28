using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class CarManager : ICarService
    {
        private readonly ICarDal _carDal;
        private readonly IValidator<Car> _validator;

        public CarManager(ICarDal carDal, IValidator<Car> validator)
        {
            _carDal = carDal;
            _validator = validator;
        }

        public async Task<List<Car>> TGetAllAsync()
        {
            return await _carDal.GetAllAsync();
        }

        public async Task<List<Car>> TGetAllCarsWithCategoryAsync()
        {
            return await _carDal.GetAllCarsWithCategoryAsync();
        }

        public async Task<List<Car>> TGetAllCarsWithDetailsAsync()
        {
            return await _carDal.GetAllCarsWithDetailsAsync();
        }

        public async Task<Car> TGetByIdAsync(int id)
        {
            return await _carDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Car entity)
        {
            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _carDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Car entity)
        {
            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _carDal.UpdateAsync(entity);
        }

        public async Task TDeleteAsync(int id)
        {
            await _carDal.DeleteAsync(id);
        }
    }
}