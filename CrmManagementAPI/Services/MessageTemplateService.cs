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
        public async Task<APIResponse> GetAllMessageTemplatesList(string? search,string? filterField,string? filterValue,string? sortField,
        string? sortOrder = "asc",int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.message_templates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||
                    x.TemplateCode.ToLower().Contains(search) ||
                    x.TemplateName.ToLower().Contains(search) ||
                    x.Channel.ToLower().Contains(search) ||
                    x.UseCase.ToLower().Contains(search) ||
                    (x.SeverityLevel != null &&
                     x.SeverityLevel.ToLower().Contains(search)) ||
                    (x.Subject != null &&
                     x.Subject.ToLower().Contains(search)) ||
                    x.BodyText.ToLower().Contains(search) ||
                    (x.Variables != null &&
                     x.Variables.ToLower().Contains(search))
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ID value.";
                            return _response;
                        }
                        break;

                    case "templatecode":
                        query = query.Where(x =>
                            x.TemplateCode.ToLower().Contains(filterValue));
                        break;

                    case "templatename":
                        query = query.Where(x =>
                            x.TemplateName.ToLower().Contains(filterValue));
                        break;

                    case "channel":
                        query = query.Where(x =>
                            x.Channel.ToLower().Contains(filterValue));
                        break;

                    case "usecase":
                        query = query.Where(x =>
                            x.UseCase.ToLower().Contains(filterValue));
                        break;

                    case "severitylevel":
                        query = query.Where(x =>
                            x.SeverityLevel != null &&
                            x.SeverityLevel.ToLower().Contains(filterValue));
                        break;

                    case "agingdaysmin":
                        if (int.TryParse(filterValue, out int agingDaysMin))
                        {
                            query = query.Where(x =>
                                x.AgingDaysMin == agingDaysMin);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid AgingDaysMin value.";
                            return _response;
                        }
                        break;

                    case "agingdaysmax":
                        if (int.TryParse(filterValue, out int agingDaysMax))
                        {
                            query = query.Where(x =>
                                x.AgingDaysMax == agingDaysMax);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid AgingDaysMax value.";
                            return _response;
                        }
                        break;

                    case "leadtimedays":
                        if (int.TryParse(filterValue, out int leadTimeDays))
                        {
                            query = query.Where(x =>
                                x.LeadTimeDays == leadTimeDays);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid LeadTimeDays value.";
                            return _response;
                        }
                        break;

                    case "subject":
                        query = query.Where(x =>
                            x.Subject != null &&
                            x.Subject.ToLower().Contains(filterValue));
                        break;

                    case "bodytext":
                        query = query.Where(x =>
                            x.BodyText.ToLower().Contains(filterValue));
                        break;

                    case "variables":
                        query = query.Where(x =>
                            x.Variables != null &&
                            x.Variables.ToLower().Contains(filterValue));
                        break;

                    case "active":
                        if (bool.TryParse(filterValue, out bool active))
                        {
                            query = query.Where(x =>
                                x.Active == active);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid Active value. Use true or false.";
                            return _response;
                        }
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid CreatedAt value.";
                            return _response;
                        }
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode =
                            System.Net.HttpStatusCode.BadRequest;
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

                    case "templatecode":
                        query = descending
                            ? query.OrderByDescending(x => x.TemplateCode)
                            : query.OrderBy(x => x.TemplateCode);
                        break;

                    case "templatename":
                        query = descending
                            ? query.OrderByDescending(x => x.TemplateName)
                            : query.OrderBy(x => x.TemplateName);
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

                    case "severitylevel":
                        query = descending
                            ? query.OrderByDescending(x => x.SeverityLevel)
                            : query.OrderBy(x => x.SeverityLevel);
                        break;

                    case "agingdaysmin":
                        query = descending
                            ? query.OrderByDescending(x => x.AgingDaysMin)
                            : query.OrderBy(x => x.AgingDaysMin);
                        break;

                    case "agingdaysmax":
                        query = descending
                            ? query.OrderByDescending(x => x.AgingDaysMax)
                            : query.OrderBy(x => x.AgingDaysMax);
                        break;

                    case "leadtimedays":
                        query = descending
                            ? query.OrderByDescending(x => x.LeadTimeDays)
                            : query.OrderBy(x => x.LeadTimeDays);
                        break;

                    case "subject":
                        query = descending
                            ? query.OrderByDescending(x => x.Subject)
                            : query.OrderBy(x => x.Subject);
                        break;

                    case "bodytext":
                        query = descending
                            ? query.OrderByDescending(x => x.BodyText)
                            : query.OrderBy(x => x.BodyText);
                        break;

                    case "variables":
                        query = descending
                            ? query.OrderByDescending(x => x.Variables)
                            : query.OrderBy(x => x.Variables);
                        break;

                    case "active":
                        query = descending
                            ? query.OrderByDescending(x => x.Active)
                            : query.OrderBy(x => x.Active);
                        break;

                    case "createdat":
                        query = descending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode =
                            System.Net.HttpStatusCode.BadRequest;
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
                _response.StatusCode =
                    System.Net.HttpStatusCode.NotFound;
                _response.ActionResponse =
                    "Data not found.";
                return _response;
            }

            _response.IsSuccess = true;
            _response.StatusCode =
                System.Net.HttpStatusCode.OK;
            _response.ActionResponse =
                "Data found successfully.";

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
