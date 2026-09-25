using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ILeadActivityService
    {
        Task<APIResponse> GetAllLeadActivity(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetLeadActivityById(int id);
        Task<APIResponse> CreateLeadActivity(CreateLeadActivity dto);
        Task<APIResponse> UpdateLeadActivity(EditLeadActivity req);
        Task<APIResponse> DeleteLeadActivity(int id);
        Task<APIResponse> GetLeadActivityDropdown();
    }
}
