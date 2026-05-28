using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    using System.Collections.Generic;

    namespace COROLA_RENTACAR.EntityLayer.Entities
    {
        public class Brand
        {
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public string LogoUrl { get; set; }
            public bool Status { get; set; }

            public List<Car> Cars { get; set; }
        }
    }
