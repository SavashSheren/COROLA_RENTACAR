using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.BusinessLayer.Abstract
{
    public interface IContactMessageService
    {
        Task<List<ContactMessage>> TGetAllAsync();
        Task<ContactMessage> TGetByIdAsync(int id);
        Task TCreateAsync(ContactMessage entity);
        Task TUpdateAsync(ContactMessage entity);
        Task TDeleteAsync(int id);
    }
}