using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _context;
        protected APIResponse _response;
        public UserController(IUserService context)
        {
            _context = context;
            _response = new();
        }
        [HttpGet("GetAllUsers")]

        public async Task<ActionResult> GetAllUser(string? employeeID, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllUser(employeeID, search, pageNumber, pageSize);

            return Ok(data);

        }
        [HttpGet("GetUSerByID")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var data = await _context.GetUserById(id);
            return Ok(data);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var data = await _context.CreateUser(dto);
            if (data == null)
            {
                return null;
            }
            _response.IsSuccess = true;
            _response.ActionResponse = "data created";
            return Ok(data);
        }

        [HttpPost("UpdateUser")]
        public async Task<ActionResult> UpdateUser([FromBody] EditUser req)
        {
            var data = await _context.UpdateUser(req);
            if (data == null)
            {

                return null;
            }
            _response.IsSuccess = true;
            _response.ActionResponse = "data found";

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var data = await _context.DeleteUser(id);
            return Ok();

        }
    }
}
