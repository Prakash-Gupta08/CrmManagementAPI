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

        public async Task<APIResponse> GetAllTendersList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tenders.AsQueryable();

            // =========================
            // Search
            // =========================
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||
                    x.TenderNumber.ToLower().Contains(search) ||
                    x.TenderTitle.ToLower().Contains(search) ||
                    x.CustomerId.ToString().Contains(search) ||
                    (x.TenderPortal != null && x.TenderPortal.ToLower().Contains(search)) ||
                    (x.GemBidNumber != null && x.GemBidNumber.ToLower().Contains(search)) ||
                    (x.EstimatedTenderValueInr != null && x.EstimatedTenderValueInr.ToString().Contains(search)) ||
                    (x.EmdAmountInr != null && x.EmdAmountInr.ToString().Contains(search)) ||
                    (x.TenderFeeInr != null && x.TenderFeeInr.ToString().Contains(search)) ||
                    (x.PbgRequiredPct != null && x.PbgRequiredPct.ToString().Contains(search)) ||
                    (x.EligibilityCriteria != null && x.EligibilityCriteria.ToLower().Contains(search)) ||
                    (x.TechnicalRequirements != null && x.TechnicalRequirements.ToLower().Contains(search)) ||
                    (x.OemRequirement != null && x.OemRequirement.ToLower().Contains(search)) ||
                    x.ConsortiumRequirement.ToString().ToLower().Contains(search) ||
                    (x.MiiClass != null && x.MiiClass.ToLower().Contains(search)) ||
                    (x.ExperienceCriteria != null && x.ExperienceCriteria.ToLower().Contains(search)) ||
                    (x.TurnoverCriteriaInr != null && x.TurnoverCriteriaInr.ToString().Contains(search)) ||
                    (x.BidOwnerId != null && x.BidOwnerId.ToString().Contains(search)) ||
                    (x.PresalesOwnerId != null && x.PresalesOwnerId.ToString().Contains(search)) ||
                    (x.CommercialOwnerId != null && x.CommercialOwnerId.ToString().Contains(search)) ||
                    (x.PartnerOem != null && x.PartnerOem.ToLower().Contains(search)) ||
                    (x.Competitors != null && x.Competitors.ToLower().Contains(search)) ||
                    x.BidStatus.ToLower().Contains(search) ||
                    (x.WinProbabilityPct != null && x.WinProbabilityPct.ToString().Contains(search)) ||
                    (x.BidValueSubmittedInr != null && x.BidValueSubmittedInr.ToString().Contains(search)) ||
                    (x.L1Bidder != null && x.L1Bidder.ToLower().Contains(search)) ||
                    (x.L1ValueInr != null && x.L1ValueInr.ToString().Contains(search)) ||
                    (x.OutcomeNotes != null && x.OutcomeNotes.ToLower().Contains(search))
                );
            }

            // =========================
            // Filter
            // =========================
            if (!string.IsNullOrEmpty(filterField) &&
                !string.IsNullOrEmpty(filterValue))
            {
                switch (filterField.ToLower())
                {
                    case "id":
                        if (int.TryParse(filterValue, out int id))
                        {
                            query = query.Where(x => x.Id == id);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid Id.";
                            return _response;
                        }
                        break;

                    case "tendernumber":
                        query = query.Where(x =>
                            x.TenderNumber.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "tendertitle":
                        query = query.Where(x =>
                            x.TenderTitle.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "customerid":
                        if (int.TryParse(filterValue, out int customerId))
                        {
                            query = query.Where(x => x.CustomerId == customerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CustomerId.";
                            return _response;
                        }
                        break;

                    case "tenderportal":
                        query = query.Where(x =>
                            x.TenderPortal != null &&
                            x.TenderPortal.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "gembidnumber":
                        query = query.Where(x =>
                            x.GemBidNumber != null &&
                            x.GemBidNumber.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "publicationdate":
                        if (DateOnly.TryParse(filterValue, out DateOnly publicationDate))
                        {
                            query = query.Where(x => x.PublicationDate == publicationDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid PublicationDate. Use yyyy-MM-dd format.";
                            return _response;
                        }
                        break;

                    case "prebidmeetingdate":
                        if (DateOnly.TryParse(filterValue, out DateOnly preBidMeetingDate))
                        {
                            query = query.Where(x => x.PreBidMeetingDate == preBidMeetingDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid PreBidMeetingDate. Use yyyy-MM-dd format.";
                            return _response;
                        }
                        break;

                    case "querysubmissiondeadline":
                        if (DateOnly.TryParse(filterValue, out DateOnly querySubmissionDeadline))
                        {
                            query = query.Where(x => x.QuerySubmissionDeadline == querySubmissionDeadline);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid QuerySubmissionDeadline. Use yyyy-MM-dd format.";
                            return _response;
                        }
                        break;

                    case "bidsubmissiondeadline":
                        if (DateTime.TryParse(filterValue, out DateTime bidSubmissionDeadline))
                        {
                            query = query.Where(x =>
                                x.BidSubmissionDeadline.HasValue &&
                                x.BidSubmissionDeadline.Value.Date == bidSubmissionDeadline.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid BidSubmissionDeadline.";
                            return _response;
                        }
                        break;

                    case "technicalopeningdate":
                        if (DateOnly.TryParse(filterValue, out DateOnly technicalOpeningDate))
                        {
                            query = query.Where(x => x.TechnicalOpeningDate == technicalOpeningDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TechnicalOpeningDate. Use yyyy-MM-dd format.";
                            return _response;
                        }
                        break;

                    case "financialopeningdate":
                        if (DateOnly.TryParse(filterValue, out DateOnly financialOpeningDate))
                        {
                            query = query.Where(x => x.FinancialOpeningDate == financialOpeningDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid FinancialOpeningDate. Use yyyy-MM-dd format.";
                            return _response;
                        }
                        break;

                    case "estimatedtendervalueinr":
                        if (decimal.TryParse(filterValue, out decimal estimatedTenderValueInr))
                        {
                            query = query.Where(x =>
                                x.EstimatedTenderValueInr == estimatedTenderValueInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid EstimatedTenderValueInr.";
                            return _response;
                        }
                        break;

                    case "emdamountinr":
                        if (decimal.TryParse(filterValue, out decimal emdAmountInr))
                        {
                            query = query.Where(x => x.EmdAmountInr == emdAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid EmdAmountInr.";
                            return _response;
                        }
                        break;

                    case "tenderfeeinr":
                        if (decimal.TryParse(filterValue, out decimal tenderFeeInr))
                        {
                            query = query.Where(x => x.TenderFeeInr == tenderFeeInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TenderFeeInr.";
                            return _response;
                        }
                        break;

                    case "pbgrequiredpct":
                        if (decimal.TryParse(filterValue, out decimal pbgRequiredPct))
                        {
                            query = query.Where(x => x.PbgRequiredPct == pbgRequiredPct);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid PbgRequiredPct.";
                            return _response;
                        }
                        break;

                    case "eligibilitycriteria":
                        query = query.Where(x =>
                            x.EligibilityCriteria != null &&
                            x.EligibilityCriteria.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "technicalrequirements":
                        query = query.Where(x =>
                            x.TechnicalRequirements != null &&
                            x.TechnicalRequirements.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "oemrequirement":
                        query = query.Where(x =>
                            x.OemRequirement != null &&
                            x.OemRequirement.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "consortiumrequirement":
                        if (bool.TryParse(filterValue, out bool consortiumRequirement))
                        {
                            query = query.Where(x =>
                                x.ConsortiumRequirement == consortiumRequirement);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ConsortiumRequirement. Use true or false.";
                            return _response;
                        }
                        break;

                    case "miiclass":
                        query = query.Where(x =>
                            x.MiiClass != null &&
                            x.MiiClass.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "experiencecriteria":
                        query = query.Where(x =>
                            x.ExperienceCriteria != null &&
                            x.ExperienceCriteria.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "turnovercriteriainr":
                        if (decimal.TryParse(filterValue, out decimal turnoverCriteriaInr))
                        {
                            query = query.Where(x =>
                                x.TurnoverCriteriaInr == turnoverCriteriaInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TurnoverCriteriaInr.";
                            return _response;
                        }
                        break;

                    case "bidownerid":
                        if (int.TryParse(filterValue, out int bidOwnerId))
                        {
                            query = query.Where(x => x.BidOwnerId == bidOwnerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid BidOwnerId.";
                            return _response;
                        }
                        break;

                    case "presalesownerid":
                        if (int.TryParse(filterValue, out int presalesOwnerId))
                        {
                            query = query.Where(x => x.PresalesOwnerId == presalesOwnerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid PresalesOwnerId.";
                            return _response;
                        }
                        break;

                    case "commercialownerid":
                        if (int.TryParse(filterValue, out int commercialOwnerId))
                        {
                            query = query.Where(x => x.CommercialOwnerId == commercialOwnerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CommercialOwnerId.";
                            return _response;
                        }
                        break;

                    case "partneroem":
                        query = query.Where(x =>
                            x.PartnerOem != null &&
                            x.PartnerOem.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "competitors":
                        query = query.Where(x =>
                            x.Competitors != null &&
                            x.Competitors.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "bidstatus":
                        query = query.Where(x =>
                            x.BidStatus.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "winprobabilitypct":
                        if (int.TryParse(filterValue, out int winProbabilityPct))
                        {
                            query = query.Where(x =>
                                x.WinProbabilityPct == winProbabilityPct);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid WinProbabilityPct.";
                            return _response;
                        }
                        break;

                    case "bidvaluesubmittedinr":
                        if (decimal.TryParse(filterValue, out decimal bidValueSubmittedInr))
                        {
                            query = query.Where(x =>
                                x.BidValueSubmittedInr == bidValueSubmittedInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid BidValueSubmittedInr.";
                            return _response;
                        }
                        break;

                    case "l1bidder":
                        query = query.Where(x =>
                            x.L1Bidder != null &&
                            x.L1Bidder.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "l1valueinr":
                        if (decimal.TryParse(filterValue, out decimal l1ValueInr))
                        {
                            query = query.Where(x =>
                                x.L1ValueInr == l1ValueInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid L1ValueInr.";
                            return _response;
                        }
                        break;

                    case "outcomenotes":
                        query = query.Where(x =>
                            x.OutcomeNotes != null &&
                            x.OutcomeNotes.ToLower().Contains(filterValue.ToLower()));
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
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CreatedAt date.";
                            return _response;
                        }
                        break;

                    case "updatedat":
                        if (DateTime.TryParse(filterValue, out DateTime updatedAt))
                        {
                            query = query.Where(x =>
                                x.UpdatedAt.Date == updatedAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid UpdatedAt date.";
                            return _response;
                        }
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        _response.ActionResponse = "Invalid filter field.";
                        return _response;
                }
            }

            // =========================
            // Sorting
            // =========================
            bool isDescending = sortOrder?.ToLower() == "desc";

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "id":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Id)
                            : query.OrderBy(x => x.Id);
                        break;

                    case "tendernumber":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TenderNumber)
                            : query.OrderBy(x => x.TenderNumber);
                        break;

                    case "tendertitle":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TenderTitle)
                            : query.OrderBy(x => x.TenderTitle);
                        break;

                    case "customerid":
                        query = isDescending
                            ? query.OrderByDescending(x => x.CustomerId)
                            : query.OrderBy(x => x.CustomerId);
                        break;

                    case "tenderportal":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TenderPortal)
                            : query.OrderBy(x => x.TenderPortal);
                        break;

                    case "gembidnumber":
                        query = isDescending
                            ? query.OrderByDescending(x => x.GemBidNumber)
                            : query.OrderBy(x => x.GemBidNumber);
                        break;

                    case "publicationdate":
                        query = isDescending
                            ? query.OrderByDescending(x => x.PublicationDate)
                            : query.OrderBy(x => x.PublicationDate);
                        break;

                    case "prebidmeetingdate":
                        query = isDescending
                            ? query.OrderByDescending(x => x.PreBidMeetingDate)
                            : query.OrderBy(x => x.PreBidMeetingDate);
                        break;

                    case "querysubmissiondeadline":
                        query = isDescending
                            ? query.OrderByDescending(x => x.QuerySubmissionDeadline)
                            : query.OrderBy(x => x.QuerySubmissionDeadline);
                        break;

                    case "bidsubmissiondeadline":
                        query = isDescending
                            ? query.OrderByDescending(x => x.BidSubmissionDeadline)
                            : query.OrderBy(x => x.BidSubmissionDeadline);
                        break;

                    case "technicalopeningdate":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TechnicalOpeningDate)
                            : query.OrderBy(x => x.TechnicalOpeningDate);
                        break;

                    case "financialopeningdate":
                        query = isDescending
                            ? query.OrderByDescending(x => x.FinancialOpeningDate)
                            : query.OrderBy(x => x.FinancialOpeningDate);
                        break;

                    case "estimatedtendervalueinr":
                        query = isDescending
                            ? query.OrderByDescending(x => x.EstimatedTenderValueInr)
                            : query.OrderBy(x => x.EstimatedTenderValueInr);
                        break;

                    case "emdamountinr":
                        query = isDescending
                            ? query.OrderByDescending(x => x.EmdAmountInr)
                            : query.OrderBy(x => x.EmdAmountInr);
                        break;

                    case "tenderfeeinr":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TenderFeeInr)
                            : query.OrderBy(x => x.TenderFeeInr);
                        break;

                    case "pbgrequiredpct":
                        query = isDescending
                            ? query.OrderByDescending(x => x.PbgRequiredPct)
                            : query.OrderBy(x => x.PbgRequiredPct);
                        break;

                    case "eligibilitycriteria":
                        query = isDescending
                            ? query.OrderByDescending(x => x.EligibilityCriteria)
                            : query.OrderBy(x => x.EligibilityCriteria);
                        break;

                    case "technicalrequirements":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TechnicalRequirements)
                            : query.OrderBy(x => x.TechnicalRequirements);
                        break;

                    case "oemrequirement":
                        query = isDescending
                            ? query.OrderByDescending(x => x.OemRequirement)
                            : query.OrderBy(x => x.OemRequirement);
                        break;

                    case "consortiumrequirement":
                        query = isDescending
                            ? query.OrderByDescending(x => x.ConsortiumRequirement)
                            : query.OrderBy(x => x.ConsortiumRequirement);
                        break;

                    case "miiclass":
                        query = isDescending
                            ? query.OrderByDescending(x => x.MiiClass)
                            : query.OrderBy(x => x.MiiClass);
                        break;

                    case "experiencecriteria":
                        query = isDescending
                            ? query.OrderByDescending(x => x.ExperienceCriteria)
                            : query.OrderBy(x => x.ExperienceCriteria);
                        break;

                    case "turnovercriteriainr":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TurnoverCriteriaInr)
                            : query.OrderBy(x => x.TurnoverCriteriaInr);
                        break;

                    case "bidownerid":
                        query = isDescending
                            ? query.OrderByDescending(x => x.BidOwnerId)
                            : query.OrderBy(x => x.BidOwnerId);
                        break;

                    case "presalesownerid":
                        query = isDescending
                            ? query.OrderByDescending(x => x.PresalesOwnerId)
                            : query.OrderBy(x => x.PresalesOwnerId);
                        break;

                    case "commercialownerid":
                        query = isDescending
                            ? query.OrderByDescending(x => x.CommercialOwnerId)
                            : query.OrderBy(x => x.CommercialOwnerId);
                        break;

                    case "partneroem":
                        query = isDescending
                            ? query.OrderByDescending(x => x.PartnerOem)
                            : query.OrderBy(x => x.PartnerOem);
                        break;

                    case "competitors":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Competitors)
                            : query.OrderBy(x => x.Competitors);
                        break;

                    case "bidstatus":
                        query = isDescending
                            ? query.OrderByDescending(x => x.BidStatus)
                            : query.OrderBy(x => x.BidStatus);
                        break;

                    case "winprobabilitypct":
                        query = isDescending
                            ? query.OrderByDescending(x => x.WinProbabilityPct)
                            : query.OrderBy(x => x.WinProbabilityPct);
                        break;

                    case "bidvaluesubmittedinr":
                        query = isDescending
                            ? query.OrderByDescending(x => x.BidValueSubmittedInr)
                            : query.OrderBy(x => x.BidValueSubmittedInr);
                        break;

                    case "l1bidder":
                        query = isDescending
                            ? query.OrderByDescending(x => x.L1Bidder)
                            : query.OrderBy(x => x.L1Bidder);
                        break;

                    case "l1valueinr":
                        query = isDescending
                            ? query.OrderByDescending(x => x.L1ValueInr)
                            : query.OrderBy(x => x.L1ValueInr);
                        break;

                    case "outcomenotes":
                        query = isDescending
                            ? query.OrderByDescending(x => x.OutcomeNotes)
                            : query.OrderBy(x => x.OutcomeNotes);
                        break;

                    case "createdat":
                        query = isDescending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);
                        break;

                    case "updatedat":
                        query = isDescending
                            ? query.OrderByDescending(x => x.UpdatedAt)
                            : query.OrderBy(x => x.UpdatedAt);
                        break;

                    default:
                        query = query.OrderBy(x => x.Id);
                        break;
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
            // No Data Found
            // =========================
            if (data == null || data.Count == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.ActionResponse = "Data not found.";
                return _response;
            }

            // =========================
            // Success Response
            // =========================
            _response.IsSuccess = true;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
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
