using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;

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

        public async Task<APIResponse> GetAllUser(string employeeID, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.users.Where(x => x.Active);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.EmployeeId.ToLower().Contains(search));
            }
            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.EmployeeId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!data.Any())
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "data not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Data found successfully.";
            _response.Result = new 
            {
            PageNumber = pageNumber, 
            PageSize = pageSize, 
            TotalCount = totalCount, 
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            Data = data,
            }; 
            return _response;
            
        }

        public async Task<APIResponse> GetUserById(int id)
        {
            var data = await _context.users.FindAsync(id);
            if(data == null)
            {
            _response.IsSuccess = false;
            _response.ActionResponse = "Incorrect user id.";
            _response.StatusCode = HttpStatusCode.NotFound;
             return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
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
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exist.";
                return _response;
            }
            var emailExists = await _context.users.AnyAsync(s => s.Email == dto.Email && s.Active == true);
            if (emailExists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "This email already registered.";
                return _response;
            }
            var mobileExists = await _context.users.AnyAsync(s => s.Mobile == dto.Mobile && s.Active == true);
            if (mobileExists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "This Mobile already registered.";
                return _response;
            }
            var user = new users
            {
                EmployeeId = dto.EmployeeId,
                FullName = dto.FullName,
                Designation = dto.Designation,
                Department = dto.Department,
                ReportingManager = dto.ReportingManager,
                Region = dto.Region,
                GovernmentVertical = dto.GovernmentVertical,
                Email = dto.Email,
                Mobile = dto.Mobile,
                Role = dto.Role,
                Active = true
            };
            _context.users.Add(user); 
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "User created successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> UpdateUser(EditUser req)
        {
            var data = await _context.users.FirstOrDefaultAsync(s => s.Id == req.Id && s.Active == true);
            if(data == null)
            {
                _response.IsSuccess=false;
                _response.ActionResponse = "User not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;   

            }
            if(string.IsNullOrWhiteSpace(data.Email))
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Enter valid email id.";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;

            }

            if (string.IsNullOrWhiteSpace(data.Role))
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Enter the role name.";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;

            }
            if (data.EmployeeId == req.EmployeeId)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Employee id already exist.";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;

            }
            data.Id = req.Id; 
            data.EmployeeId = req.EmployeeId;
            data.FullName = req.FullName;
            data.Designation = req.Designation;
            data.Department = req.Department;
            data.ReportingManager = req.ReportingManager;
            data.Region = req.Region;
            data.GovernmentVertical = req.GovernmentVertical;
            data.Email = req.Email;
            data.Mobile = req.Mobile;
            data.Role = req.Role;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "User updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result= data;
            return _response;

        }

        public async Task<APIResponse> DeleteUser(int id)
        {
            var data = await _context.users
                .FirstOrDefaultAsync(x => x.Id == id && x.Active == true);

            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "User not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.Active = false;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "User deleted successfully.";
            _response.Result = new
            {
                Id = data.Id,
                EmployeeId = data.EmployeeId,
                Active = data.Active
            };

            return _response;
        }


    }
    
}
