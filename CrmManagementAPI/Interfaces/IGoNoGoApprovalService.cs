using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IGoNoGoApprovalService
    {
        Task<APIResponse> GetAllGoNoGoApproval(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetGoNoGoApprovalById(int id);
        Task<APIResponse> CreateGoNoGoApproval(CreateGoNoGoApproval dto);
        Task<APIResponse> UpdateGoNoGoApproval(EditGoNoGoApproval req);
        Task<APIResponse> DeleteGoNoGoApproval(int id);
    }
}
