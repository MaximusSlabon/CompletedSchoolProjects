using System;
using HelpdeskDAL;
using Xunit;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CaseStudyTests
{
    public class DAOTests
    {
        private readonly ITestOutputHelper output;

        public DAOTests(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeDAO dao = new EmployeeDAO();
            Employee selectedEmployee = await dao.GetByEmail("bs@abc.com");
            Assert.NotNull(selectedEmployee);
        }
        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeDAO dao = new HelpdeskDAL.EmployeeDAO();
            Employee selectedEmployee = await dao.GetById(3);
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            EmployeeDAO dao = new HelpdeskDAL.EmployeeDAO();
            List<Employee> selectedEmployees = await dao.GetAll();
            Assert.NotNull(selectedEmployees);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeDAO dao = new HelpdeskDAL.EmployeeDAO();
            Employee newEmployee = new Employee
            {
                FirstName = "Herbert",
                LastName = "Flemmings",
                PhoneNo = "(555)-555-1233",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "m_slabon129233@fanshaweonline.ca",
                IsTech = null,
                StaffPicture = null,
            };

            int test = 0;
            test = await dao.Add(newEmployee);

            Assert.True(test > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeDAO dao = new HelpdeskDAL.EmployeeDAO();
            Employee employeeForUpdate = await dao.GetByEmail("ms@abc.com");

            if (employeeForUpdate != null)
            {
                string oldPhoneNo = employeeForUpdate.PhoneNo;
                string newPhoneNo = oldPhoneNo == "(555)-444-4444" ? "(555)-333-3333" : "(555)-444-4444";
                employeeForUpdate.PhoneNo = newPhoneNo;
            }
            Assert.True(await dao.Update(employeeForUpdate) == UpdateStatus.Ok);
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeDAO dao = new HelpdeskDAL.EmployeeDAO();
            //Is the employee in question even in the system?
            Employee employeeToDelete = await dao.GetByEmail("ms@abc.com");
            int test = await dao.Delete(employeeToDelete.Id);
            Assert.True(test != -1);
        }
        [Fact]
        public async Task Employee_ConcurrencyTest()
        {
            EmployeeDAO dao1 = new EmployeeDAO();
            EmployeeDAO dao2 = new EmployeeDAO();

            Employee employeeForUpdate1 = await dao1.GetByEmail("ms@abc.com");
            Employee employeeForUpdate2 = await dao2.GetByEmail("ms@abc.com");

            if (employeeForUpdate1 != null)
            {
                string oldPhoneNo = employeeForUpdate1.PhoneNo;
                string newPhoneNo = oldPhoneNo == "(555)-444-4444" ? "(555)-333-3333" : "(555)-444-4444";
                employeeForUpdate1.PhoneNo = newPhoneNo;
                if (await dao1.Update(employeeForUpdate1) == UpdateStatus.Ok)
                {
                    employeeForUpdate2.PhoneNo = "666-666-6666";
                    Assert.True(await dao2.Update(employeeForUpdate2) == UpdateStatus.Stale);
                }
                else
                {
                    Assert.True(false);
                }
            }
        }

        [Fact]
        public async Task Employee_LoadPicsTest()
        {
            try
            {
                DALUtil util = new DALUtil();
                Assert.True(await util.AddEmployeePicsToDb());

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
        }

        [Fact]
        public async Task Call_ComprehensiveTest() {
            CallDAO cdao = new CallDAO();
            EmployeeDAO edao = new EmployeeDAO();
            ProblemDAO pdao = new ProblemDAO();

            Employee slabon = await edao.GetByEmail("ms@abc.com");
            Employee burner = await edao.GetByEmail("bb@abc.com");
            Problem badDrive = await pdao.GetByDescription("Hard Drive Failure");

            Call call = new Call {
                DateOpened = DateTime.Now,
                DateClosed = null,
                OpenStatus = true,
                EmployeeId = slabon.Id,
                TechId = burner.Id,
                ProblemId = badDrive.Id,
                Notes = "Slabon's drive is shot, Burner to fix it"
            };

            //add test
            int newCallId = await cdao.Add(call);
            output.WriteLine("New Call Generated - Id = " + newCallId);
            call = await cdao.GetById(newCallId);

            byte[] oldTimer = call.Timer;
            output.WriteLine("New Call Retrieved");
            call.Notes += "\n Ordered new drive!";

            //update test
            if (await cdao.Update(call) == UpdateStatus.Ok)
            {
                output.WriteLine("Call was updated " + call.Notes);
            }
            else { 
                output.WriteLine("Call was not updated !");
            }

            //concurrency test
            call.Timer = oldTimer;
            call.Notes = "doesn't matter data stale now";
            if (await cdao.Update(call) == UpdateStatus.Stale) {
                output.WriteLine("Call was not updated due to stale data");
            }

            //delete test
            cdao = new CallDAO();

            call = await cdao.GetById(newCallId);

            if (await cdao.Delete(newCallId) == 1)
            {
                output.WriteLine("Call was deleted!");
            }
            else {
                output.WriteLine("Call was not deleted!");
            }

            Assert.Null(await cdao.GetById(newCallId));
        }
    }
}
