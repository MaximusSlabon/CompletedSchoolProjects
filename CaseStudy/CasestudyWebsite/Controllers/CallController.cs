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
    public class CallController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                CallViewModel vm = new CallViewModel();
                List<CallViewModel> allCalls = await vm.GetAll();
                return Ok(allCalls);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod().Name + " "
                    + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ActionResult> Put(CallViewModel viewmodel)
        {
            try
            {
                int retVal = await viewmodel.Update();

                return retVal switch
                {
                    1 => Ok(new { msg = "call updated!" }),
                    -1 => Ok(new { msg = "call not updated!" }),
                    -2 => Ok(new { msg = "Data is stale call not updated!" }),
                    _ => Ok(new { msg = "call not updated!" }),
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
        public async Task<ActionResult> Post(CallViewModel viewModel)
        {
            try
            {
                await viewModel.Add();
                return viewModel.Id > 1
                    ? Ok(new { msg = "call " + viewModel.Id + " added!" })
                    : Ok(new { msg = "call " + viewModel.Id + " not added!" });

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
            try
            {
                CallViewModel vm = new CallViewModel { Id = id };
                return await vm.Delete() == 1
                    ? Ok(new { msg = "Emoloyee deleted!" })
                    : Ok(new { msg = "Emoloyee not deleted!" });
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
