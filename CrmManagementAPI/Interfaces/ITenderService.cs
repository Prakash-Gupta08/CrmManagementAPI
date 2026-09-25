using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ITenderService
    {
        Task<APIResponse> GetAllTender(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetTenderById(int id);
        Task<APIResponse> CreateTender(CreateTender dto);
        Task<APIResponse> UpdateTender(EditTender req);
        Task<APIResponse> DeleteTender(int id);
    }
}
