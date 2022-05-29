using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using HelpdeskViewModels;
using System.Collections.Generic;

namespace CasestudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                EmployeeViewModel viewModel = new EmployeeViewModel
                {
                    Email = email
                };
                await viewModel.GetByEmail();
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " "
                    + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                EmployeeViewModel vm = new EmployeeViewModel();
                List<EmployeeViewModel> allEmployees = await vm.GetAll();
                return Ok(allEmployees);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " "
                    + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ActionResult> Put(EmployeeViewModel viewmodel)
        {
            try
            {
                int retVal = await viewmodel.Update();

                return retVal switch
                {
                    1 => Ok(new { msg = "Employee " + viewmodel.Lastname + " updated!" }),
                    -1 => Ok(new { msg = "Employee " + viewmodel.Lastname + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for  " + viewmodel.Lastname + ", Employee not updated!" }),
                    _ => Ok(new { msg = "Employee " + viewmodel.Lastname + " not updated!" }),
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " "
                    + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(EmployeeViewModel viewModel)
        {
            try
            {
                await viewModel.Add();
                return viewModel.Id > 1
                    ? Ok(new { msg = "Employee " + viewModel.Lastname + " added!" })
                    : Ok(new { msg = "Employee " + viewModel.Lastname + "not added!" });

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " "
                    + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try {
                EmployeeViewModel vm = new EmployeeViewModel { Id = id };
                return await vm.Delete() == 1
                    ? Ok(new { msg = "Emoloyee " + vm.Lastname + " deleted!" })
                    : Ok(new { msg = "Emoloyee " + vm.Lastname + " not deleted!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " "
                    + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
