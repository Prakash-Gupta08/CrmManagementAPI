using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoNoGoApprovalController : ControllerBase
    {
        private readonly IGoNoGoApprovalService _context;
        protected APIResponse _response;
        public GoNoGoApprovalController(IGoNoGoApprovalService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllGoNoGoApproval")]
        public async Task<ActionResult> GetAllGoNoGoApproval(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllGoNoGoApproval(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetGoNoGoApprovalByID")]
        public async Task<ActionResult> GetGoNoGoApprovalById(int id)
        {
            var data = await _context.GetGoNoGoApprovalById(id);
            return Ok(data);
        }

        [HttpPost("CreateGoNoGoApproval")]
        public async Task<ActionResult> CreateGoNoGoApproval([FromBody] CreateGoNoGoApproval dto)
        {
            var data = await _context.CreateGoNoGoApproval(dto);
            return Ok(data);
        }

        [HttpPost("UpdateGoNoGoApproval")]
        public async Task<ActionResult> UpdateGoNoGoApproval([FromBody] EditGoNoGoApproval req)
        {
            var data = await _context.UpdateGoNoGoApproval(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGoNoGoApproval(int id)
        {
            var data = await _context.DeleteGoNoGoApproval(id);
            return Ok(data);
        }
    }
}
