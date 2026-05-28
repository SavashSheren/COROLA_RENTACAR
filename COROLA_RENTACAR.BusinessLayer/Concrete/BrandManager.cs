using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationException = FluentValidation.ValidationException;


namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class BrandManager : IBrandService
    {
        private readonly IBrandDal _brandDal;
        private readonly IValidator<Brand> _validator;

        public BrandManager(IBrandDal branddal, IValidator<Brand> validator)
        {
            _brandDal = branddal;
            _validator = validator;
        }

        public async Task TDeleteAsync(int id)
        {
            await _brandDal.DeleteAsync(id);
        }

        public async Task<List<Brand>> TGetAllAsync()
        {
            return await _brandDal.GetAllAsync();
        }

        public async Task<Brand> TGetByIdAsync(int id)
        {
            return await _brandDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Brand entity)
        {

            var result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
            await _brandDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Brand entity)
        {
            await _brandDal.UpdateAsync(entity);
        }
    }
}
