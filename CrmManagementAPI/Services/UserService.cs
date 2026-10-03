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

        public async Task<APIResponse> GetAllUser(string? employeeID, string? search = null, int pageNumber = 1, int pageSize = 10)
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

        public async Task<APIResponse> GetAllUsersList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.users
                .Where(x => x.Active == true);

            // =========================
            // Search
            // =========================
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||
                    x.EmployeeId.ToLower().Contains(search) ||
                    x.FullName.ToLower().Contains(search) ||
                    (x.Designation != null && x.Designation.ToLower().Contains(search)) ||
                    (x.Department != null && x.Department.ToLower().Contains(search)) ||
                    (x.ReportingManager != null && x.ReportingManager.ToLower().Contains(search)) ||
                    (x.Region != null && x.Region.ToLower().Contains(search)) ||
                    (x.GovernmentVertical != null && x.GovernmentVertical.ToLower().Contains(search)) ||
                    x.Email.ToLower().Contains(search) ||
                    (x.Mobile != null && x.Mobile.ToLower().Contains(search)) ||
                    x.Role.ToLower().Contains(search) ||
                    x.Active.ToString().ToLower().Contains(search)
                );
            }

            // =========================
            // Filter
            // =========================
            if (!string.IsNullOrEmpty(filterField) &&
                !string.IsNullOrEmpty(filterValue))
            {
                switch (filterField.ToLower())
                {
                    case "id":
                        if (int.TryParse(filterValue, out int id))
                        {
                            query = query.Where(x => x.Id == id);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid Id.";
                            return _response;
                        }
                        break;

                    case "employeeid":
                        query = query.Where(x =>
                            x.EmployeeId.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "fullname":
                        query = query.Where(x =>
                            x.FullName.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "designation":
                        query = query.Where(x =>
                            x.Designation != null &&
                            x.Designation.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "department":
                        query = query.Where(x =>
                            x.Department != null &&
                            x.Department.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "reportingmanager":
                        query = query.Where(x =>
                            x.ReportingManager != null &&
                            x.ReportingManager.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "region":
                        query = query.Where(x =>
                            x.Region != null &&
                            x.Region.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "governmentvertical":
                        query = query.Where(x =>
                            x.GovernmentVertical != null &&
                            x.GovernmentVertical.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "email":
                        query = query.Where(x =>
                            x.Email.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "mobile":
                        query = query.Where(x =>
                            x.Mobile != null &&
                            x.Mobile.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "role":
                        query = query.Where(x =>
                            x.Role.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "active":
                        if (bool.TryParse(filterValue, out bool active))
                        {
                            query = query.Where(x => x.Active == active);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid Active value. Use true or false.";
                            return _response;
                        }
                        break;

                    case "createdat":
                        if (DateTime.TryParse(filterValue, out DateTime createdAt))
                        {
                            query = query.Where(x =>
                                x.CreatedAt.Date == createdAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CreatedAt date.";
                            return _response;
                        }
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        _response.ActionResponse = "Invalid filter field.";
                        return _response;
                }
            }

            // =========================
            // Sorting
            // =========================
            bool isDescending = sortOrder?.ToLower() == "desc";

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "id":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Id)
                            : query.OrderBy(x => x.Id);
                        break;

                    case "employeeid":
                        query = isDescending
                            ? query.OrderByDescending(x => x.EmployeeId)
                            : query.OrderBy(x => x.EmployeeId);
                        break;

                    case "fullname":
                        query = isDescending
                            ? query.OrderByDescending(x => x.FullName)
                            : query.OrderBy(x => x.FullName);
                        break;

                    case "designation":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Designation)
                            : query.OrderBy(x => x.Designation);
                        break;

                    case "department":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Department)
                            : query.OrderBy(x => x.Department);
                        break;

                    case "reportingmanager":
                        query = isDescending
                            ? query.OrderByDescending(x => x.ReportingManager)
                            : query.OrderBy(x => x.ReportingManager);
                        break;

                    case "region":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Region)
                            : query.OrderBy(x => x.Region);
                        break;

                    case "governmentvertical":
                        query = isDescending
                            ? query.OrderByDescending(x => x.GovernmentVertical)
                            : query.OrderBy(x => x.GovernmentVertical);
                        break;

                    case "email":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Email)
                            : query.OrderBy(x => x.Email);
                        break;

                    case "mobile":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Mobile)
                            : query.OrderBy(x => x.Mobile);
                        break;

                    case "role":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Role)
                            : query.OrderBy(x => x.Role);
                        break;

                    case "active":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Active)
                            : query.OrderBy(x => x.Active);
                        break;

                    case "createdat":
                        query = isDescending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);
                        break;

                    default:
                        query = query.OrderBy(x => x.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }

            // =========================
            // Pagination
            // =========================
            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // =========================
            // No Data Found
            // =========================
            if (data == null || data.Count == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.ActionResponse = "Data not found.";
                return _response;
            }

            // =========================
            // Success Response
            // =========================
            _response.IsSuccess = true;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.ActionResponse = "Data found successfully.";

            _response.Result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = data
            };

            return _response;
        }


    }
    
}
