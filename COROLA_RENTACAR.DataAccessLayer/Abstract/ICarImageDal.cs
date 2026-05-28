using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.DataAccessLayer.Abstract
{
    public interface ICarImageDal : IGenericDal<CarImage>
    {
        Task<List<CarImage>> GetAllCarImagesWithCarAsync();
        Task<List<CarImage>> GetCarImagesByCarIdAsync(int carId);
        Task SetCoverImageAsync(int carImageId);
    }
}