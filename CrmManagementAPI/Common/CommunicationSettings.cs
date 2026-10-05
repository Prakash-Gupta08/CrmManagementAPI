namespace CrmManagementAPI.Common
{
    /// <summary>
    /// Text values used in message_outbox / message_templates.
    /// If your database uses different spellings, change them HERE only.
    /// </summary>
    public static class CommConstants
    {
        // channel
        public const string Email = "Email";
        public const string WhatsApp = "WhatsApp";

        // use_case (message_outbox.use_case)
        public const string UcPaymentReminder = "Payment Reminder";
        public const string UcEscalation = "Escalation";
        public const string UcTenderAlert = "Tender Alert";

        // status (message_outbox.status)
        public const string StSent = "Sent";
        public const string StSimulated = "Sent (Simulated)";
        public const string StFailed = "Failed";
        public const string StQueued = "Queued";

        // notes marker so we can tell manual / automatic messages apart
        public const string NoteManual = "MANUAL";
        public const string NoteAutoPayment = "AUTO|PAYMENT|";   // + tier label, e.g. AUTO|PAYMENT|T+30
        public const string NoteAutoTender = "AUTO|TENDER|";     // + window label, e.g. AUTO|TENDER|T-7
        public const string NoteAutoAny = "AUTO|";

        // invoices that are shown in "Payment Reminders" (invoice_status values)
        public static readonly string[] ReminderInvoiceStatuses =
        {
            "Overdue", "Over Due", "Partially Paid", "Partially Paid "
        };

        // tenders in these statuses never get deadline alerts
        public static readonly string[] ClosedTenderStatuses = { "Won", "Lost", "Cancelled" };
    }

    // ---------- appsettings.json  ->  "EmailSettings" ----------
    public class EmailSettings
    {
        public bool Enabled { get; set; } = false;          // false = emails are only simulated
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;   // Gmail: App Password
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Communications Desk";
        public string CompanyName { get; set; } = "AAA Technologies Ltd";
    }

    // ---------- appsettings.json  ->  "Automation" ----------
    public class AutomationJobSettings
    {
        public bool Enabled { get; set; } = true;
        public string RunAtIst { get; set; } = "09:30";     // HH:mm in IST
        public int ResendEveryDays { get; set; } = 3;       // used by payment reminders
    }

    public class AutomationSettings
    {
        public AutomationJobSettings PaymentReminders { get; set; } = new() { RunAtIst = "09:30", ResendEveryDays = 3 };
        public AutomationJobSettings TenderAlerts { get; set; } = new() { RunAtIst = "08:00" };
    }
}