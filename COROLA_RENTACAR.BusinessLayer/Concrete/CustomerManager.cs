using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using CorolaDtoLayer.Dto.CostumerDto;
using AutoMapper;
using COROLA_RENTACAR.EntityLayer.Entities;
using CorolaDtoLayer.Dto.CostomerDto;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class CustomerManager : ICustomerService
    {
        private readonly ICustomerDal _customerDal;
        private readonly IMapper _mapper;

        public CustomerManager(ICustomerDal customerDal, IMapper mapper)
        {
            _mapper = mapper;
            _customerDal = customerDal;
        }


        public async Task CreateCustomerAsync(CreateCustomerDto dto)
        {
            var value = _mapper.Map<Customer>(dto);
            await _customerDal.InsertAsync(value);
        }

        public async Task DeleteCustomerAsync(int id)
        {
           await _customerDal.DeleteAsync(id);
        }

        public async Task<List<ResultCustomerDto>> GetAllCustomerAsync()
        {
            var values = await _customerDal.GetAllAsync();
            return _mapper.Map<List<ResultCustomerDto>>(values);
        }

        public async Task UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            var value = _mapper.Map<Customer>(dto);
            await _customerDal.UpdateAsync(value);
        }

        public async Task<GetCostomerByIdDto> GetCustomerByIdAsync(int id)
        {
            var values = await _customerDal.GetByIdAsync(id);
            return _mapper.Map<GetCostomerByIdDto>(values);
        }
    }
}