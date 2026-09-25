using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadActivityController : ControllerBase
    {
        private readonly ILeadActivityService _context;
        protected APIResponse _response;
        public LeadActivityController(ILeadActivityService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllLeadActivity")]
        public async Task<ActionResult> GetAllLeadActivity(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllLeadActivity(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetLeadActivityByID")]
        public async Task<ActionResult> GetLeadActivityById(int id)
        {
            var data = await _context.GetLeadActivityById(id);
            return Ok(data);
        }

        [HttpPost("CreateLeadActivity")]
        public async Task<ActionResult> CreateLeadActivity([FromBody] CreateLeadActivity dto)
        {
            var data = await _context.CreateLeadActivity(dto);
            return Ok(data);
        }

        [HttpPost("UpdateLeadActivity")]
        public async Task<ActionResult> UpdateLeadActivity([FromBody] EditLeadActivity req)
        {
            var data = await _context.UpdateLeadActivity(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLeadActivity(int id)
        {
            var data = await _context.DeleteLeadActivity(id);
            return Ok(data);
        }
    }
}
