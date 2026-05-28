using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace COROLA_RENTACAR.EntityLayer.Entities
{
    public class CarImage
    {
        public int CarImageId { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public string ImageUrl { get; set; }
        public bool IsCoverImage { get; set; }
    }
}
