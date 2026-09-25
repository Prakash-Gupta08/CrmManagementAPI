using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMilestoneController : ControllerBase
    {
        private readonly IProjectMilestoneService _context;
        protected APIResponse _response;
        public ProjectMilestoneController(IProjectMilestoneService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllProjectMilestone")]
        public async Task<ActionResult> GetAllProjectMilestone(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllProjectMilestone(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetProjectMilestoneByID")]
        public async Task<ActionResult> GetProjectMilestoneById(int id)
        {
            var data = await _context.GetProjectMilestoneById(id);
            return Ok(data);
        }

        [HttpPost("CreateProjectMilestone")]
        public async Task<ActionResult> CreateProjectMilestone([FromBody] CreateProjectMilestone dto)
        {
            var data = await _context.CreateProjectMilestone(dto);
            return Ok(data);
        }

        [HttpPost("UpdateProjectMilestone")]
        public async Task<ActionResult> UpdateProjectMilestone([FromBody] EditProjectMilestone req)
        {
            var data = await _context.UpdateProjectMilestone(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProjectMilestone(int id)
        {
            var data = await _context.DeleteProjectMilestone(id);
            return Ok(data);
        }
    }
}
