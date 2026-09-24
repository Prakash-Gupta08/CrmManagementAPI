using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CrmManagementAPI.Services
{
    public class UserService : IUserService
    {
    private readonly db_context _context;
    protected APIResponse _response;
    public UserService(db_context sqlDbcontext)
    {
    _context = sqlDbcontext;
    _response = new APIResponse();

    }

        public async Task<APIResponse> GetAllUser()
        {
            var data = await _context.users.Where(s => s.Active == true).ToListAsync();
            if (data == null) 
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "data not found.";
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.ActionResponse = "Data found successfully.";
            _response.Result = data;
            return _response;
            
        }

        public async Task<APIResponse> GetUserById(int id)
        {
            var data = await _context.users.FindAsync(id);
            if(data == null)
            {
            _response.IsSuccess = false;
            _response.ActionResponse = "Incorrect user id.";
            _response.StatusCode = System.Net.HttpStatusCode.NotFound;
             return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.ActionResponse = $"User id with {id} found successfully.";
            _response.Result = data;
            return _response;

        }

        public async Task<APIResponse> CreateUser(CreateUserDto dto)
        {
            var data = await _context.users.FirstOrDefaultAsync(s => s.EmployeeId == dto.EmployeeId && s.Active == true);
            if(data != null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record id already exist.";
                return _response;
            }
            if(data.Mobile == dto.Mobile)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.ActionResponse = "Mobile no already exist.";
                return _response;
            }
            if(data.Email == dto.Email)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.ActionResponse = "This email already registered.";
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.ActionResponse = "User created successfully.";
            _response.Result = data;
            return _response;
        }



    }
    
}
