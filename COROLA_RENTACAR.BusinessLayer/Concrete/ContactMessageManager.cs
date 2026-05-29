using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class ContactMessageManager : IContactMessageService
    {
        private readonly IContactMessageDal _contactMessageDal;

        public ContactMessageManager(IContactMessageDal contactMessageDal)
        {
            _contactMessageDal = contactMessageDal;
        }

        public async Task<List<ContactMessage>> TGetAllAsync()
        {
            return await _contactMessageDal.GetAllAsync();
        }

        public async Task<ContactMessage> TGetByIdAsync(int id)
        {
            return await _contactMessageDal.GetByIdAsync(id);
        }

        public async Task TCreateAsync(ContactMessage entity)
        {
            await _contactMessageDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(ContactMessage entity)
        {
            await _contactMessageDal.UpdateAsync(entity);
        }

        public async Task TDeleteAsync(int id)
        {
            await _contactMessageDal.DeleteAsync(id);
        }
    }
}