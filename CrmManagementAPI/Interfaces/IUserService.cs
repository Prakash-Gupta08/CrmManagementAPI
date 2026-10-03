using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IUserService
    {
        Task<APIResponse> GetAllUser(string? employeeID, string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetUserById(int id);
        Task<APIResponse> CreateUser(CreateUserDto dto);
        Task<APIResponse> UpdateUser(EditUser req);
        Task<APIResponse> DeleteUser(int id);
        Task<APIResponse> GetAllUsersList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10);


    }
}
