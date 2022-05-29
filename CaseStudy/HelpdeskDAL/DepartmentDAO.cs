using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class DepartmentDAO
    {
        readonly IRepository<Department> repository;
        public DepartmentDAO()
        {
            repository = new HelpdeskRepository<Department>();
        }
        public async Task<List<Department>> GetAll()
        {
            List<Department> allEmployees = new List<Department>();
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

    }
}
