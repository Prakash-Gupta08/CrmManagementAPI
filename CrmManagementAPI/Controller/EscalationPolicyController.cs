using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EscalationPolicyController : ControllerBase
    {
        private readonly IEscalationPolicyService _context;
        protected APIResponse _response;
        public EscalationPolicyController(IEscalationPolicyService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllEscalationPolicy")]
        public async Task<ActionResult> GetAllEscalationPolicy(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllEscalationPolicy(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetEscalationPolicyByID")]
        public async Task<ActionResult> GetEscalationPolicyById(int id)
        {
            var data = await _context.GetEscalationPolicyById(id);
            return Ok(data);
        }

        [HttpPost("CreateEscalationPolicy")]
        public async Task<ActionResult> CreateEscalationPolicy([FromBody] CreateEscalationPolicy dto)
        {
            var data = await _context.CreateEscalationPolicy(dto);
            return Ok(data);
        }

        [HttpPost("UpdateEscalationPolicy")]
        public async Task<ActionResult> UpdateEscalationPolicy([FromBody] EditEscalationPolicy req)
        {
            var data = await _context.UpdateEscalationPolicy(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEscalationPolicy(int id)
        {
            var data = await _context.DeleteEscalationPolicy(id);
            return Ok(data);
        }
    }
}
