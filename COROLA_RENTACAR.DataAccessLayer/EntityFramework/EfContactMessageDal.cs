using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Concrete;
using COROLA_RENTACAR.DataAccessLayer.Repository;
using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.DataAccessLayer.EntityFramework
{
    public class EfContactMessageDal : GenericRepository<ContactMessage>, IContactMessageDal
    {
        public EfContactMessageDal(CorolaContext context) : base(context)
        {
        }
    }
}