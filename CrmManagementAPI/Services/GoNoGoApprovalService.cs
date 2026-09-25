using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class GoNoGoApprovalService : IGoNoGoApprovalService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public GoNoGoApprovalService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllGoNoGoApproval(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.go_nogo_approvals.AsQueryable();

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

        public async Task<APIResponse> GetGoNoGoApprovalById(int id)
        {
            var data = await _context.go_nogo_approvals.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect gonogoapproval id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateGoNoGoApproval(CreateGoNoGoApproval dto)
        {
            var entity = new go_nogo_approvals
            {
                TenderId = dto.TenderId,
                RequestedById = dto.RequestedById,
                ApproverId = dto.ApproverId,
                RequestDate = dto.RequestDate,
                DecisionDate = dto.DecisionDate,
                TenderValueInr = dto.TenderValueInr,
                EligibilityCompliance = dto.EligibilityCompliance,
                TechnicalCompliance = dto.TechnicalCompliance,
                OemAvailability = dto.OemAvailability,
                CompetitionAssessment = dto.CompetitionAssessment,
                ExpectedMarginPct = dto.ExpectedMarginPct,
                PaymentTerms = dto.PaymentTerms,
                ProjectRisk = dto.ProjectRisk,
                ResourceAvailability = dto.ResourceAvailability,
                WinProbabilityPct = dto.WinProbabilityPct,
                Decision = dto.Decision,
                ApproverRemarks = dto.ApproverRemarks,
                Conditions = dto.Conditions,
                CreatedAt = dto.CreatedAt,
            };
            _context.go_nogo_approvals.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "GoNoGoApproval created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateGoNoGoApproval(EditGoNoGoApproval req)
        {
            var data = await _context.go_nogo_approvals.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "GoNoGoApproval not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.TenderId = req.TenderId;
            data.RequestedById = req.RequestedById;
            data.ApproverId = req.ApproverId;
            data.RequestDate = req.RequestDate;
            data.DecisionDate = req.DecisionDate;
            data.TenderValueInr = req.TenderValueInr;
            data.EligibilityCompliance = req.EligibilityCompliance;
            data.TechnicalCompliance = req.TechnicalCompliance;
            data.OemAvailability = req.OemAvailability;
            data.CompetitionAssessment = req.CompetitionAssessment;
            data.ExpectedMarginPct = req.ExpectedMarginPct;
            data.PaymentTerms = req.PaymentTerms;
            data.ProjectRisk = req.ProjectRisk;
            data.ResourceAvailability = req.ResourceAvailability;
            data.WinProbabilityPct = req.WinProbabilityPct;
            data.Decision = req.Decision;
            data.ApproverRemarks = req.ApproverRemarks;
            data.Conditions = req.Conditions;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "GoNoGoApproval updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteGoNoGoApproval(int id)
        {
            var data = await _context.go_nogo_approvals.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "GoNoGoApproval not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.go_nogo_approvals.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "GoNoGoApproval deleted successfully.";
            return _response;
        }
    }
}
