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

        public async Task<APIResponse> GetAllLeadList(
    string? search,
    string? filterField,
    string? filterValue,
    string? sortField,
    string? sortOrder = "asc",
    int pageNumber = 1,
    int pageSize = 10)
        {
            var query = _context.leads.AsQueryable();

            // =========================
            // Search
            // =========================

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||
                    x.LeadCode.ToLower().Contains(search) ||
                    x.OrganizationName.ToLower().Contains(search) ||
                    x.MinistryParent!.ToLower().Contains(search) ||
                    x.Category!.ToLower().Contains(search) ||
                    x.State!.ToLower().Contains(search) ||
                    x.DistrictCity!.ToLower().Contains(search) ||
                    x.ContactName!.ToLower().Contains(search) ||
                    x.ContactDesignation!.ToLower().Contains(search) ||
                    x.ContactEmail!.ToLower().Contains(search) ||
                    x.ContactMobile!.ToLower().Contains(search) ||
                    x.Source!.ToLower().Contains(search) ||
                    x.OpportunityTitle.ToLower().Contains(search) ||
                    x.OpportunitySummary!.ToLower().Contains(search) ||
                    x.FiscalYear!.ToLower().Contains(search) ||
                    x.BudgetLineItem!.ToLower().Contains(search) ||
                    x.DecisionMaker!.ToLower().Contains(search) ||
                    x.CompetitionStatus!.ToLower().Contains(search) ||
                    x.FitNotes!.ToLower().Contains(search) ||
                    x.LeadStage.ToLower().Contains(search) ||
                    x.DisqualificationReason!.ToLower().Contains(search) ||
                    x.BantBudget!.ToLower().Contains(search) ||
                    x.BantAuthority!.ToLower().Contains(search) ||
                    x.BantNeed!.ToLower().Contains(search) ||
                    x.BantTimeline!.ToLower().Contains(search) ||
                    x.NextAction!.ToLower().Contains(search)
                );
            }

            // =========================
            // Filter
            // =========================

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

                    case "leadcode":

                        query = query.Where(x =>
                            x.LeadCode.ToLower().Contains(filterValue));

                        break;

                    case "organizationname":

                        query = query.Where(x =>
                            x.OrganizationName.ToLower().Contains(filterValue));

                        break;

                    case "ministryparent":

                        query = query.Where(x =>
                            x.MinistryParent != null &&
                            x.MinistryParent.ToLower().Contains(filterValue));

                        break;

                    case "category":

                        query = query.Where(x =>
                            x.Category != null &&
                            x.Category.ToLower().Contains(filterValue));

                        break;

                    case "state":

                        query = query.Where(x =>
                            x.State != null &&
                            x.State.ToLower().Contains(filterValue));

                        break;

                    case "districtcity":

                        query = query.Where(x =>
                            x.DistrictCity != null &&
                            x.DistrictCity.ToLower().Contains(filterValue));

                        break;

                    case "contactname":

                        query = query.Where(x =>
                            x.ContactName != null &&
                            x.ContactName.ToLower().Contains(filterValue));

                        break;

                    case "contactdesignation":

                        query = query.Where(x =>
                            x.ContactDesignation != null &&
                            x.ContactDesignation.ToLower().Contains(filterValue));

                        break;

                    case "contactemail":

                        query = query.Where(x =>
                            x.ContactEmail != null &&
                            x.ContactEmail.ToLower().Contains(filterValue));

                        break;

                    case "contactmobile":

                        query = query.Where(x =>
                            x.ContactMobile != null &&
                            x.ContactMobile.ToLower().Contains(filterValue));

                        break;

                    case "source":

                        query = query.Where(x =>
                            x.Source != null &&
                            x.Source.ToLower().Contains(filterValue));

                        break;

                    case "opportunitytitle":

                        query = query.Where(x =>
                            x.OpportunityTitle.ToLower().Contains(filterValue));

                        break;

                    case "opportunitysummary":

                        query = query.Where(x =>
                            x.OpportunitySummary != null &&
                            x.OpportunitySummary.ToLower().Contains(filterValue));

                        break;

                    case "estimatedvalueinr":

                        if (decimal.TryParse(
                            filterValue,
                            out decimal estimatedValueInr))
                        {
                            query = query.Where(x =>
                                x.EstimatedValueInr == estimatedValueInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid EstimatedValueInr value.";

                            return _response;
                        }

                        break;

                    case "expectedrfpdate":

                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly expectedRfpDate))
                        {
                            query = query.Where(x =>
                                x.ExpectedRfpDate == expectedRfpDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ExpectedRfpDate value. Use yyyy-MM-dd.";

                            return _response;
                        }

                        break;

                    case "fiscalyear":

                        query = query.Where(x =>
                            x.FiscalYear != null &&
                            x.FiscalYear.ToLower().Contains(filterValue));

                        break;

                    case "budgetlineitem":

                        query = query.Where(x =>
                            x.BudgetLineItem != null &&
                            x.BudgetLineItem.ToLower().Contains(filterValue));

                        break;

                    case "budgetconfirmed":

                        if (bool.TryParse(filterValue, out bool budgetConfirmed))
                        {
                            query = query.Where(x =>
                                x.BudgetConfirmed == budgetConfirmed);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid BudgetConfirmed value. Use true or false.";

                            return _response;
                        }

                        break;

                    case "decisionmaker":

                        query = query.Where(x =>
                            x.DecisionMaker != null &&
                            x.DecisionMaker.ToLower().Contains(filterValue));

                        break;

                    case "competitionstatus":

                        query = query.Where(x =>
                            x.CompetitionStatus != null &&
                            x.CompetitionStatus.ToLower().Contains(filterValue));

                        break;

                    case "fitnotes":

                        query = query.Where(x =>
                            x.FitNotes != null &&
                            x.FitNotes.ToLower().Contains(filterValue));

                        break;

                    case "leadstage":

                        query = query.Where(x =>
                            x.LeadStage.ToLower().Contains(filterValue));

                        break;

                    case "disqualificationreason":

                        query = query.Where(x =>
                            x.DisqualificationReason != null &&
                            x.DisqualificationReason.ToLower().Contains(filterValue));

                        break;

                    case "qualificationscore":

                        if (int.TryParse(
                            filterValue,
                            out int qualificationScore))
                        {
                            query = query.Where(x =>
                                x.QualificationScore == qualificationScore);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid QualificationScore value.";

                            return _response;
                        }

                        break;

                    case "bantbudget":

                        query = query.Where(x =>
                            x.BantBudget != null &&
                            x.BantBudget.ToLower().Contains(filterValue));

                        break;

                    case "bantauthority":

                        query = query.Where(x =>
                            x.BantAuthority != null &&
                            x.BantAuthority.ToLower().Contains(filterValue));

                        break;

                    case "bantneed":

                        query = query.Where(x =>
                            x.BantNeed != null &&
                            x.BantNeed.ToLower().Contains(filterValue));

                        break;

                    case "banttimeline":

                        query = query.Where(x =>
                            x.BantTimeline != null &&
                            x.BantTimeline.ToLower().Contains(filterValue));

                        break;

                    case "nextaction":

                        query = query.Where(x =>
                            x.NextAction != null &&
                            x.NextAction.ToLower().Contains(filterValue));

                        break;

                    case "nextactiondate":

                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly nextActionDate))
                        {
                            query = query.Where(x =>
                                x.NextActionDate == nextActionDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid NextActionDate value. Use yyyy-MM-dd.";

                            return _response;
                        }

                        break;

                    case "ownerid":

                        if (int.TryParse(filterValue, out int ownerId))
                        {
                            query = query.Where(x => x.OwnerId == ownerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid OwnerId value.";

                            return _response;
                        }

                        break;

                    case "convertedtenderid":

                        if (int.TryParse(
                            filterValue,
                            out int convertedTenderId))
                        {
                            query = query.Where(x =>
                                x.ConvertedTenderId == convertedTenderId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ConvertedTenderId value.";

                            return _response;
                        }

                        break;

                    case "convertedcustomerid":

                        if (int.TryParse(
                            filterValue,
                            out int convertedCustomerId))
                        {
                            query = query.Where(x =>
                                x.ConvertedCustomerId == convertedCustomerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ConvertedCustomerId value.";

                            return _response;
                        }

                        break;

                    case "convertedat":

                        if (DateTime.TryParse(
                            filterValue,
                            out DateTime convertedAt))
                        {
                            query = query.Where(x =>
                                x.ConvertedAt != null &&
                                x.ConvertedAt.Value.Date == convertedAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ConvertedAt value.";

                            return _response;
                        }

                        break;

                    case "createdat":

                        if (DateTime.TryParse(
                            filterValue,
                            out DateTime createdAt))
                        {
                            query = query.Where(x =>
                                x.CreatedAt.Date == createdAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid CreatedAt value.";

                            return _response;
                        }

                        break;

                    case "updatedat":

                        if (DateTime.TryParse(
                            filterValue,
                            out DateTime updatedAt))
                        {
                            query = query.Where(x =>
                                x.UpdatedAt.Date == updatedAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid UpdatedAt value.";

                            return _response;
                        }

                        break;

                    default:

                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ActionResponse =
                            $"Invalid filter field: {filterField}";

                        return _response;
                }
            }

            // =========================
            // Sorting
            // =========================

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

                    case "leadcode":

                        query = descending
                            ? query.OrderByDescending(x => x.LeadCode)
                            : query.OrderBy(x => x.LeadCode);

                        break;

                    case "organizationname":

                        query = descending
                            ? query.OrderByDescending(x => x.OrganizationName)
                            : query.OrderBy(x => x.OrganizationName);

                        break;

                    case "ministryparent":

                        query = descending
                            ? query.OrderByDescending(x => x.MinistryParent)
                            : query.OrderBy(x => x.MinistryParent);

                        break;

                    case "category":

                        query = descending
                            ? query.OrderByDescending(x => x.Category)
                            : query.OrderBy(x => x.Category);

                        break;

                    case "state":

                        query = descending
                            ? query.OrderByDescending(x => x.State)
                            : query.OrderBy(x => x.State);

                        break;

                    case "districtcity":

                        query = descending
                            ? query.OrderByDescending(x => x.DistrictCity)
                            : query.OrderBy(x => x.DistrictCity);

                        break;

                    case "contactname":

                        query = descending
                            ? query.OrderByDescending(x => x.ContactName)
                            : query.OrderBy(x => x.ContactName);

                        break;

                    case "contactdesignation":

                        query = descending
                            ? query.OrderByDescending(x => x.ContactDesignation)
                            : query.OrderBy(x => x.ContactDesignation);

                        break;

                    case "contactemail":

                        query = descending
                            ? query.OrderByDescending(x => x.ContactEmail)
                            : query.OrderBy(x => x.ContactEmail);

                        break;

                    case "contactmobile":

                        query = descending
                            ? query.OrderByDescending(x => x.ContactMobile)
                            : query.OrderBy(x => x.ContactMobile);

                        break;

                    case "source":

                        query = descending
                            ? query.OrderByDescending(x => x.Source)
                            : query.OrderBy(x => x.Source);

                        break;

                    case "opportunitytitle":

                        query = descending
                            ? query.OrderByDescending(x => x.OpportunityTitle)
                            : query.OrderBy(x => x.OpportunityTitle);

                        break;

                    case "opportunitysummary":

                        query = descending
                            ? query.OrderByDescending(x => x.OpportunitySummary)
                            : query.OrderBy(x => x.OpportunitySummary);

                        break;

                    case "estimatedvalueinr":

                        query = descending
                            ? query.OrderByDescending(x => x.EstimatedValueInr)
                            : query.OrderBy(x => x.EstimatedValueInr);

                        break;

                    case "expectedrfpdate":

                        query = descending
                            ? query.OrderByDescending(x => x.ExpectedRfpDate)
                            : query.OrderBy(x => x.ExpectedRfpDate);

                        break;

                    case "fiscalyear":

                        query = descending
                            ? query.OrderByDescending(x => x.FiscalYear)
                            : query.OrderBy(x => x.FiscalYear);

                        break;

                    case "budgetlineitem":

                        query = descending
                            ? query.OrderByDescending(x => x.BudgetLineItem)
                            : query.OrderBy(x => x.BudgetLineItem);

                        break;

                    case "budgetconfirmed":

                        query = descending
                            ? query.OrderByDescending(x => x.BudgetConfirmed)
                            : query.OrderBy(x => x.BudgetConfirmed);

                        break;

                    case "decisionmaker":

                        query = descending
                            ? query.OrderByDescending(x => x.DecisionMaker)
                            : query.OrderBy(x => x.DecisionMaker);

                        break;

                    case "competitionstatus":

                        query = descending
                            ? query.OrderByDescending(x => x.CompetitionStatus)
                            : query.OrderBy(x => x.CompetitionStatus);

                        break;

                    case "fitnotes":

                        query = descending
                            ? query.OrderByDescending(x => x.FitNotes)
                            : query.OrderBy(x => x.FitNotes);

                        break;

                    case "leadstage":

                        query = descending
                            ? query.OrderByDescending(x => x.LeadStage)
                            : query.OrderBy(x => x.LeadStage);

                        break;

                    case "disqualificationreason":

                        query = descending
                            ? query.OrderByDescending(x => x.DisqualificationReason)
                            : query.OrderBy(x => x.DisqualificationReason);

                        break;

                    case "qualificationscore":

                        query = descending
                            ? query.OrderByDescending(x => x.QualificationScore)
                            : query.OrderBy(x => x.QualificationScore);

                        break;

                    case "bantbudget":

                        query = descending
                            ? query.OrderByDescending(x => x.BantBudget)
                            : query.OrderBy(x => x.BantBudget);

                        break;

                    case "bantauthority":

                        query = descending
                            ? query.OrderByDescending(x => x.BantAuthority)
                            : query.OrderBy(x => x.BantAuthority);

                        break;

                    case "bantneed":

                        query = descending
                            ? query.OrderByDescending(x => x.BantNeed)
                            : query.OrderBy(x => x.BantNeed);

                        break;

                    case "banttimeline":

                        query = descending
                            ? query.OrderByDescending(x => x.BantTimeline)
                            : query.OrderBy(x => x.BantTimeline);

                        break;

                    case "nextaction":

                        query = descending
                            ? query.OrderByDescending(x => x.NextAction)
                            : query.OrderBy(x => x.NextAction);

                        break;

                    case "nextactiondate":

                        query = descending
                            ? query.OrderByDescending(x => x.NextActionDate)
                            : query.OrderBy(x => x.NextActionDate);

                        break;

                    case "ownerid":

                        query = descending
                            ? query.OrderByDescending(x => x.OwnerId)
                            : query.OrderBy(x => x.OwnerId);

                        break;

                    case "convertedtenderid":

                        query = descending
                            ? query.OrderByDescending(x => x.ConvertedTenderId)
                            : query.OrderBy(x => x.ConvertedTenderId);

                        break;

                    case "convertedcustomerid":

                        query = descending
                            ? query.OrderByDescending(x => x.ConvertedCustomerId)
                            : query.OrderBy(x => x.ConvertedCustomerId);

                        break;

                    case "convertedat":

                        query = descending
                            ? query.OrderByDescending(x => x.ConvertedAt)
                            : query.OrderBy(x => x.ConvertedAt);

                        break;

                    case "createdat":

                        query = descending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);

                        break;

                    case "updatedat":

                        query = descending
                            ? query.OrderByDescending(x => x.UpdatedAt)
                            : query.OrderBy(x => x.UpdatedAt);

                        break;

                    default:

                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ActionResponse =
                            $"Invalid sort field: {sortField}";

                        return _response;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }

            // =========================
            // Pagination
            // =========================

            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // =========================
            // No Data
            // =========================

            if (!data.Any())
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ActionResponse = "Data not found.";

                return _response;
            }

            // =========================
            // Success Response
            // =========================

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
