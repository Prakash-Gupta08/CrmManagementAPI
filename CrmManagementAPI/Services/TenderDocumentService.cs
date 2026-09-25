using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class TenderDocumentService : ITenderDocumentService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public TenderDocumentService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllTenderDocument(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tender_documents.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.DocumentName != null && s.DocumentName.ToLower().Contains(search));
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

        public async Task<APIResponse> GetTenderDocumentById(int id)
        {
            var data = await _context.tender_documents.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect tenderdocument id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateTenderDocument(CreateTenderDocument dto)
        {
            var entity = new tender_documents
            {
                TenderId = dto.TenderId,
                DocumentType = dto.DocumentType,
                DocumentName = dto.DocumentName,
                DocumentUrl = dto.DocumentUrl,
                UploadedById = dto.UploadedById,
                UploadedAt = dto.UploadedAt,
                Notes = dto.Notes,
            };
            _context.tender_documents.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "TenderDocument created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateTenderDocument(EditTenderDocument req)
        {
            var data = await _context.tender_documents.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "TenderDocument not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.TenderId = req.TenderId;
            data.DocumentType = req.DocumentType;
            data.DocumentName = req.DocumentName;
            data.DocumentUrl = req.DocumentUrl;
            data.UploadedById = req.UploadedById;
            data.UploadedAt = req.UploadedAt;
            data.Notes = req.Notes;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "TenderDocument updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteTenderDocument(int id)
        {
            var data = await _context.tender_documents.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "TenderDocument not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.tender_documents.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "TenderDocument deleted successfully.";
            return _response;
        }
    }
}
