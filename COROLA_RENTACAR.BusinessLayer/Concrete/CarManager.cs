using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class CarManager : ICarService
    {
        private readonly ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            _carDal = carDal;
        }

        public async Task TDeleteAsync(int id)
        {
            await _carDal.DeleteAsync(id);
        }

        public async Task<List<Car>> TGetAllAsync()
        {
           return await _carDal.GetAllAsync();
        }

        public async Task<List<Car>> TGetAllCarsWithCategoryAsync()
        {
           return await _carDal.GetAllCarsWithCategoryAsync();
        }

        public async Task<Car> TGetByIdAsync(int id)
        {
           return await _carDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Car entity)
        {
            await _carDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Car entity)
        {
            await _carDal.UpdateAsync(entity);
        }
    }
}
