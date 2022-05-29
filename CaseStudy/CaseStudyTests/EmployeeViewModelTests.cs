using System;
using Xunit;
using System.Collections.Generic;
using HelpdeskViewModels;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CaseStudyTests
{
    public class EmployeeViewModelTests
    {
        private readonly ITestOutputHelper output;

        public EmployeeViewModelTests(ITestOutputHelper output)
        {
            this.output = output;

        }

        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeViewModel vm = null;

            try
            {
                vm = new EmployeeViewModel { Id = 1 };
                await vm.GetById();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeViewModel vm = null;

            try
            {
                vm = new EmployeeViewModel { Email = "ms@abc.com" };
                await vm.GetByEmail();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            EmployeeViewModel vm = null;
            List<EmployeeViewModel> vml = null;
            try
            {
                vm = new EmployeeViewModel();
                vml = await vm.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
            Assert.NotNull(vml);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            try
            {
                EmployeeViewModel vm = new EmployeeViewModel
                {
                    Title = "Mr.",
                    Firstname = "Maximus",
                    Lastname = "Slabon",
                    Email = "ms@abc.com",
                    Phoneno = "(555)-444-4444",
                    DepartmentId = 100
                };

                await vm.Add();
                Assert.True(vm.Id > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            int EmployeesUpdated = -1;

            try
            {
                EmployeeViewModel vm = new EmployeeViewModel
                {
                    Email = "ms@abc.com"
                };
                await vm.GetByEmail();
                vm.Phoneno = "(333)-333-3333";
                EmployeesUpdated = await vm.Update();
                Assert.True(EmployeesUpdated > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            int employeesDeleted = 0;
            try
            {
                EmployeeViewModel vm = new EmployeeViewModel {Id = 3002 };
                await vm.GetById();
                employeesDeleted = await vm.Delete();
                Assert.True(employeesDeleted > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
        }

        [Fact]
        public async Task Employee_ConcurrencyTest()
        {
            int EmployeesUpdated = -1;

            try
            {
                EmployeeViewModel vm1 = new EmployeeViewModel { Email = "ms@abc.com" };
                EmployeeViewModel vm2 = new EmployeeViewModel { Email = "ms@abc.com" };
                await vm1.GetByEmail();
                await vm2.GetByEmail();
                vm1.Phoneno = vm1.Phoneno == "(555)555-5551" ? "(555)555 -5552" : "(555)555-5551";
                if (await vm1.Update() == 1)
                {
                    vm2.Phoneno = "(666)666-6666";
                    EmployeesUpdated = await vm2.Update();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - " + ex.Message);
            }
            Assert.True(EmployeesUpdated == -2);
        }

        [Fact]
        public async Task Call_ComprehensiveVMTest() {

            CallViewModel cvm = new CallViewModel();
            EmployeeViewModel evm = new EmployeeViewModel();
            ProblemViewModel pvm = new ProblemViewModel();
            cvm.DateOpened = DateTime.Now;
            cvm.DateClosed = null;
            cvm.OpenStatus = true;
            evm.Email = "ms@abc.com";
            await evm.GetByEmail();
            cvm.EmployeeId = evm.Id;
            evm.Email = "bb@abc.com";
            await evm.GetByEmail();
            cvm.TechId = evm.Id;
            pvm.Description = "Memory Upgrade";
            await pvm.GetByDescription();
            cvm.ProblemId = pvm.Id;
            cvm.Notes = "Slabon has bad RAM, Burner to fix it";
            await cvm.Add();
            output.WriteLine("New Call Generated - Id = " + cvm.Id);
            int id = cvm.Id;
            await cvm.GetById();
            cvm.Notes += "\n Ordered new Ram!";

            if (await cvm.Update() == 1)
            {
                output.WriteLine("Call was updated " + cvm.Notes);
            }
            else {
                output.WriteLine("Call was not updated !");
            }

            cvm.Notes = "Another change to comments that should not work";
            if (await cvm.Update() == -2)
            {
                output.WriteLine("Call was not updated data was stale");
            }

            cvm = new CallViewModel
            {
                Id = id
            };

            await cvm.GetById();

            if(await cvm.Delete() == 1)
            {
                output.WriteLine("Call was deleted!");
            }
            else {
                output.WriteLine("Call was not deleted!");
            }

            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async () => await cvm.GetById());
        }
    }
}
