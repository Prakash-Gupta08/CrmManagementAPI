using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public PaymentService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllPayment(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.payments.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.ReferenceNumber != null && s.ReferenceNumber.ToLower().Contains(search));
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

        public async Task<APIResponse> GetPaymentById(int id)
        {
            var data = await _context.payments.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect payment id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreatePayment(CreatePayment dto)
        {
            var entity = new payments
            {
                InvoiceId = dto.InvoiceId,
                PaymentDate = dto.PaymentDate,
                AmountReceivedInr = dto.AmountReceivedInr,
                TdsDeductedInr = dto.TdsDeductedInr,
                OtherDeductionInr = dto.OtherDeductionInr,
                PaymentMode = dto.PaymentMode,
                ReferenceNumber = dto.ReferenceNumber,
                BankName = dto.BankName,
                Notes = dto.Notes,
                RecordedById = dto.RecordedById,
                CreatedAt = dto.CreatedAt,
            };
            _context.payments.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Payment created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdatePayment(EditPayment req)
        {
            var data = await _context.payments.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Payment not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.InvoiceId = req.InvoiceId;
            data.PaymentDate = req.PaymentDate;
            data.AmountReceivedInr = req.AmountReceivedInr;
            data.TdsDeductedInr = req.TdsDeductedInr;
            data.OtherDeductionInr = req.OtherDeductionInr;
            data.PaymentMode = req.PaymentMode;
            data.ReferenceNumber = req.ReferenceNumber;
            data.BankName = req.BankName;
            data.Notes = req.Notes;
            data.RecordedById = req.RecordedById;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "Payment updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeletePayment(int id)
        {
            var data = await _context.payments.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Payment not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.payments.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Payment deleted successfully.";
            return _response;
        }
    }
}
