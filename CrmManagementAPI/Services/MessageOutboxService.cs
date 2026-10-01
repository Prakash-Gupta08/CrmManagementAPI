using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<APIResponse> GetAllMessageOutboxList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.message_outbox.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(search) || x.Channel.ToLower().Contains(search) ||
                    x.UseCase.ToLower().Contains(search) ||(x.RecipientName != null && x.RecipientName.ToLower().Contains(search)) ||
                    (x.RecipientEmail != null && x.RecipientEmail.ToLower().Contains(search)) ||(x.RecipientMobile != null && x.RecipientMobile.ToLower().Contains(search)) ||
                    (x.Subject != null && x.Subject.ToLower().Contains(search)) ||x.BodyRendered.ToLower().Contains(search) ||
                    x.Status.ToLower().Contains(search) ||(x.DeliveryRef != null && x.DeliveryRef.ToLower().Contains(search)) ||
                    (x.ErrorMessage != null && x.ErrorMessage.ToLower().Contains(search)) ||(x.Notes != null && x.Notes.ToLower().Contains(search))
                );
            }

            if (!string.IsNullOrWhiteSpace(filterField) &&
                !string.IsNullOrWhiteSpace(filterValue))
            {
                filterField = filterField.Trim().ToLower();
                filterValue = filterValue.Trim().ToLower();
                switch (filterField)
                {
                    case "id":
                        if (int.TryParse(filterValue, out int id))
                        {
                            query = query.Where(x => x.Id == id);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ID value.";
                            return _response;
                        }
                        break;

                    case "channel":
                        query = query.Where(x =>
                            x.Channel.ToLower().Contains(filterValue));
                        break;

                    case "usecase":
                        query = query.Where(x =>
                            x.UseCase.ToLower().Contains(filterValue));
                        break;

                    case "templateid":
                        if (int.TryParse(filterValue, out int templateId))
                        {
                            query = query.Where(x =>
                                x.TemplateId == templateId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TemplateId value.";
                            return _response;
                        }
                        break;

                    case "invoiceid":
                        if (int.TryParse(filterValue, out int invoiceId))
                        {
                            query = query.Where(x =>
                                x.InvoiceId == invoiceId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid InvoiceId value.";
                            return _response;
                        }
                        break;

                    case "tenderid":
                        if (int.TryParse(filterValue, out int tenderId))
                        {
                            query = query.Where(x =>
                                x.TenderId == tenderId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TenderId value.";
                            return _response;
                        }
                        break;

                    case "customerid":
                        if (int.TryParse(filterValue, out int customerId))
                        {
                            query = query.Where(x =>
                                x.CustomerId == customerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CustomerId value.";
                            return _response;
                        }
                        break;

                    case "recipientname":
                        query = query.Where(x =>
                            x.RecipientName != null &&
                            x.RecipientName.ToLower().Contains(filterValue));
                        break;

                    case "recipientemail":
                        query = query.Where(x =>
                            x.RecipientEmail != null &&
                            x.RecipientEmail.ToLower().Contains(filterValue));
                        break;

                    case "recipientmobile":
                        query = query.Where(x =>
                            x.RecipientMobile != null &&
                            x.RecipientMobile.ToLower().Contains(filterValue));
                        break;

                    case "subject":
                        query = query.Where(x =>
                            x.Subject != null &&
                            x.Subject.ToLower().Contains(filterValue));
                        break;

                    case "bodyrendered":
                        query = query.Where(x =>
                            x.BodyRendered.ToLower().Contains(filterValue));
                        break;

                    case "status":
                        query = query.Where(x =>
                            x.Status.ToLower().Contains(filterValue));
                        break;

                    case "sentbyid":
                        if (int.TryParse(filterValue, out int sentById))
                        {
                            query = query.Where(x =>
                                x.SentById == sentById);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid SentById value.";
                            return _response;
                        }
                        break;

                    case "scheduledat":
                        if (DateTime.TryParse(filterValue, out DateTime scheduledAt))
                        {
                            query = query.Where(x =>
                                x.ScheduledAt != null &&
                                x.ScheduledAt.Value.Date == scheduledAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ScheduledAt value.";
                            return _response;
                        }
                        break;

                    case "sentat":
                        if (DateTime.TryParse(filterValue, out DateTime sentAt))
                        {
                            query = query.Where(x =>
                                x.SentAt != null &&
                                x.SentAt.Value.Date == sentAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid SentAt value.";
                            return _response;
                        }
                        break;

                    case "deliveryref":
                        query = query.Where(x =>
                            x.DeliveryRef != null &&
                            x.DeliveryRef.ToLower().Contains(filterValue));
                        break;

                    case "errormessage":
                        query = query.Where(x =>
                            x.ErrorMessage != null &&
                            x.ErrorMessage.ToLower().Contains(filterValue));
                        break;

                    case "notes":
                        query = query.Where(x =>
                            x.Notes != null &&
                            x.Notes.ToLower().Contains(filterValue));
                        break;

                    case "createdat":
                        if (DateTime.TryParse(filterValue, out DateTime createdAt))
                        {
                            query = query.Where(x =>
                                x.CreatedAt.Date == createdAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CreatedAt value.";
                            return _response;
                        }
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        _response.ActionResponse =
                            $"Invalid filter field: {filterField}";
                        return _response;
                }
            }

            sortField = sortField?.Trim().ToLower();
            sortOrder = sortOrder?.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(sortField))
            {
                bool descending = sortOrder == "desc";

                switch (sortField)
                {
                    case "id":
                        query = descending
                            ? query.OrderByDescending(x => x.Id)
                            : query.OrderBy(x => x.Id);
                        break;

                    case "channel":
                        query = descending
                            ? query.OrderByDescending(x => x.Channel)
                            : query.OrderBy(x => x.Channel);
                        break;

                    case "usecase":
                        query = descending
                            ? query.OrderByDescending(x => x.UseCase)
                            : query.OrderBy(x => x.UseCase);
                        break;

                    case "templateid":
                        query = descending
                            ? query.OrderByDescending(x => x.TemplateId)
                            : query.OrderBy(x => x.TemplateId);
                        break;

                    case "invoiceid":
                        query = descending
                            ? query.OrderByDescending(x => x.InvoiceId)
                            : query.OrderBy(x => x.InvoiceId);
                        break;

                    case "tenderid":
                        query = descending
                            ? query.OrderByDescending(x => x.TenderId)
                            : query.OrderBy(x => x.TenderId);
                        break;

                    case "customerid":
                        query = descending
                            ? query.OrderByDescending(x => x.CustomerId)
                            : query.OrderBy(x => x.CustomerId);
                        break;

                    case "recipientname":
                        query = descending
                            ? query.OrderByDescending(x => x.RecipientName)
                            : query.OrderBy(x => x.RecipientName);
                        break;

                    case "recipientemail":
                        query = descending
                            ? query.OrderByDescending(x => x.RecipientEmail)
                            : query.OrderBy(x => x.RecipientEmail);
                        break;

                    case "recipientmobile":
                        query = descending
                            ? query.OrderByDescending(x => x.RecipientMobile)
                            : query.OrderBy(x => x.RecipientMobile);
                        break;

                    case "subject":
                        query = descending
                            ? query.OrderByDescending(x => x.Subject)
                            : query.OrderBy(x => x.Subject);
                        break;

                    case "bodyrendered":
                        query = descending
                            ? query.OrderByDescending(x => x.BodyRendered)
                            : query.OrderBy(x => x.BodyRendered);
                        break;

                    case "status":
                        query = descending
                            ? query.OrderByDescending(x => x.Status)
                            : query.OrderBy(x => x.Status);
                        break;

                    case "sentbyid":
                        query = descending
                            ? query.OrderByDescending(x => x.SentById)
                            : query.OrderBy(x => x.SentById);
                        break;

                    case "scheduledat":
                        query = descending
                            ? query.OrderByDescending(x => x.ScheduledAt)
                            : query.OrderBy(x => x.ScheduledAt);
                        break;

                    case "sentat":
                        query = descending
                            ? query.OrderByDescending(x => x.SentAt)
                            : query.OrderBy(x => x.SentAt);
                        break;

                    case "deliveryref":
                        query = descending
                            ? query.OrderByDescending(x => x.DeliveryRef)
                            : query.OrderBy(x => x.DeliveryRef);
                        break;

                    case "errormessage":
                        query = descending
                            ? query.OrderByDescending(x => x.ErrorMessage)
                            : query.OrderBy(x => x.ErrorMessage);
                        break;

                    case "notes":
                        query = descending
                            ? query.OrderByDescending(x => x.Notes)
                            : query.OrderBy(x => x.Notes);
                        break;

                    case "createdat":
                        query = descending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        _response.ActionResponse =
                            $"Invalid sort field: {sortField}";
                        return _response;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }

            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!data.Any())
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.ActionResponse = "Data not found.";
                return _response;
            }

            _response.IsSuccess = true;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.ActionResponse = "Data found successfully.";

            _response.Result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = data
            };

            return _response;
        }
    }
}
