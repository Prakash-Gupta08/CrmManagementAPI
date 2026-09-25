using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderController : ControllerBase
    {
        private readonly ITenderService _context;
        protected APIResponse _response;
        public TenderController(ITenderService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllTender")]
        public async Task<ActionResult> GetAllTender(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllTender(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetTenderByID")]
        public async Task<ActionResult> GetTenderById(int id)
        {
            var data = await _context.GetTenderById(id);
            return Ok(data);
        }

        [HttpPost("CreateTender")]
        public async Task<ActionResult> CreateTender([FromBody] CreateTender dto)
        {
            var data = await _context.CreateTender(dto);
            return Ok(data);
        }

        [HttpPost("UpdateTender")]
        public async Task<ActionResult> UpdateTender([FromBody] EditTender req)
        {
            var data = await _context.UpdateTender(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTender(int id)
        {
            var data = await _context.DeleteTender(id);
            return Ok(data);
        }
    }
}
