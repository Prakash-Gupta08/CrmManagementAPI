using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class LeadActivityService : ILeadActivityService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public LeadActivityService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllLeadActivity(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.lead_activities.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.ActivityType != null && s.ActivityType.ToLower().Contains(search));
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

        public async Task<APIResponse> GetLeadActivityById(int id)
        {
            var data = await _context.lead_activities.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect leadactivity id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateLeadActivity(CreateLeadActivity dto)
        {
            var entity = new lead_activities
            {
                LeadId = dto.LeadId,
                OwnerId = dto.OwnerId,
                ActivityType = dto.ActivityType,
                ActivityDate = dto.ActivityDate,
                FromStage = dto.FromStage,
                ToStage = dto.ToStage,
                Notes = dto.Notes,
                NextAction = dto.NextAction,
                NextActionDate = dto.NextActionDate,
                CreatedAt = dto.CreatedAt,
            };
            _context.lead_activities.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "LeadActivity created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateLeadActivity(EditLeadActivity req)
        {
            var data = await _context.lead_activities.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "LeadActivity not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.LeadId = req.LeadId;
            data.OwnerId = req.OwnerId;
            data.ActivityType = req.ActivityType;
            data.ActivityDate = req.ActivityDate;
            data.FromStage = req.FromStage;
            data.ToStage = req.ToStage;
            data.Notes = req.Notes;
            data.NextAction = req.NextAction;
            data.NextActionDate = req.NextActionDate;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "LeadActivity updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteLeadActivity(int id)
        {
            var data = await _context.lead_activities.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "LeadActivity not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.lead_activities.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "LeadActivity deleted successfully.";
            return _response;
        }
    }
}
