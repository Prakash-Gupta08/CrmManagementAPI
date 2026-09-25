using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model.Dashboard;

namespace CrmManagementAPI.Interfaces
{
    public interface IOverviewService
    {
        Task<APIResponse> GetDashboardData();
        Task<APIResponse> GetCustomerCategoryOverview();
    }
}
