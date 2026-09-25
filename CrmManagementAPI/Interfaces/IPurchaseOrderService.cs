using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<APIResponse> GetAllPurchaseOrder(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetPurchaseOrderById(int id);
        Task<APIResponse> CreatePurchaseOrder(CreatePurchaseOrder dto);
        Task<APIResponse> UpdatePurchaseOrder(EditPurchaseOrder req);
        Task<APIResponse> DeletePurchaseOrder(int id);
    }
}
