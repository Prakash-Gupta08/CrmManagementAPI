using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class TenderAlertService : ITenderAlertService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public TenderAlertService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllTenderAlert(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tender_alerts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.AlertType != null && s.AlertType.ToLower().Contains(search));
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

        public async Task<APIResponse> GetTenderAlertById(int id)
        {
            var data = await _context.tender_alerts.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect tenderalert id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateTenderAlert(CreateTenderAlert dto)
        {
            var entity = new tender_alerts
            {
                TenderId = dto.TenderId,
                AlertType = dto.AlertType,
                AlertDate = dto.AlertDate,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedAt = dto.CreatedAt,
            };
            _context.tender_alerts.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "TenderAlert created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateTenderAlert(EditTenderAlert req)
        {
            var data = await _context.tender_alerts.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "TenderAlert not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.TenderId = req.TenderId;
            data.AlertType = req.AlertType;
            data.AlertDate = req.AlertDate;
            data.Status = req.Status;
            data.Notes = req.Notes;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "TenderAlert updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteTenderAlert(int id)
        {
            var data = await _context.tender_alerts.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "TenderAlert not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.tender_alerts.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "TenderAlert deleted successfully.";
            return _response;
        }

        public async Task<APIResponse> GetAllTenderAlertsList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tender_alerts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||
                    x.TenderId.ToString().Contains(search) ||
                    x.AlertType.ToLower().Contains(search) ||
                    x.Status.ToLower().Contains(search) ||
                    (x.Notes != null && x.Notes.ToLower().Contains(search))
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

                    case "tenderid":
                        if (int.TryParse(filterValue, out int tenderId))
                        {
                            query = query.Where(x => x.TenderId == tenderId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TenderId.";
                            return _response;
                        }
                        break;

                    case "alerttype":
                        query = query.Where(x =>
                            x.AlertType.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "alertdate":
                        if (DateOnly.TryParse(filterValue, out DateOnly alertDate))
                        {
                            query = query.Where(x => x.AlertDate == alertDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid AlertDate. Use yyyy-MM-dd format.";
                            return _response;
                        }
                        break;

                    case "status":
                        query = query.Where(x =>
                            x.Status.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "notes":
                        query = query.Where(x =>
                            x.Notes != null &&
                            x.Notes.ToLower().Contains(filterValue.ToLower()));
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

                    case "tenderid":
                        query = isDescending
                            ? query.OrderByDescending(x => x.TenderId)
                            : query.OrderBy(x => x.TenderId);
                        break;

                    case "alerttype":
                        query = isDescending
                            ? query.OrderByDescending(x => x.AlertType)
                            : query.OrderBy(x => x.AlertType);
                        break;

                    case "alertdate":
                        query = isDescending
                            ? query.OrderByDescending(x => x.AlertDate)
                            : query.OrderBy(x => x.AlertDate);
                        break;

                    case "status":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Status)
                            : query.OrderBy(x => x.Status);
                        break;

                    case "notes":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Notes)
                            : query.OrderBy(x => x.Notes);
                        break;

                    case "createdat":
                        query = isDescending
                            ? query.OrderByDescending(x => x.CreatedAt)
                            : query.OrderBy(x => x.CreatedAt);
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
