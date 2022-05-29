using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class EmployeeDAO
    {
        readonly IRepository<Employee> repository;
        public EmployeeDAO()
        {
            repository = new HelpdeskRepository<Employee>();
        }
        public async Task<Employee> GetById(int id)
        {
            Employee selectedEmployee = null;
            try
            {
                selectedEmployee = await repository.GetOne(stu => stu.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return selectedEmployee;
        }

        public async Task<Employee> GetByEmail(string email)
        {
            Employee selectedEmployee = null;
            try {
                selectedEmployee = await repository.GetOne(stu => stu.Email == email);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return selectedEmployee;
        }

        public async Task<List<Employee>> GetAll()
        {
            List<Employee> allEmployees = new List<Employee>();
            try
            {
                allEmployees = await repository.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return allEmployees;
        }

        public async Task<int> Add(Employee newEmployee)
        {
            try
            { 
                await repository.Add(newEmployee);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return newEmployee.Id;
        }

        public async Task<UpdateStatus> Update(Employee updatedEmployee)
        {
            UpdateStatus employeeUpdated = UpdateStatus.Failed;
            try
            {
                employeeUpdated = await repository.Update(updatedEmployee);
            }
            catch (DbUpdateConcurrencyException)
            {
                employeeUpdated = UpdateStatus.Stale;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return employeeUpdated;
        }

        public async Task<int> Delete(int id)
        {
            int employeesDeleted = -1;
            try
            {
                employeesDeleted = await repository.Delete(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return employeesDeleted;
        }
    }   
}
