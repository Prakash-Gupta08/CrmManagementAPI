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

        public async Task<APIResponse> GetAllTenderDocumentsList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tender_documents.AsQueryable();

            // =========================
            // Search
            // =========================
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||
                    x.TenderId.ToString().Contains(search) ||
                    x.DocumentType.ToLower().Contains(search) ||
                    x.DocumentName.ToLower().Contains(search) ||
                    x.DocumentUrl.ToLower().Contains(search) ||
                    (x.UploadedById != null && x.UploadedById.ToString().Contains(search)) ||
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

                    case "documenttype":
                        query = query.Where(x =>
                            x.DocumentType.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "documentname":
                        query = query.Where(x =>
                            x.DocumentName.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "documenturl":
                        query = query.Where(x =>
                            x.DocumentUrl.ToLower().Contains(filterValue.ToLower()));
                        break;

                    case "uploadedbyid":
                        if (int.TryParse(filterValue, out int uploadedById))
                        {
                            query = query.Where(x =>
                                x.UploadedById == uploadedById);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid UploadedById.";
                            return _response;
                        }
                        break;

                    case "uploadedat":
                        if (DateTime.TryParse(filterValue, out DateTime uploadedAt))
                        {
                            query = query.Where(x =>
                                x.UploadedAt.Date == uploadedAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid UploadedAt date.";
                            return _response;
                        }
                        break;

                    case "notes":
                        query = query.Where(x =>
                            x.Notes != null &&
                            x.Notes.ToLower().Contains(filterValue.ToLower()));
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

                    case "documenttype":
                        query = isDescending
                            ? query.OrderByDescending(x => x.DocumentType)
                            : query.OrderBy(x => x.DocumentType);
                        break;

                    case "documentname":
                        query = isDescending
                            ? query.OrderByDescending(x => x.DocumentName)
                            : query.OrderBy(x => x.DocumentName);
                        break;

                    case "documenturl":
                        query = isDescending
                            ? query.OrderByDescending(x => x.DocumentUrl)
                            : query.OrderBy(x => x.DocumentUrl);
                        break;

                    case "uploadedbyid":
                        query = isDescending
                            ? query.OrderByDescending(x => x.UploadedById)
                            : query.OrderBy(x => x.UploadedById);
                        break;

                    case "uploadedat":
                        query = isDescending
                            ? query.OrderByDescending(x => x.UploadedAt)
                            : query.OrderBy(x => x.UploadedAt);
                        break;

                    case "notes":
                        query = isDescending
                            ? query.OrderByDescending(x => x.Notes)
                            : query.OrderBy(x => x.Notes);
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
