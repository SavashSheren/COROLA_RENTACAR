using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.EntityLayer.Entities
{
    public class Location
    {
        public int LocationId { get; set; }
        public int AuthorizedPerson{ get; set; }
        public string LocationName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
       
    }

}
