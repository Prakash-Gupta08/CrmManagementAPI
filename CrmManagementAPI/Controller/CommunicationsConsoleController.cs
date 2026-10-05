using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model.Communications;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationsConsoleController : ControllerBase
    {
        private readonly ICommunicationsConsoleService _service;

        public CommunicationsConsoleController(ICommunicationsConsoleService service)
        {
            _service = service;
        }

        // ------------------------------------------------------------------
        // Header cards (Outstanding / Tender deadlines / Messages sent / Escalations)
        // ------------------------------------------------------------------
        [HttpGet("GetSummary")]
        public async Task<ActionResult> GetSummary() => Ok(await _service.GetSummary());

        // ------------------------------------------------------------------
        // Tab 1 - Payment Reminders
        // ------------------------------------------------------------------

        // Escalation ladder only (T+0, T+7, T+15, T+30, T+45)
        [HttpGet("GetEscalationLadder")]
        public async Task<ActionResult> GetEscalationLadder() => Ok(await _service.GetEscalationLadder());

        // Ladder + outstanding invoices (whole tab in one call)
        [HttpGet("GetPaymentReminders")]
        public async Task<ActionResult> GetPaymentReminders() => Ok(await _service.GetPaymentReminders());

        // Opens the "Send Email - PAY_ESC_T45" pop-up (recipient, subject, body already filled)
        [HttpGet("PreviewPaymentReminder")]
        public async Task<ActionResult> PreviewPaymentReminder(int invoiceId, string? templateCode = null, string? channel = null)
            => Ok(await _service.PreviewPaymentReminder(invoiceId, templateCode, channel));

        // "Send" button of the pop-up
        [HttpPost("SendPaymentReminder")]
        public async Task<ActionResult> SendPaymentReminder([FromBody] CommSendRequest req)
            => Ok(await _service.SendPaymentReminder(req));

        // ------------------------------------------------------------------
        // Tab 2 - Tender Alerts
        // ------------------------------------------------------------------
        [HttpGet("GetTenderAlerts")]
        public async Task<ActionResult> GetTenderAlerts(int days = 15)
            => Ok(await _service.GetTenderAlerts(days));

        [HttpGet("PreviewTenderAlert")]
        public async Task<ActionResult> PreviewTenderAlert(int tenderId, string? templateCode = null, string? channel = null)
            => Ok(await _service.PreviewTenderAlert(tenderId, templateCode, channel));

        [HttpPost("SendTenderAlert")]
        public async Task<ActionResult> SendTenderAlert([FromBody] CommSendRequest req)
            => Ok(await _service.SendTenderAlert(req));

        // ------------------------------------------------------------------
        // Tab 3 - Templates
        // ------------------------------------------------------------------
        [HttpGet("GetTemplates")]
        public async Task<ActionResult> GetTemplates(string? search = null, string? useCase = null,
            string? channel = null, bool includeInactive = false)
            => Ok(await _service.GetTemplates(search, useCase, channel, includeInactive));

        // ------------------------------------------------------------------
        // Tab 4 - Outbox
        // ------------------------------------------------------------------
        [HttpGet("GetOutbox")]
        public async Task<ActionResult> GetOutbox(string? search = null, string? channel = null,
            string? status = null, string? useCase = null, int pageNumber = 1, int pageSize = 20)
            => Ok(await _service.GetOutbox(search, channel, status, useCase, pageNumber, pageSize));

        [HttpGet("GetOutboxMessage")]
        public async Task<ActionResult> GetOutboxMessage(int id) => Ok(await _service.GetOutboxMessage(id));

        // ------------------------------------------------------------------
        // Tab 5 - Automation
        // ------------------------------------------------------------------
        [HttpGet("GetAutomation")]
        public async Task<ActionResult> GetAutomation() => Ok(await _service.GetAutomation());

        // manual "run now" buttons (useful for testing; the scheduler calls the same methods daily)
        [HttpPost("RunPaymentReminderAutomation")]
        public async Task<ActionResult> RunPaymentReminderAutomation()
            => Ok(await _service.RunPaymentReminderAutomation());

        [HttpPost("RunTenderAlertAutomation")]
        public async Task<ActionResult> RunTenderAlertAutomation()
            => Ok(await _service.RunTenderAlertAutomation());
    }
}