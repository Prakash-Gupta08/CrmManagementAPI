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
    }
}
