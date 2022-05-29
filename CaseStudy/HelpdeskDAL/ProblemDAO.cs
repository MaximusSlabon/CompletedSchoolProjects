using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class ProblemDAO
    {
        readonly IRepository<Problem> repository;
        public ProblemDAO()
        {
            repository = new HelpdeskRepository<Problem>();
        }
        public string Description { get; set; }

        public async Task<Problem> GetByDescription(string desc)
        {
            Problem selectedProblem = null;
            try
            {
                selectedProblem = await repository.GetOne(pru =>  pru.Description == desc);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return selectedProblem;
        }

        public async Task<Problem> GetById(int id)
        {
            Problem selectedProblem = null;
            try
            {
                selectedProblem = await repository.GetOne(pru => pru.Id == id);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return selectedProblem;
        }

        public async Task<List<Problem>> GetAll()
        {
            List<Problem> allProblems = new List<Problem>();
            try
            {
                allProblems = await repository.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return allProblems;
        }
    }
}
