using CrmManagementAPI.CommonResponse;

namespace CrmManagementAPI.Interfaces
{
    public interface ITenderCommandCentreService
    {
        Task<APIResponse> GetAll();                                   // whole page in one call
        Task<APIResponse> GetSummary();                               // KPI cards
        Task<APIResponse> GetPipelineByStage();                       // bar chart
        Task<APIResponse> GetPipelineByCategory();                    // donut
        Task<APIResponse> GetUpcomingDeadlines(int top = 15);
        Task<APIResponse> GetGoNoGoQueue(int top = 10);
        Task<APIResponse> GetRecentActivities(int top = 10);
        Task<APIResponse> GetBillingSummary();                        // Execution - Billing - Collection cards
        Task<APIResponse> GetPoRegister(int top = 20);
        Task<APIResponse> GetReceivableAging();
        Task<APIResponse> GetOutstandingInvoices(int top = 15);
        Task<APIResponse> GetUpcomingMilestones(int top = 10);
        Task<APIResponse> GetTendersByPortal();
    }

}
