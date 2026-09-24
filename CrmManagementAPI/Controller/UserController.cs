using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
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

        public async Task<ActionResult> GetAllUser()
        {
            var data = await _context.GetAllUser();
            
            return Ok(data);

        }
        [HttpGet("GetUSerByID")]
        public async Task<ActionResult> GetUserById(int id)
        {
        var data = await _context.GetUserById(id);
            return Ok(data);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(CreateUserDto dto)
        {
            var data = await _context.CreateUser(dto);
            if(data == null)
            {
                return null;
            }
            return Ok(data);
        }


    }
}
