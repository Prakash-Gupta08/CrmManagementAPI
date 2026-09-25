using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderActivityController : ControllerBase
    {
        private readonly ITenderActivityService _context;
        protected APIResponse _response;
        public TenderActivityController(ITenderActivityService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllTenderActivity")]
        public async Task<ActionResult> GetAllTenderActivity(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllTenderActivity(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetTenderActivityByID")]
        public async Task<ActionResult> GetTenderActivityById(int id)
        {
            var data = await _context.GetTenderActivityById(id);
            return Ok(data);
        }

        [HttpPost("CreateTenderActivity")]
        public async Task<ActionResult> CreateTenderActivity([FromBody] CreateTenderActivity dto)
        {
            var data = await _context.CreateTenderActivity(dto);
            return Ok(data);
        }

        [HttpPost("UpdateTenderActivity")]
        public async Task<ActionResult> UpdateTenderActivity([FromBody] EditTenderActivity req)
        {
            var data = await _context.UpdateTenderActivity(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTenderActivity(int id)
        {
            var data = await _context.DeleteTenderActivity(id);
            return Ok(data);
        }
    }
}
