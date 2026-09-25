using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ITenderActivityService
    {
        Task<APIResponse> GetAllTenderActivity(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetTenderActivityById(int id);
        Task<APIResponse> CreateTenderActivity(CreateTenderActivity dto);
        Task<APIResponse> UpdateTenderActivity(EditTenderActivity req);
        Task<APIResponse> DeleteTenderActivity(int id);
    }
}
