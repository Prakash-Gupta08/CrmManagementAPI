using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverviewController : ControllerBase
    {
        private readonly IOverviewService _context;
        protected APIResponse _response;
        public OverviewController(IOverviewService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetOverviewData")]
        public async Task<ActionResult> GetDashboardData()
        {
            var data = await _context.GetDashboardData(); 

            return Ok(data);

        }

        [HttpGet("GetCategoryData")]
        public async Task<ActionResult> GetCustomerCategoryOverview()
        {
            var data = await _context.GetCustomerCategoryOverview();
            return Ok(data);

        }
    }
}
