using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Concrete;
using COROLA_RENTACAR.DataAccessLayer.Repository;
using COROLA_RENTACAR.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace COROLA_RENTACAR.DataAccessLayer.EntityFramework
{
    public class EfCarImageDal : GenericRepository<CarImage>, ICarImageDal
    {
        private readonly CorolaContext _context;

        public EfCarImageDal(CorolaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CarImage>> GetAllCarImagesWithCarAsync()
        {
            return await _context.CarImages
                .Include(x => x.Car)
                .ThenInclude(x => x.Brand)
                .Include(x => x.Car)
                .ThenInclude(x => x.Category)
                .OrderByDescending(x => x.IsCoverImage)
                .ThenByDescending(x => x.CarImageId)
                .ToListAsync();
        }

        public async Task<List<CarImage>> GetCarImagesByCarIdAsync(int carId)
        {
            return await _context.CarImages
                .Where(x => x.CarId == carId)
                .OrderByDescending(x => x.IsCoverImage)
                .ThenByDescending(x => x.CarImageId)
                .ToListAsync();
        }

        public async Task SetCoverImageAsync(int carImageId)
        {
            var selectedImage = await _context.CarImages
                .FirstOrDefaultAsync(x => x.CarImageId == carImageId);

            if (selectedImage == null)
            {
                return;
            }

            var carImages = await _context.CarImages
                .Where(x => x.CarId == selectedImage.CarId)
                .ToListAsync();

            foreach (var image in carImages)
            {
                image.IsCoverImage = image.CarImageId == carImageId;
            }

            await _context.SaveChangesAsync();
        }
    }
}