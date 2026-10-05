using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Model.Communications;

namespace CrmManagementAPI.Interfaces
{
    public interface ICommunicationsConsoleService
    {
        // header cards
        Task<APIResponse> GetSummary();

        // tab 1 - Payment Reminders
        Task<APIResponse> GetEscalationLadder();
        Task<APIResponse> GetPaymentReminders();
        Task<APIResponse> PreviewPaymentReminder(int invoiceId, string? templateCode, string? channel);
        Task<APIResponse> SendPaymentReminder(CommSendRequest req);

        // tab 2 - Tender Alerts
        Task<APIResponse> GetTenderAlerts(int days = 15);
        Task<APIResponse> PreviewTenderAlert(int tenderId, string? templateCode, string? channel);
        Task<APIResponse> SendTenderAlert(CommSendRequest req);

        // tab 3 - Templates
        Task<APIResponse> GetTemplates(string? search, string? useCase, string? channel, bool includeInactive = false);

        // tab 4 - Outbox
        Task<APIResponse> GetOutbox(string? search, string? channel, string? status, string? useCase,
            int pageNumber = 1, int pageSize = 20);
        Task<APIResponse> GetOutboxMessage(int id);

        // tab 5 - Automation
        Task<APIResponse> GetAutomation();
        Task<APIResponse> RunPaymentReminderAutomation();
        Task<APIResponse> RunTenderAlertAutomation();
    }
}