using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderDocumentController : ControllerBase
    {
        private readonly ITenderDocumentService _context;
        protected APIResponse _response;
        public TenderDocumentController(ITenderDocumentService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllTenderDocument")]
        public async Task<ActionResult> GetAllTenderDocument(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllTenderDocument(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetTenderDocumentByID")]
        public async Task<ActionResult> GetTenderDocumentById(int id)
        {
            var data = await _context.GetTenderDocumentById(id);
            return Ok(data);
        }

        [HttpPost("CreateTenderDocument")]
        public async Task<ActionResult> CreateTenderDocument([FromBody] CreateTenderDocument dto)
        {
            var data = await _context.CreateTenderDocument(dto);
            return Ok(data);
        }

        [HttpPost("UpdateTenderDocument")]
        public async Task<ActionResult> UpdateTenderDocument([FromBody] EditTenderDocument req)
        {
            var data = await _context.UpdateTenderDocument(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTenderDocument(int id)
        {
            var data = await _context.DeleteTenderDocument(id);
            return Ok(data);
        }
    }
}
