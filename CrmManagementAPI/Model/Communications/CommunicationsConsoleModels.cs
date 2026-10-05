namespace CrmManagementAPI.Model.Communications
{
    // =====================================================================
    // Header KPI cards
    // =====================================================================
    public class CommSummaryResponse
    {
        // "Outstanding · reminders due"
        public int ReminderInvoiceCount { get; set; }
        public decimal ReminderOutstandingInr { get; set; }
        public string ReminderOutstandingText { get; set; } = string.Empty;   // "₹31.9 L"

        // "Tender deadlines · next 15 days"
        public int TenderDeadlineCount { get; set; }
        public int CriticalTenderCount { get; set; }
        public string TenderDeadlineSubText { get; set; } = string.Empty;     // "0 critical (T-1 / today)"

        // "Messages sent · last 30 days"
        public int MessagesSent30d { get; set; }
        public int EmailSent30d { get; set; }
        public int WhatsAppSent30d { get; set; }
        public string MessagesSubText { get; set; } = string.Empty;           // "2 email · 3 WhatsApp"

        // "Escalations · last 30 days"
        public int Escalations30d { get; set; }
        public int Reminders30d { get; set; }
        public int Alerts30d { get; set; }
        public string EscalationsSubText { get; set; } = string.Empty;        // "0 reminders · 5 alerts"
    }

    // =====================================================================
    // Tab 1 - Payment Reminders
    // =====================================================================
    public class CommEscalationTier
    {
        public int AgingDays { get; set; }
        public string Label { get; set; } = string.Empty;          // "T+30"
        public string PolicyName { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;       // "Escalation-L1"
        public string SeverityLabel { get; set; } = string.Empty;  // "ESCALATION-L1"
        public string Channel { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;   // "PAY_ESC_T30"
        public string? TemplateName { get; set; }
        public string EscalateToRole { get; set; } = string.Empty; // "Sales Manager"
        public string ButtonLabel { get; set; } = string.Empty;    // "Esc L1"
    }

    public class CommReminderInvoiceItem
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string? PoNumber { get; set; }
        public DateOnly? DueDate { get; set; }
        public int DaysOverdue { get; set; }
        public decimal OutstandingInr { get; set; }
        public string OutstandingText { get; set; } = string.Empty;
        public string InvoiceStatus { get; set; } = string.Empty;

        // matching tier from escalation_policy
        public string? TierLabel { get; set; }                     // "T+45"
        public string? TierSeverity { get; set; }                  // "Escalation-L2"
        public string? TierTemplateCode { get; set; }              // "PAY_ESC_T45"
        public string? ButtonLabel { get; set; }                   // "Send Esc L2"

        // "To: Rekha Sharma <rekha.sharma@unionbankofindia.com>"
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientMobile { get; set; }
        public string? AccountOwner { get; set; }

        public int RemindersSent { get; set; }
        public DateTime? LastReminderAt { get; set; }
    }

    public class CommPaymentRemindersResponse
    {
        public int InvoiceCount { get; set; }
        public decimal TotalOutstandingInr { get; set; }
        public string TotalOutstandingText { get; set; } = string.Empty;
        public string HeadingText { get; set; } = string.Empty;    // "2 invoices · ₹31.9 L outstanding"
        public List<CommEscalationTier> Ladder { get; set; } = new();
        public List<CommReminderInvoiceItem> Invoices { get; set; } = new();
    }

    // =====================================================================
    // Preview (the "Send Email" pop-up) and Send
    // =====================================================================
    public class CommPreviewResponse
    {
        public string Title { get; set; } = string.Empty;          // "Send Email — PAY_ESC_T45"
        public string TemplateCode { get; set; } = string.Empty;
        public string? TemplateName { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientMobile { get; set; }
        public string? CcEmail { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Footer { get; set; } = string.Empty;         // "Live email via Gmail/Outlook · logged to outbox on send"
        public Dictionary<string, string> Variables { get; set; } = new();
    }

    public class CommSendRequest
    {
        public int? InvoiceId { get; set; }                        // for payment reminders
        public int? TenderId { get; set; }                         // for tender alerts
        public string? TemplateCode { get; set; }
        public string Channel { get; set; } = "Email";             // Email | WhatsApp
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientMobile { get; set; }
        public string? CcEmail { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;           // user may have edited it in the pop-up
        public int? SentById { get; set; }
    }

    public class CommSendResult
    {
        public int OutboxId { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;         // Sent | Sent (Simulated) | Failed
        public string? DeliveryRef { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // =====================================================================
    // Tab 2 - Tender Alerts
    // =====================================================================
    public class CommTenderAlertItem
    {
        public int TenderId { get; set; }
        public string TenderNumber { get; set; } = string.Empty;
        public string TenderTitle { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string Customer { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public int DaysLeft { get; set; }
        public string DaysLeftText { get; set; } = string.Empty;   // "5d left"
        public decimal ValueInr { get; set; }
        public string ValueText { get; set; } = string.Empty;      // "₹48.0 L"
        public string BidStatus { get; set; } = string.Empty;
        public string Window { get; set; } = string.Empty;         // "T-7"
        public string WindowLabel { get; set; } = string.Empty;    // "T-7 Informational"
        public bool IsCritical { get; set; }                       // T-1 / today
        public int? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerMobile { get; set; }
        public int AlertsSent { get; set; }
    }

    public class CommTenderAlertsResponse
    {
        public int Days { get; set; }
        public int Count { get; set; }
        public string HeadingText { get; set; } = string.Empty;    // "1 tenders with deadlines in next 15 days"
        public List<CommTenderAlertItem> Items { get; set; } = new();
    }

    // =====================================================================
    // Tab 3 - Templates
    // =====================================================================
    public class CommTemplateItem
    {
        public int Id { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string? SeverityLevel { get; set; }
        public int? AgingDaysMin { get; set; }
        public int? AgingDaysMax { get; set; }
        public int? LeadTimeDays { get; set; }
        public string? Subject { get; set; }
        public string BodyText { get; set; } = string.Empty;
        public string? Variables { get; set; }
        public bool Active { get; set; }
    }

    public class CommTemplateGroup
    {
        public string UseCase { get; set; } = string.Empty;        // group heading, e.g. "Escalation"
        public List<CommTemplateItem> Templates { get; set; } = new();
    }

    public class CommTemplatesResponse
    {
        public int TotalCount { get; set; }
        public string HeadingText { get; set; } = string.Empty;    // "10 templates · used automatically based on aging / deadline"
        public List<CommTemplateGroup> Groups { get; set; } = new();
    }

    // =====================================================================
    // Tab 4 - Outbox
    // =====================================================================
    public class CommOutboxItem
    {
        public int Id { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;          // subject (email) or first line of body (WhatsApp)
        public string BodyPreview { get; set; } = string.Empty;
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientMobile { get; set; }
        public string? CustomerName { get; set; }
        public string? TenderNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? SentByName { get; set; }
        public bool IsAuto { get; set; }
        public DateTime When { get; set; }
        public string? DeliveryRef { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class CommOutboxMessageDetail : CommOutboxItem
    {
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    // =====================================================================
    // Tab 5 - Automation
    // =====================================================================
    public class CommAutomationJob
    {
        public string Key { get; set; } = string.Empty;            // payment | tender
        public string Name { get; set; } = string.Empty;           // "Auto Payment Reminders"
        public bool Enabled { get; set; }
        public string Schedule { get; set; } = string.Empty;       // "Daily · 09:30 IST"
        public string Description { get; set; } = string.Empty;
        public DateTime NextRunIst { get; set; }
        public DateTime? LastAutoSentAt { get; set; }
    }

    public class CommAutoSentItem
    {
        public int Id { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? RecipientName { get; set; }
        public DateTime When { get; set; }
    }

    public class CommAutomationResponse
    {
        public string HeadingText { get; set; } = string.Empty;    // "Scheduled workflows"
        public List<CommAutomationJob> Jobs { get; set; } = new();
        public int AutoSentLast7Days { get; set; }
        public string AutoSentText { get; set; } = string.Empty;   // "0 auto-fired messages in the last 7 days"
        public List<CommAutoSentItem> AutoSent { get; set; } = new();
    }

    public class CommRunResult
    {
        public string Job { get; set; } = string.Empty;
        public int Checked { get; set; }
        public int Sent { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
        public List<string> Details { get; set; } = new();
    }
}