using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Concrete;
using COROLA_RENTACAR.DataAccessLayer.Repository;
using COROLA_RENTACAR.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.DataAccessLayer.EntityFramework
{
    public class EfCategoryDal : GenericRepository<Category>, ICategoryDal
    {
        public EfCategoryDal(CorolaContext context) : base(context)
        {
        }
    }
}
