using Azure;
using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model.Dashboard;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrmManagementAPI.Services
{
    public class OverviewService : IOverviewService
    {
        private readonly db_context _context;
        protected APIResponse _response;
        public OverviewService(db_context sqlDbContext)
        {
            _context = sqlDbContext;
            _response = new APIResponse();

        }
        public async Task<APIResponse> GetDashboardData()
        {
            var userData = await _context.users.CountAsync(s => s.Active == true);
            if(userData == null)
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "User not active";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;

            }
            var customerData = await _context.customers.CountAsync(s => s.IsActive == true);
            if(customerData == null) 
            {
                _response.IsSuccess = false;
                _response.ActionResponse = "Customer not active";
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

            var customerAddedThisWeek = await _context.customers.CountAsync(x => x.CreatedAt >= startOfWeek);

            _response.IsSuccess = true;
            _response.ActionResponse = "Dashboard data found successfully.";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = new OverviewResponse
            {
                TotalCustomer = customerData,
                TotalUser = userData,
                CustomerAddedThisWeek = customerAddedThisWeek,
                
            };
            return _response;

        }

        public async Task<APIResponse> GetCustomerCategoryOverview()
        {
            var data = await _context.customers.Where(x => x.IsActive == true).GroupBy(x => x.Category)
                .Select(x => new 
                {
                    Category = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Count).ToListAsync();

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.ActionResponse = "Category data found successfully.";
            _response.Result = data;

            return _response;
        }
    }
}
