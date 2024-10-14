using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SP_000.DTOs.Employee;
using SP_000.Helpers;
using SP_000.Models;
using SP_000.Repositories.Interfaces;

namespace SP_000.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class EmployeeController : Controller
    {
        /*** Properties ***/
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /*** Constructor ***/
        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /*** Methods ***/
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] BaseQuery query
        )
        {
            return await GetEmployees(query);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetAllSearchedEmployees(
            [FromQuery] BaseQuery query,
            [FromQuery] EmployeeSearchQuery searchQuery
        )
        {
            return await GetEmployees(query, searchQuery);
        }

        private async Task<IActionResult> GetEmployees(
            BaseQuery query,
            EmployeeSearchQuery? searchQuery = null
        )
        {
            try
            {
                var employees = await _unitOfWork.repoEmployee.GetAllSearched(query: query, searchQuery: searchQuery);
                var employeeListDTOs = _mapper.Map<IEnumerable<EmployeeSummaryDTO>>(employees.data);

                return Ok(new
                {
                    employees.totalRecords,
                    employees.totalPages,
                    pageSize = query.PageSize,
                    data = employeeListDTOs
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _unitOfWork.repoEmployee.Get(e => e.Id == id);
                if (employee == null) return NotFound();

                var employeeDetailedDTO = _mapper.Map<EmployeeDTO>(employee);

                return Ok(employeeDetailedDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetTotalEmployeesCount()
        {
            try
            {
                var employees = await _unitOfWork.repoEmployee.GetAll();
                int count = employees.data.Count();

                return Ok(new { totalEmployees = count });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewEmployee(Employee employee)
        {
            DateTime currentTimestamp = DateTime.Now;
            employee.CreatedAt = currentTimestamp;
            employee.UpdatedAt = currentTimestamp;
            employee.CreatedBy = 0;
            employee.UpdatedBy = 0;

            try
            {
                await _unitOfWork.repoEmployee.Add(employee);
                await _unitOfWork.Save();

                return CreatedAtAction(
                    nameof(GetEmployeeById),
                    new { id = employee.Id },
                    employee
                );
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDTO employeeUpdateDTO)
        {
            try
            {
                var existingEmployee = await _unitOfWork.repoEmployee.Get(e => e.Id == id);
                if (existingEmployee == null) return NotFound();

                existingEmployee.UpdatedAt = DateTime.Now;
                existingEmployee.UpdatedBy = 0;

                var newEmployee = _mapper.Map<Employee>(employeeUpdateDTO);
                _unitOfWork.repoEmployee.Update(existingEmployee, newEmployee);
                await _unitOfWork.Save();

                return await GetEmployeeById(id);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _unitOfWork.repoEmployee.Get(e => e.Id == id);
            if (employee == null) return NotFound();

            _unitOfWork.repoEmployee.Remove(employee);
            await _unitOfWork.Save();

            return Ok(new { id });
        }
    }
}