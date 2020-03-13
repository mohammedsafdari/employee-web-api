using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class EmployeeVehicleViewModel
    {
        public int Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
        public string Department { get; set; }
        public bool TwoWheeler { get; set; }
        public bool FourWheeler { get; set; }
    }
}
