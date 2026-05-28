using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Concrete;
using COROLA_RENTACAR.DataAccessLayer.Repository;
using COROLA_RENTACAR.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.DataAccessLayer.EntityFramework
{
    public class EfCarDal : GenericRepository<Car>, ICarDal
    {
        public EfCarDal(CorolaContext context) : base(context)
        {

        }

        public async Task<List<Car>> GetAllCarsWithCategoryAsync()
        {
            var context = new CorolaContext();
            var values = await context.Cars.Include(x => x.Category).ToListAsync();
            return values;
        }
    }
}
