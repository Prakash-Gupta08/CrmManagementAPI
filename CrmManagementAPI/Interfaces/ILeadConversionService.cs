using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ILeadConversionService
    {
        Task<APIResponse> GetAllLeadConversion(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetLeadConversionById(int id);
        Task<APIResponse> CreateLeadConversion(CreateLeadConversion dto);
        Task<APIResponse> UpdateLeadConversion(EditLeadConversion req);
        Task<APIResponse> DeleteLeadConversion(int id);
    }
}
