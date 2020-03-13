using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using WebApi.Models;

namespace WebApi.Controllers
{
    [EnableCors("AllowMyOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeVehicleViewModel>>> GetEmployees()
        {
            var empList = await _context.Employees.ToListAsync();

            List<EmployeeVehicleViewModel> employeeList = new List<EmployeeVehicleViewModel>();

            foreach (var item in empList)
            {
                string[] name = item.Name.Split(" ");

                EmployeeVehicleViewModel empViewModel = new EmployeeVehicleViewModel
                {
                    Code = item.Code,
                    FirstName = name[0],
                    LastName = name[1],
                    Type = item.Type,
                    Department = item.Department
                };

                var vehicleList = await _context.Vehicles.Where(v => v.EmployeeCode == item.Code).ToListAsync();
                Vehicle vehicle = null;
                int count = 0;

                foreach (var veh in vehicleList)
                {
                    count++;
                    vehicle = veh;
                }

                if(count==0)
                {
                    empViewModel.TwoWheeler = false;
                    empViewModel.FourWheeler = false;
                }
                else if(count==1 && vehicle.VehicleType == "Two Wheeler")
                {
                    empViewModel.TwoWheeler = true;
                    empViewModel.FourWheeler = false;
                }
                else if (count == 1 && vehicle.VehicleType == "Four Wheeler")
                {
                    empViewModel.TwoWheeler = false;
                    empViewModel.FourWheeler = true;
                }
                else
                {
                    empViewModel.TwoWheeler = true;
                    empViewModel.FourWheeler = true;
                }

                employeeList.Add(empViewModel);
            }

            return employeeList;
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, EmployeeVehicleViewModel employee)
        {
            if (id != employee.Code)
            {
                return BadRequest();
            }

            Employee emp = new Employee
            {
                Code = employee.Code,
                Name = employee.FirstName + " " + employee.LastName,
                Type = employee.Type,
                Department = employee.Department
            };

            _context.Entry(emp).State = EntityState.Modified;

            var vehicleList = await _context.Vehicles.Where(e => e.EmployeeCode == employee.Code).ToListAsync();

            foreach (var item in vehicleList)
            {
                _context.Vehicles.Remove(item);
            }

            if (employee.TwoWheeler == true && employee.FourWheeler == false)
            {
                Vehicle vehicle = new Vehicle
                {
                    EmployeeCode = employee.Code,
                    VehicleType = "Two Wheeler"
                };

                _context.Vehicles.Add(vehicle);
            }
            if (employee.TwoWheeler == false && employee.FourWheeler == true)
            {
                Vehicle vehicle = new Vehicle
                {
                    EmployeeCode = employee.Code,
                    VehicleType = "Four Wheeler"
                };

                _context.Vehicles.Add(vehicle);
            }
            if (employee.TwoWheeler == true && employee.FourWheeler == true)
            {
                Vehicle vehicle = new Vehicle
                {
                    EmployeeCode = employee.Code,
                    VehicleType = "Two Wheeler"
                };
                _context.Vehicles.Add(vehicle);

                Vehicle vehicle1 = new Vehicle
                {
                    EmployeeCode = employee.Code,
                    VehicleType = "Four Wheeler"
                };
                _context.Vehicles.Add(vehicle1);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employees
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(EmployeeVehicleViewModel employee)
        {
            Employee emp = new Employee
            {
                Name = employee.FirstName + " " + employee.LastName,
                Type = employee.Type,
                Department = employee.Department
            };

            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            var empList = await _context.Employees.Where(e => e.Name == emp.Name).ToListAsync();
            Employee emp1 = null;

            foreach (var item in empList)
            {
                 emp1 = item;
            }

            if(employee.TwoWheeler==true&&employee.FourWheeler==false)
            {
                Vehicle vehicle = new Vehicle
                {
                    EmployeeCode = emp1.Code,
                    VehicleType = "Two Wheeler"
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }
            if(employee.TwoWheeler==false&&employee.FourWheeler==true)
            {
                Vehicle vehicle = new Vehicle
                {
                    EmployeeCode = emp1.Code,
                    VehicleType = "Four Wheeler"
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }
            if(employee.TwoWheeler==true&&employee.FourWheeler==true)
            {
                Vehicle vehicle = new Vehicle
                {
                    EmployeeCode = emp1.Code,
                    VehicleType = "Two Wheeler"
                };
                _context.Vehicles.Add(vehicle);

                Vehicle vehicle1 = new Vehicle
                {
                    EmployeeCode = emp1.Code,
                    VehicleType = "Four Wheeler"
                };
                _context.Vehicles.Add(vehicle1);

                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetEmployee", new { id = emp.Code }, emp);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Code == id);
        }
    }
}
