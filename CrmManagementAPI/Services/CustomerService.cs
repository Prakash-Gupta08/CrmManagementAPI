using CrmManagementAPI.AppDbContext;
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
    }
}
