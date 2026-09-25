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
    }
}
