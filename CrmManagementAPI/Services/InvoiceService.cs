using CrmManagementAPI.AppDbContext;
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
    }
}
