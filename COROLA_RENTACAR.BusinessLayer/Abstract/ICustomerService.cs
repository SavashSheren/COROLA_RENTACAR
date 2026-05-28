using COROLA_RENTACAR.EntityLayer.Entities;
using CorolaDtoLayer.Dto.CostomerDto;
using CorolaDtoLayer.Dto.CostumerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.BusinessLayer.Abstract
{
    public interface ICustomerService 
    {
        Task<List<ResultCustomerDto>> GetAllCustomerAsync();
        Task<GetCustomerByIdDto> GetCustomerByIdAsync(int id);
        Task CreateCustomerAsync(CreateCustomerDto dto);
        Task UpdateCustomerAsync(UpdateCustomerDto dto);
        Task DeleteCustomerAsync(int id);
    }
}
