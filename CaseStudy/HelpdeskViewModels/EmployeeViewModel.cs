using System;
using HelpdeskDAL;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace HelpdeskViewModels
{
    public class EmployeeViewModel
    {
        private readonly EmployeeDAO _dao;

        public string Title { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phoneno { get; set; }
        public string Timer { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int Id { get; set; }
        public bool? IsTech { get; set; }
        public string StaffPicture64 { get; set; }

        public EmployeeViewModel()
        {
            _dao = new EmployeeDAO();
        }
        public async Task GetById()
        {
            try
            {
                Employee stu = await _dao.GetById(Id);
                Title = stu.Title;
                Firstname = stu.FirstName;
                Lastname = stu.LastName;
                Phoneno = stu.PhoneNo;
                Email = stu.Email;
                Id = stu.Id;
                DepartmentId = stu.DepartmentId;
                IsTech = stu.IsTech ?? false;
                if (stu.StaffPicture != null)
                {
                    StaffPicture64 = Convert.ToBase64String(stu.StaffPicture);
                }
                Timer = Convert.ToBase64String(stu.Timer);
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Lastname = "not found";
            }
            catch (Exception ex)
            {
                Lastname = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
        }
        public async Task GetByEmail()
        {
            try
            {
                Employee stu = await _dao.GetByEmail(Email);
                Title = stu.Title;
                Firstname = stu.FirstName;
                Lastname = stu.LastName;
                Phoneno = stu.PhoneNo;
                Email = stu.Email;
                Id = stu.Id;
                DepartmentId = stu.DepartmentId;
                IsTech = stu.IsTech ?? false;
                if (stu.StaffPicture != null)
                {
                    StaffPicture64 = Convert.ToBase64String(stu.StaffPicture);
                }
                Timer = Convert.ToBase64String(stu.Timer);

            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Email = "not found";
            }
            catch (Exception ex)
            {
                Email = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<List<EmployeeViewModel>> GetAll()
        {
            List<EmployeeViewModel> allVms = new List<EmployeeViewModel>();
            try
            {
                List<Employee> allEmployees = await _dao.GetAll();
                foreach (Employee stu in allEmployees)
                {
                    EmployeeViewModel stuVm = new EmployeeViewModel
                    {
                        Title = stu.Title,
                        Firstname = stu.FirstName,
                        Lastname = stu.LastName,
                        Phoneno = stu.PhoneNo,
                        Email = stu.Email,
                        Id = stu.Id,
                        DepartmentId = stu.DepartmentId,
                        Timer = Convert.ToBase64String(stu.Timer)
                    };
                    stuVm.IsTech = stu.IsTech ?? false;

                    if (stu.StaffPicture != null)
                    stuVm.StaffPicture64 = Convert.ToBase64String(stu.StaffPicture);

                    allVms.Add(stuVm);
                }

            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Lastname = "not found";
            }
            catch (Exception ex)
            {
                Lastname = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }

        public async Task Add()
        {
            Id = -1;
            try
            {
                Employee stu = new Employee
                {
                    Title = Title,
                    FirstName = Firstname,
                    LastName = Lastname,
                    PhoneNo = Phoneno,
                    Email = Email,
                    DepartmentId = DepartmentId
                };
                Id = await _dao.Add(stu);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int> Update()
        {
            UpdateStatus status = UpdateStatus.Failed;
            try
            {
                Employee stu = new Employee
                {
                    Title = Title,
                    FirstName = Firstname,
                    LastName = Lastname,
                    PhoneNo = Phoneno,
                    Email = Email,
                    Id = Id,
                    DepartmentId = DepartmentId
                };
                if (StaffPicture64 != null)
                {
                    stu.StaffPicture = Convert.FromBase64String(StaffPicture64);
                }
                stu.Timer = Convert.FromBase64String(Timer);
                status = await _dao.Update(stu);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return Convert.ToInt16(status);
        }

        public async Task<int> Delete()
        {
            int EmployeeDeleted = -1;
            try
            {
                EmployeeDeleted = await _dao.Delete(Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                throw;
            }
            return EmployeeDeleted;
        }
    }
}

