using AutoMapper;
using SP_000.DTOs.Employee;
using SP_000.Models;

namespace SP_000.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<Employee, EmployeeSummaryDTO>();
            CreateMap<Employee, EmployeeLiteDTO>();
            CreateMap<EmployeeUpdateDTO, Employee>();
        }
    }
}