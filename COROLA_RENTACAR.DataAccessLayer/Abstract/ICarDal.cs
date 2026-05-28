using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.DataAccessLayer.Abstract
{
    public interface ICarDal : IGenericDal<Car>
    {
        Task<List<Car>> GetAllCarsWithCategoryAsync();
        Task<List<Car>> GetAllCarsWithDetailsAsync();
    }
}