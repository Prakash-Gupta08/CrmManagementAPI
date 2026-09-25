using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface ICustomerService
    {
        Task<APIResponse> GetAllCustomer( string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetCustomerById(int id);
        Task<APIResponse> CreateCustomer(CreateCustomer dto); 
        Task<APIResponse> UpdateCustomer(EditCustomer req);
        Task<APIResponse> DeleteCustomer(int id);
        Task<APIResponse> GetCategoryDropdown();
        Task<APIResponse> GetProjectRiskDropdown();
        Task<APIResponse> GetDecisionDropdown();


    }
}
