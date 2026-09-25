using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageOutboxController : ControllerBase
    {
        private readonly IMessageOutboxService _context;
        protected APIResponse _response;
        public MessageOutboxController(IMessageOutboxService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllMessageOutbox")]
        public async Task<ActionResult> GetAllMessageOutbox(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllMessageOutbox(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetMessageOutboxByID")]
        public async Task<ActionResult> GetMessageOutboxById(int id)
        {
            var data = await _context.GetMessageOutboxById(id);
            return Ok(data);
        }

        [HttpPost("CreateMessageOutbox")]
        public async Task<ActionResult> CreateMessageOutbox([FromBody] CreateMessageOutbox dto)
        {
            var data = await _context.CreateMessageOutbox(dto);
            return Ok(data);
        }

        [HttpPost("UpdateMessageOutbox")]
        public async Task<ActionResult> UpdateMessageOutbox([FromBody] EditMessageOutbox req)
        {
            var data = await _context.UpdateMessageOutbox(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessageOutbox(int id)
        {
            var data = await _context.DeleteMessageOutbox(id);
            return Ok(data);
        }
    }
}
