using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.BusinessLayer.Abstract
{
    public interface ICarImageService : IGenericService<CarImage>
    {
        Task<List<CarImage>> TGetAllCarImagesWithCarAsync();
        Task<List<CarImage>> TGetCarImagesByCarIdAsync(int carId);
        Task TSetCoverImageAsync(int carImageId);
    }
}