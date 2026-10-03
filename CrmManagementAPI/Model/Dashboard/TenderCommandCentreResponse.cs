namespace CrmManagementAPI.Model.Dashboard
{
    // ---------- 1. KPI cards (top of the page) ----------
    public class TccSummaryResponse
    {
        public int ActiveTenders { get; set; }
        public decimal ActivePipelineInr { get; set; }
        public string ActivePipelineText { get; set; } = string.Empty;   // "₹7.64 Cr pipeline"

        public decimal WeightedPipelineInr { get; set; }                 // value x probability
        public string WeightedPipelineText { get; set; } = string.Empty; // "₹4.67 Cr"

        public int WonCount { get; set; }
        public decimal WonValueInr { get; set; }
        public string WonValueText { get; set; } = string.Empty;         // "₹5.13 Cr"

        public int LostCount { get; set; }
        public decimal WinRatePct { get; set; }                          // won / (won + lost)

        public int BidsDueIn7Days { get; set; }
        public int PendingGoNoGo { get; set; }
    }

    // ---------- 2. Pipeline value by stage ----------
    public class TccStageValue
    {
        public string Stage { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal ValueInr { get; set; }
        public string ValueText { get; set; } = string.Empty;
    }

    // ---------- 3. Active pipeline by customer category ----------
    public class TccCategoryValue
    {
        public string Category { get; set; } = string.Empty;
        public decimal ValueInr { get; set; }
        public string ValueText { get; set; } = string.Empty;
        public decimal SharePct { get; set; }
    }

    public class TccCategoryPipelineResponse
    {
        public decimal TotalInr { get; set; }
        public string TotalText { get; set; } = string.Empty;            // centre of the donut "₹13 Cr"
        public List<TccCategoryValue> Items { get; set; } = new();
    }

    // ---------- 4. Upcoming bid deadlines ----------
    public class TccDeadlineItem
    {
        public int TenderId { get; set; }
        public string TenderNumber { get; set; } = string.Empty;
        public string TenderTitle { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string? Owner { get; set; }
        public DateTime? Deadline { get; set; }
        public int DaysLeft { get; set; }
        public string BidStatus { get; set; } = string.Empty;
        public decimal ValueInr { get; set; }
        public string ValueText { get; set; } = string.Empty;
        public int WinProbabilityPct { get; set; }
    }

    // ---------- 5. Go / No-Go approval queue ----------
    public class TccGoNoGoItem
    {
        public int ApprovalId { get; set; }
        public string TenderNumber { get; set; } = string.Empty;
        public string TenderTitle { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public decimal ValueInr { get; set; }
        public string ValueText { get; set; } = string.Empty;
        public decimal? MarginPct { get; set; }
        public int? WinProbabilityPct { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string Approver { get; set; } = string.Empty;
        public DateOnly RequestDate { get; set; }
        public string Decision { get; set; } = "Pending";
        public string? ProjectRisk { get; set; }
    }

    // ---------- 6. Recent bid team activities ----------
    public class TccActivityItem
    {
        public int ActivityId { get; set; }
        public string TenderNumber { get; set; } = string.Empty;
        public string TenderTitle { get; set; } = string.Empty;
        public string? Owner { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string? Outcome { get; set; }
        public string? NextAction { get; set; }
        public DateOnly? NextActionDate { get; set; }
    }

    // ---------- 7. Execution - Billing - Collection cards ----------
    public class TccBillingSummaryResponse
    {
        public int ActivePos { get; set; }
        public decimal TotalBilledInr { get; set; }
        public string TotalBilledText { get; set; } = string.Empty;
        public decimal CollectedInr { get; set; }
        public string CollectedText { get; set; } = string.Empty;
        public decimal OutstandingInr { get; set; }
        public string OutstandingText { get; set; } = string.Empty;
        public decimal OverdueInr { get; set; }
        public string OverdueText { get; set; } = string.Empty;
        public decimal DisputedInr { get; set; }
        public string DisputedText { get; set; } = string.Empty;
    }

    // ---------- 8. Purchase Order register ----------
    public class TccPoRegisterItem
    {
        public int PoId { get; set; }
        public string PoNumber { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public DateOnly PoDate { get; set; }
        public string? Owner { get; set; }
        public string? TenderNumber { get; set; }
        public DateOnly? PbgValidityDate { get; set; }
        public string PoStatus { get; set; } = string.Empty;
        public decimal PoValueInr { get; set; }
        public decimal BilledInr { get; set; }
        public decimal CollectedInr { get; set; }
        public decimal OutstandingInr { get; set; }
        public decimal CollectedPct { get; set; }
        public string PoValueText { get; set; } = string.Empty;
        public string BilledText { get; set; } = string.Empty;
        public string CollectedText { get; set; } = string.Empty;
        public string OutstandingText { get; set; } = string.Empty;
    }

    // ---------- 9. Accounts receivable aging ----------
    public class TccAgingBucket
    {
        public string Bucket { get; set; } = string.Empty;   // 0-30d, 31-60d, 61-90d, 90d+
        public int InvoiceCount { get; set; }
        public decimal AmountInr { get; set; }
        public string AmountText { get; set; } = string.Empty;
    }

    // ---------- 10. Outstanding invoices ----------
    public class TccOutstandingInvoiceItem
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string PoNumber { get; set; } = string.Empty;
        public DateOnly? DueDate { get; set; }
        public int DaysOverdue { get; set; }                 // > 0 overdue
        public int DaysToGo { get; set; }                    // > 0 not yet due
        public string InvoiceStatus { get; set; } = string.Empty;
        public decimal InvoiceValueInr { get; set; }
        public decimal ReceivedInr { get; set; }
        public decimal OutstandingInr { get; set; }
        public string InvoiceValueText { get; set; } = string.Empty;
        public string ReceivedText { get; set; } = string.Empty;
        public string OutstandingText { get; set; } = string.Empty;
    }

    // ---------- 11. Upcoming project milestones ----------
    public class TccMilestoneItem
    {
        public int MilestoneId { get; set; }
        public string MilestoneName { get; set; } = string.Empty;
        public string? MilestoneType { get; set; }
        public string PoNumber { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public DateOnly? PlannedDate { get; set; }
        public int DaysLeft { get; set; }                    // negative = overdue
        public bool IsOverdue { get; set; }
        public decimal UnlocksInr { get; set; }
        public string UnlocksText { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // ---------- 12. Tenders by portal ----------
    public class TccPortalItem
    {
        public string Portal { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal SharePct { get; set; }
    }

    public class TccPortalResponse
    {
        public int Total { get; set; }
        public List<TccPortalItem> Items { get; set; } = new();
    }

    // ---------- One call for the whole page ----------
    public class TenderCommandCentreResponse
    {
        public TccSummaryResponse Summary { get; set; } = new();
        public List<TccStageValue> PipelineByStage { get; set; } = new();
        public TccCategoryPipelineResponse PipelineByCategory { get; set; } = new();
        public List<TccDeadlineItem> UpcomingDeadlines { get; set; } = new();
        public List<TccGoNoGoItem> GoNoGoQueue { get; set; } = new();
        public List<TccActivityItem> RecentActivities { get; set; } = new();
        public TccBillingSummaryResponse Billing { get; set; } = new();
        public List<TccPoRegisterItem> PoRegister { get; set; } = new();
        public List<TccAgingBucket> ReceivableAging { get; set; } = new();
        public List<TccOutstandingInvoiceItem> OutstandingInvoices { get; set; } = new();
        public List<TccMilestoneItem> UpcomingMilestones { get; set; } = new();
        public TccPortalResponse TendersByPortal { get; set; } = new();
    }
}
