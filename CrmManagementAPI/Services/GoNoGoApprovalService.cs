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

        public async Task<APIResponse> GetAllGoNoGoApprovalsList( string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.go_nogo_approvals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) || x.TenderId.ToString().Contains(search) ||
                    x.RequestedById.ToString().Contains(search) || x.ApproverId.ToString().Contains(search) ||
                    x.EligibilityCompliance!.ToLower().Contains(search) || x.TechnicalCompliance!.ToLower().Contains(search) ||
                    x.OemAvailability!.ToLower().Contains(search) || x.CompetitionAssessment!.ToLower().Contains(search) ||
                    x.PaymentTerms!.ToLower().Contains(search) || x.ProjectRisk!.ToLower().Contains(search) ||
                    x.ResourceAvailability!.ToLower().Contains(search) || x.Decision!.ToLower().Contains(search) ||
                    x.ApproverRemarks!.ToLower().Contains(search) || x.Conditions!.ToLower().Contains(search)
                );
            }

            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue))
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

                    case "tenderid":
                        if (int.TryParse(filterValue, out int tenderId))
                        {
                            query = query.Where(x => x.TenderId == tenderId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TenderId value.";

                            return _response;
                        }
                        break;

                    case "requestedbyid":
                        if (int.TryParse(filterValue, out int requestedById))
                        {
                            query = query.Where(x => x.RequestedById == requestedById);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid RequestedById value.";

                            return _response;
                        }
                        break;

                    case "approverid":

                        if (int.TryParse(filterValue, out int approverId))
                        {
                            query = query.Where(x => x.ApproverId == approverId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ApproverId value.";

                            return _response;
                        }
                        break;

                    case "requestdate":
                        if (DateOnly.TryParse(filterValue, out DateOnly requestDate))
                        {
                            query = query.Where(x => x.RequestDate == requestDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid RequestDate value. Use yyyy-MM-dd.";

                            return _response;
                        }

                        break;

                    case "decisiondate":
                        if (DateOnly.TryParse(filterValue, out DateOnly decisionDate))
                        {
                            query = query.Where(x => x.DecisionDate == decisionDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid DecisionDate value. Use yyyy-MM-dd.";

                            return _response;
                        }
                        break;

                    case "tendervalueinr":
                        if (decimal.TryParse(filterValue, out decimal tenderValueInr))
                        {
                            query = query.Where(x => x.TenderValueInr == tenderValueInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TenderValueInr value.";

                            return _response;
                        }
                        break;

                    case "eligibilitycompliance":
                        query = query.Where(x =>
                            x.EligibilityCompliance != null &&
                            x.EligibilityCompliance.ToLower().Contains(filterValue));
                        break;

                    case "technicalcompliance":
                        query = query.Where(x =>
                            x.TechnicalCompliance != null &&
                            x.TechnicalCompliance.ToLower().Contains(filterValue));
                        break;

                    case "oemavailability":
                        query = query.Where(x =>
                            x.OemAvailability != null &&
                            x.OemAvailability.ToLower().Contains(filterValue));
                        break;

                    case "competitionassessment":
                        query = query.Where(x =>
                            x.CompetitionAssessment != null &&
                            x.CompetitionAssessment.ToLower().Contains(filterValue));
                        break;

                    case "expectedmarginpct":
                        if (decimal.TryParse(filterValue, out decimal expectedMarginPct))
                        {
                            query = query.Where(x => x.ExpectedMarginPct == expectedMarginPct);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ExpectedMarginPct value.";

                            return _response;
                        }
                        break;

                    case "paymentterms":
                        query = query.Where(x =>
                            x.PaymentTerms != null &&
                            x.PaymentTerms.ToLower().Contains(filterValue));
                        break;

                    case "projectrisk":
                        query = query.Where(x =>
                            x.ProjectRisk != null &&
                            x.ProjectRisk.ToLower().Contains(filterValue));
                        break;

                    case "resourceavailability":

                        query = query.Where(x =>
                            x.ResourceAvailability != null &&
                            x.ResourceAvailability.ToLower().Contains(filterValue));

                        break;

                    case "winprobabilitypct":
                        if (int.TryParse(filterValue, out int winProbabilityPct))
                        {
                            query = query.Where(x => x.WinProbabilityPct == winProbabilityPct);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid WinProbabilityPct value.";

                            return _response;
                        }
                        break;

                    case "decision":

                        query = query.Where(x =>
                            x.Decision != null &&
                            x.Decision.ToLower().Contains(filterValue));

                        break;

                    case "approverremarks":
                        query = query.Where(x =>
                            x.ApproverRemarks != null &&
                            x.ApproverRemarks.ToLower().Contains(filterValue));
                        break;

                    case "conditions":

                        query = query.Where(x =>
                            x.Conditions != null &&
                            x.Conditions.ToLower().Contains(filterValue));
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
                        query = descending
                            ? query.OrderByDescending(x => x.Id)
                            : query.OrderBy(x => x.Id);
                        break;

                    case "tenderid":
                        query = descending
                            ? query.OrderByDescending(x => x.TenderId)
                            : query.OrderBy(x => x.TenderId);
                        break;

                    case "requestedbyid":
                        query = descending
                            ? query.OrderByDescending(x => x.RequestedById)
                            : query.OrderBy(x => x.RequestedById);
                        break;

                    case "approverid":
                        query = descending
                            ? query.OrderByDescending(x => x.ApproverId)
                            : query.OrderBy(x => x.ApproverId);
                        break;

                    case "requestdate":
                        query = descending
                            ? query.OrderByDescending(x => x.RequestDate)
                            : query.OrderBy(x => x.RequestDate);
                        break;

                    case "decisiondate":
                        query = descending
                            ? query.OrderByDescending(x => x.DecisionDate)
                            : query.OrderBy(x => x.DecisionDate);
                        break;

                    case "tendervalueinr":
                        query = descending
                            ? query.OrderByDescending(x => x.TenderValueInr)
                            : query.OrderBy(x => x.TenderValueInr);
                        break;

                    case "eligibilitycompliance":
                        query = descending
                            ? query.OrderByDescending(x => x.EligibilityCompliance)
                            : query.OrderBy(x => x.EligibilityCompliance);
                        break;

                    case "technicalcompliance":
                        query = descending
                            ? query.OrderByDescending(x => x.TechnicalCompliance)
                            : query.OrderBy(x => x.TechnicalCompliance);
                        break;

                    case "oemavailability":
                        query = descending
                            ? query.OrderByDescending(x => x.OemAvailability)
                            : query.OrderBy(x => x.OemAvailability);
                        break;

                    case "competitionassessment":
                        query = descending
                            ? query.OrderByDescending(x => x.CompetitionAssessment)
                            : query.OrderBy(x => x.CompetitionAssessment);
                        break;

                    case "expectedmarginpct":
                        query = descending
                            ? query.OrderByDescending(x => x.ExpectedMarginPct)
                            : query.OrderBy(x => x.ExpectedMarginPct);
                        break;

                    case "paymentterms":
                        query = descending
                            ? query.OrderByDescending(x => x.PaymentTerms)
                            : query.OrderBy(x => x.PaymentTerms);
                        break;

                    case "projectrisk":
                        query = descending
                            ? query.OrderByDescending(x => x.ProjectRisk)
                            : query.OrderBy(x => x.ProjectRisk);
                        break;

                    case "resourceavailability":
                        query = descending
                            ? query.OrderByDescending(x => x.ResourceAvailability)
                            : query.OrderBy(x => x.ResourceAvailability);
                        break;

                    case "winprobabilitypct":
                        query = descending
                            ? query.OrderByDescending(x => x.WinProbabilityPct)
                            : query.OrderBy(x => x.WinProbabilityPct);
                        break;

                    case "decision":
                        query = descending
                            ? query.OrderByDescending(x => x.Decision)
                            : query.OrderBy(x => x.Decision);
                        break;

                    case "approverremarks":
                        query = descending
                            ? query.OrderByDescending(x => x.ApproverRemarks)
                            : query.OrderBy(x => x.ApproverRemarks);
                        break;

                    case "conditions":
                        query = descending
                            ? query.OrderByDescending(x => x.Conditions)
                            : query.OrderBy(x => x.Conditions);
                        break;

                    case "createdat":
                        query = descending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);
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
