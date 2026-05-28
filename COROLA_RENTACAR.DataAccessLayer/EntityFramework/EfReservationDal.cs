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
    public class EfReservationDal : GenericRepository<Reservation>, IReservationDal
    {
        public EfReservationDal(CorolaContext context) : base(context)
        {
        }
    }
}
