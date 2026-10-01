using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ILeadService
    {
        Task<APIResponse> GetAllLead(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetLeadById(int id);
        Task<APIResponse> CreateLead(CreateLead dto);
        Task<APIResponse> UpdateLead(EditLead req);
        Task<APIResponse> DeleteLead(int id);
        Task<APIResponse> GetAllLeadList(
   string? search,
   string? filterField,
   string? filterValue,
   string? sortField,
   string? sortOrder = "asc",
   int pageNumber = 1,
   int pageSize = 10);
    }
}
