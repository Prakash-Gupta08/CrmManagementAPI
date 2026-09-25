using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IEscalationPolicyService
    {
        Task<APIResponse> GetAllEscalationPolicy(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetEscalationPolicyById(int id);
        Task<APIResponse> CreateEscalationPolicy(CreateEscalationPolicy dto);
        Task<APIResponse> UpdateEscalationPolicy(EditEscalationPolicy req);
        Task<APIResponse> DeleteEscalationPolicy(int id);
    }
}
