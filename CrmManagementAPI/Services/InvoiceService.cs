using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.Common;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public InvoiceService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllInvoice(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.invoices.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.InvoiceNumber != null && s.InvoiceNumber.ToLower().Contains(search));
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

        public async Task<APIResponse> GetInvoiceById(int id)
        {
            var data = await _context.invoices.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect invoice id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateInvoice(CreateInvoice dto)
        {
            var exists = await _context.invoices.AnyAsync(s => s.InvoiceNumber == dto.InvoiceNumber);
            if (exists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exists.";
                return _response;
            }

            var entity = new invoices
            {
                InvoiceNumber = dto.InvoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                PoId = dto.PoId,
                MilestoneId = dto.MilestoneId,
                CustomerId = dto.CustomerId,
                TaxableAmountInr = dto.TaxableAmountInr,
                GstPct = dto.GstPct,
                GstAmountInr = dto.GstAmountInr,
                TotalInvoiceValueInr = dto.TotalInvoiceValueInr,
                SubmissionDate = dto.SubmissionDate,
                AcknowledgementDate = dto.AcknowledgementDate,
                AcknowledgementRef = dto.AcknowledgementRef,
                PaymentDueDate = dto.PaymentDueDate,
                PaymentReceivedInr = dto.PaymentReceivedInr,
                TdsAmountInr = dto.TdsAmountInr,
                OtherDeductionsInr = dto.OtherDeductionsInr,
                OutstandingAmountInr = dto.OutstandingAmountInr,
                InvoiceStatus = dto.InvoiceStatus,
                DisputeNotes = dto.DisputeNotes,
                Notes = dto.Notes,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
            };
            _context.invoices.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Invoice created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateInvoice(EditInvoice req)
        {
            var data = await _context.invoices.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Invoice not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.InvoiceNumber = req.InvoiceNumber;
            data.InvoiceDate = req.InvoiceDate;
            data.PoId = req.PoId;
            data.MilestoneId = req.MilestoneId;
            data.CustomerId = req.CustomerId;
            data.TaxableAmountInr = req.TaxableAmountInr;
            data.GstPct = req.GstPct;
            data.GstAmountInr = req.GstAmountInr;
            data.TotalInvoiceValueInr = req.TotalInvoiceValueInr;
            data.SubmissionDate = req.SubmissionDate;
            data.AcknowledgementDate = req.AcknowledgementDate;
            data.AcknowledgementRef = req.AcknowledgementRef;
            data.PaymentDueDate = req.PaymentDueDate;
            data.PaymentReceivedInr = req.PaymentReceivedInr;
            data.TdsAmountInr = req.TdsAmountInr;
            data.OtherDeductionsInr = req.OtherDeductionsInr;
            data.OutstandingAmountInr = req.OutstandingAmountInr;
            data.InvoiceStatus = req.InvoiceStatus;
            data.DisputeNotes = req.DisputeNotes;
            data.Notes = req.Notes;
            data.CreatedAt = req.CreatedAt;
            data.UpdatedAt = req.UpdatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "Invoice updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteInvoice(int id)
        {
            var data = await _context.invoices.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Invoice not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.invoices.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Invoice deleted successfully.";
            return _response;
        }

        public async Task<APIResponse> GetInvoiceStatusDropdown()
        {
            var data = CategoryConstants.Invoice_status
                .Select(x => new
                {
                    Value = x,
                    Label = x
                })
                .ToList();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Decision dropdown data found successfully.";
            _response.Result = data;

            return _response;
        }

        public async Task<APIResponse> GetAllInvoiceList(string? search, string? filterField, string? filterValue, string? sortField, string? sortOrder = "asc",
        int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.invoices.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.InvoiceNumber.ToLower().Contains(search) || x.AcknowledgementRef!.ToLower().Contains(search) ||
                    x.InvoiceStatus.ToLower().Contains(search) || x.DisputeNotes!.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)
                );
            }

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

                    case "invoicenumber":
                        query = query.Where(x =>x.InvoiceNumber.ToLower().Contains(filterValue));
                        break;

                    case "invoicedate":
                        if (DateOnly.TryParse(filterValue, out DateOnly invoiceDate))
                        {
                            query = query.Where(x => x.InvoiceDate == invoiceDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid InvoiceDate value. Use yyyy-MM-dd.";

                            return _response;
                        }
                        break;

                    case "poid":
                        if (int.TryParse(filterValue, out int poId))
                        {
                            query = query.Where(x => x.PoId == poId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid PoId value.";

                            return _response;
                        }
                        break;

                    case "milestoneid":
                        if (int.TryParse(filterValue, out int milestoneId))
                        {
                            query = query.Where(x => x.MilestoneId == milestoneId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid MilestoneId value.";

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
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid CustomerId value.";

                            return _response;
                        }
                        break;

                    case "taxableamountinr":
                        if (decimal.TryParse(filterValue, out decimal taxableAmountInr))
                        {
                            query = query.Where(x =>x.TaxableAmountInr == taxableAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid TaxableAmountInr value.";

                            return _response;
                        }
                        break;

                    case "gstpct":
                        if (decimal.TryParse(filterValue, out decimal gstPct))
                        {
                            query = query.Where(x => x.GstPct == gstPct);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid GstPct value.";

                            return _response;
                        }
                        break;

                    case "gstamountinr":
                        if (decimal.TryParse(filterValue, out decimal gstAmountInr))
                        {
                            query = query.Where(x =>x.GstAmountInr == gstAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid GstAmountInr value.";

                            return _response;
                        }
                        break;

                    case "totalinvoicevalueinr":
                        if (decimal.TryParse(filterValue,out decimal totalInvoiceValueInr))
                        {
                            query = query.Where(x =>x.TotalInvoiceValueInr == totalInvoiceValueInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid TotalInvoiceValueInr value.";

                            return _response;
                        }
                        break;

                    case "submissiondate":
                        if (DateOnly.TryParse(filterValue,out DateOnly submissionDate))
                        {
                            query = query.Where(x =>x.SubmissionDate == submissionDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid SubmissionDate value. Use yyyy-MM-dd.";

                            return _response;
                        }
                        break;

                    case "acknowledgementdate":
                        if (DateOnly.TryParse(filterValue,out DateOnly acknowledgementDate))
                        {
                            query = query.Where(x =>x.AcknowledgementDate == acknowledgementDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid AcknowledgementDate value. Use yyyy-MM-dd.";

                            return _response;
                        }
                        break;

                    case "acknowledgementref":
                        query = query.Where(x =>x.AcknowledgementRef != null &&x.AcknowledgementRef.ToLower().Contains(filterValue));
                        break;

                    case "paymentduedate":
                        if (DateOnly.TryParse(filterValue,out DateOnly paymentDueDate))
                        {
                            query = query.Where(x =>x.PaymentDueDate == paymentDueDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid PaymentDueDate value. Use yyyy-MM-dd.";

                            return _response;
                        }
                        break;

                    case "paymentreceivedinr":
                        if (decimal.TryParse(filterValue,out decimal paymentReceivedInr))
                        {
                            query = query.Where(x =>x.PaymentReceivedInr == paymentReceivedInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid PaymentReceivedInr value.";

                            return _response;
                        }
                        break;

                    case "tdsamountinr":
                        if (decimal.TryParse(filterValue,out decimal tdsAmountInr))
                        {
                            query = query.Where(x =>x.TdsAmountInr == tdsAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TdsAmountInr value.";

                            return _response;
                        }
                        break;

                    case "otherdeductionsinr":
                        if (decimal.TryParse(filterValue,out decimal otherDeductionsInr))
                        {
                            query = query.Where(x =>x.OtherDeductionsInr == otherDeductionsInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid OtherDeductionsInr value.";

                            return _response;
                        }
                        break;

                    case "outstandingamountinr":
                        if (decimal.TryParse(filterValue,out decimal outstandingAmountInr))
                        {
                            query = query.Where(x => x.OutstandingAmountInr == outstandingAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid OutstandingAmountInr value.";

                            return _response;
                        }
                        break;

                    case "invoicestatus":
                        query = query.Where(x =>x.InvoiceStatus.ToLower().Contains(filterValue));
                        break;

                    case "disputenotes":
                        query = query.Where(x =>x.DisputeNotes != null &&x.DisputeNotes.ToLower().Contains(filterValue));
                        break;

                    case "notes":
                        query = query.Where(x => x.Notes != null && x.Notes.ToLower().Contains(filterValue));
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
                        query = descending ? query.OrderByDescending(x => x.Id): query.OrderBy(x => x.Id);
                        break;

                    case "invoicenumber":
                        query = descending? query.OrderByDescending(x => x.InvoiceNumber): query.OrderBy(x => x.InvoiceNumber);
                        break;

                    case "invoicedate":
                        query = descending? query.OrderByDescending(x => x.InvoiceDate): query.OrderBy(x => x.InvoiceDate);
                        break;

                    case "poid":
                        query = descending? query.OrderByDescending(x => x.PoId): query.OrderBy(x => x.PoId);
                        break;

                    case "milestoneid":
                        query = descending? query.OrderByDescending(x => x.MilestoneId): query.OrderBy(x => x.MilestoneId);
                        break;

                    case "customerid":
                        query = descending? query.OrderByDescending(x => x.CustomerId): query.OrderBy(x => x.CustomerId);
                        break;

                    case "taxableamountinr":
                        query = descending? query.OrderByDescending(x => x.TaxableAmountInr): query.OrderBy(x => x.TaxableAmountInr);
                        break;

                    case "gstpct":
                        query = descending? query.OrderByDescending(x => x.GstPct): query.OrderBy(x => x.GstPct);
                        break;

                    case "gstamountinr":
                        query = descending? query.OrderByDescending(x => x.GstAmountInr): query.OrderBy(x => x.GstAmountInr);
                        break;

                    case "totalinvoicevalueinr":
                        query = descending? query.OrderByDescending(x => x.TotalInvoiceValueInr): query.OrderBy(x => x.TotalInvoiceValueInr);
                        break;

                    case "submissiondate":
                        query = descending? query.OrderByDescending(x => x.SubmissionDate): query.OrderBy(x => x.SubmissionDate);
                        break;

                    case "acknowledgementdate":
                        query = descending? query.OrderByDescending(x => x.AcknowledgementDate): query.OrderBy(x => x.AcknowledgementDate);
                        break;

                    case "acknowledgementref":
                        query = descending? query.OrderByDescending(x => x.AcknowledgementRef): query.OrderBy(x => x.AcknowledgementRef);
                        break;

                    case "paymentduedate":
                        query = descending? query.OrderByDescending(x => x.PaymentDueDate): query.OrderBy(x => x.PaymentDueDate);
                        break;

                    case "paymentreceivedinr":
                        query = descending? query.OrderByDescending(x => x.PaymentReceivedInr): query.OrderBy(x => x.PaymentReceivedInr);
                        break;

                    case "tdsamountinr":
                        query = descending? query.OrderByDescending(x => x.TdsAmountInr): query.OrderBy(x => x.TdsAmountInr);
                        break;

                    case "otherdeductionsinr":
                        query = descending? query.OrderByDescending(x => x.OtherDeductionsInr): query.OrderBy(x => x.OtherDeductionsInr);
                        break;

                    case "outstandingamountinr":
                        query = descending? query.OrderByDescending(x => x.OutstandingAmountInr): query.OrderBy(x => x.OutstandingAmountInr);
                        break;

                    case "invoicestatus":
                        query = descending? query.OrderByDescending(x => x.InvoiceStatus): query.OrderBy(x => x.InvoiceStatus);
                        break;

                    case "disputenotes":
                        query = descending? query.OrderByDescending(x => x.DisputeNotes): query.OrderBy(x => x.DisputeNotes);
                        break;

                    case "notes":
                        query = descending? query.OrderByDescending(x => x.Notes): query.OrderBy(x => x.Notes);
                        break;

                    case "createdat":
                        query = descending? query.OrderByDescending(x => x.CreatedAt): query.OrderBy(x => x.CreatedAt);
                        break;

                    case "updatedat":
                        query = descending? query.OrderByDescending(x => x.UpdatedAt): query.OrderBy(x => x.UpdatedAt);
                        break;

                    default:
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ActionResponse =$"Invalid sort field: {sortField}";

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
