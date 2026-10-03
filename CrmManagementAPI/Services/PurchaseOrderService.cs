using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public PurchaseOrderService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllPurchaseOrder(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.purchase_orders.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.ProjectName != null && s.ProjectName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.ProjectName)
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

        public async Task<APIResponse> GetPurchaseOrderById(int id)
        {
            var data = await _context.purchase_orders.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect purchaseorder id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreatePurchaseOrder(CreatePurchaseOrder dto)
        {
            var exists = await _context.purchase_orders.AnyAsync(s => s.PoNumber == dto.PoNumber);
            if (exists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exists.";
                return _response;
            }

            var entity = new purchase_orders
            {
                PoNumber = dto.PoNumber,
                PoDate = dto.PoDate,
                CustomerId = dto.CustomerId,
                TenderId = dto.TenderId,
                ProjectName = dto.ProjectName,
                PoValueInr = dto.PoValueInr,
                GstPct = dto.GstPct,
                GstAmountInr = dto.GstAmountInr,
                TotalContractValueInr = dto.TotalContractValueInr,
                StartDate = dto.StartDate,
                CompletionDate = dto.CompletionDate,
                DeliverySchedule = dto.DeliverySchedule,
                PaymentTerms = dto.PaymentTerms,
                WarrantyMonths = dto.WarrantyMonths,
                AmcYears = dto.AmcYears,
                SlaTerms = dto.SlaTerms,
                LdPenaltyTerms = dto.LdPenaltyTerms,
                PbgAmountInr = dto.PbgAmountInr,
                PbgValidityDate = dto.PbgValidityDate,
                PerformanceObligations = dto.PerformanceObligations,
                PoStatus = dto.PoStatus,
                AccountOwnerId = dto.AccountOwnerId,
                ProjectManagerId = dto.ProjectManagerId,
                Notes = dto.Notes,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
            };
            _context.purchase_orders.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "PurchaseOrder created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdatePurchaseOrder(EditPurchaseOrder req)
        {
            var data = await _context.purchase_orders.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "PurchaseOrder not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.PoNumber = req.PoNumber;
            data.PoDate = req.PoDate;
            data.CustomerId = req.CustomerId;
            data.TenderId = req.TenderId;
            data.ProjectName = req.ProjectName;
            data.PoValueInr = req.PoValueInr;
            data.GstPct = req.GstPct;
            data.GstAmountInr = req.GstAmountInr;
            data.TotalContractValueInr = req.TotalContractValueInr;
            data.StartDate = req.StartDate;
            data.CompletionDate = req.CompletionDate;
            data.DeliverySchedule = req.DeliverySchedule;
            data.PaymentTerms = req.PaymentTerms;
            data.WarrantyMonths = req.WarrantyMonths;
            data.AmcYears = req.AmcYears;
            data.SlaTerms = req.SlaTerms;
            data.LdPenaltyTerms = req.LdPenaltyTerms;
            data.PbgAmountInr = req.PbgAmountInr;
            data.PbgValidityDate = req.PbgValidityDate;
            data.PerformanceObligations = req.PerformanceObligations;
            data.PoStatus = req.PoStatus;
            data.AccountOwnerId = req.AccountOwnerId;
            data.ProjectManagerId = req.ProjectManagerId;
            data.Notes = req.Notes;
            data.CreatedAt = req.CreatedAt;
            data.UpdatedAt = req.UpdatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "PurchaseOrder updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeletePurchaseOrder(int id)
        {
            var data = await _context.purchase_orders.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "PurchaseOrder not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.purchase_orders.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "PurchaseOrder deleted successfully.";
            return _response;
        }


        public async Task<APIResponse> GetAllPurchaseOrdersList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.purchase_orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||x.PoNumber.ToLower().Contains(search) ||
                    x.CustomerId.ToString().Contains(search) ||x.TenderId.ToString().Contains(search) ||
                    x.ProjectName.ToLower().Contains(search) ||x.PoValueInr.ToString().Contains(search) ||
                    x.GstPct.ToString().Contains(search) ||x.GstAmountInr.ToString().Contains(search) ||
                    x.TotalContractValueInr.ToString().Contains(search) ||(x.DeliverySchedule != null && x.DeliverySchedule.ToLower().Contains(search)) ||
                    (x.PaymentTerms != null && x.PaymentTerms.ToLower().Contains(search)) || x.WarrantyMonths.ToString().Contains(search) ||x.AmcYears.ToString().Contains(search) ||
                    (x.SlaTerms != null && x.SlaTerms.ToLower().Contains(search)) || (x.LdPenaltyTerms != null && x.LdPenaltyTerms.ToLower().Contains(search)) ||
                    x.PbgAmountInr.ToString().Contains(search) || (x.PerformanceObligations != null && x.PerformanceObligations.ToLower().Contains(search)) ||
                    x.PoStatus.ToLower().Contains(search) || x.AccountOwnerId.ToString().Contains(search) ||
                    x.ProjectManagerId.ToString().Contains(search) || (x.Notes != null && x.Notes.ToLower().Contains(search))
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ID value.";
                            return _response;
                        }
                        break;

                    case "ponumber":
                        query = query.Where(x =>
                            x.PoNumber.ToLower().Contains(filterValue));
                        break;

                    case "podate":
                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly poDate))
                        {
                            query = query.Where(x =>
                                x.PoDate == poDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid PoDate value.";
                            return _response;
                        }
                        break;

                    case "customerid":
                        if (int.TryParse(filterValue, out int customerId))
                        {
                            query = query.Where(x => x.CustomerId == customerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CustomerId value.";
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid TenderId value.";
                            return _response;
                        }
                        break;

                    case "projectname":
                        query = query.Where(x =>
                            x.ProjectName.ToLower().Contains(filterValue));
                        break;

                    case "povalueinr":
                        if (decimal.TryParse(
                            filterValue,
                            out decimal poValueInr))
                        {
                            query = query.Where(x =>
                                x.PoValueInr == poValueInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid PoValueInr value.";
                            return _response;
                        }
                        break;

                    case "gstpct":
                        if (decimal.TryParse(
                            filterValue,
                            out decimal gstPct))
                        {
                            query = query.Where(x =>
                                x.GstPct == gstPct);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid GstPct value.";
                            return _response;
                        }
                        break;

                    case "gstamountinr":
                        if (decimal.TryParse(
                            filterValue,
                            out decimal gstAmountInr))
                        {
                            query = query.Where(x =>
                                x.GstAmountInr == gstAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid GstAmountInr value.";
                            return _response;
                        }
                        break;

                    case "totalcontractvalueinr":
                        if (decimal.TryParse(
                            filterValue,
                            out decimal totalContractValueInr))
                        {
                            query = query.Where(x =>
                                x.TotalContractValueInr == totalContractValueInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid TotalContractValueInr value.";
                            return _response;
                        }
                        break;

                    case "startdate":
                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly startDate))
                        {
                            query = query.Where(x =>
                                x.StartDate == startDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid StartDate value.";
                            return _response;
                        }
                        break;

                    case "completiondate":
                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly completionDate))
                        {
                            query = query.Where(x =>
                                x.CompletionDate == completionDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid CompletionDate value.";
                            return _response;
                        }
                        break;

                    case "deliveryschedule":
                        query = query.Where(x =>
                            x.DeliverySchedule != null &&
                            x.DeliverySchedule.ToLower().Contains(filterValue));
                        break;

                    case "paymentterms":
                        query = query.Where(x =>
                            x.PaymentTerms != null &&
                            x.PaymentTerms.ToLower().Contains(filterValue));
                        break;

                    case "warrantymonths":
                        if (int.TryParse(
                            filterValue,
                            out int warrantyMonths))
                        {
                            query = query.Where(x =>
                                x.WarrantyMonths == warrantyMonths);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid WarrantyMonths value.";
                            return _response;
                        }
                        break;

                    case "amcyears":
                        if (int.TryParse(
                            filterValue,
                            out int amcYears))
                        {
                            query = query.Where(x =>
                                x.AmcYears == amcYears);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid AmcYears value.";
                            return _response;
                        }
                        break;

                    case "slaterms":
                        query = query.Where(x =>
                            x.SlaTerms != null &&
                            x.SlaTerms.ToLower().Contains(filterValue));
                        break;

                    case "ldpenaltyterms":
                        query = query.Where(x =>
                            x.LdPenaltyTerms != null &&
                            x.LdPenaltyTerms.ToLower().Contains(filterValue));
                        break;

                    case "pbgamountinr":
                        if (decimal.TryParse(
                            filterValue,
                            out decimal pbgAmountInr))
                        {
                            query = query.Where(x =>
                                x.PbgAmountInr == pbgAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid PbgAmountInr value.";
                            return _response;
                        }
                        break;

                    case "pbgvaliditydate":
                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly pbgValidityDate))
                        {
                            query = query.Where(x =>
                                x.PbgValidityDate == pbgValidityDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid PbgValidityDate value.";
                            return _response;
                        }
                        break;

                    case "performanceobligations":
                        query = query.Where(x =>
                            x.PerformanceObligations != null &&
                            x.PerformanceObligations.ToLower().Contains(filterValue));
                        break;

                    case "postatus":
                        query = query.Where(x =>
                            x.PoStatus.ToLower().Contains(filterValue));
                        break;

                    case "accountownerid":
                        if (int.TryParse(
                            filterValue,
                            out int accountOwnerId))
                        {
                            query = query.Where(x =>
                                x.AccountOwnerId == accountOwnerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid AccountOwnerId value.";
                            return _response;
                        }
                        break;

                    case "projectmanagerid":
                        if (int.TryParse(
                            filterValue,
                            out int projectManagerId))
                        {
                            query = query.Where(x =>
                                x.ProjectManagerId == projectManagerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ProjectManagerId value.";
                            return _response;
                        }
                        break;

                    case "notes":
                        query = query.Where(x =>
                            x.Notes != null &&
                            x.Notes.ToLower().Contains(filterValue));
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid UpdatedAt value.";
                            return _response;
                        }
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode =
                            System.Net.HttpStatusCode.BadRequest;
                        _response.ActionResponse =
                            $"Invalid filter field: {filterField}";
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

                    case "ponumber":
                        query = descending
                            ? query.OrderByDescending(x => x.PoNumber)
                            : query.OrderBy(x => x.PoNumber);
                        break;

                    case "podate":
                        query = descending
                            ? query.OrderByDescending(x => x.PoDate)
                            : query.OrderBy(x => x.PoDate);
                        break;

                    case "customerid":
                        query = descending
                            ? query.OrderByDescending(x => x.CustomerId)
                            : query.OrderBy(x => x.CustomerId);
                        break;

                    case "tenderid":
                        query = descending
                            ? query.OrderByDescending(x => x.TenderId)
                            : query.OrderBy(x => x.TenderId);
                        break;

                    case "projectname":
                        query = descending
                            ? query.OrderByDescending(x => x.ProjectName)
                            : query.OrderBy(x => x.ProjectName);
                        break;

                    case "povalueinr":
                        query = descending
                            ? query.OrderByDescending(x => x.PoValueInr)
                            : query.OrderBy(x => x.PoValueInr);
                        break;

                    case "gstpct":
                        query = descending
                            ? query.OrderByDescending(x => x.GstPct)
                            : query.OrderBy(x => x.GstPct);
                        break;

                    case "gstamountinr":
                        query = descending
                            ? query.OrderByDescending(x => x.GstAmountInr)
                            : query.OrderBy(x => x.GstAmountInr);
                        break;

                    case "totalcontractvalueinr":
                        query = descending
                            ? query.OrderByDescending(x => x.TotalContractValueInr)
                            : query.OrderBy(x => x.TotalContractValueInr);
                        break;

                    case "startdate":
                        query = descending
                            ? query.OrderByDescending(x => x.StartDate)
                            : query.OrderBy(x => x.StartDate);
                        break;

                    case "completiondate":
                        query = descending
                            ? query.OrderByDescending(x => x.CompletionDate)
                            : query.OrderBy(x => x.CompletionDate);
                        break;

                    case "deliveryschedule":
                        query = descending
                            ? query.OrderByDescending(x => x.DeliverySchedule)
                            : query.OrderBy(x => x.DeliverySchedule);
                        break;

                    case "paymentterms":
                        query = descending
                            ? query.OrderByDescending(x => x.PaymentTerms)
                            : query.OrderBy(x => x.PaymentTerms);
                        break;

                    case "warrantymonths":
                        query = descending
                            ? query.OrderByDescending(x => x.WarrantyMonths)
                            : query.OrderBy(x => x.WarrantyMonths);
                        break;

                    case "amcyears":
                        query = descending
                            ? query.OrderByDescending(x => x.AmcYears)
                            : query.OrderBy(x => x.AmcYears);
                        break;

                    case "slaterms":
                        query = descending
                            ? query.OrderByDescending(x => x.SlaTerms)
                            : query.OrderBy(x => x.SlaTerms);
                        break;

                    case "ldpenaltyterms":
                        query = descending
                            ? query.OrderByDescending(x => x.LdPenaltyTerms)
                            : query.OrderBy(x => x.LdPenaltyTerms);
                        break;

                    case "pbgamountinr":
                        query = descending
                            ? query.OrderByDescending(x => x.PbgAmountInr)
                            : query.OrderBy(x => x.PbgAmountInr);
                        break;

                    case "pbgvaliditydate":
                        query = descending
                            ? query.OrderByDescending(x => x.PbgValidityDate)
                            : query.OrderBy(x => x.PbgValidityDate);
                        break;

                    case "performanceobligations":
                        query = descending
                            ? query.OrderByDescending(x => x.PerformanceObligations)
                            : query.OrderBy(x => x.PerformanceObligations);
                        break;

                    case "postatus":
                        query = descending
                            ? query.OrderByDescending(x => x.PoStatus)
                            : query.OrderBy(x => x.PoStatus);
                        break;

                    case "accountownerid":
                        query = descending
                            ? query.OrderByDescending(x => x.AccountOwnerId)
                            : query.OrderBy(x => x.AccountOwnerId);
                        break;

                    case "projectmanagerid":
                        query = descending
                            ? query.OrderByDescending(x => x.ProjectManagerId)
                            : query.OrderBy(x => x.ProjectManagerId);
                        break;

                    case "notes":
                        query = descending
                            ? query.OrderByDescending(x => x.Notes)
                            : query.OrderBy(x => x.Notes);
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
                        _response.StatusCode =
                            System.Net.HttpStatusCode.BadRequest;
                        _response.ActionResponse =
                            $"Invalid sort field: {sortField}";
                        return _response;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }

            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!data.Any())
            {
                _response.IsSuccess = false;
                _response.StatusCode =
                    System.Net.HttpStatusCode.NotFound;
                _response.ActionResponse =
                    "Data not found.";
                return _response;
            }

            _response.IsSuccess = true;
            _response.StatusCode =
                System.Net.HttpStatusCode.OK;
            _response.ActionResponse =
                "Data found successfully.";

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
