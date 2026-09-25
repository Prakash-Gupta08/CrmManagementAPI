using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly ILeadService _context;
        protected APIResponse _response;
        public LeadController(ILeadService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllLead")]
        public async Task<ActionResult> GetAllLead(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllLead(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetLeadByID")]
        public async Task<ActionResult> GetLeadById(int id)
        {
            var data = await _context.GetLeadById(id);
            return Ok(data);
        }

        [HttpPost("CreateLead")]
        public async Task<ActionResult> CreateLead([FromBody] CreateLead dto)
        {
            var data = await _context.CreateLead(dto);
            return Ok(data);
        }

        [HttpPost("UpdateLead")]
        public async Task<ActionResult> UpdateLead([FromBody] EditLead req)
        {
            var data = await _context.UpdateLead(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLead(int id)
        {
            var data = await _context.DeleteLead(id);
            return Ok(data);
        }
    }
}
