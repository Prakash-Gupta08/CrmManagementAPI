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

        public async Task<APIResponse> GetAllLeadConversionsList(string? search,string? filterField,string? filterValue,string? sortField,
        string? sortOrder = "asc",int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.lead_conversions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x => x.Id.ToString().Contains(search) ||x.LeadId.ToString().Contains(search) ||
                    x.TenderId.ToString().Contains(search) || x.CustomerId.ToString().Contains(search) ||
                    x.ConvertedById.ToString().Contains(search) || x.ConversionNotes!.ToLower().Contains(search)
                );
            }

            if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue))
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

                    case "leadid":
                        if (int.TryParse(filterValue, out int leadId))
                        {
                            query = query.Where(x => x.LeadId == leadId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid LeadId value.";

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
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid TenderId value.";

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

                    case "convertedbyid":
                        if (int.TryParse(filterValue, out int convertedById))
                        {
                            query = query.Where(x => x.ConvertedById == convertedById);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ConvertedById value.";
                            return _response;
                        }

                        break;

                    case "conversionnotes":
                        query = query.Where(x =>x.ConversionNotes != null && x.ConversionNotes.ToLower().Contains(filterValue));
                        break;

                    case "convertedat":
                        if (DateTime.TryParse(filterValue, out DateTime convertedAt))
                        {
                            query = query.Where(x =>x.ConvertedAt.Date == convertedAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ConvertedAt value.";

                            return _response;
                        }
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

                    case "leadid":
                        query = descending ? query.OrderByDescending(x => x.LeadId): query.OrderBy(x => x.LeadId);
                        break;

                    case "tenderid":
                        query = descending? query.OrderByDescending(x => x.TenderId): query.OrderBy(x => x.TenderId);
                        break;

                    case "customerid":
                        query = descending? query.OrderByDescending(x => x.CustomerId): query.OrderBy(x => x.CustomerId);
                        break;

                    case "convertedbyid":
                        query = descending? query.OrderByDescending(x => x.ConvertedById): query.OrderBy(x => x.ConvertedById);
                        break;

                    case "conversionnotes":
                        query = descending? query.OrderByDescending(x => x.ConversionNotes): query.OrderBy(x => x.ConversionNotes);
                        break;

                    case "convertedat":
                        query = descending? query.OrderByDescending(x => x.ConvertedAt): query.OrderBy(x => x.ConvertedAt);
                        break;

                    default:

                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ActionResponse = $"Invalid sort field: {sortField}";

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
