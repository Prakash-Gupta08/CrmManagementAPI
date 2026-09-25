using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class ProjectMilestoneService : IProjectMilestoneService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public ProjectMilestoneService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllProjectMilestone(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.project_milestones.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.MilestoneName != null && s.MilestoneName.ToLower().Contains(search));
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

        public async Task<APIResponse> GetProjectMilestoneById(int id)
        {
            var data = await _context.project_milestones.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect projectmilestone id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateProjectMilestone(CreateProjectMilestone dto)
        {
            var entity = new project_milestones
            {
                PoId = dto.PoId,
                MilestoneName = dto.MilestoneName,
                MilestoneType = dto.MilestoneType,
                PlannedDate = dto.PlannedDate,
                ActualDate = dto.ActualDate,
                BillingPct = dto.BillingPct,
                BillingAmountInr = dto.BillingAmountInr,
                Status = dto.Status,
                CompletionNotes = dto.CompletionNotes,
                Billable = dto.Billable,
                Invoiced = dto.Invoiced,
                SortOrder = dto.SortOrder,
                CreatedAt = dto.CreatedAt,
            };
            _context.project_milestones.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "ProjectMilestone created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateProjectMilestone(EditProjectMilestone req)
        {
            var data = await _context.project_milestones.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "ProjectMilestone not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.PoId = req.PoId;
            data.MilestoneName = req.MilestoneName;
            data.MilestoneType = req.MilestoneType;
            data.PlannedDate = req.PlannedDate;
            data.ActualDate = req.ActualDate;
            data.BillingPct = req.BillingPct;
            data.BillingAmountInr = req.BillingAmountInr;
            data.Status = req.Status;
            data.CompletionNotes = req.CompletionNotes;
            data.Billable = req.Billable;
            data.Invoiced = req.Invoiced;
            data.SortOrder = req.SortOrder;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "ProjectMilestone updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteProjectMilestone(int id)
        {
            var data = await _context.project_milestones.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "ProjectMilestone not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.project_milestones.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "ProjectMilestone deleted successfully.";
            return _response;
        }
    }
}
