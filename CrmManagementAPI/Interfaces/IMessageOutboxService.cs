using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IMessageOutboxService
    {
        Task<APIResponse> GetAllMessageOutbox(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetMessageOutboxById(int id);
        Task<APIResponse> CreateMessageOutbox(CreateMessageOutbox dto);
        Task<APIResponse> UpdateMessageOutbox(EditMessageOutbox req);
        Task<APIResponse> DeleteMessageOutbox(int id);
    }
}
