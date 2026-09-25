using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class TenderService : ITenderService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public TenderService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllTender(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tenders.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.TenderTitle != null && s.TenderTitle.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.TenderTitle)
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

        public async Task<APIResponse> GetTenderById(int id)
        {
            var data = await _context.tenders.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect tender id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateTender(CreateTender dto)
        {
            var exists = await _context.tenders.AnyAsync(s => s.TenderNumber == dto.TenderNumber);
            if (exists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exists.";
                return _response;
            }

            var entity = new tenders
            {
                TenderNumber = dto.TenderNumber,
                TenderTitle = dto.TenderTitle,
                CustomerId = dto.CustomerId,
                TenderPortal = dto.TenderPortal,
                GemBidNumber = dto.GemBidNumber,
                PublicationDate = dto.PublicationDate,
                PreBidMeetingDate = dto.PreBidMeetingDate,
                QuerySubmissionDeadline = dto.QuerySubmissionDeadline,
                BidSubmissionDeadline = dto.BidSubmissionDeadline,
                TechnicalOpeningDate = dto.TechnicalOpeningDate,
                FinancialOpeningDate = dto.FinancialOpeningDate,
                EstimatedTenderValueInr = dto.EstimatedTenderValueInr,
                EmdAmountInr = dto.EmdAmountInr,
                TenderFeeInr = dto.TenderFeeInr,
                PbgRequiredPct = dto.PbgRequiredPct,
                EligibilityCriteria = dto.EligibilityCriteria,
                TechnicalRequirements = dto.TechnicalRequirements,
                OemRequirement = dto.OemRequirement,
                ConsortiumRequirement = dto.ConsortiumRequirement,
                MiiClass = dto.MiiClass,
                ExperienceCriteria = dto.ExperienceCriteria,
                TurnoverCriteriaInr = dto.TurnoverCriteriaInr,
                BidOwnerId = dto.BidOwnerId,
                PresalesOwnerId = dto.PresalesOwnerId,
                CommercialOwnerId = dto.CommercialOwnerId,
                PartnerOem = dto.PartnerOem,
                Competitors = dto.Competitors,
                BidStatus = dto.BidStatus,
                WinProbabilityPct = dto.WinProbabilityPct,
                BidValueSubmittedInr = dto.BidValueSubmittedInr,
                L1Bidder = dto.L1Bidder,
                L1ValueInr = dto.L1ValueInr,
                OutcomeNotes = dto.OutcomeNotes,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
            };
            _context.tenders.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Tender created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateTender(EditTender req)
        {
            var data = await _context.tenders.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Tender not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.TenderNumber = req.TenderNumber;
            data.TenderTitle = req.TenderTitle;
            data.CustomerId = req.CustomerId;
            data.TenderPortal = req.TenderPortal;
            data.GemBidNumber = req.GemBidNumber;
            data.PublicationDate = req.PublicationDate;
            data.PreBidMeetingDate = req.PreBidMeetingDate;
            data.QuerySubmissionDeadline = req.QuerySubmissionDeadline;
            data.BidSubmissionDeadline = req.BidSubmissionDeadline;
            data.TechnicalOpeningDate = req.TechnicalOpeningDate;
            data.FinancialOpeningDate = req.FinancialOpeningDate;
            data.EstimatedTenderValueInr = req.EstimatedTenderValueInr;
            data.EmdAmountInr = req.EmdAmountInr;
            data.TenderFeeInr = req.TenderFeeInr;
            data.PbgRequiredPct = req.PbgRequiredPct;
            data.EligibilityCriteria = req.EligibilityCriteria;
            data.TechnicalRequirements = req.TechnicalRequirements;
            data.OemRequirement = req.OemRequirement;
            data.ConsortiumRequirement = req.ConsortiumRequirement;
            data.MiiClass = req.MiiClass;
            data.ExperienceCriteria = req.ExperienceCriteria;
            data.TurnoverCriteriaInr = req.TurnoverCriteriaInr;
            data.BidOwnerId = req.BidOwnerId;
            data.PresalesOwnerId = req.PresalesOwnerId;
            data.CommercialOwnerId = req.CommercialOwnerId;
            data.PartnerOem = req.PartnerOem;
            data.Competitors = req.Competitors;
            data.BidStatus = req.BidStatus;
            data.WinProbabilityPct = req.WinProbabilityPct;
            data.BidValueSubmittedInr = req.BidValueSubmittedInr;
            data.L1Bidder = req.L1Bidder;
            data.L1ValueInr = req.L1ValueInr;
            data.OutcomeNotes = req.OutcomeNotes;
            data.CreatedAt = req.CreatedAt;
            data.UpdatedAt = req.UpdatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "Tender updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteTender(int id)
        {
            var data = await _context.tenders.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Tender not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.tenders.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Tender deleted successfully.";
            return _response;
        }
    }
}
