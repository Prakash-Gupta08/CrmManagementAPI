using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.Common;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class LeadActivityService : ILeadActivityService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public LeadActivityService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllLeadActivity(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.lead_activities.AsQueryable();

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

        public async Task<APIResponse> GetLeadActivityById(int id)
        {
            var data = await _context.lead_activities.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect leadactivity id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateLeadActivity(CreateLeadActivity dto)
        {
            var entity = new lead_activities
            {
                LeadId = dto.LeadId,
                OwnerId = dto.OwnerId,
                ActivityType = dto.ActivityType,
                ActivityDate = dto.ActivityDate,
                FromStage = dto.FromStage,
                ToStage = dto.ToStage,
                Notes = dto.Notes,
                NextAction = dto.NextAction,
                NextActionDate = dto.NextActionDate,
                CreatedAt = dto.CreatedAt,
            };
            _context.lead_activities.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "LeadActivity created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateLeadActivity(EditLeadActivity req)
        {
            var data = await _context.lead_activities.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "LeadActivity not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.LeadId = req.LeadId;
            data.OwnerId = req.OwnerId;
            data.ActivityType = req.ActivityType;
            data.ActivityDate = req.ActivityDate;
            data.FromStage = req.FromStage;
            data.ToStage = req.ToStage;
            data.Notes = req.Notes;
            data.NextAction = req.NextAction;
            data.NextActionDate = req.NextActionDate;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "LeadActivity updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteLeadActivity(int id)
        {
            var data = await _context.lead_activities.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "LeadActivity not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.lead_activities.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "LeadActivity deleted successfully.";
            return _response;
        }
        
        public async Task<APIResponse> GetLeadActivityDropdown()
        {
            var data = CategoryConstants.Activity_type
                .Select(x => new
                { 
                    Value = x,
                    Label = x
                })
                .ToList();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Activity type dropdown data found successfully.";
            _response.Result = data;

            return _response;
        }

        public async Task<APIResponse> GetAllLeadActivitiesList(string? search,string? filterField,string? filterValue,string? sortField,
        string? sortOrder = "asc",int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.lead_activities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) || x.LeadId.ToString().Contains(search) ||
                    x.OwnerId.ToString()!.Contains(search) || x.ActivityType.ToLower().Contains(search) ||
                    x.FromStage!.ToLower().Contains(search) || x.ToStage!.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search) ||x.NextAction!.ToLower().Contains(search)
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

                    case "ownerid":
                        if (int.TryParse(filterValue, out int ownerId))
                        {
                            query = query.Where(x => x.OwnerId == ownerId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid OwnerId value.";

                            return _response;
                        }
                        break;

                    case "activitytype":
                        query = query.Where(x =>x.ActivityType.ToLower().Contains(filterValue));
                        break;

                    case "activitydate":
                        if (DateTime.TryParse(filterValue, out DateTime activityDate))
                        {
                            query = query.Where(x =>x.ActivityDate.Date == activityDate.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid ActivityDate value.";

                            return _response;
                        }
                        break;

                    case "fromstage":
                        query = query.Where(x =>x.FromStage != null && x.FromStage.ToLower().Contains(filterValue));
                        break;

                    case "tostage":
                        query = query.Where(x =>x.ToStage != null && x.ToStage.ToLower().Contains(filterValue));

                        break;

                    case "notes":
                        query = query.Where(x =>x.Notes != null && x.Notes.ToLower().Contains(filterValue));
                        break;

                    case "nextaction":
                        query = query.Where(x =>x.NextAction != null && x.NextAction.ToLower().Contains(filterValue));
                        break;

                    case "nextactiondate":
                        if (DateOnly.TryParse(filterValue,out DateOnly nextActionDate))
                        {
                            query = query.Where(x => x.NextActionDate == nextActionDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid NextActionDate value. Use yyyy-MM-dd.";

                            return _response;
                        }
                        break;

                    case "createdat":
                        if (DateTime.TryParse(filterValue,out DateTime createdAt))
                        {
                            query = query.Where(x =>x.CreatedAt.Date == createdAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid CreatedAt value.";

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
                        query = descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                        break;

                    case "leadid":
                        query = descending? query.OrderByDescending(x => x.LeadId): query.OrderBy(x => x.LeadId);
                        break;

                    case "ownerid":
                        query = descending? query.OrderByDescending(x => x.OwnerId): query.OrderBy(x => x.OwnerId);
                        break;

                    case "activitytype":
                        query = descending? query.OrderByDescending(x => x.ActivityType): query.OrderBy(x => x.ActivityType);
                        break;

                    case "activitydate":
                        query = descending? query.OrderByDescending(x => x.ActivityDate): query.OrderBy(x => x.ActivityDate);
                        break;

                    case "fromstage":
                        query = descending? query.OrderByDescending(x => x.FromStage): query.OrderBy(x => x.FromStage);
                        break;

                    case "tostage":
                        query = descending? query.OrderByDescending(x => x.ToStage): query.OrderBy(x => x.ToStage);
                        break;

                    case "notes":
                        query = descending? query.OrderByDescending(x => x.Notes): query.OrderBy(x => x.Notes);
                        break;

                    case "nextaction":
                        query = descending? query.OrderByDescending(x => x.NextAction): query.OrderBy(x => x.NextAction);
                        break;

                    case "nextactiondate":
                        query = descending? query.OrderByDescending(x => x.NextActionDate): query.OrderBy(x => x.NextActionDate);
                        break;

                    case "createdat":
                        query = descending? query.OrderByDescending(x => x.CreatedAt): query.OrderBy(x => x.CreatedAt);
                        break;

                    default:

                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ActionResponse =$"Invalid sort field: {sortField}";

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
