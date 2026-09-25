using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class LeadConversionService : ILeadConversionService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public LeadConversionService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllLeadConversion(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.lead_conversions.AsQueryable();

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

        public async Task<APIResponse> GetLeadConversionById(int id)
        {
            var data = await _context.lead_conversions.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect leadconversion id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateLeadConversion(CreateLeadConversion dto)
        {
            var exists = await _context.lead_conversions.AnyAsync(s => s.LeadId == dto.LeadId);
            if (exists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exists.";
                return _response;
            }

            var entity = new lead_conversions
            {
                LeadId = dto.LeadId,
                TenderId = dto.TenderId,
                CustomerId = dto.CustomerId,
                ConvertedById = dto.ConvertedById,
                ConversionNotes = dto.ConversionNotes,
                ConvertedAt = dto.ConvertedAt,
            };
            _context.lead_conversions.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "LeadConversion created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateLeadConversion(EditLeadConversion req)
        {
            var data = await _context.lead_conversions.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "LeadConversion not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.LeadId = req.LeadId;
            data.TenderId = req.TenderId;
            data.CustomerId = req.CustomerId;
            data.ConvertedById = req.ConvertedById;
            data.ConversionNotes = req.ConversionNotes;
            data.ConvertedAt = req.ConvertedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "LeadConversion updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteLeadConversion(int id)
        {
            var data = await _context.lead_conversions.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "LeadConversion not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.lead_conversions.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "LeadConversion deleted successfully.";
            return _response;
        }
    }
}
