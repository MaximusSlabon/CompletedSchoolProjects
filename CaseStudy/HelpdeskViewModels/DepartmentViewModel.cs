using System;
using HelpdeskDAL;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Timer { get; set; }

        private readonly DepartmentDAO _dao;

        public DepartmentViewModel() {
            _dao = new DepartmentDAO();
        }

        public async Task<List<DepartmentViewModel>> GetAll()
        {
            List<DepartmentViewModel> allVms = new List<DepartmentViewModel>();
            try
            {
                List<Department> allEmployees = await _dao.GetAll();
                foreach (Department stu in allEmployees)
                {
                    DepartmentViewModel stuVm = new DepartmentViewModel
                    {
                        Id = stu.Id,
                        Name = stu.DepartmentName,
                        Timer = Convert.ToBase64String(stu.Timer)
                    };
                    allVms.Add(stuVm);
                }

            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Name = "not found";
            }
            catch (Exception ex)
            {
                Name = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }
    }
}
