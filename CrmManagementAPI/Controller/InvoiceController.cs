using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _context;
        protected APIResponse _response;
        public InvoiceController(IInvoiceService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllInvoice")]
        public async Task<ActionResult> GetAllInvoice(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllInvoice(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetInvoiceByID")]
        public async Task<ActionResult> GetInvoiceById(int id)
        {
            var data = await _context.GetInvoiceById(id);
            return Ok(data);
        }

        [HttpPost("CreateInvoice")]
        public async Task<ActionResult> CreateInvoice([FromBody] CreateInvoice dto)
        {
            var data = await _context.CreateInvoice(dto);
            return Ok(data);
        }

        [HttpPost("UpdateInvoice")]
        public async Task<ActionResult> UpdateInvoice([FromBody] EditInvoice req)
        {
            var data = await _context.UpdateInvoice(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInvoice(int id)
        {
            var data = await _context.DeleteInvoice(id);
            return Ok(data);
        }
    }
}
