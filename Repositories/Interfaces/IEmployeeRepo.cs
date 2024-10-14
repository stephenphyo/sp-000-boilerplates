using SP_000.Models;

namespace SP_000.Repositories.Interfaces
{
    public interface IEmployeeRepo : IRepository<Employee>
    {
        void Update(Employee existingEmployee, Employee newEmployee);
    }
}