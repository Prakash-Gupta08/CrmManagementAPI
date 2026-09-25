using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ITenderDocumentService
    {
        Task<APIResponse> GetAllTenderDocument(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetTenderDocumentById(int id);
        Task<APIResponse> CreateTenderDocument(CreateTenderDocument dto);
        Task<APIResponse> UpdateTenderDocument(EditTenderDocument req);
        Task<APIResponse> DeleteTenderDocument(int id);
    }
}
