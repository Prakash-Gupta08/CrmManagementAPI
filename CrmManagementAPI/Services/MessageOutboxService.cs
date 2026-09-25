using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class MessageOutboxService : IMessageOutboxService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public MessageOutboxService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllMessageOutbox(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.message_outbox.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.RecipientName != null && s.RecipientName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.Id)
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

        public async Task<APIResponse> GetMessageOutboxById(int id)
        {
            var data = await _context.message_outbox.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect messageoutbox id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateMessageOutbox(CreateMessageOutbox dto)
        {
            var entity = new message_outbox
            {
                Channel = dto.Channel,
                UseCase = dto.UseCase,
                TemplateId = dto.TemplateId,
                InvoiceId = dto.InvoiceId,
                TenderId = dto.TenderId,
                CustomerId = dto.CustomerId,
                RecipientName = dto.RecipientName,
                RecipientEmail = dto.RecipientEmail,
                RecipientMobile = dto.RecipientMobile,
                Subject = dto.Subject,
                BodyRendered = dto.BodyRendered,
                Status = dto.Status,
                SentById = dto.SentById,
                ScheduledAt = dto.ScheduledAt,
                SentAt = dto.SentAt,
                DeliveryRef = dto.DeliveryRef,
                ErrorMessage = dto.ErrorMessage,
                Notes = dto.Notes,
                CreatedAt = dto.CreatedAt,
            };
            _context.message_outbox.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "MessageOutbox created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateMessageOutbox(EditMessageOutbox req)
        {
            var data = await _context.message_outbox.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "MessageOutbox not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.Channel = req.Channel;
            data.UseCase = req.UseCase;
            data.TemplateId = req.TemplateId;
            data.InvoiceId = req.InvoiceId;
            data.TenderId = req.TenderId;
            data.CustomerId = req.CustomerId;
            data.RecipientName = req.RecipientName;
            data.RecipientEmail = req.RecipientEmail;
            data.RecipientMobile = req.RecipientMobile;
            data.Subject = req.Subject;
            data.BodyRendered = req.BodyRendered;
            data.Status = req.Status;
            data.SentById = req.SentById;
            data.ScheduledAt = req.ScheduledAt;
            data.SentAt = req.SentAt;
            data.DeliveryRef = req.DeliveryRef;
            data.ErrorMessage = req.ErrorMessage;
            data.Notes = req.Notes;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "MessageOutbox updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteMessageOutbox(int id)
        {
            var data = await _context.message_outbox.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "MessageOutbox not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.message_outbox.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "MessageOutbox deleted successfully.";
            return _response;
        }
    }
}
