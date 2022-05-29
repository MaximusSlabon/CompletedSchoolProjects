using HelpdeskDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskViewModels
{
    public class ProblemViewModel
    {
        private readonly ProblemDAO _dao;

        public int Id { get; set; }
        public string Description { get; set; }
        public string Timer { get; set; }

        public ProblemViewModel()
        {
            _dao = new ProblemDAO();
        }

        public async Task GetByDescription()
        {
            try
            {
                Problem prob = await _dao.GetByDescription(Description);
                Id = prob.Id;
                Description = prob.Description;
                Timer = Convert.ToBase64String(prob.Timer);
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine(nre.Message);
                Description = "not found";
            }
            catch (Exception e)
            {
                Description = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + e.Message);
                throw;
            }
        }

        public async Task<List<ProblemViewModel>> GetAll()
        {
            List<ProblemViewModel> allVms = new List<ProblemViewModel>();
            try
            {
                List<Problem> allEmployees = await _dao.GetAll();
                foreach (Problem stu in allEmployees)
                {
                    ProblemViewModel stuVm = new ProblemViewModel
                    {
                        Description = stu.Description,
                        Id = stu.Id,
                        Timer = Convert.ToBase64String(stu.Timer)
                    };
                    allVms.Add(stuVm);
                }

            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Description = "not found";
            }
            catch (Exception ex)
            {
                Description = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }
    }
}
