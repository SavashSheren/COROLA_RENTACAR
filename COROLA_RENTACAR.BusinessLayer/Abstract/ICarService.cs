using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.BusinessLayer.Abstract
{
    public interface ICarService : IGenericService<Car>
    {
        Task<List<Car>> TGetAllCarsWithCategoryAsync();
        Task<List<Car>> TGetAllCarsWithDetailsAsync();
    }
}