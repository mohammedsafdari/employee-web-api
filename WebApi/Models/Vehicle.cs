using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeCode { get; set; }
        public string VehicleType { get; set; }
    }
}
