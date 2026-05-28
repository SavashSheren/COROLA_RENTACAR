using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryDal _categorydal;

        public CategoryManager(ICategoryDal categorydal)
        {
            _categorydal = categorydal;
        }

        public async Task TDeleteAsync(int id)
        {
            await _categorydal.DeleteAsync(id);
        }

        public async Task<List<Category>> TGetAllAsync()
        {
            return await _categorydal.GetAllAsync();
        }

        public async Task<Category> TGetByIdAsync(int id)
        {
            return await _categorydal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Category entity)
        {

            await _categorydal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Category entity)
        {
            await _categorydal.UpdateAsync(entity);
        }
    }
}
