using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Concrete;
using COROLA_RENTACAR.DataAccessLayer.Repository;
using COROLA_RENTACAR.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace COROLA_RENTACAR.DataAccessLayer.EntityFramework
{
    public class EfCarDal : GenericRepository<Car>, ICarDal
    {
        private readonly CorolaContext _context;

        public EfCarDal(CorolaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAllCarsWithCategoryAsync()
        {
            return await _context.Cars
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<List<Car>> GetAllCarsWithDetailsAsync()
        {
            return await _context.Cars
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .ToListAsync();
        }
    }
}