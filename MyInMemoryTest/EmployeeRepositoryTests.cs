using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyInMemoryTest.Production;
using MyInMemoryTest.Production.Entities;
using NUnit.Framework;

namespace MyInMemoryTest
{
    public class EmployeeRepositoryTests
    {
        private IEmployeeRepository _repository;
        private EmployeeContext _context;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase("MockDB")
                .Options;

            _context = new EmployeeContext(options);
            _context.Database.EnsureDeleted();
            
            _repository = new EmployeeRepository(_context);
        }

        [Test]
        public void add_a_employee()
        {
            // arrange : add 1 employee
            var newEmployee = new EmployeeDto()
            {
                FirstName = "Nathan",
                LastName = "Lu",
                Department = DepartmentEnum.海外企業開發組
            };
            // act : save the employee data
            _repository.AddAsync(newEmployee);
            
            // assert : check the data was existed in repository
            var query =  _repository.FindByDepartmentId(DepartmentEnum.海外企業開發組);
            _context.Employees.Where(e => e.Name == newEmployee.FirstName + " " + newEmployee.LastName)
                .Select(e => e).Count().Should().Be(1);
           // query.Result[0].Name.Should().Be(newEmployee.FirstName + " " +newEmployee.LastName);
        }

        [Test]
        public void find_海外企業開發組()
        {
            // arrange : no employee
            // act : find employee by department id 
            var query =  _repository.FindByDepartmentId(DepartmentEnum.海外企業開發組);
            // assert : 0
            query.Result.Count.Should().Be(0);

            // arrange : add 3 employees with difference department
            _repository.AddAsync(new EmployeeDto()
            {
                FirstName = "Nathan",
                LastName = "Lu",
                Department = DepartmentEnum.海外企業開發組
            });
            _repository.AddAsync(new EmployeeDto()
            {
                FirstName = "Jason",
                LastName = "Su",
                Department = DepartmentEnum.火箭隊與她快樂夥伴
            });
            _repository.AddAsync(new EmployeeDto()
            {
                FirstName = "James",
                LastName = "Wu",
                Department = DepartmentEnum.海外企業開發組
            });
            
            // act : find employee that the department is 海外企業開發組
            var queryHaiWai =  _repository.FindByDepartmentId(DepartmentEnum.海外企業開發組);
            
            // assert : 
            queryHaiWai.Result.Count.Should().Be(2);
            queryHaiWai.Result.Select(x => x).Should().BeEquivalentTo(new List<Employee>()
            {
                new Employee()
                {
                    Name = "Nathan Lu",
                    DepartmentId = (int)DepartmentEnum.海外企業開發組
                },
                new Employee()
                {
                    Name = "James Wu",
                    DepartmentId = (int)DepartmentEnum.海外企業開發組
                }
            }, options => options.Excluding(su => su.Id));
        } 
    }
}