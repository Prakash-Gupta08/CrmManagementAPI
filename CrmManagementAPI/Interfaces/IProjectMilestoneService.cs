using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IProjectMilestoneService
    {
        Task<APIResponse> GetAllProjectMilestone(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetProjectMilestoneById(int id);
        Task<APIResponse> CreateProjectMilestone(CreateProjectMilestone dto);
        Task<APIResponse> UpdateProjectMilestone(EditProjectMilestone req);
        Task<APIResponse> DeleteProjectMilestone(int id);
    }
}
