using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IUserService
    {
        Task<APIResponse> GetAllUser();
        Task<APIResponse> GetUserById(int id);
        Task<APIResponse> CreateUser(CreateUserDto dto);
       

    }
}
