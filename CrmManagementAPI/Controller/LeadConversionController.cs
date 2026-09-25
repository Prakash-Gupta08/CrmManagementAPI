using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadConversionController : ControllerBase
    {
        private readonly ILeadConversionService _context;
        protected APIResponse _response;
        public LeadConversionController(ILeadConversionService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllLeadConversion")]
        public async Task<ActionResult> GetAllLeadConversion(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllLeadConversion(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetLeadConversionByID")]
        public async Task<ActionResult> GetLeadConversionById(int id)
        {
            var data = await _context.GetLeadConversionById(id);
            return Ok(data);
        }

        [HttpPost("CreateLeadConversion")]
        public async Task<ActionResult> CreateLeadConversion([FromBody] CreateLeadConversion dto)
        {
            var data = await _context.CreateLeadConversion(dto);
            return Ok(data);
        }

        [HttpPost("UpdateLeadConversion")]
        public async Task<ActionResult> UpdateLeadConversion([FromBody] EditLeadConversion req)
        {
            var data = await _context.UpdateLeadConversion(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLeadConversion(int id)
        {
            var data = await _context.DeleteLeadConversion(id);
            return Ok(data);
        }
    }
}
