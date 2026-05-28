using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorolaDtoLayer.Dto.CostomerDto
{
    public class GetCustomerByIdDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DriverLicenseNumber { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
