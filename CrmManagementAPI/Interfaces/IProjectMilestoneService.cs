using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Interfaces
{
    public interface IProjectMilestoneService
    {
        Task<APIResponse> GetAllProjectMilestone(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetProjectMilestoneById(int id);
        Task<APIResponse> CreateProjectMilestone(CreateProjectMilestone dto);
        Task<APIResponse> UpdateProjectMilestone(EditProjectMilestone req);
        Task<APIResponse> DeleteProjectMilestone(int id);
        Task<APIResponse> GetAllProjectMilestonesList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10);
    }
}
