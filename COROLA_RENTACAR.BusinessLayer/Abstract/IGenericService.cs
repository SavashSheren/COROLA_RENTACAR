using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.BusinessLayer.Abstract
{
    public interface IGenericService<T> where T:class
    {
        Task TInsertAsync(T entity);
        Task TUpdateAsync(T entity);
        Task TDeleteAsync(int id);
        Task<List<T>> TGetAllAsync();
        Task<T> TGetByIdAsync(int id);
    }
}
