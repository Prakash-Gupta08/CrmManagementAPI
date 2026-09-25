using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ITenderAlertService
    {
        Task<APIResponse> GetAllTenderAlert(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetTenderAlertById(int id);
        Task<APIResponse> CreateTenderAlert(CreateTenderAlert dto);
        Task<APIResponse> UpdateTenderAlert(EditTenderAlert req);
        Task<APIResponse> DeleteTenderAlert(int id);
    }
}
