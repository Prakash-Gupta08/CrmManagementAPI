using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<APIResponse> GetAllPayment(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetPaymentById(int id);
        Task<APIResponse> CreatePayment(CreatePayment dto);
        Task<APIResponse> UpdatePayment(EditPayment req);
        Task<APIResponse> DeletePayment(int id);
    }
}
