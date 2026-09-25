using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class LeadService : ILeadService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public LeadService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllLead(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.leads.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.OrganizationName != null && s.OrganizationName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.OrganizationName)
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

        public async Task<APIResponse> GetLeadById(int id)
        {
            var data = await _context.leads.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect lead id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateLead(CreateLead dto)
        {
            var exists = await _context.leads.AnyAsync(s => s.LeadCode == dto.LeadCode);
            if (exists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exists.";
                return _response;
            }

            var entity = new leads
            {
                LeadCode = dto.LeadCode,
                OrganizationName = dto.OrganizationName,
                MinistryParent = dto.MinistryParent,
                Category = dto.Category,
                State = dto.State,
                DistrictCity = dto.DistrictCity,
                ContactName = dto.ContactName,
                ContactDesignation = dto.ContactDesignation,
                ContactEmail = dto.ContactEmail,
                ContactMobile = dto.ContactMobile,
                Source = dto.Source,
                OpportunityTitle = dto.OpportunityTitle,
                OpportunitySummary = dto.OpportunitySummary,
                EstimatedValueInr = dto.EstimatedValueInr,
                ExpectedRfpDate = dto.ExpectedRfpDate,
                FiscalYear = dto.FiscalYear,
                BudgetLineItem = dto.BudgetLineItem,
                BudgetConfirmed = dto.BudgetConfirmed,
                DecisionMaker = dto.DecisionMaker,
                CompetitionStatus = dto.CompetitionStatus,
                FitNotes = dto.FitNotes,
                LeadStage = dto.LeadStage,
                DisqualificationReason = dto.DisqualificationReason,
                QualificationScore = dto.QualificationScore,
                BantBudget = dto.BantBudget,
                BantAuthority = dto.BantAuthority,
                BantNeed = dto.BantNeed,
                BantTimeline = dto.BantTimeline,
                NextAction = dto.NextAction,
                NextActionDate = dto.NextActionDate,
                OwnerId = dto.OwnerId,
                ConvertedTenderId = dto.ConvertedTenderId,
                ConvertedCustomerId = dto.ConvertedCustomerId,
                ConvertedAt = dto.ConvertedAt,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
            };
            _context.leads.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Lead created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateLead(EditLead req)
        {
            var data = await _context.leads.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Lead not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.LeadCode = req.LeadCode;
            data.OrganizationName = req.OrganizationName;
            data.MinistryParent = req.MinistryParent;
            data.Category = req.Category;
            data.State = req.State;
            data.DistrictCity = req.DistrictCity;
            data.ContactName = req.ContactName;
            data.ContactDesignation = req.ContactDesignation;
            data.ContactEmail = req.ContactEmail;
            data.ContactMobile = req.ContactMobile;
            data.Source = req.Source;
            data.OpportunityTitle = req.OpportunityTitle;
            data.OpportunitySummary = req.OpportunitySummary;
            data.EstimatedValueInr = req.EstimatedValueInr;
            data.ExpectedRfpDate = req.ExpectedRfpDate;
            data.FiscalYear = req.FiscalYear;
            data.BudgetLineItem = req.BudgetLineItem;
            data.BudgetConfirmed = req.BudgetConfirmed;
            data.DecisionMaker = req.DecisionMaker;
            data.CompetitionStatus = req.CompetitionStatus;
            data.FitNotes = req.FitNotes;
            data.LeadStage = req.LeadStage;
            data.DisqualificationReason = req.DisqualificationReason;
            data.QualificationScore = req.QualificationScore;
            data.BantBudget = req.BantBudget;
            data.BantAuthority = req.BantAuthority;
            data.BantNeed = req.BantNeed;
            data.BantTimeline = req.BantTimeline;
            data.NextAction = req.NextAction;
            data.NextActionDate = req.NextActionDate;
            data.OwnerId = req.OwnerId;
            data.ConvertedTenderId = req.ConvertedTenderId;
            data.ConvertedCustomerId = req.ConvertedCustomerId;
            data.ConvertedAt = req.ConvertedAt;
            data.CreatedAt = req.CreatedAt;
            data.UpdatedAt = req.UpdatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "Lead updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteLead(int id)
        {
            var data = await _context.leads.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Lead not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.leads.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Lead deleted successfully.";
            return _response;
        }
    }
}
