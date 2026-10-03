using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class ProjectMilestoneService : IProjectMilestoneService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public ProjectMilestoneService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();
        }

        public async Task<APIResponse> GetAllProjectMilestone(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.project_milestones.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.MilestoneName != null && s.MilestoneName.ToLower().Contains(search));
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

        public async Task<APIResponse> GetProjectMilestoneById(int id)
        {
            var data = await _context.project_milestones.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect projectmilestone id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"Record with id {id} found successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> CreateProjectMilestone(CreateProjectMilestone dto)
        {
            var entity = new project_milestones
            {
                PoId = dto.PoId,
                MilestoneName = dto.MilestoneName,
                MilestoneType = dto.MilestoneType,
                PlannedDate = dto.PlannedDate,
                ActualDate = dto.ActualDate,
                BillingPct = dto.BillingPct,
                BillingAmountInr = dto.BillingAmountInr,
                Status = dto.Status,
                CompletionNotes = dto.CompletionNotes,
                Billable = dto.Billable,
                Invoiced = dto.Invoiced,
                SortOrder = dto.SortOrder,
                CreatedAt = dto.CreatedAt,
            };
            _context.project_milestones.Add(entity);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "ProjectMilestone created successfully.";
            _response.Result = entity;
            return _response;
        }

        public async Task<APIResponse> UpdateProjectMilestone(EditProjectMilestone req)
        {
            var data = await _context.project_milestones.FirstOrDefaultAsync(s => s.Id == req.Id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "ProjectMilestone not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.PoId = req.PoId;
            data.MilestoneName = req.MilestoneName;
            data.MilestoneType = req.MilestoneType;
            data.PlannedDate = req.PlannedDate;
            data.ActualDate = req.ActualDate;
            data.BillingPct = req.BillingPct;
            data.BillingAmountInr = req.BillingAmountInr;
            data.Status = req.Status;
            data.CompletionNotes = req.CompletionNotes;
            data.Billable = req.Billable;
            data.Invoiced = req.Invoiced;
            data.SortOrder = req.SortOrder;
            data.CreatedAt = req.CreatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "ProjectMilestone updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> DeleteProjectMilestone(int id)
        {
            var data = await _context.project_milestones.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "ProjectMilestone not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            _context.project_milestones.Remove(data);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "ProjectMilestone deleted successfully.";
            return _response;
        }

        public async Task<APIResponse> GetAllProjectMilestonesList(string? search, string? filterField, string? filterValue, string? sortField,
        string? sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.project_milestones.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Id.ToString().Contains(search) ||x.PoId.ToString().Contains(search) ||
                    x.MilestoneName.ToLower().Contains(search) ||(x.MilestoneType != null && x.MilestoneType.ToLower().Contains(search)) ||
                    x.BillingPct.ToString().Contains(search) ||x.BillingAmountInr.ToString().Contains(search) ||
                    x.Status.ToLower().Contains(search) ||(x.CompletionNotes != null && x.CompletionNotes.ToLower().Contains(search)) ||
                    x.SortOrder.ToString().Contains(search)
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
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid ID value.";
                            return _response;
                        }
                        break;

                    case "poid":
                        if (int.TryParse(filterValue, out int poId))
                        {
                            query = query.Where(x => x.PoId == poId);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid PoId value.";
                            return _response;
                        }
                        break;

                    case "milestonename":
                        query = query.Where(x =>
                            x.MilestoneName.ToLower().Contains(filterValue));
                        break;

                    case "milestonetype":
                        query = query.Where(x =>
                            x.MilestoneType != null &&
                            x.MilestoneType.ToLower().Contains(filterValue));
                        break;

                    case "planneddate":
                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly plannedDate))
                        {
                            query = query.Where(x =>
                                x.PlannedDate == plannedDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid PlannedDate value.";
                            return _response;
                        }
                        break;

                    case "actualdate":
                        if (DateOnly.TryParse(
                            filterValue,
                            out DateOnly actualDate))
                        {
                            query = query.Where(x =>
                                x.ActualDate == actualDate);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid ActualDate value.";
                            return _response;
                        }
                        break;

                    case "billingpct":
                        if (decimal.TryParse(
                            filterValue,
                            out decimal billingPct))
                        {
                            query = query.Where(x =>
                                x.BillingPct == billingPct);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid BillingPct value.";
                            return _response;
                        }
                        break;

                    case "billingamountinr":
                        if (decimal.TryParse(
                            filterValue,
                            out decimal billingAmountInr))
                        {
                            query = query.Where(x =>
                                x.BillingAmountInr == billingAmountInr);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid BillingAmountInr value.";
                            return _response;
                        }
                        break;

                    case "status":
                        query = query.Where(x =>
                            x.Status.ToLower().Contains(filterValue));
                        break;

                    case "completionnotes":
                        query = query.Where(x =>
                            x.CompletionNotes != null &&
                            x.CompletionNotes.ToLower().Contains(filterValue));
                        break;

                    case "billable":
                        if (bool.TryParse(filterValue, out bool billable))
                        {
                            query = query.Where(x =>
                                x.Billable == billable);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid Billable value. Use true or false.";
                            return _response;
                        }
                        break;

                    case "invoiced":
                        if (bool.TryParse(filterValue, out bool invoiced))
                        {
                            query = query.Where(x =>
                                x.Invoiced == invoiced);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid Invoiced value. Use true or false.";
                            return _response;
                        }
                        break;

                    case "sortorder":
                        if (int.TryParse(filterValue, out int sortOrderValue))
                        {
                            query = query.Where(x =>
                                x.SortOrder == sortOrderValue);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode =
                                System.Net.HttpStatusCode.BadRequest;
                            _response.ActionResponse =
                                "Invalid SortOrder value.";
                            return _response;
                        }
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

                    case "poid":
                        query = descending
                            ? query.OrderByDescending(x => x.PoId)
                            : query.OrderBy(x => x.PoId);
                        break;

                    case "milestonename":
                        query = descending
                            ? query.OrderByDescending(x => x.MilestoneName)
                            : query.OrderBy(x => x.MilestoneName);
                        break;

                    case "milestonetype":
                        query = descending
                            ? query.OrderByDescending(x => x.MilestoneType)
                            : query.OrderBy(x => x.MilestoneType);
                        break;

                    case "planneddate":
                        query = descending
                            ? query.OrderByDescending(x => x.PlannedDate)
                            : query.OrderBy(x => x.PlannedDate);
                        break;

                    case "actualdate":
                        query = descending
                            ? query.OrderByDescending(x => x.ActualDate)
                            : query.OrderBy(x => x.ActualDate);
                        break;

                    case "billingpct":
                        query = descending
                            ? query.OrderByDescending(x => x.BillingPct)
                            : query.OrderBy(x => x.BillingPct);
                        break;

                    case "billingamountinr":
                        query = descending
                            ? query.OrderByDescending(x => x.BillingAmountInr)
                            : query.OrderBy(x => x.BillingAmountInr);
                        break;

                    case "status":
                        query = descending
                            ? query.OrderByDescending(x => x.Status)
                            : query.OrderBy(x => x.Status);
                        break;

                    case "completionnotes":
                        query = descending
                            ? query.OrderByDescending(x => x.CompletionNotes)
                            : query.OrderBy(x => x.CompletionNotes);
                        break;

                    case "billable":
                        query = descending
                            ? query.OrderByDescending(x => x.Billable)
                            : query.OrderBy(x => x.Billable);
                        break;

                    case "invoiced":
                        query = descending
                            ? query.OrderByDescending(x => x.Invoiced)
                            : query.OrderBy(x => x.Invoiced);
                        break;

                    case "sortorder":
                        query = descending
                            ? query.OrderByDescending(x => x.SortOrder)
                            : query.OrderBy(x => x.SortOrder);
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
