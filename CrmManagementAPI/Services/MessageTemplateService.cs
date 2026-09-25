using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class MessageTemplateService : IMessageTemplateService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public MessageTemplateService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllMessageTemplate(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.message_templates.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.TemplateName != null && s.TemplateName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.TemplateName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!data.Any())
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "data not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Data found successfully.";
            _response.Result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = data,
            };
            return _response;
        }

        public async Task<APIResponse> GetMessageTemplateById(int id)
        {
            var data = await _context.message_templates.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect messagetemplate id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateMessageTemplate(CreateMessageTemplate dto)
        {
            var exists = await _context.message_templates.AnyAsync(s => s.TemplateCode == dto.TemplateCode);
            if (exists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exists.";
                return _response;
            }

            var entity = new message_templates
            {
                TemplateCode = dto.TemplateCode,
                TemplateName = dto.TemplateName,
                Channel = dto.Channel,
                UseCase = dto.UseCase,
                SeverityLevel = dto.SeverityLevel,
                AgingDaysMin = dto.AgingDaysMin,
                AgingDaysMax = dto.AgingDaysMax,
                LeadTimeDays = dto.LeadTimeDays,
                Subject = dto.Subject,
                BodyText = dto.BodyText,
                Variables = dto.Variables,
                Active = dto.Active,
                CreatedAt = dto.CreatedAt,
            };
            _context.message_templates.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "MessageTemplate created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateMessageTemplate(EditMessageTemplate req)
        {
            var data = await _context.message_templates.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "MessageTemplate not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.TemplateCode = req.TemplateCode;
            data.TemplateName = req.TemplateName;
            data.Channel = req.Channel;
            data.UseCase = req.UseCase;
            data.SeverityLevel = req.SeverityLevel;
            data.AgingDaysMin = req.AgingDaysMin;
            data.AgingDaysMax = req.AgingDaysMax;
            data.LeadTimeDays = req.LeadTimeDays;
            data.Subject = req.Subject;
            data.BodyText = req.BodyText;
            data.Variables = req.Variables;
            data.Active = req.Active;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "MessageTemplate updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteMessageTemplate(int id)
        {
            var data = await _context.message_templates.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "MessageTemplate not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.message_templates.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "MessageTemplate deleted successfully.";
            return _response;
        }
    }
}
