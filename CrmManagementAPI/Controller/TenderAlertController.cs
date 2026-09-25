using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderAlertController : ControllerBase
    {
        private readonly ITenderAlertService _context;
        protected APIResponse _response;
        public TenderAlertController(ITenderAlertService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllTenderAlert")]
        public async Task<ActionResult> GetAllTenderAlert(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllTenderAlert(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetTenderAlertByID")]
        public async Task<ActionResult> GetTenderAlertById(int id)
        {
            var data = await _context.GetTenderAlertById(id);
            return Ok(data);
        }

        [HttpPost("CreateTenderAlert")]
        public async Task<ActionResult> CreateTenderAlert([FromBody] CreateTenderAlert dto)
        {
            var data = await _context.CreateTenderAlert(dto);
            return Ok(data);
        }

        [HttpPost("UpdateTenderAlert")]
        public async Task<ActionResult> UpdateTenderAlert([FromBody] EditTenderAlert req)
        {
            var data = await _context.UpdateTenderAlert(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTenderAlert(int id)
        {
            var data = await _context.DeleteTenderAlert(id);
            return Ok(data);
        }
    }
}
