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
    public class CustomerService : ICustomerService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public CustomerService(db_context sqlDbcontext)
        {
            _context = sqlDbcontext;
            _response = new APIResponse();

        }

        public async Task<APIResponse> GetAllCustomer(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.customers.Where(x => x.IsActive == true);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.MinistryParent.ToLower().Contains(search));
            }
            var totalCount = await query.CountAsync();

            var data = await query.OrderBy(x => x.MinistryParent)
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

        public async Task<APIResponse> GetCustomerById(int id)
        {
            var data = await _context.customers.FindAsync(id);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Incorrect user id.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = $"User id with {id} found successfully.";
            _response.Result = data;
            return _response;

        }
        public async Task<APIResponse> CreateCustomer(CreateCustomer dto)
        {
            var data = await _context.customers.FirstOrDefaultAsync(s => s.OrganizationName == dto.OrganizationName && s.IsActive == true);
            if (data != null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "Record already exist.";
                return _response;
            }
            var emailExists = await _context.customers.AnyAsync(s => s.KeyContactEmail == dto.KeyContactEmail && s.IsActive == true);
            if (emailExists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "This email already registered.";
                return _response;
            }
            var mobileExists = await _context.customers.AnyAsync(s => s.KeyContactMobile == dto.KeyContactMobile && s.IsActive == true);
            if (mobileExists)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ActionResponse = "This Mobile already registered.";
                return _response;
            }
            var customer = new customers
            {
                OrganizationName = dto.OrganizationName,
                MinistryParent = dto.MinistryParent,
                Category = dto.Category,
                State = dto.State,
                DistrictCity = dto.DistrictCity,
                OfficeAddress = dto.OfficeAddress,
                Website = dto.Website,
                GemSellerId = dto.GemSellerId,
                Gstin = dto.Gstin,
                AccountOwner = dto.AccountOwner,
                KeyContactName = dto.KeyContactName,
                KeyContactDesignation = dto.KeyContactDesignation,
                KeyContactEmail = dto.KeyContactEmail,
                KeyContactMobile = dto.KeyContactMobile,
                Notes = dto.Notes,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                IsActive = dto.IsActive,
            };
            _context.customers.Add(customer);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Customers created successfully.";
            _response.Result = data;
            return _response;
        }

        public async Task<APIResponse> UpdateCustomer(EditCustomer req)
        {
            var data = await _context.customers.FirstOrDefaultAsync(s => s.Id == req.Id && s.IsActive == true);
            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Customer not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;

            }
            if (string.IsNullOrWhiteSpace(data.KeyContactEmail))
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Enter valid email id.";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;

            }

            if (string.IsNullOrWhiteSpace(data.KeyContactDesignation))
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Enter the role name.";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;

            }
            if (data.KeyContactMobile == req.KeyContactMobile)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Employee id already exist.";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;

            }
            data.Id = req.Id;
            data.OrganizationName = req.OrganizationName;
            data.MinistryParent = req.MinistryParent;
            data.Category = req.Category;
            data.State = req.State;
            data.DistrictCity = req.DistrictCity;
            data.OfficeAddress = req.OfficeAddress;
            data.Website = req.Website;
            data.GemSellerId = req.GemSellerId;
            data.Gstin = req.Gstin;
            data.AccountOwner = req.AccountOwner;
            data.KeyContactName = req.KeyContactName;
            data.KeyContactDesignation = req.KeyContactDesignation;
            data.KeyContactEmail = req.KeyContactEmail;
            data.KeyContactMobile = req.KeyContactMobile;
            data.Notes = req.Notes;
            data.CreatedAt = req.CreatedAt;
            data.UpdatedAt = req.UpdatedAt;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.ActionResponse = "User updated successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = data;
            return _response;

        }
        public async Task<APIResponse> DeleteCustomer(int id)
        {
            var data = await _context.customers
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);

            if (data == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Customer not found.";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }

            data.IsActive = false;

            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Customer deleted successfully.";
            return _response;
        

        }

        public async Task<APIResponse> GetCategoryDropdown()
        {
            var data = CategoryConstants.Categories
                .Select(x => new
                {
                    Value = x,
                    Label = x
                })
                .ToList();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Category dropdown data found successfully.";
            _response.Result = data;

            return _response;
        }
        public async Task<APIResponse> GetProjectRiskDropdown()
        {
            var data = CategoryConstants.Project_risk
                .Select(x => new
                {
                    Value = x,
                    Label = x
                })
                .ToList();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Project risk dropdown data found successfully.";
            _response.Result = data;

            return _response;
        }
        public async Task<APIResponse> GetDecisionDropdown()
        {
            var data = CategoryConstants.Decision
                .Select(x => new
                {
                    Value = x,
                    Label = x
                })
                .ToList();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Decision dropdown data found successfully.";
            _response.Result = data;

            return _response;
        }

        public async Task<APIResponse> GetAllCustomerList(string? search, string? filterField, string? filterValue,string? sortField,
        string? sortOrder = "asc",int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.customers.Where(x => x.IsActive == true);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x => x.OrganizationName.ToLower().Contains(search) ||

                    (x.MinistryParent != null && x.MinistryParent.ToLower().Contains(search)) ||

                    (x.Category != null && x.Category.ToLower().Contains(search)) ||

                    (x.State != null && x.State.ToLower().Contains(search)) ||

                    (x.DistrictCity != null && x.DistrictCity.ToLower().Contains(search)) ||

                    (x.OfficeAddress != null && x.OfficeAddress.ToLower().Contains(search)) ||

                    (x.Website != null && x.Website.ToLower().Contains(search)) ||

                    (x.GemSellerId != null && x.GemSellerId.ToLower().Contains(search)) ||

                    (x.Gstin != null && x.Gstin.ToLower().Contains(search)) ||

                    (x.AccountOwner != null && x.AccountOwner.ToLower().Contains(search)) ||

                    (x.KeyContactName != null && x.KeyContactName.ToLower().Contains(search)) ||

                    (x.KeyContactDesignation != null && x.KeyContactDesignation.ToLower().Contains(search)) ||

                    (x.KeyContactEmail != null && x.KeyContactEmail.ToLower().Contains(search)) ||

                    (x.KeyContactMobile != null && x.KeyContactMobile.ToLower().Contains(search)) ||

                    (x.Notes != null && x.Notes.ToLower().Contains(search))
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
                            _response.ActionResponse =
                                "Invalid ID value.";

                            return _response;
                        }
                        break;

                    case "organizationname":

                        query = query.Where(x =>
                            x.OrganizationName.ToLower()
                                .Contains(filterValue));

                        break;


                    // Ministry Parent
                    case "ministryparent":

                        query = query.Where(x =>
                            x.MinistryParent != null &&
                            x.MinistryParent.ToLower()
                                .Contains(filterValue));

                        break;


                    // Category
                    case "category":

                        query = query.Where(x =>
                            x.Category != null &&
                            x.Category.ToLower()
                                .Contains(filterValue));

                        break;


                    // State
                    case "state":

                        query = query.Where(x =>
                            x.State != null &&
                            x.State.ToLower()
                                .Contains(filterValue));

                        break;


                    // District / City
                    case "districtcity":

                        query = query.Where(x =>
                            x.DistrictCity != null &&
                            x.DistrictCity.ToLower()
                                .Contains(filterValue));

                        break;


                    // Office Address
                    case "officeaddress":

                        query = query.Where(x =>
                            x.OfficeAddress != null &&
                            x.OfficeAddress.ToLower()
                                .Contains(filterValue));

                        break;


                    // Website
                    case "website":

                        query = query.Where(x =>
                            x.Website != null &&
                            x.Website.ToLower()
                                .Contains(filterValue));

                        break;


                    // GeM Seller ID
                    case "gemsellerid":

                        query = query.Where(x =>
                            x.GemSellerId != null &&
                            x.GemSellerId.ToLower()
                                .Contains(filterValue));

                        break;


                    // GSTIN
                    case "gstin":

                        query = query.Where(x =>
                            x.Gstin != null &&
                            x.Gstin.ToLower()
                                .Contains(filterValue));

                        break;


                    // Account Owner
                    case "accountowner":

                        query = query.Where(x =>
                            x.AccountOwner != null &&
                            x.AccountOwner.ToLower()
                                .Contains(filterValue));

                        break;


                    // Key Contact Name
                    case "keycontactname":

                        query = query.Where(x =>
                            x.KeyContactName != null &&
                            x.KeyContactName.ToLower()
                                .Contains(filterValue));

                        break;


                    // Key Contact Designation
                    case "keycontactdesignation":

                        query = query.Where(x =>
                            x.KeyContactDesignation != null &&
                            x.KeyContactDesignation.ToLower()
                                .Contains(filterValue));

                        break;


                    // Key Contact Email
                    case "keycontactemail":

                        query = query.Where(x =>
                            x.KeyContactEmail != null &&
                            x.KeyContactEmail.ToLower()
                                .Contains(filterValue));

                        break;


                    // Key Contact Mobile
                    case "keycontactmobile":

                        query = query.Where(x =>
                            x.KeyContactMobile != null &&
                            x.KeyContactMobile.ToLower()
                                .Contains(filterValue));

                        break;


                    // Notes
                    case "notes":
                        query = query.Where(x => x.Notes != null && x.Notes.ToLower().Contains(filterValue));
                        break;

                    case "createdat":
                        if (DateTime.TryParse(filterValue, out DateTime createdAt))
                        {
                            query = query.Where(x => x.CreatedAt.Date == createdAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse ="Invalid CreatedAt date.";

                            return _response;
                        }

                        break;

                    case "updatedat":
                        if (DateTime.TryParse(filterValue, out DateTime updatedAt))
                        {
                            query = query.Where(x => x.UpdatedAt.Date == updatedAt.Date);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid UpdatedAt date.";

                            return _response;
                        }

                        break;

                    case "isactive":
                        if (bool.TryParse(filterValue, out bool isActive))
                        {
                            query = query.Where(x => x.IsActive == isActive);
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ActionResponse = "Invalid IsActive value. Use true or false.";

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


                    case "organizationname":
                        query = descending ? query.OrderByDescending(x => x.OrganizationName) : query.OrderBy(x => x.OrganizationName);
                        break;


                    case "ministryparent":
                        query = descending ? query.OrderByDescending(x => x.MinistryParent) : query.OrderBy(x => x.MinistryParent);
                        break;


                    case "category":
                        query = descending ? query.OrderByDescending(x => x.Category) : query.OrderBy(x => x.Category);
                        break;


                    case "state":
                        query = descending ? query.OrderByDescending(x => x.State): query.OrderBy(x => x.State);
                        break;


                    case "districtcity":
                        query = descending ? query.OrderByDescending(x => x.DistrictCity) : query.OrderBy(x => x.DistrictCity);
                        break;


                    case "officeaddress":
                        query = descending ? query.OrderByDescending(x => x.OfficeAddress) : query.OrderBy(x => x.OfficeAddress);
                        break;


                    case "website":
                        query = descending ? query.OrderByDescending(x => x.Website): query.OrderBy(x => x.Website);
                        break;


                    case "gemsellerid":
                        query = descending? query.OrderByDescending(x => x.GemSellerId): query.OrderBy(x => x.GemSellerId);
                        break;


                    case "gstin":
                        query = descending ? query.OrderByDescending(x => x.Gstin) : query.OrderBy(x => x.Gstin);
                        break;


                    case "accountowner":
                        query = descending ? query.OrderByDescending(x => x.AccountOwner) : query.OrderBy(x => x.AccountOwner);
                        break;


                    case "keycontactname":
                        query = descending ? query.OrderByDescending(x => x.KeyContactName) : query.OrderBy(x => x.KeyContactName);
                        break;


                    case "keycontactdesignation":
                        query = descending? query.OrderByDescending(x => x.KeyContactDesignation): query.OrderBy(x => x.KeyContactDesignation);
                        break;


                    case "keycontactemail":
                        query = descending ? query.OrderByDescending(x => x.KeyContactEmail) : query.OrderBy(x => x.KeyContactEmail);
                        break;


                    case "keycontactmobile":
                        query = descending ? query.OrderByDescending(x => x.KeyContactMobile) : query.OrderBy(x => x.KeyContactMobile);
                        break;


                    case "notes":
                        query = descending ? query.OrderByDescending(x => x.Notes) : query.OrderBy(x => x.Notes);
                        break;


                    case "createdat":
                        query = descending? query.OrderByDescending(x => x.CreatedAt): query.OrderBy(x => x.CreatedAt);
                        break;


                    case "updatedat":
                        query = descending ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt);
                        break;


                    case "isactive":
                        query = descending ? query.OrderByDescending(x => x.IsActive): query.OrderBy(x => x.IsActive);
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
                query = query.OrderBy(x => x.OrganizationName);
            }

            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var data = await query.OrderBy(x => x.Id)
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
