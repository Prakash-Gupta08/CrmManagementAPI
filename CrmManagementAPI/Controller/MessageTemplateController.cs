using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageTemplateController : ControllerBase
    {
        private readonly IMessageTemplateService _context;
        protected APIResponse _response;
        public MessageTemplateController(IMessageTemplateService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllMessageTemplate")]
        public async Task<ActionResult> GetAllMessageTemplate(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllMessageTemplate(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetMessageTemplateByID")]
        public async Task<ActionResult> GetMessageTemplateById(int id)
        {
            var data = await _context.GetMessageTemplateById(id);
            return Ok(data);
        }

        [HttpPost("CreateMessageTemplate")]
        public async Task<ActionResult> CreateMessageTemplate([FromBody] CreateMessageTemplate dto)
        {
            var data = await _context.CreateMessageTemplate(dto);
            return Ok(data);
        }

        [HttpPost("UpdateMessageTemplate")]
        public async Task<ActionResult> UpdateMessageTemplate([FromBody] EditMessageTemplate req)
        {
            var data = await _context.UpdateMessageTemplate(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessageTemplate(int id)
        {
            var data = await _context.DeleteMessageTemplate(id);
            return Ok(data);
        }
    }
}
