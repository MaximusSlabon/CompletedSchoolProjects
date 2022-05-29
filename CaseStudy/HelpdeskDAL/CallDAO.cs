using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskDAL
{

    public class CallDAO
    {

        readonly IRepository<Call> repository;

        public CallDAO() {
            repository = new HelpdeskRepository<Call>();
        }

        public async Task<Call> GetById(int id)
        {
            Call selectedCall = null;
            try
            {
                selectedCall = await repository.GetOne(stu => stu.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return selectedCall;
        }
        public async Task<List<Call>> GetAll()
        {
            List<Call> allCalls = new List<Call>();
            try
            {
                allCalls = await repository.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return allCalls;
        }

        public async Task<int> Add(Call newCall)
        {
            try
            {
                await repository.Add(newCall);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return newCall.Id;
        }

        public async Task<UpdateStatus> Update(Call updatedCall)
        {
            UpdateStatus callUpdated = UpdateStatus.Failed;
            try
            {
                callUpdated = await repository.Update(updatedCall);
            }
            catch (DbUpdateConcurrencyException)
            {
                callUpdated = UpdateStatus.Stale;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return callUpdated;
        }

        public async Task<int> Delete(int id)
        {
            int callDeleted = -1;
            try
            {
                callDeleted = await repository.Delete(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return callDeleted;
        }
    }
}
