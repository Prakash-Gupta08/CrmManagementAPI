using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class EscalationPolicyService : IEscalationPolicyService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public EscalationPolicyService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllEscalationPolicy(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.escalation_policy.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.PolicyName != null && s.PolicyName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.PolicyName)
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

        public async Task<APIResponse> GetEscalationPolicyById(int id)
        {
            var data = await _context.escalation_policy.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect escalationpolicy id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateEscalationPolicy(CreateEscalationPolicy dto)
        {
            var entity = new escalation_policy
            {
                PolicyName = dto.PolicyName,
                AgingDays = dto.AgingDays,
                SeverityLevel = dto.SeverityLevel,
                Channel = dto.Channel,
                TemplateCode = dto.TemplateCode,
                EscalateToRole = dto.EscalateToRole,
                Active = dto.Active,
            };
            _context.escalation_policy.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "EscalationPolicy created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateEscalationPolicy(EditEscalationPolicy req)
        {
            var data = await _context.escalation_policy.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "EscalationPolicy not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.PolicyName = req.PolicyName;
            data.AgingDays = req.AgingDays;
            data.SeverityLevel = req.SeverityLevel;
            data.Channel = req.Channel;
            data.TemplateCode = req.TemplateCode;
            data.EscalateToRole = req.EscalateToRole;
            data.Active = req.Active;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "EscalationPolicy updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteEscalationPolicy(int id)
        {
            var data = await _context.escalation_policy.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "EscalationPolicy not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.escalation_policy.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "EscalationPolicy deleted successfully.";
            return _response;
        }

        public async Task<APIResponse> GetAllEscalationPolicyList(string? search , string? filterField ,string? filterValue ,
        string? sortField ,string? sortOrder = "asc",int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.escalation_policy.Where(x => x.Active == true);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x => x.PolicyName.ToLower().Contains(search) ||
                    x.SeverityLevel.ToLower().Contains(search) || x.Channel.ToLower().Contains(search) ||
                    x.TemplateCode.ToLower().Contains(search) || x.EscalateToRole.ToLower().Contains(search)
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
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ID value.";

                            return _response;
                        }
                        break;

                    case "policyname":
                        query = query.Where(x => x.PolicyName.ToLower().Contains(filterValue));
                        break;

                    case "agingdays":
                        if (int.TryParse(filterValue, out int agingDays))
                        {
                            query = query.Where(x => x.AgingDays == agingDays);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid AgingDays value.";

                            return _response;
                        }
                        break;

                    case "severitylevel":
                        query = query.Where(x => x.SeverityLevel.ToLower().Contains(filterValue));
                        break;

                    case "channel":
                        query = query.Where(x => x.Channel.ToLower().Contains(filterValue));

                        break;

                    case "templatecode":
                        query = query.Where(x => x.TemplateCode.ToLower().Contains(filterValue));
                        break;

                    case "escalatetorole":
                        query = query.Where(x => x.EscalateToRole.ToLower().Contains(filterValue));
                        break;

                    case "active":
                        if (bool.TryParse(filterValue, out bool active))
                        {
                            query = query.Where(x => x.Active == active);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid Active value. Use true or false.";

                            return _response;
                        }

                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ActionResponse = $"Invalid filter field: {filterField}";

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
                        query = descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                        break;

                    case "policyname":
                        query = descending ? query.OrderByDescending(x => x.PolicyName) : query.OrderBy(x => x.PolicyName);
                        break;

                    case "agingdays":
                        query = descending ? query.OrderByDescending(x => x.AgingDays) : query.OrderBy(x => x.AgingDays);
                        break;

                    case "severitylevel":
                        query = descending ? query.OrderByDescending(x => x.SeverityLevel): query.OrderBy(x => x.SeverityLevel);
                        break;

                    case "channel":
                        query = descending ? query.OrderByDescending(x => x.Channel) : query.OrderBy(x => x.Channel);
                        break;

                    case "templatecode":
                        query = descending ? query.OrderByDescending(x => x.TemplateCode) : query.OrderBy(x => x.TemplateCode);
                        break;

                    case "escalatetorole":
                        query = descending ? query.OrderByDescending(x => x.EscalateToRole) : query.OrderBy(x => x.EscalateToRole);
                        break;

                    case "active":
                        query = descending ? query.OrderByDescending(x => x.Active) : query.OrderBy(x => x.Active);
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ActionResponse = $"Invalid sort field: {sortField}";

                        return _response;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!data.Any())
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ActionResponse = "Data not found.";

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
                TotalPages = totalPages,
                Data = data
            };
            return _response;
        }

    }
}
