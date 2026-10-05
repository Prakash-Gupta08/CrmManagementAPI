using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.Common;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Data;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model.Communications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CrmManagementAPI.Services
{
    /// <summary>
    /// Communications Console (Payment Reminders, Tender Alerts, Templates, Outbox, Automation).
    /// Tables are related with LINQ "join ... on ... equals ..." only - NO OnModelCreating needed.
    /// </summary>
    public class CommunicationsConsoleService : ICommunicationsConsoleService
    {
        private readonly db_context _context;
        private readonly IEmailSender _email;
        private readonly EmailSettings _mail;
        private readonly AutomationSettings _auto;

        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        public CommunicationsConsoleService(
            db_context context,
            IEmailSender email,
            IOptions<EmailSettings> mailOptions,
            IOptions<AutomationSettings> autoOptions)
        {
            _context = context;
            _email = email;
            _mail = mailOptions.Value;
            _auto = autoOptions.Value;
        }

        // =====================================================================
        // Small helpers
        // =====================================================================
        private static APIResponse Ok(object result, string message = "Data found successfully.") => new()
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ActionResponse = message,
            Result = result
        };

        private static APIResponse Fail(HttpStatusCode code, string message) => new()
        {
            IsSuccess = false,
            StatusCode = code,
            ActionResponse = message
        };

        // India time (IST = UTC + 5:30) - works on Windows and Linux without time-zone ids
        public static DateTime IstNow() => DateTime.UtcNow.AddHours(5).AddMinutes(30);
        private static DateOnly IstToday() => DateOnly.FromDateTime(IstNow());

        // 1 decimal, same as the screenshots: ₹31.9 L, ₹15.0 L, ₹48.0 L
        private static string Inr1(decimal value)
        {
            var abs = Math.Abs(value);
            if (abs >= 10000000m) return "₹" + (value / 10000000m).ToString("0.0", Inv) + " Cr";
            if (abs >= 100000m) return "₹" + (value / 100000m).ToString("0.0", Inv) + " L";
            return "₹" + value.ToString("0", Inv);
        }

        private static string Render(string? text, Dictionary<string, string> vars)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return Regex.Replace(text, @"\{\{\s*([A-Za-z0-9_]+)\s*\}\}",
                m => vars.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value);
        }

        private static string Truncate(string? text, int max)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= max ? text : text.Substring(0, max - 3) + "...";
        }

        private static string FirstLine(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var line = text.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                           .FirstOrDefault() ?? string.Empty;
            return Truncate(line, 140);
        }

        private static bool IsSentStatus(string? status)
            => !string.IsNullOrEmpty(status) && status.StartsWith(CommConstants.StSent, StringComparison.OrdinalIgnoreCase);

        private static bool IsEscalationSeverity(string? severity)
            => !string.IsNullOrWhiteSpace(severity) && severity.StartsWith("Escalation", StringComparison.OrdinalIgnoreCase);

        // "Escalation-L2" -> "Esc L2",  "Gentle" -> "Gentle"
        private static string ShortSeverity(string severity)
        {
            var m = Regex.Match(severity ?? "", @"^Escalation[\s\-_]*L?(\d+)$", RegexOptions.IgnoreCase);
            return m.Success ? "Esc L" + m.Groups[1].Value : severity ?? string.Empty;
        }

        private string? ValidateRecipient(string channel, string? email, string? mobile)
        {
            if (string.Equals(channel, CommConstants.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(email)) return "Recipient email is required.";
                if (!MailAddress.TryCreate(email, out _)) return "Recipient email is not a valid email address.";
            }
            else if (string.Equals(channel, CommConstants.WhatsApp, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(mobile)) return "Recipient mobile number is required for WhatsApp.";
            }
            else
            {
                return "Channel must be Email or WhatsApp.";
            }
            return null;
        }

        private static string NormalizeChannel(string? channel, string fallback = CommConstants.Email)
        {
            if (string.IsNullOrWhiteSpace(channel)) return fallback;
            return channel.Trim().Equals(CommConstants.WhatsApp, StringComparison.OrdinalIgnoreCase)
                ? CommConstants.WhatsApp
                : CommConstants.Email;
        }

        // =====================================================================
        // Internal row classes (used by LINQ projections)
        // =====================================================================
        private sealed class ReminderRow
        {
            public int InvoiceId { get; set; }
            public string InvoiceNumber { get; set; } = string.Empty;
            public int CustomerId { get; set; }
            public string Customer { get; set; } = string.Empty;
            public string? PoNumber { get; set; }
            public DateOnly? DueDate { get; set; }
            public decimal Outstanding { get; set; }
            public string InvoiceStatus { get; set; } = string.Empty;
            public string? ContactName { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactMobile { get; set; }
            public string? AccountOwner { get; set; }
        }

        private sealed class TenderAlertRow
        {
            public int TenderId { get; set; }
            public string TenderNumber { get; set; } = string.Empty;
            public string TenderTitle { get; set; } = string.Empty;
            public int CustomerId { get; set; }
            public string Customer { get; set; } = string.Empty;
            public DateTime? Deadline { get; set; }
            public decimal Value { get; set; }
            public string BidStatus { get; set; } = string.Empty;
            public int? OwnerId { get; set; }
            public string? OwnerName { get; set; }
            public string? OwnerEmail { get; set; }
            public string? OwnerMobile { get; set; }
        }

        private sealed class InvoiceCtx
        {
            public invoices Invoice { get; set; } = null!;
            public customers Customer { get; set; } = null!;
            public purchase_orders? Po { get; set; }
        }

        private sealed class TenderCtx
        {
            public tenders Tender { get; set; } = null!;
            public customers Customer { get; set; } = null!;
            public users? Owner { get; set; }
        }

        // =====================================================================
        // Shared loaders  (all relations are LINQ joins)
        // =====================================================================

        // escalation_policy  LEFT JOIN  message_templates (on template_code)
        private async Task<List<CommEscalationTier>> LoadTiers()
        {
            var rows = await (
                from p in _context.escalation_policy
                join tplJoin in _context.message_templates on p.TemplateCode equals tplJoin.TemplateCode into tplGroup
                from tpl in tplGroup.DefaultIfEmpty()
                where p.Active
                orderby p.AgingDays
                select new
                {
                    p.AgingDays,
                    p.PolicyName,
                    p.SeverityLevel,
                    p.Channel,
                    p.TemplateCode,
                    TemplateName = tpl != null ? tpl.TemplateName : null,
                    p.EscalateToRole
                }).ToListAsync();

            return rows.Select(r => new CommEscalationTier
            {
                AgingDays = r.AgingDays,
                Label = "T+" + r.AgingDays,
                PolicyName = r.PolicyName,
                Severity = r.SeverityLevel,
                SeverityLabel = r.SeverityLevel.ToUpperInvariant(),
                Channel = r.Channel,
                TemplateCode = r.TemplateCode,
                TemplateName = r.TemplateName,
                EscalateToRole = r.EscalateToRole,
                ButtonLabel = ShortSeverity(r.SeverityLevel)
            }).ToList();
        }

        // highest tier whose aging_days <= days overdue
        private static CommEscalationTier? TierFor(int daysOverdue, List<CommEscalationTier> tiers)
            => tiers.Where(t => t.AgingDays <= daysOverdue).OrderByDescending(t => t.AgingDays).FirstOrDefault();

        // invoices  JOIN  customers  LEFT JOIN  purchase_orders
        private async Task<List<ReminderRow>> LoadReminderRows()
        {
            return await (
                from i in _context.invoices
                join c in _context.customers on i.CustomerId equals c.Id
                join poJoin in _context.purchase_orders on i.PoId equals poJoin.Id into poGroup
                from po in poGroup.DefaultIfEmpty()
                where i.OutstandingAmountInr > 0
                      && CommConstants.ReminderInvoiceStatuses.Contains(i.InvoiceStatus)
                orderby i.PaymentDueDate
                select new ReminderRow
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    CustomerId = c.Id,
                    Customer = c.OrganizationName,
                    PoNumber = po != null ? po.PoNumber : null,
                    DueDate = i.PaymentDueDate,
                    Outstanding = i.OutstandingAmountInr,
                    InvoiceStatus = i.InvoiceStatus,
                    ContactName = c.KeyContactName,
                    ContactEmail = c.KeyContactEmail,
                    ContactMobile = c.KeyContactMobile,
                    AccountOwner = c.AccountOwner
                }).ToListAsync();
        }

        // tenders  JOIN  customers  LEFT JOIN  users (bid owner)
        private async Task<List<TenderAlertRow>> LoadTenderAlertRows(int days)
        {
            var now = IstNow();
            var to = now.Date.AddDays(days + 1);

            return await (
                from t in _context.tenders
                join c in _context.customers on t.CustomerId equals c.Id
                join uJoin in _context.users on t.BidOwnerId equals (int?)uJoin.Id into uGroup
                from owner in uGroup.DefaultIfEmpty()
                where !CommConstants.ClosedTenderStatuses.Contains(t.BidStatus)
                      && t.BidSubmissionDeadline != null
                      && t.BidSubmissionDeadline >= now
                      && t.BidSubmissionDeadline < to
                orderby t.BidSubmissionDeadline
                select new TenderAlertRow
                {
                    TenderId = t.Id,
                    TenderNumber = t.TenderNumber,
                    TenderTitle = t.TenderTitle,
                    CustomerId = c.Id,
                    Customer = c.OrganizationName,
                    Deadline = t.BidSubmissionDeadline,
                    Value = t.EstimatedTenderValueInr ?? 0,
                    BidStatus = t.BidStatus,
                    OwnerId = owner != null ? (int?)owner.Id : null,
                    OwnerName = owner != null ? owner.FullName : null,
                    OwnerEmail = owner != null ? owner.Email : null,
                    OwnerMobile = owner != null ? owner.Mobile : null
                }).ToListAsync();
        }

        private async Task<InvoiceCtx?> LoadInvoiceCtx(int invoiceId)
        {
            return await (
                from i in _context.invoices
                join c in _context.customers on i.CustomerId equals c.Id
                join poJoin in _context.purchase_orders on i.PoId equals poJoin.Id into poGroup
                from po in poGroup.DefaultIfEmpty()
                where i.Id == invoiceId
                select new InvoiceCtx { Invoice = i, Customer = c, Po = po }).FirstOrDefaultAsync();
        }

        private async Task<TenderCtx?> LoadTenderCtx(int tenderId)
        {
            return await (
                from t in _context.tenders
                join c in _context.customers on t.CustomerId equals c.Id
                join uJoin in _context.users on t.BidOwnerId equals (int?)uJoin.Id into uGroup
                from owner in uGroup.DefaultIfEmpty()
                where t.Id == tenderId
                select new TenderCtx { Tender = t, Customer = c, Owner = owner }).FirstOrDefaultAsync();
        }

        // tender alert window:  <=0 -> T-0, 1 -> T-1, <=3 -> T-3, <=7 -> T-7, else T-15
        private static (int WindowDays, string Label, string Severity) TenderWindow(int daysLeft)
        {
            if (daysLeft <= 0) return (0, "T-0", "Critical");
            if (daysLeft <= 1) return (1, "T-1", "Critical");
            if (daysLeft <= 3) return (3, "T-3", "Urgent");
            if (daysLeft <= 7) return (7, "T-7", "Informational");
            return (15, "T-15", "Upcoming");
        }

        private static int TenderDaysLeft(DateTime deadline, DateTime now) => (deadline.Date - now.Date).Days;

        // =====================================================================
        // Template variables
        // =====================================================================
        private Dictionary<string, string> InvoiceVars(InvoiceCtx x, int daysOverdue)
        {
            var inv = x.Invoice;
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["contact_name"] = string.IsNullOrWhiteSpace(x.Customer.KeyContactName) ? "Sir / Madam" : x.Customer.KeyContactName!,
                ["customer_name"] = x.Customer.OrganizationName,
                ["organization_name"] = x.Customer.OrganizationName,
                ["invoice_number"] = inv.InvoiceNumber,
                ["invoice_date"] = inv.InvoiceDate.ToString("dd MMM yyyy", Inv),
                ["due_date"] = inv.PaymentDueDate.HasValue ? inv.PaymentDueDate.Value.ToString("dd MMM yyyy", Inv) : "-",
                ["days_overdue"] = Math.Max(daysOverdue, 0).ToString(Inv),
                ["outstanding_inr"] = Inr1(inv.OutstandingAmountInr),
                ["outstanding_amount"] = Inr1(inv.OutstandingAmountInr),
                ["invoice_value_inr"] = Inr1(inv.TotalInvoiceValueInr),
                ["po_number"] = x.Po?.PoNumber ?? "-",
                ["project_name"] = x.Po?.ProjectName ?? "-",
                ["account_owner"] = x.Customer.AccountOwner ?? string.Empty,
                ["sender_name"] = _mail.FromName,
                ["company_name"] = _mail.CompanyName
            };
        }

        private Dictionary<string, string> TenderVars(TenderCtx x, int daysLeft, string windowLabel)
        {
            var t = x.Tender;
            var ownerName = x.Owner?.FullName ?? "Team";
            var deadlineText = t.BidSubmissionDeadline.HasValue
                ? t.BidSubmissionDeadline.Value.ToString("dd MMM yyyy, hh:mm tt", Inv)
                : "-";
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["contact_name"] = ownerName,
                ["owner_name"] = ownerName,
                ["tender_number"] = t.TenderNumber,
                ["tender_title"] = t.TenderTitle,
                ["customer_name"] = x.Customer.OrganizationName,
                ["organization_name"] = x.Customer.OrganizationName,
                ["deadline"] = deadlineText,
                ["deadline_date"] = t.BidSubmissionDeadline.HasValue ? t.BidSubmissionDeadline.Value.ToString("dd MMM yyyy", Inv) : "-",
                ["days_left"] = Math.Max(daysLeft, 0).ToString(Inv),
                ["tender_value_inr"] = Inr1(t.EstimatedTenderValueInr ?? 0),
                ["value_inr"] = Inr1(t.EstimatedTenderValueInr ?? 0),
                ["bid_status"] = t.BidStatus,
                ["window"] = windowLabel,
                ["sender_name"] = _mail.FromName,
                ["company_name"] = _mail.CompanyName
            };
        }

        // =====================================================================
        // Template resolving
        // =====================================================================
        private async Task<message_templates?> ResolvePaymentTemplate(string? templateCode, CommEscalationTier? tier, string channel)
        {
            var code = !string.IsNullOrWhiteSpace(templateCode) ? templateCode : tier?.TemplateCode;
            if (string.IsNullOrWhiteSpace(code)) return null;

            var baseTemplate = await _context.message_templates.FirstOrDefaultAsync(t => t.TemplateCode == code);
            if (baseTemplate == null) return null;

            // user asked a different channel -> look for the same use case + severity in that channel
            if (!string.Equals(baseTemplate.Channel, channel, StringComparison.OrdinalIgnoreCase))
            {
                var sameChannel = await _context.message_templates.FirstOrDefaultAsync(t =>
                    t.Active && t.Channel == channel && t.UseCase == baseTemplate.UseCase
                    && t.SeverityLevel == baseTemplate.SeverityLevel);
                if (sameChannel != null) return sameChannel;
            }
            return baseTemplate;
        }

        private async Task<message_templates?> ResolveTenderTemplate(string? templateCode, int windowDays, string channel)
        {
            if (!string.IsNullOrWhiteSpace(templateCode))
                return await _context.message_templates.FirstOrDefaultAsync(t => t.TemplateCode == templateCode);

            var list = await _context.message_templates
                .Where(t => t.Active && t.Channel == channel && t.UseCase.Contains("Tender"))
                .ToListAsync();
            if (list.Count == 0) return null;

            return list.FirstOrDefault(t => t.LeadTimeDays == windowDays)
                   ?? list.Where(t => t.LeadTimeDays.HasValue)
                          .OrderBy(t => Math.Abs(t.LeadTimeDays!.Value - windowDays))
                          .FirstOrDefault()
                   ?? list.First();
        }

        // =====================================================================
        // Core: log into message_outbox and deliver
        //   Email    -> real SMTP (Gmail / Outlook) when EmailSettings.Enabled, else simulated
        //   WhatsApp -> always simulated (recorded to outbox)
        // =====================================================================
        private async Task<message_outbox> Dispatch(
            string channel, string useCase, int? templateId, int? invoiceId, int? tenderId, int? customerId,
            string? recipientName, string? recipientEmail, string? recipientMobile, string? ccEmail,
            string? subject, string body, int? sentById, string notes)
        {
            var now = IstNow();
            var row = new message_outbox
            {
                Channel = channel,
                UseCase = useCase,
                TemplateId = templateId,
                InvoiceId = invoiceId,
                TenderId = tenderId,
                CustomerId = customerId,
                RecipientName = recipientName,
                RecipientEmail = recipientEmail,
                RecipientMobile = recipientMobile,
                Subject = subject,
                BodyRendered = body,
                Status = CommConstants.StQueued,
                SentById = sentById,
                Notes = notes,
                CreatedAt = now
            };

            try
            {
                if (channel == CommConstants.Email && _email.IsConfigured)
                {
                    await _email.SendAsync(recipientEmail!, recipientName, subject ?? string.Empty, body, ccEmail);
                    row.Status = CommConstants.StSent;
                    row.DeliveryRef = "SMTP-" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpperInvariant();
                }
                else
                {
                    // WhatsApp (always) or Email while EmailSettings.Enabled = false
                    row.Status = CommConstants.StSimulated;
                    row.DeliveryRef = (channel == CommConstants.WhatsApp ? "WA-SIM-" : "MAIL-SIM-")
                                      + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpperInvariant();
                }
                row.SentAt = IstNow();
            }
            catch (Exception ex)
            {
                row.Status = CommConstants.StFailed;
                row.ErrorMessage = Truncate(ex.InnerException?.Message ?? ex.Message, 1000);
            }

            _context.message_outbox.Add(row);
            await _context.SaveChangesAsync();
            return row;
        }

        private static APIResponse SendResponse(message_outbox row, string okMessage)
        {
            var result = new CommSendResult
            {
                OutboxId = row.Id,
                Channel = row.Channel,
                Status = row.Status,
                DeliveryRef = row.DeliveryRef,
                ErrorMessage = row.ErrorMessage
            };

            if (row.Status == CommConstants.StFailed)
            {
                return new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ActionResponse = "Message could not be sent. It is saved in the outbox as Failed.",
                    Result = result
                };
            }
            return Ok(result, okMessage);
        }

        // =====================================================================
        // Header cards
        // =====================================================================
        public async Task<APIResponse> GetSummary()
        {
            var now = IstNow();
            var today = IstToday();

            // reminders due
            var reminderRows = await LoadReminderRows();
            var reminderTotal = reminderRows.Sum(r => r.Outstanding);

            // tender deadlines next 15 days
            var tenderRows = await LoadTenderAlertRows(15);
            var critical = tenderRows.Count(r => TenderDaysLeft(r.Deadline!.Value, now) <= 1);

            // messages last 30 days
            var from30 = now.AddDays(-30);
            var msgs = await _context.message_outbox
                .Where(m => (m.SentAt ?? m.CreatedAt) >= from30)
                .Select(m => new { m.Channel, m.Status, m.UseCase })
                .ToListAsync();

            var sent = msgs.Where(m => IsSentStatus(m.Status)).ToList();
            var emailSent = sent.Count(m => m.Channel == CommConstants.Email);
            var waSent = sent.Count(m => m.Channel == CommConstants.WhatsApp);

            var escalations = msgs.Count(m => m.UseCase.Contains("Escalat", StringComparison.OrdinalIgnoreCase));
            var reminders = msgs.Count(m => m.UseCase.Contains("Remind", StringComparison.OrdinalIgnoreCase));
            var alerts = msgs.Count(m => m.UseCase.Contains("Tender", StringComparison.OrdinalIgnoreCase));

            var result = new CommSummaryResponse
            {
                ReminderInvoiceCount = reminderRows.Count,
                ReminderOutstandingInr = reminderTotal,
                ReminderOutstandingText = Inr1(reminderTotal),

                TenderDeadlineCount = tenderRows.Count,
                CriticalTenderCount = critical,
                TenderDeadlineSubText = $"{critical} critical (T-1 / today)",

                MessagesSent30d = sent.Count,
                EmailSent30d = emailSent,
                WhatsAppSent30d = waSent,
                MessagesSubText = $"{emailSent} email · {waSent} WhatsApp",

                Escalations30d = escalations,
                Reminders30d = reminders,
                Alerts30d = alerts,
                EscalationsSubText = $"{reminders} reminders · {alerts} alerts"
            };

            _ = today; // (kept for readability / future use)
            return Ok(result);
        }

        // =====================================================================
        // Tab 1 - Payment Reminders
        // =====================================================================
        public async Task<APIResponse> GetEscalationLadder()
        {
            return Ok(await LoadTiers());
        }

        public async Task<APIResponse> GetPaymentReminders()
        {
            var today = IstToday();
            var tiers = await LoadTiers();
            var rows = await LoadReminderRows();

            // how many reminders were already sent for each invoice (outbox)
            var ids = rows.Select(r => r.InvoiceId).ToList();
            var sentInfo = await _context.message_outbox
                .Where(m => m.InvoiceId != null && ids.Contains(m.InvoiceId.Value) && m.Status.StartsWith("Sent"))
                .GroupBy(m => m.InvoiceId)
                .Select(g => new { InvoiceId = g.Key, Count = g.Count(), Last = g.Max(x => x.SentAt ?? x.CreatedAt) })
                .ToListAsync();
            var sentLookup = sentInfo.Where(s => s.InvoiceId.HasValue).ToDictionary(s => s.InvoiceId!.Value);

            var items = rows.Select(r =>
            {
                var days = r.DueDate.HasValue ? today.DayNumber - r.DueDate.Value.DayNumber : 0;
                var tier = TierFor(days, tiers);
                sentLookup.TryGetValue(r.InvoiceId, out var info);

                return new CommReminderInvoiceItem
                {
                    InvoiceId = r.InvoiceId,
                    InvoiceNumber = r.InvoiceNumber,
                    CustomerId = r.CustomerId,
                    Customer = r.Customer,
                    PoNumber = r.PoNumber,
                    DueDate = r.DueDate,
                    DaysOverdue = Math.Max(days, 0),
                    OutstandingInr = r.Outstanding,
                    OutstandingText = Inr1(r.Outstanding),
                    InvoiceStatus = r.InvoiceStatus,
                    TierLabel = tier?.Label,
                    TierSeverity = tier?.Severity,
                    TierTemplateCode = tier?.TemplateCode,
                    ButtonLabel = tier != null ? "Send " + tier.ButtonLabel : null,
                    RecipientName = r.ContactName,
                    RecipientEmail = r.ContactEmail,
                    RecipientMobile = r.ContactMobile,
                    AccountOwner = r.AccountOwner,
                    RemindersSent = info?.Count ?? 0,
                    LastReminderAt = info?.Last
                };
            })
            .OrderByDescending(i => i.DaysOverdue)
            .ToList();

            var total = items.Sum(i => i.OutstandingInr);

            return Ok(new CommPaymentRemindersResponse
            {
                InvoiceCount = items.Count,
                TotalOutstandingInr = total,
                TotalOutstandingText = Inr1(total),
                HeadingText = $"{items.Count} invoices · {Inr1(total)} outstanding",
                Ladder = tiers,
                Invoices = items
            });
        }

        // the "Send Email - PAY_ESC_T45" pop-up
        public async Task<APIResponse> PreviewPaymentReminder(int invoiceId, string? templateCode, string? channel)
        {
            var ctx = await LoadInvoiceCtx(invoiceId);
            if (ctx == null) return Fail(HttpStatusCode.NotFound, "Invoice not found.");

            var days = ctx.Invoice.PaymentDueDate.HasValue
                ? IstToday().DayNumber - ctx.Invoice.PaymentDueDate.Value.DayNumber
                : 0;

            var tiers = await LoadTiers();
            var tier = TierFor(days, tiers);
            var ch = NormalizeChannel(channel);

            var tpl = await ResolvePaymentTemplate(templateCode, tier, ch);
            if (tpl == null)
                return Fail(HttpStatusCode.NotFound, "No message template found for this invoice (check escalation_policy.template_code and message_templates).");

            var vars = InvoiceVars(ctx, days);
            var subject = Render(tpl.Subject, vars);
            var body = Render(tpl.BodyText, vars);

            string? cc = null;
            if (IsEscalationSeverity(tpl.SeverityLevel) && !string.IsNullOrWhiteSpace(ctx.Customer.AccountOwner))
            {
                var ownerName = ctx.Customer.AccountOwner;
                cc = await _context.users
                    .Where(u => u.Active && u.FullName == ownerName)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();
            }

            return Ok(new CommPreviewResponse
            {
                Title = $"Send {tpl.Channel} — {tpl.TemplateCode}",
                TemplateCode = tpl.TemplateCode,
                TemplateName = tpl.TemplateName,
                Channel = tpl.Channel,
                UseCase = IsEscalationSeverity(tpl.SeverityLevel) ? CommConstants.UcEscalation : CommConstants.UcPaymentReminder,
                RecipientName = ctx.Customer.KeyContactName,
                RecipientEmail = ctx.Customer.KeyContactEmail,
                RecipientMobile = ctx.Customer.KeyContactMobile,
                CcEmail = cc,
                Subject = subject,
                Body = body,
                Footer = "Live email via Gmail/Outlook · logged to outbox on send",
                Variables = vars
            });
        }

        public async Task<APIResponse> SendPaymentReminder(CommSendRequest req)
        {
            if (req.InvoiceId == null) return Fail(HttpStatusCode.BadRequest, "InvoiceId is required.");
            if (string.IsNullOrWhiteSpace(req.Body)) return Fail(HttpStatusCode.BadRequest, "Message body is required.");

            var channel = NormalizeChannel(req.Channel);
            var error = ValidateRecipient(channel, req.RecipientEmail, req.RecipientMobile);
            if (error != null) return Fail(HttpStatusCode.BadRequest, error);

            var ctx = await LoadInvoiceCtx(req.InvoiceId.Value);
            if (ctx == null) return Fail(HttpStatusCode.NotFound, "Invoice not found.");

            var days = ctx.Invoice.PaymentDueDate.HasValue
                ? IstToday().DayNumber - ctx.Invoice.PaymentDueDate.Value.DayNumber
                : 0;

            var tiers = await LoadTiers();
            var tier = TierFor(days, tiers);
            var tpl = await ResolvePaymentTemplate(req.TemplateCode, tier, channel);

            var severity = tpl?.SeverityLevel ?? tier?.Severity;
            var useCase = IsEscalationSeverity(severity) ? CommConstants.UcEscalation : CommConstants.UcPaymentReminder;

            var row = await Dispatch(channel, useCase, tpl?.Id, ctx.Invoice.Id, null, ctx.Customer.Id,
                req.RecipientName, req.RecipientEmail, req.RecipientMobile, req.CcEmail,
                req.Subject, req.Body, req.SentById, CommConstants.NoteManual);

            return SendResponse(row, "Payment reminder sent successfully.");
        }

        // =====================================================================
        // Tab 2 - Tender Alerts
        // =====================================================================
        public async Task<APIResponse> GetTenderAlerts(int days = 15)
        {
            var now = IstNow();
            var rows = await LoadTenderAlertRows(days);

            var ids = rows.Select(r => r.TenderId).ToList();
            var sentInfo = await _context.message_outbox
                .Where(m => m.TenderId != null && ids.Contains(m.TenderId.Value) && m.Status.StartsWith("Sent"))
                .GroupBy(m => m.TenderId)
                .Select(g => new { TenderId = g.Key, Count = g.Count() })
                .ToListAsync();
            var sentLookup = sentInfo.Where(s => s.TenderId.HasValue).ToDictionary(s => s.TenderId!.Value, s => s.Count);

            var items = rows.Select(r =>
            {
                var left = TenderDaysLeft(r.Deadline!.Value, now);
                var w = TenderWindow(left);
                sentLookup.TryGetValue(r.TenderId, out var count);

                return new CommTenderAlertItem
                {
                    TenderId = r.TenderId,
                    TenderNumber = r.TenderNumber,
                    TenderTitle = r.TenderTitle,
                    CustomerId = r.CustomerId,
                    Customer = r.Customer,
                    Deadline = r.Deadline!.Value,
                    DaysLeft = left,
                    DaysLeftText = left <= 0 ? "today" : left + "d left",
                    ValueInr = r.Value,
                    ValueText = Inr1(r.Value),
                    BidStatus = r.BidStatus,
                    Window = w.Label,
                    WindowLabel = w.Label + " " + w.Severity,
                    IsCritical = left <= 1,
                    OwnerId = r.OwnerId,
                    OwnerName = r.OwnerName,
                    OwnerEmail = r.OwnerEmail,
                    OwnerMobile = r.OwnerMobile,
                    AlertsSent = count
                };
            }).ToList();

            return Ok(new CommTenderAlertsResponse
            {
                Days = days,
                Count = items.Count,
                HeadingText = $"{items.Count} tenders with deadlines in next {days} days",
                Items = items
            });
        }

        public async Task<APIResponse> PreviewTenderAlert(int tenderId, string? templateCode, string? channel)
        {
            var ctx = await LoadTenderCtx(tenderId);
            if (ctx == null) return Fail(HttpStatusCode.NotFound, "Tender not found.");

            var now = IstNow();
            var left = ctx.Tender.BidSubmissionDeadline.HasValue ? TenderDaysLeft(ctx.Tender.BidSubmissionDeadline.Value, now) : 0;
            var w = TenderWindow(left);
            var ch = NormalizeChannel(channel);

            var tpl = await ResolveTenderTemplate(templateCode, w.WindowDays, ch);
            if (tpl == null)
                return Fail(HttpStatusCode.NotFound, "No Tender Alert template found (message_templates.use_case should contain 'Tender').");

            var vars = TenderVars(ctx, left, w.Label);

            return Ok(new CommPreviewResponse
            {
                Title = $"Send {tpl.Channel} — {tpl.TemplateCode}",
                TemplateCode = tpl.TemplateCode,
                TemplateName = tpl.TemplateName,
                Channel = tpl.Channel,
                UseCase = CommConstants.UcTenderAlert,
                RecipientName = ctx.Owner?.FullName,
                RecipientEmail = ctx.Owner?.Email,
                RecipientMobile = ctx.Owner?.Mobile,
                Subject = Render(tpl.Subject, vars),
                Body = Render(tpl.BodyText, vars),
                Footer = "Live email via Gmail/Outlook · WhatsApp simulated · logged to outbox on send",
                Variables = vars
            });
        }

        public async Task<APIResponse> SendTenderAlert(CommSendRequest req)
        {
            if (req.TenderId == null) return Fail(HttpStatusCode.BadRequest, "TenderId is required.");
            if (string.IsNullOrWhiteSpace(req.Body)) return Fail(HttpStatusCode.BadRequest, "Message body is required.");

            var channel = NormalizeChannel(req.Channel);
            var error = ValidateRecipient(channel, req.RecipientEmail, req.RecipientMobile);
            if (error != null) return Fail(HttpStatusCode.BadRequest, error);

            var ctx = await LoadTenderCtx(req.TenderId.Value);
            if (ctx == null) return Fail(HttpStatusCode.NotFound, "Tender not found.");

            int? templateId = null;
            if (!string.IsNullOrWhiteSpace(req.TemplateCode))
            {
                templateId = await _context.message_templates
                    .Where(t => t.TemplateCode == req.TemplateCode)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefaultAsync();
            }

            var row = await Dispatch(channel, CommConstants.UcTenderAlert, templateId, null, ctx.Tender.Id, ctx.Customer.Id,
                req.RecipientName, req.RecipientEmail, req.RecipientMobile, req.CcEmail,
                req.Subject, req.Body, req.SentById, CommConstants.NoteManual);

            return SendResponse(row, "Tender alert sent successfully.");
        }

        // =====================================================================
        // Tab 3 - Templates
        // =====================================================================
        public async Task<APIResponse> GetTemplates(string? search, string? useCase, string? channel, bool includeInactive = false)
        {
            var q = _context.message_templates.AsQueryable();

            if (!includeInactive) q = q.Where(t => t.Active);
            if (!string.IsNullOrWhiteSpace(useCase)) q = q.Where(t => t.UseCase == useCase);
            if (!string.IsNullOrWhiteSpace(channel)) q = q.Where(t => t.Channel == channel);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(t => t.TemplateName.ToLower().Contains(s)
                              || t.TemplateCode.ToLower().Contains(s)
                              || t.BodyText.ToLower().Contains(s));
            }

            var list = await q.OrderBy(t => t.UseCase).ThenBy(t => t.AgingDaysMin).ThenBy(t => t.LeadTimeDays).ThenBy(t => t.TemplateName)
                              .ToListAsync();

            var groups = list
                .GroupBy(t => t.UseCase)
                .Select(g => new CommTemplateGroup
                {
                    UseCase = g.Key,
                    Templates = g.Select(t => new CommTemplateItem
                    {
                        Id = t.Id,
                        TemplateCode = t.TemplateCode,
                        TemplateName = t.TemplateName,
                        Channel = t.Channel,
                        UseCase = t.UseCase,
                        SeverityLevel = t.SeverityLevel,
                        AgingDaysMin = t.AgingDaysMin,
                        AgingDaysMax = t.AgingDaysMax,
                        LeadTimeDays = t.LeadTimeDays,
                        Subject = t.Subject,
                        BodyText = t.BodyText,
                        Variables = t.Variables,
                        Active = t.Active
                    }).ToList()
                }).ToList();

            return Ok(new CommTemplatesResponse
            {
                TotalCount = list.Count,
                HeadingText = $"{list.Count} templates · used automatically based on aging / deadline",
                Groups = groups
            });
        }

        // =====================================================================
        // Tab 4 - Outbox   (message_outbox LEFT JOIN tenders / invoices / users, customers resolved after)
        // =====================================================================
        public async Task<APIResponse> GetOutbox(string? search, string? channel, string? status, string? useCase,
            int pageNumber = 1, int pageSize = 20)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;

            var q = _context.message_outbox.AsQueryable();
            if (!string.IsNullOrWhiteSpace(channel)) q = q.Where(m => m.Channel == channel);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(m => m.Status == status);
            if (!string.IsNullOrWhiteSpace(useCase)) q = q.Where(m => m.UseCase == useCase);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(m => (m.Subject != null && m.Subject.ToLower().Contains(s))
                              || (m.RecipientName != null && m.RecipientName.ToLower().Contains(s))
                              || m.BodyRendered.ToLower().Contains(s));
            }

            var total = await q.CountAsync();

            var rows = await (
                from m in q
                join tJoin in _context.tenders on m.TenderId equals (int?)tJoin.Id into tGroup
                from tender in tGroup.DefaultIfEmpty()
                join iJoin in _context.invoices on m.InvoiceId equals (int?)iJoin.Id into iGroup
                from inv in iGroup.DefaultIfEmpty()
                join uJoin in _context.users on m.SentById equals (int?)uJoin.Id into uGroup
                from sender in uGroup.DefaultIfEmpty()
                orderby (m.SentAt ?? m.CreatedAt) descending
                select new
                {
                    m.Id,
                    m.Channel,
                    m.Status,
                    m.UseCase,
                    m.Subject,
                    m.BodyRendered,
                    m.RecipientName,
                    m.RecipientEmail,
                    m.RecipientMobile,
                    m.CustomerId,
                    m.Notes,
                    m.DeliveryRef,
                    m.ErrorMessage,
                    When = m.SentAt ?? m.CreatedAt,
                    TenderNumber = tender != null ? tender.TenderNumber : null,
                    TenderCustomerId = tender != null ? (int?)tender.CustomerId : null,
                    InvoiceNumber = inv != null ? inv.InvoiceNumber : null,
                    InvoiceCustomerId = inv != null ? (int?)inv.CustomerId : null,
                    SenderName = sender != null ? sender.FullName : null
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // customer names for the rows on this page
            var customerIds = rows
                .Select(r => r.CustomerId ?? r.TenderCustomerId ?? r.InvoiceCustomerId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();
            var customerNames = await _context.customers
                .Where(c => customerIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.OrganizationName);

            var data = rows.Select(r =>
            {
                var custId = r.CustomerId ?? r.TenderCustomerId ?? r.InvoiceCustomerId;
                string? custName = null;
                if (custId.HasValue) customerNames.TryGetValue(custId.Value, out custName);

                return new CommOutboxItem
                {
                    Id = r.Id,
                    Channel = r.Channel,
                    Status = r.Status,
                    UseCase = r.UseCase,
                    Title = !string.IsNullOrWhiteSpace(r.Subject) ? r.Subject! : FirstLine(r.BodyRendered),
                    BodyPreview = Truncate(r.BodyRendered, 160),
                    RecipientName = r.RecipientName,
                    RecipientEmail = r.RecipientEmail,
                    RecipientMobile = r.RecipientMobile,
                    CustomerName = custName,
                    TenderNumber = r.TenderNumber,
                    InvoiceNumber = r.InvoiceNumber,
                    SentByName = r.SenderName,
                    IsAuto = r.Notes != null && r.Notes.StartsWith(CommConstants.NoteAutoAny),
                    When = r.When,
                    DeliveryRef = r.DeliveryRef,
                    ErrorMessage = r.ErrorMessage
                };
            }).ToList();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
                HeadingText = $"Sent history · {total} messages · newest first",
                Data = data
            });
        }

        public async Task<APIResponse> GetOutboxMessage(int id)
        {
            var m = await _context.message_outbox.FirstOrDefaultAsync(x => x.Id == id);
            if (m == null) return Fail(HttpStatusCode.NotFound, "Message not found.");

            var tenderNumber = m.TenderId.HasValue
                ? await _context.tenders.Where(t => t.Id == m.TenderId.Value).Select(t => t.TenderNumber).FirstOrDefaultAsync()
                : null;
            var invoiceNumber = m.InvoiceId.HasValue
                ? await _context.invoices.Where(i => i.Id == m.InvoiceId.Value).Select(i => i.InvoiceNumber).FirstOrDefaultAsync()
                : null;
            var customerName = m.CustomerId.HasValue
                ? await _context.customers.Where(c => c.Id == m.CustomerId.Value).Select(c => c.OrganizationName).FirstOrDefaultAsync()
                : null;

            return Ok(new CommOutboxMessageDetail
            {
                Id = m.Id,
                Channel = m.Channel,
                Status = m.Status,
                UseCase = m.UseCase,
                Title = !string.IsNullOrWhiteSpace(m.Subject) ? m.Subject! : FirstLine(m.BodyRendered),
                BodyPreview = Truncate(m.BodyRendered, 160),
                Subject = m.Subject,
                Body = m.BodyRendered,
                Notes = m.Notes,
                RecipientName = m.RecipientName,
                RecipientEmail = m.RecipientEmail,
                RecipientMobile = m.RecipientMobile,
                CustomerName = customerName,
                TenderNumber = tenderNumber,
                InvoiceNumber = invoiceNumber,
                IsAuto = m.Notes != null && m.Notes.StartsWith(CommConstants.NoteAutoAny),
                When = m.SentAt ?? m.CreatedAt,
                DeliveryRef = m.DeliveryRef,
                ErrorMessage = m.ErrorMessage
            });
        }

        // =====================================================================
        // Tab 5 - Automation
        // =====================================================================
        private static TimeSpan ParseRunAt(string? text, TimeSpan fallback)
            => TimeSpan.TryParse(text, Inv, out var ts) ? ts : fallback;

        private static DateTime NextRun(TimeSpan runAt)
        {
            var now = IstNow();
            var next = now.Date + runAt;
            return next > now ? next : next.AddDays(1);
        }

        public async Task<APIResponse> GetAutomation()
        {
            var now = IstNow();
            var since = now.AddDays(-7);
            var tiers = await LoadTiers();

            var ladderText = string.Join(" · ", tiers
                .Where(t => t.AgingDays > 0)
                .Select(t => $"{t.Label} {t.Severity}"));

            var payRunAt = ParseRunAt(_auto.PaymentReminders.RunAtIst, new TimeSpan(9, 30, 0));
            var tenderRunAt = ParseRunAt(_auto.TenderAlerts.RunAtIst, new TimeSpan(8, 0, 0));

            var lastPayment = await _context.message_outbox
                .Where(m => m.Notes != null && m.Notes.StartsWith(CommConstants.NoteAutoPayment))
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => (DateTime?)m.CreatedAt)
                .FirstOrDefaultAsync();

            var lastTender = await _context.message_outbox
                .Where(m => m.Notes != null && m.Notes.StartsWith(CommConstants.NoteAutoTender))
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => (DateTime?)m.CreatedAt)
                .FirstOrDefaultAsync();

            var jobs = new List<CommAutomationJob>
            {
                new()
                {
                    Key = "payment",
                    Name = "Auto Payment Reminders",
                    Enabled = _auto.PaymentReminders.Enabled,
                    Schedule = $"Daily · {payRunAt:hh\\:mm} IST",
                    Description = $"Scans overdue invoices, fires the matching reminder tier ({ladderText}). " +
                                  $"Re-sends same tier every {_auto.PaymentReminders.ResendEveryDays} days. Escalations CC the account owner.",
                    NextRunIst = NextRun(payRunAt),
                    LastAutoSentAt = lastPayment
                },
                new()
                {
                    Key = "tender",
                    Name = "Auto Tender Deadline Alerts",
                    Enabled = _auto.TenderAlerts.Enabled,
                    Schedule = $"Daily · {tenderRunAt:hh\\:mm} IST",
                    Description = "Fires T-7 / T-3 / T-1 / T-0 alerts to the bid owner. WhatsApp is simulated (recorded to outbox); " +
                                  "email goes live via Gmail/Outlook. Each (tender, window) fires once.",
                    NextRunIst = NextRun(tenderRunAt),
                    LastAutoSentAt = lastTender
                }
            };

            var auto = await _context.message_outbox
                .Where(m => m.Notes != null && m.Notes.StartsWith(CommConstants.NoteAutoAny)
                            && (m.SentAt ?? m.CreatedAt) >= since)
                .OrderByDescending(m => m.SentAt ?? m.CreatedAt)
                .Select(m => new CommAutoSentItem
                {
                    Id = m.Id,
                    Channel = m.Channel,
                    Status = m.Status,
                    UseCase = m.UseCase,
                    Title = m.Subject ?? m.BodyRendered,
                    RecipientName = m.RecipientName,
                    When = m.SentAt ?? m.CreatedAt
                })
                .Take(50)
                .ToListAsync();

            foreach (var a in auto) a.Title = FirstLine(a.Title);

            return Ok(new CommAutomationResponse
            {
                HeadingText = "Scheduled workflows",
                Jobs = jobs,
                AutoSentLast7Days = auto.Count,
                AutoSentText = $"{auto.Count} auto-fired messages in the last 7 days",
                AutoSent = auto
            });
        }

        // ---------------------------------------------------------------------
        // Job 1: Auto Payment Reminders
        // ---------------------------------------------------------------------
        public async Task<APIResponse> RunPaymentReminderAutomation()
        {
            var run = new CommRunResult { Job = "Auto Payment Reminders" };
            var now = IstNow();
            var today = IstToday();
            var resendDays = Math.Max(_auto.PaymentReminders.ResendEveryDays, 1);

            var tiers = (await LoadTiers())
                .Where(t => t.AgingDays > 0 && !string.IsNullOrWhiteSpace(t.TemplateCode))
                .ToList();
            var rows = await LoadReminderRows();
            run.Checked = rows.Count;

            if (rows.Count == 0 || tiers.Count == 0)
            {
                run.Details.Add("Nothing to do (no reminder invoices or no active escalation policy).");
                return Ok(run, "Auto payment reminders finished.");
            }

            var templates = await _context.message_templates
                .Where(t => t.Active)
                .ToDictionaryAsync(t => t.TemplateCode);

            var ownerEmails = await _context.users
                .Where(u => u.Active)
                .Select(u => new { u.FullName, u.Email })
                .ToListAsync();

            var ids = rows.Select(r => r.InvoiceId).ToList();
            var lastAuto = await _context.message_outbox
                .Where(m => m.InvoiceId != null && ids.Contains(m.InvoiceId.Value)
                            && m.Notes != null && m.Notes.StartsWith(CommConstants.NoteAutoPayment)
                            && m.Status.StartsWith("Sent"))
                .Select(m => new { m.InvoiceId, m.TemplateId, At = m.SentAt ?? m.CreatedAt })
                .ToListAsync();

            foreach (var r in rows)
            {
                var days = r.DueDate.HasValue ? today.DayNumber - r.DueDate.Value.DayNumber : 0;
                var tier = TierFor(days, tiers);
                if (tier == null) { run.Skipped++; run.Details.Add($"{r.InvoiceNumber}: no matching tier ({days}d overdue)."); continue; }

                if (!templates.TryGetValue(tier.TemplateCode, out var tpl))
                { run.Skipped++; run.Details.Add($"{r.InvoiceNumber}: template {tier.TemplateCode} not found."); continue; }

                if (!MailAddress.TryCreate(r.ContactEmail, out _))
                { run.Skipped++; run.Details.Add($"{r.InvoiceNumber}: customer contact email missing/invalid."); continue; }

                // same tier is re-sent only every N days
                var last = lastAuto
                    .Where(x => x.InvoiceId == r.InvoiceId && x.TemplateId == tpl.Id)
                    .Select(x => (DateTime?)x.At)
                    .Max();
                if (last.HasValue && (now - last.Value).TotalDays < resendDays)
                { run.Skipped++; run.Details.Add($"{r.InvoiceNumber}: {tier.Label} already sent on {last:dd MMM}."); continue; }

                var ctx = await LoadInvoiceCtx(r.InvoiceId);
                if (ctx == null) { run.Skipped++; continue; }

                var vars = InvoiceVars(ctx, days);

                // escalations CC the account owner
                string? cc = null;
                if (IsEscalationSeverity(tpl.SeverityLevel) && !string.IsNullOrWhiteSpace(r.AccountOwner))
                    cc = ownerEmails.FirstOrDefault(u => string.Equals(u.FullName, r.AccountOwner, StringComparison.OrdinalIgnoreCase))?.Email;

                var useCase = IsEscalationSeverity(tpl.SeverityLevel) ? CommConstants.UcEscalation : CommConstants.UcPaymentReminder;

                var row = await Dispatch(CommConstants.Email, useCase, tpl.Id, r.InvoiceId, null, r.CustomerId,
                    r.ContactName, r.ContactEmail, r.ContactMobile, cc,
                    Render(tpl.Subject, vars), Render(tpl.BodyText, vars), null,
                    CommConstants.NoteAutoPayment + tier.Label);

                if (row.Status == CommConstants.StFailed) { run.Failed++; run.Details.Add($"{r.InvoiceNumber}: FAILED - {row.ErrorMessage}"); }
                else { run.Sent++; run.Details.Add($"{r.InvoiceNumber}: {tier.Label} {tier.Severity} sent."); }
            }

            return Ok(run, "Auto payment reminders finished.");
        }

        // ---------------------------------------------------------------------
        // Job 2: Auto Tender Deadline Alerts   (each tender + window fires once)
        // ---------------------------------------------------------------------
        public async Task<APIResponse> RunTenderAlertAutomation()
        {
            var run = new CommRunResult { Job = "Auto Tender Deadline Alerts" };
            var now = IstNow();

            // only tenders inside the last window (T-7 .. T-0)
            var rows = await LoadTenderAlertRows(7);
            run.Checked = rows.Count;

            if (rows.Count == 0)
            {
                run.Details.Add("Nothing to do (no tender deadlines in the next 7 days).");
                return Ok(run, "Auto tender alerts finished.");
            }

            var ids = rows.Select(r => r.TenderId).ToList();
            var fired = await _context.message_outbox
                .Where(m => m.TenderId != null && ids.Contains(m.TenderId.Value)
                            && m.Notes != null && m.Notes.StartsWith(CommConstants.NoteAutoTender)
                            && m.Status != CommConstants.StFailed)
                .Select(m => new { m.TenderId, m.Notes, m.Channel })
                .ToListAsync();

            foreach (var r in rows)
            {
                var left = TenderDaysLeft(r.Deadline!.Value, now);
                var w = TenderWindow(left);
                var marker = CommConstants.NoteAutoTender + w.Label;

                var ctx = await LoadTenderCtx(r.TenderId);
                if (ctx == null) { run.Skipped++; continue; }
                var vars = TenderVars(ctx, left, w.Label);

                // Email + WhatsApp to the bid owner
                foreach (var channel in new[] { CommConstants.Email, CommConstants.WhatsApp })
                {
                    if (fired.Any(f => f.TenderId == r.TenderId && f.Notes == marker && f.Channel == channel))
                    { run.Skipped++; run.Details.Add($"{r.TenderNumber}: {w.Label} {channel} already fired."); continue; }

                    var error = ValidateRecipient(channel, r.OwnerEmail, r.OwnerMobile);
                    if (error != null)
                    { run.Skipped++; run.Details.Add($"{r.TenderNumber}: {channel} skipped - {error}"); continue; }

                    var tpl = await ResolveTenderTemplate(null, w.WindowDays, channel);
                    if (tpl == null)
                    { run.Skipped++; run.Details.Add($"{r.TenderNumber}: no {channel} tender template."); continue; }

                    var row = await Dispatch(channel, CommConstants.UcTenderAlert, tpl.Id, null, r.TenderId, r.CustomerId,
                        r.OwnerName, r.OwnerEmail, r.OwnerMobile, null,
                        Render(tpl.Subject, vars), Render(tpl.BodyText, vars), null, marker);

                    if (row.Status == CommConstants.StFailed) { run.Failed++; run.Details.Add($"{r.TenderNumber}: {channel} FAILED - {row.ErrorMessage}"); }
                    else { run.Sent++; run.Details.Add($"{r.TenderNumber}: {w.Label} {channel} sent."); }
                }
            }

            return Ok(run, "Auto tender alerts finished.");
        }
    }
}