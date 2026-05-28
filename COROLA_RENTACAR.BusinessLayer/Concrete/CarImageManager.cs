using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class CarImageManager : ICarImageService
    {
        private readonly ICarImageDal _carImageDal;
        private readonly IValidator<CarImage> _validator;

        public CarImageManager(ICarImageDal carImageDal, IValidator<CarImage> validator)
        {
            _carImageDal = carImageDal;
            _validator = validator;
        }

        public async Task<List<CarImage>> TGetAllAsync()
        {
            return await _carImageDal.GetAllAsync();
        }

        public async Task<List<CarImage>> TGetAllCarImagesWithCarAsync()
        {
            return await _carImageDal.GetAllCarImagesWithCarAsync();
        }

        public async Task<List<CarImage>> TGetCarImagesByCarIdAsync(int carId)
        {
            return await _carImageDal.GetCarImagesByCarIdAsync(carId);
        }

        public async Task<CarImage> TGetByIdAsync(int id)
        {
            return await _carImageDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(CarImage entity)
        {
            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _carImageDal.InsertAsync(entity);

            if (entity.IsCoverImage)
            {
                await _carImageDal.SetCoverImageAsync(entity.CarImageId);
            }
        }

        public async Task TUpdateAsync(CarImage entity)
        {
            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _carImageDal.UpdateAsync(entity);

            if (entity.IsCoverImage)
            {
                await _carImageDal.SetCoverImageAsync(entity.CarImageId);
            }
        }

        public async Task TDeleteAsync(int id)
        {
            await _carImageDal.DeleteAsync(id);
        }

        public async Task TSetCoverImageAsync(int carImageId)
        {
            await _carImageDal.SetCoverImageAsync(carImageId);
        }
    }
}