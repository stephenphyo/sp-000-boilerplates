using SP_000.Data;
using SP_000.Models;
using SP_000.Repositories.Interfaces;

namespace SP_000.Repositories
{
    public class EmployeeRepo : Repository<Employee>, IEmployeeRepo
    {
        /*** Constructor ***/
        public EmployeeRepo(AppDbContext db) : base(db) { }

        /*** Methods ***/
        public void Update(Employee existingEmployee, Employee newEmployee)
        {
            existingEmployee.FirstName = newEmployee.FirstName ?? existingEmployee.FirstName;
            existingEmployee.LastName = newEmployee.LastName ?? existingEmployee.LastName;
            existingEmployee.Email = newEmployee.Email ?? existingEmployee.Email;
            existingEmployee.Phone = newEmployee.Phone ?? existingEmployee.Phone;
            existingEmployee.Address = newEmployee.Address ?? existingEmployee.Address;
        }
    }
}