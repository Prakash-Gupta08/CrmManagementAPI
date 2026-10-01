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
        public async Task<APIResponse> GetAllPaymentsList(string? search,string? filterField,string? filterValue,string? sortField,
        string? sortOrder = "asc",int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.payments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||x.InvoiceId.ToString().Contains(search) ||
                    x.AmountReceivedInr.ToString().Contains(search) ||x.TdsDeductedInr.ToString().Contains(search) ||
                    x.OtherDeductionInr.ToString().Contains(search) ||(x.PaymentMode != null && x.PaymentMode.ToLower().Contains(search)) ||
                    (x.ReferenceNumber != null && x.ReferenceNumber.ToLower().Contains(search)) ||
                    (x.BankName != null && x.BankName.ToLower().Contains(search)) ||
                    (x.Notes != null && x.Notes.ToLower().Contains(search)) ||
                    (x.RecordedById != null && x.RecordedById.ToString().Contains(search))
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ID value.";
                            return _response;
                        }
                        break;

                    case "invoiceid":
                        if (int.TryParse(filterValue, out int invoiceId))
                        {
                            query = query.Where(x =>
                                x.InvoiceId == invoiceId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid InvoiceId value.";
                            return _response;
                        }
                        break;

                    case "paymentdate":
                        if (DateOnly.TryParse(filterValue, out DateOnly paymentDate))
                        {
                            query = query.Where(x =>
                                x.PaymentDate == paymentDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid PaymentDate value.";
                            return _response;
                        }
                        break;

                    case "amountreceivedinr":
                        if (decimal.TryParse(filterValue, out decimal amountReceivedInr))
                        {
                            query = query.Where(x =>
                                x.AmountReceivedInr == amountReceivedInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid AmountReceivedInr value.";
                            return _response;
                        }
                        break;

                    case "tdsdeductedinr":
                        if (decimal.TryParse(filterValue, out decimal tdsDeductedInr))
                        {
                            query = query.Where(x =>
                                x.TdsDeductedInr == tdsDeductedInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid TdsDeductedInr value.";
                            return _response;
                        }
                        break;

                    case "otherdeductioninr":
                        if (decimal.TryParse(filterValue, out decimal otherDeductionInr))
                        {
                            query = query.Where(x =>
                                x.OtherDeductionInr == otherDeductionInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid OtherDeductionInr value.";
                            return _response;
                        }
                        break;

                    case "paymentmode":
                        query = query.Where(x =>
                            x.PaymentMode != null &&
                            x.PaymentMode.ToLower().Contains(filterValue));
                        break;

                    case "referencenumber":
                        query = query.Where(x =>
                            x.ReferenceNumber != null &&
                            x.ReferenceNumber.ToLower().Contains(filterValue));
                        break;

                    case "bankname":
                        query = query.Where(x =>
                            x.BankName != null &&
                            x.BankName.ToLower().Contains(filterValue));
                        break;

                    case "notes":
                        query = query.Where(x =>
                            x.Notes != null &&
                            x.Notes.ToLower().Contains(filterValue));
                        break;

                    case "recordedbyid":
                        if (int.TryParse(filterValue, out int recordedById))
                        {
                            query = query.Where(x =>
                                x.RecordedById == recordedById);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid RecordedById value.";
                            return _response;
                        }
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid CreatedAt value.";
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

                    case "invoiceid":
                        query = descending
                            ? query.OrderByDescending(x => x.InvoiceId)
                            : query.OrderBy(x => x.InvoiceId);
                        break;

                    case "paymentdate":
                        query = descending
                            ? query.OrderByDescending(x => x.PaymentDate)
                            : query.OrderBy(x => x.PaymentDate);
                        break;

                    case "amountreceivedinr":
                        query = descending
                            ? query.OrderByDescending(x => x.AmountReceivedInr)
                            : query.OrderBy(x => x.AmountReceivedInr);
                        break;

                    case "tdsdeductedinr":
                        query = descending
                            ? query.OrderByDescending(x => x.TdsDeductedInr)
                            : query.OrderBy(x => x.TdsDeductedInr);
                        break;

                    case "otherdeductioninr":
                        query = descending
                            ? query.OrderByDescending(x => x.OtherDeductionInr)
                            : query.OrderBy(x => x.OtherDeductionInr);
                        break;

                    case "paymentmode":
                        query = descending
                            ? query.OrderByDescending(x => x.PaymentMode)
                            : query.OrderBy(x => x.PaymentMode);
                        break;

                    case "referencenumber":
                        query = descending
                            ? query.OrderByDescending(x => x.ReferenceNumber)
                            : query.OrderBy(x => x.ReferenceNumber);
                        break;

                    case "bankname":
                        query = descending
                            ? query.OrderByDescending(x => x.BankName)
                            : query.OrderBy(x => x.BankName);
                        break;

                    case "notes":
                        query = descending
                            ? query.OrderByDescending(x => x.Notes)
                            : query.OrderBy(x => x.Notes);
                        break;

                    case "recordedbyid":
                        query = descending
                            ? query.OrderByDescending(x => x.RecordedById)
                            : query.OrderBy(x => x.RecordedById);
                        break;

                    case "createdat":
                        query = descending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);
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
