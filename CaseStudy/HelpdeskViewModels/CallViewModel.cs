using System;
using HelpdeskDAL;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskViewModels
{
    public class CallViewModel
    {
        private readonly CallDAO _dao;

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProblemId { get; set; }
        public string EmployeeName { get; set; }
        public string ProblemDescription { get; set; }
        public string TechName { get; set; }
        public int TechId { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public bool OpenStatus { get; set; }
        public string Notes { get; set; }
        public string Timer { get; set; }

        public CallViewModel()
        {
            _dao = new CallDAO();
        }

        public async Task<List<CallViewModel>> GetAll() {
            List<CallViewModel> allVms = new List<CallViewModel>();

            try {
                List<Call> allCalls = new List<Call>(); //create a new list

                allCalls = await _dao.GetAll();

                //create employee and problem dao
                EmployeeDAO eDao = new EmployeeDAO();
                ProblemDAO pDao = new ProblemDAO();

                Employee eTemp = new Employee();
                Employee tTemp = new Employee();
                Problem pTemp = new Problem();

                foreach (Call c in allCalls) {

                    eTemp = await eDao.GetById(c.EmployeeId);
                    tTemp = await eDao.GetById(c.TechId);
                    pTemp = await pDao.GetById(c.ProblemId);

                    CallViewModel tempCall = new CallViewModel {
                        EmployeeName = eTemp.LastName,
                        TechName = tTemp.LastName,
                        ProblemDescription = pTemp.Description,
                        Id = c.Id,
                        EmployeeId = c.EmployeeId,
                        ProblemId = c.ProblemId,
                        TechId = c.TechId,
                        DateOpened = c.DateOpened,
                        DateClosed = c.DateClosed,
                        OpenStatus = c.OpenStatus,
                        Notes = c.Notes,
                        Timer = Convert.ToBase64String(c.Timer)
                    };

                    allVms.Add(tempCall);
                }
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Notes = "not found";
            }
            catch (Exception ex)
            {
                Notes = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }

        public async Task GetById()
        {
            try
            {
                Call call = await _dao.GetById(Id);
                Id = call.Id;
                EmployeeId = call.EmployeeId;
                ProblemId = call.ProblemId;
                //EmployeeName = call.Employee.FirstName + " " + call.Employee.LastName;
                //ProblemDescription = call.Problem.Description;
                //TechName = (call.Tech.FirstName + " " + call.Tech.LastName);
                TechId = call.TechId;
                DateOpened = call.DateOpened;
                DateClosed = call.DateClosed;
                OpenStatus = call.OpenStatus;
                Notes = call.Notes;
                Timer = Convert.ToBase64String(call.Timer);
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine(nre.Message);
                EmployeeName = "not found";
            }
            catch (Exception e)
            {
                EmployeeName = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + e.Message);
                throw;
            }
        }
        public async Task Add()
        {
            Id = -1;
            try
            {
                Call call = new Call
                {
                    EmployeeId = EmployeeId,
                    ProblemId = ProblemId,
                    TechId = TechId,
                    DateOpened = DateOpened,
                    DateClosed = DateClosed,
                    OpenStatus = OpenStatus,
                    Notes = Notes,

                };
                Id = await _dao.Add(call);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                   MethodBase.GetCurrentMethod().Name + " " + e.Message);
                throw;
            }
        }
        public async Task<int> Update()
        {
            UpdateStatus status = UpdateStatus.Failed;
            try
            {
                Call call = new Call
                {
                    Id = Id,
                    EmployeeId = EmployeeId,
                    ProblemId = ProblemId,
                    TechId = TechId,
                    DateOpened = DateOpened,
                    DateClosed = DateClosed,
                    OpenStatus = OpenStatus,
                    Notes = Notes
                };
                call.Timer = Convert.FromBase64String(Timer);
                status = await _dao.Update(call);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + e.Message);
                throw;
            }
            return Convert.ToInt16(status);
        }
        public async Task<int> Delete()
        {
            int callDeleted = -1;
            try
            {
                callDeleted = await _dao.Delete(Id);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + e.Message);
                throw;
            }
            return callDeleted;
        }
    }
}
