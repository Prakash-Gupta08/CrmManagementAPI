using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IInvoiceService
    {
        Task<APIResponse> GetAllInvoice(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetInvoiceById(int id);
        Task<APIResponse> CreateInvoice(CreateInvoice dto);
        Task<APIResponse> UpdateInvoice(EditInvoice req);
        Task<APIResponse> DeleteInvoice(int id);
        Task<APIResponse> GetInvoiceStatusDropdown();
        Task<APIResponse> GetAllInvoiceList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10);
    }
}
