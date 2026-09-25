using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
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
    }
}
