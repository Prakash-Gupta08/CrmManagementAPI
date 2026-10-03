using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class TenderActivityService : ITenderActivityService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public TenderActivityService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllTenderActivity(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tender_activities.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.ActivityType != null && s.ActivityType.ToLower().Contains(search));
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

        public async Task<APIResponse> GetTenderActivityById(int id)
        {
            var data = await _context.tender_activities.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect tenderactivity id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateTenderActivity(CreateTenderActivity dto)
        {
            var entity = new tender_activities
            {
                TenderId = dto.TenderId,
                OwnerId = dto.OwnerId,
                ActivityType = dto.ActivityType,
                ActivityDate = dto.ActivityDate,
                Outcome = dto.Outcome,
                NextAction = dto.NextAction,
                NextActionDate = dto.NextActionDate,
                Notes = dto.Notes,
                CreatedAt = dto.CreatedAt,
            };
            _context.tender_activities.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "TenderActivity created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateTenderActivity(EditTenderActivity req)
        {
            var data = await _context.tender_activities.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "TenderActivity not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.TenderId = req.TenderId;
            data.OwnerId = req.OwnerId;
            data.ActivityType = req.ActivityType;
            data.ActivityDate = req.ActivityDate;
            data.Outcome = req.Outcome;
            data.NextAction = req.NextAction;
            data.NextActionDate = req.NextActionDate;
            data.Notes = req.Notes;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "TenderActivity updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteTenderActivity(int id)
        {
            var data = await _context.tender_activities.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "TenderActivity not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.tender_activities.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "TenderActivity deleted successfully.";
            return _response;
        }

        public async Task<APIResponse> GetAllTenderActivitiesList(string? search, string? filterField, string? filterValue, string? sortField, string? sortOrder = "asc",
        int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.tender_activities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||
                    x.TenderId.ToString().Contains(search) ||
                    (x.OwnerId != null &&
                     x.OwnerId.ToString().Contains(search)) ||
                    x.ActivityType.ToLower().Contains(search) ||
                    (x.Outcome != null &&
                     x.Outcome.ToLower().Contains(search)) ||
                    (x.NextAction != null &&
                     x.NextAction.ToLower().Contains(search)) ||
                    (x.Notes != null &&
                     x.Notes.ToLower().Contains(search))
                );
            }

            // =========================
            // FILTER
            // =========================
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

                    case "tenderid":
                        if (int.TryParse(filterValue, out int tenderId))
                        {
                            query = query.Where(x =>
                                x.TenderId == tenderId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid TenderId value.";
                            return _response;
                        }
                        break;

                    case "ownerid":
                        if (int.TryParse(filterValue, out int ownerId))
                        {
                            query = query.Where(x =>
                                x.OwnerId == ownerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid OwnerId value.";
                            return _response;
                        }
                        break;

                    case "activitytype":
                        query = query.Where(x =>
                            x.ActivityType.ToLower().Contains(filterValue));
                        break;

                    case "activitydate":
                        if (DateTime.TryParse(
                            filterValue,
                            out DateTime activityDate))
                        {
                            query = query.Where(x =>
                                x.ActivityDate.Date == activityDate.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ActivityDate value.";
                            return _response;
                        }
                        break;

                    case "outcome":
                        query = query.Where(x =>
                            x.Outcome != null &&
                            x.Outcome.ToLower().Contains(filterValue));
                        break;

                    case "nextaction":
                        query = query.Where(x =>
                            x.NextAction != null &&
                            x.NextAction.ToLower().Contains(filterValue));
                        break;

                    case "nextactiondate":
                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly nextActionDate))
                        {
                            query = query.Where(x =>
                                x.NextActionDate == nextActionDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid NextActionDate value.";
                            return _response;
                        }
                        break;

                    case "notes":
                        query = query.Where(x =>
                            x.Notes != null &&
                            x.Notes.ToLower().Contains(filterValue));
                        break;

                    case "createdat":
                        if (DateTime.TryParse(
                            filterValue,
                            out DateTime createdAt))
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

            // =========================
            // SORT
            // =========================
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

                    case "tenderid":
                        query = descending
                            ? query.OrderByDescending(x => x.TenderId)
                            : query.OrderBy(x => x.TenderId);
                        break;

                    case "ownerid":
                        query = descending
                            ? query.OrderByDescending(x => x.OwnerId)
                            : query.OrderBy(x => x.OwnerId);
                        break;

                    case "activitytype":
                        query = descending
                            ? query.OrderByDescending(x => x.ActivityType)
                            : query.OrderBy(x => x.ActivityType);
                        break;

                    case "activitydate":
                        query = descending
                            ? query.OrderByDescending(x => x.ActivityDate)
                            : query.OrderBy(x => x.ActivityDate);
                        break;

                    case "outcome":
                        query = descending
                            ? query.OrderByDescending(x => x.Outcome)
                            : query.OrderBy(x => x.Outcome);
                        break;

                    case "nextaction":
                        query = descending
                            ? query.OrderByDescending(x => x.NextAction)
                            : query.OrderBy(x => x.NextAction);
                        break;

                    case "nextactiondate":
                        query = descending
                            ? query.OrderByDescending(x => x.NextActionDate)
                            : query.OrderBy(x => x.NextActionDate);
                        break;

                    case "notes":
                        query = descending
                            ? query.OrderByDescending(x => x.Notes)
                            : query.OrderBy(x => x.Notes);
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

            // =========================
            // PAGINATION
            // =========================
            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // =========================
            // NO DATA
            // =========================
            if (!data.Any())
            {
                _response.IsSuccess = false;
                _response.StatusCode =
                    System.Net.HttpStatusCode.NotFound;
                _response.ActionResponse =
                    "Data not found.";
                return _response;
            }

            // =========================
            // SUCCESS RESPONSE
            // =========================
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
