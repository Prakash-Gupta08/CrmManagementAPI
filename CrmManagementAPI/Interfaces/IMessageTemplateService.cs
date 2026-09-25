using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model;

namespace CrmManagementAPI.Interfaces
{
    public interface IMessageTemplateService
    {
        Task<APIResponse> GetAllMessageTemplate(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<APIResponse> GetMessageTemplateById(int id);
        Task<APIResponse> CreateMessageTemplate(CreateMessageTemplate dto);
        Task<APIResponse> UpdateMessageTemplate(EditMessageTemplate req);
        Task<APIResponse> DeleteMessageTemplate(int id);
    }
}
