using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using CasestudyWebsite.Reports;
using System.Threading.Tasks;

namespace CasestudyWebsite.Controllers
{
    public class ReportController : Controller
    {

        private IWebHostEnvironment _env;

        public ReportController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("api/employeereport")]

        public async Task<IActionResult> GetEmployeeReport()
        {
            EmployeeReport emprep = new EmployeeReport();
            await emprep.generateReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }

        [Route("api/callreport")]

        public async Task<IActionResult> GetCallReport()
        {
            CallReport callrep = new CallReport();
            await callrep.generateReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }
    }
}
